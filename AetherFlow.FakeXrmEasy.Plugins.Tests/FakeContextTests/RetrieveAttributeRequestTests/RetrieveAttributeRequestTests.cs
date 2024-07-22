using NUnit.Framework;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using AetherFlow.FakeXrmEasy.Plugins.Extensions;
using System;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.RetrieveAttributeRequestTests
{
    public class RetrieveAttributeTests
    {
        [Test]
        public static void When_retrieve_attribute_request_is_called_correctly_attribute_is_returned()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            var nameAttribute = new StringAttributeMetadata()
            {
                LogicalName = "name",
                RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired)
            };
            entityMetadata.SetAttributeCollection(new[] { nameAttribute });

            ctx.InitializeMetadata(entityMetadata);

            RetrieveAttributeRequest req = new RetrieveAttributeRequest()
            {
                EntityLogicalName = "account",
                LogicalName = "name"
            };

            var response = service.Execute(req) as RetrieveAttributeResponse;
            Assert.NotNull(response.AttributeMetadata);
            Assert.AreEqual(AttributeRequiredLevel.ApplicationRequired, response.AttributeMetadata.RequiredLevel.Value);
            Assert.AreEqual("name", response.AttributeMetadata.LogicalName);
        }

        [Test]
        public static void When_retrieve_attribute_request_is_without_entity_logical_name_exception_is_raised()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            RetrieveAttributeRequest req = new RetrieveAttributeRequest()
            {
                EntityLogicalName = null,
                LogicalName = "name"
            };

            Assert.Throws<Exception>(() => service.Execute(req));
        }

        [Test]
        public static void When_retrieve_attribute_request_is_without_logical_name_exception_is_raised()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            ctx.InitializeMetadata(entityMetadata);

            RetrieveAttributeRequest req = new RetrieveAttributeRequest()
            {
                EntityLogicalName = "account",
                LogicalName = null
            };

            Assert.Throws<Exception>(() => service.Execute(req));
        }

        [Test]
        public static void When_retrieve_attribute_request_is_without_being_initialised_exception_is_raised()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            RetrieveAttributeRequest req = new RetrieveAttributeRequest()
            {
                EntityLogicalName = "account",
                LogicalName = "name"
            };

            Assert.Throws<Exception>(() => service.Execute(req));
        }

        [Test]
        public static void When_retrieve_attribute_request_is_initialised_but_attribute_doesnt_exists_exception_is_raised()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            ctx.InitializeMetadata(entityMetadata);

            RetrieveAttributeRequest req = new RetrieveAttributeRequest()
            {
                EntityLogicalName = "account",
                LogicalName = "name"
            };

            Assert.Throws<Exception>(() => service.Execute(req));
        }


    }
}
