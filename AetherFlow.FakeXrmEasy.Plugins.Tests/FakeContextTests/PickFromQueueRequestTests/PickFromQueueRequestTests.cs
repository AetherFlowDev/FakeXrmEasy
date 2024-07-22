using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.ServiceModel;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.PickFromQueueRequestTests
{
    public class PickFromQueueRequestTests
    {
        [Test]
        public void When_can_execute_is_called_with_an_invalid_request_result_is_false()
        {
            var executor = new PickFromQueueRequestExecutor();
            var anotherRequest = new RetrieveMultipleRequest();
            Assert.False(executor.CanExecute(anotherRequest));
        }

        [Test]
        public void When_a_request_is_called_worker_is_set()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var email = new Entity
            {
                LogicalName = Crm.Email.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var queue = new Entity
            {
                LogicalName = Crm.Queue.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var user = new Entity
            {
                LogicalName = Crm.SystemUser.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var queueItem = new Entity
            {
                LogicalName = Crm.QueueItem.EntityLogicalName,
                Id = Guid.NewGuid(),
                Attributes =
                {
                    { "queueid", queue.ToEntityReference() },
                    { "objectid", email.ToEntityReference() }
                }
            };

            context.Initialize(new[]
            {
                queue, email, user, queueItem
            });

            var executor = new PickFromQueueRequestExecutor();

            var req = new PickFromQueueRequest
            {
                QueueItemId = queueItem.Id,
                WorkerId = user.Id,
            };

            var before = DateTime.Now.Ticks;
            executor.Execute(req, context);
            var after = DateTime.Now.Ticks;

            var queueItemUpdated = service.Retrieve(Crm.QueueItem.EntityLogicalName, queueItem.Id, new ColumnSet(true));

            Assert.AreEqual(user.ToEntityReference(), queueItemUpdated.GetAttributeValue<EntityReference>("workerid"));
            Assert.True(before <= queueItemUpdated.GetAttributeValue<DateTime>("workeridmodifiedon").Ticks);
            Assert.True(after >= queueItemUpdated.GetAttributeValue<DateTime>("workeridmodifiedon").Ticks);
        }

        [Test]
        public void When_a_request_is_called_with_removal_queueitem_is_deleted()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var email = new Entity
            {
                LogicalName = Crm.Email.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var queue = new Entity
            {
                LogicalName = Crm.Queue.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var user = new Entity
            {
                LogicalName = Crm.SystemUser.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var queueItem = new Entity
            {
                LogicalName = Crm.QueueItem.EntityLogicalName,
                Id = Guid.NewGuid(),
                Attributes =
                {
                    { "queueid", queue.ToEntityReference() },
                    { "objectid", email.ToEntityReference() }
                }
            };

            context.Initialize(new[]
            {
                queue, email, user, queueItem
            });

            var executor = new PickFromQueueRequestExecutor();

            var req = new PickFromQueueRequest
            {
                QueueItemId = queueItem.Id,
                WorkerId = user.Id,
                RemoveQueueItem = true
            };

            executor.Execute(req, context);

            Assert.IsEmpty(context.Data[Crm.QueueItem.EntityLogicalName]);
        }

        [Test]
        public void When_a_request_is_for_non_existing_woker_an_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var queue = new Entity
            {
                LogicalName = Crm.Queue.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            var queueItem = new Entity
            {
                LogicalName = Crm.QueueItem.EntityLogicalName,
                Id = Guid.NewGuid(),
                Attributes =
                {
                    { "queueid", queue.ToEntityReference() },
                    { "objectid", Guid.NewGuid() }
                }
            };

            context.Initialize(new[]
            {
                queue, queueItem
            });

            var executor = new PickFromQueueRequestExecutor();

            var req = new PickFromQueueRequest
            {
                QueueItemId = queueItem.Id,
                WorkerId = Guid.NewGuid(),
            };

            Assert.Throws<FaultException<OrganizationServiceFault>>(() => executor.Execute(req, context));
        }

        [Test]
        public void When_a_request_is_for_non_existing_queueitem_an_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var user = new Entity
            {
                LogicalName = Crm.SystemUser.EntityLogicalName,
                Id = Guid.NewGuid(),
            };

            context.Initialize(new[]
            {
                user
            });

            var executor = new PickFromQueueRequestExecutor();

            var req = new PickFromQueueRequest
            {
                QueueItemId = Guid.NewGuid(),
                WorkerId = user.Id,
            };

            Assert.Throws<FaultException<OrganizationServiceFault>>(() => executor.Execute(req, context));
        }
    }
}