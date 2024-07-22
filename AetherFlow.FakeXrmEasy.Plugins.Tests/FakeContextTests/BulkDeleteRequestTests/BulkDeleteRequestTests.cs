using System;
using System.Reflection;
using System.ServiceModel;
using Crm;
using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.BulkDeleteRequestTests
{
    public class BulkDeleteRequestTests
    {
        [Test]
        public void When_can_execute_is_called_with_an_invalid_request_result_is_false()
        {
            var executor = new BulkDeleteRequestExecutor();
            var anotherRequest = new RetrieveMultipleRequest();
            Assert.False(executor.CanExecute(anotherRequest));
        }

        [Test]
        public void When_execute_is_called_with_a_null_jobname_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var executor = new BulkDeleteRequestExecutor();
            BulkDeleteRequest req = new BulkDeleteRequest
            {
                JobName = null
            };
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => executor.Execute(req, context));
        }

        [Test]
        public void When_execute_is_called_with_a_null_queryset_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var executor = new BulkDeleteRequestExecutor();
            BulkDeleteRequest req = new BulkDeleteRequest
            {
                JobName = "Dummy Job",
                QuerySet = null
            };
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => executor.Execute(req, context));
        }

        [Test]
        public void When_execute_is_called_with_a_null_ccrecipients_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var executor = new BulkDeleteRequestExecutor();
            BulkDeleteRequest req = new BulkDeleteRequest
            {
                JobName = "Dummy Job",
                QuerySet = new[]
                {
                    new QueryExpression("account"),
                },
                CCRecipients = null
            };
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => executor.Execute(req, context));
        }

        [Test]
        public void When_execute_is_called_with_a_null_torecipients_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var executor = new BulkDeleteRequestExecutor();
            BulkDeleteRequest req = new BulkDeleteRequest
            {
                JobName = "Dummy Job",
                QuerySet = new[]
                {
                    new QueryExpression("account"),
                },
                CCRecipients = new[]
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                ToRecipients = null
            };
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => executor.Execute(req, context));
        }

        [Test]
        public void Check_if_contacts_have_been_deleted_after_sending_request()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            // initialize data
            var parentAccountId = Guid.NewGuid();
            var keepName = "Keep";

            var contactA = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = "Delete",
                LastName = "Me1",
                ParentCustomerId = new EntityReference(Account.EntityLogicalName, parentAccountId)
            };

            var contactB = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = "Delete",
                LastName = "Me2",
                ParentCustomerId = new EntityReference(Account.EntityLogicalName, parentAccountId)
            };

            var contactC = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = keepName,
                LastName = "Me",
                ParentCustomerId = new EntityReference(Account.EntityLogicalName, Guid.NewGuid())
            };

            context.Initialize(new[] { contactA, contactB, contactC });

            var query = new QueryExpression
            {
                EntityName = Contact.EntityLogicalName,
                Distinct = false,
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("parentcustomerid", ConditionOperator.Equal, parentAccountId )
                    }
                }
            };

            // execute
            var request = new BulkDeleteRequest
            {
                JobName = $"Delete Contacts of Account '{parentAccountId}'",
                QuerySet = new[]
                {
                    query
                },
                ToRecipients = new[] { Guid.NewGuid() },
                CCRecipients = new Guid[] { },
                SendEmailNotification = false,
                RecurrencePattern = string.Empty
            };

            var response = (BulkDeleteResponse)service.Execute(request);

            // validate
            var deletedContacts = (from c in context.CreateQuery<Contact>()
                                   where Equals(c.ParentCustomerId, new EntityReference(Account.EntityLogicalName, parentAccountId))
                                   select c);
            var allContacts = (from c in context.CreateQuery<Contact>()
                               select c);

            var asyncOperation = (from a in context.CreateQuery<AsyncOperation>()
                                  where a.AsyncOperationId == response.JobId
                                  select a);

            Assert.NotNull(response);
            Assert.IsInstanceOf<BulkDeleteResponse>(response);
            Assert.NotNull(response.JobId);
            Assert.AreNotEqual(Guid.Empty, response.JobId);
            Assert.AreEqual(0, deletedContacts.Count());
            Assert.AreEqual(1, allContacts.Count());
            Assert.AreEqual(keepName, allContacts.First().FirstName);
            Assert.AreEqual(1, asyncOperation.Count());
            Assert.AreEqual(AsyncOperationState.Completed, asyncOperation.First().StateCode);
        }
    }
}