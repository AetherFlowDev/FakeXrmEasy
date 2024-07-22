using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests
{
    public class FakeXrmEasyTestRetrieve
    {
        [Test]
        public void When_retrieve_is_invoked_with_an_empty_logical_name_an_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var ex = Assert.Throws<InvalidOperationException>(() => service.Retrieve(null, Guid.Empty, new ColumnSet()));
            Assert.AreEqual(ex.Message, "The entity logical name must not be null or empty.");

            ex = Assert.Throws<InvalidOperationException>(() => service.Retrieve("", Guid.Empty, new ColumnSet()));
            Assert.AreEqual(ex.Message, "The entity logical name must not be null or empty.");

            ex = Assert.Throws<InvalidOperationException>(() => service.Retrieve("     ", Guid.Empty, new ColumnSet()));
            Assert.AreEqual(ex.Message, "The entity logical name must not be null or empty.");
        }

        [Test]
        public void When_retrieve_is_invoked_with_an_empty_guid_an_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.Retrieve("account", Guid.Empty, new ColumnSet(true)));
            Assert.AreEqual(ex.Message, "account With Id = 00000000-0000-0000-0000-000000000000 Does Not Exist");
        }

        [Test]
        public void When_retrieve_is_invoked_with_a_null_columnset_exception_is_thrown()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.Retrieve("account", Guid.NewGuid(), null));
            Assert.AreEqual(ex.Message, "Required field 'ColumnSet' is missing");
        }

        [Test]
        public void When_retrieve_is_invoked_with_a_non_existing_logical_name_an_exception_is_thrown()
        {
            var context = new XrmFakedContext();

            var service = context.GetOrganizationService();

            var ex = Assert.Throws<InvalidOperationException>(() => service.Retrieve("account", Guid.NewGuid(), new ColumnSet(true)));
            Assert.AreEqual("The entity logical name account is not valid.", ex.Message);
        }

        [Test]
        public void When_retrieve_is_invoked_with_non_existing_entity_null_is_returned()
        {
            var context = new XrmFakedContext();

            //Initialize the context with a single entity
            var guid = Guid.NewGuid();
            var data = new List<Entity>() {
                new Entity("account") { Id = guid }
            }.AsQueryable();

            context.Initialize(data);

            var service = context.GetOrganizationService();

            var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.Retrieve("account", Guid.NewGuid(), new ColumnSet()));
            Assert.AreEqual((uint)0x80040217, (uint)ex.Detail.ErrorCode);
        }

        [Test]
        public void When_retrieve_is_invoked_with_an_existing_entity_that_entity_is_returned()
        {
            var context = new XrmFakedContext();

            //Initialize the context with a single entity
            var guid = Guid.NewGuid();
            var data = new List<Entity>() {
                new Entity("account") { Id = guid }
            }.AsQueryable();

            context.Initialize(data);

            var service = context.GetOrganizationService();

            var result = service.Retrieve("account", guid, new ColumnSet());
            Assert.AreEqual(result.Id, data.FirstOrDefault().Id);
        }

        [Test]
        public void When_retrieve_is_invoked_with_an_existing_entity_and_all_columns_all_the_attributes_are_returned()
        {
            var context = new XrmFakedContext();

            //Initialize the context with a single entity
            var guid = Guid.NewGuid();
            var data = new List<Entity>() {
                new Entity("account") { Id = guid }
            }.AsQueryable();

            context.Initialize(data);

            var service = context.GetOrganizationService();

            var result = service.Retrieve("account", guid, new ColumnSet(true));
            Assert.AreEqual(result.Id, data.FirstOrDefault().Id);
            Assert.AreEqual(result.Attributes.Count, 7);
        }

        [Test]
        public void When_retrieve_is_invoked_with_an_existing_entity_and_only_one_column_only_that_one_is_retrieved()
        {
            var context = new XrmFakedContext();

            //Initialize the context with a single entity
            var guid = Guid.NewGuid();
            var entity = new Entity("account") { Id = guid };
            entity["name"] = "Test account";
            entity["createdon"] = DateTime.UtcNow;

            var data = new List<Entity>() { entity }.AsQueryable();
            context.Initialize(data);

            var service = context.GetOrganizationService();

            var result = service.Retrieve("account", guid, new ColumnSet(new string[] { "name" }));
            Assert.AreEqual(result.Id, data.FirstOrDefault().Id);
            Assert.True(result.Attributes.Count == 1);
            Assert.AreEqual(result["name"], "Test account");
        }

        [Test]
        public void When_retrieve_is_invoked_with_an_existing_entity_and_proxy_types_the_returned_entity_must_be_of_the_appropiate_subclass()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();

            //Initialize the context with a single entity
            var guid = Guid.NewGuid();
            var account = new Account() { Id = guid };
            account.Name = "Test account";

            var data = new List<Entity>() { account }.AsQueryable();
            context.Initialize(data);

            var service = context.GetOrganizationService();

            var result = service.Retrieve("account", guid, new ColumnSet(new string[] { "name" }));

            Assert.True(result is Account);
        }

        [Test]
        public void When_retrieving_entity_that_does_not_exist_with_proxy_types_entity_name_should_be_known()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Account));

            var service = context.GetOrganizationService();
            Assert.Throws<FaultException<OrganizationServiceFault>>(() => service.Retrieve("account", Guid.NewGuid(), new ColumnSet(true)));
        }

        [Test]
        public void Should_Not_Fail_On_Retrieving_Entity_With_Entity_Collection_Attributes()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            var party = new ActivityParty
            {
                PartyId = new EntityReference("systemuser", Guid.NewGuid())
            };

            var email = new Email
            {
                Id = Guid.NewGuid(),
                To = new[] { party }
            };

            service.Create(email);

            Assert.DoesNotThrow(() => service.Retrieve(email.LogicalName, email.Id, new ColumnSet(true)));
        }
    }
}