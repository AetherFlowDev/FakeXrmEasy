using Crm;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.InitializeFromRequestTests
{
    public class InitializeFromRequestTests
    {
        [Test]
        public void When_Calling_InitializeFromRequest_Should_Return_InitializeFromResponse()
        {
            var ctx = new XrmFakedContext
            {
                ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact))
            };
            var service = ctx.GetOrganizationService();
            var lead = new Entity
            {
                LogicalName = "Lead",
                Id = Guid.NewGuid(),
                ["FirstName"] = "Arjen",
                ["LastName"] = "Stortelder"
            };

            ctx.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference("Lead", lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = "Contact",
                TargetFieldType = TargetFieldType.All
            };

            Assert.IsInstanceOf<InitializeFromResponse>(service.Execute(req));
        }

        [Test]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_As_Entity_Of_Type_TargetEntityName()
        {
            var ctx = new XrmFakedContext
            {
                ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact))
            };

            var service = ctx.GetOrganizationService();

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            ctx.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)service.Execute(req);
            Assert.IsInstanceOf<Contact>(result.Entity);
            Assert.AreEqual(Contact.EntityLogicalName, result.Entity.LogicalName);
        }

        [Test]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_With_Attributes_Set_From_The_Mapping()
        {
            var ctx = new XrmFakedContext
            {
                ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact))
            };

            var service = ctx.GetOrganizationService();

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            ctx.Initialize(new List<Entity> { lead });
            ctx.AddAttributeMapping(Lead.EntityLogicalName, "firstname", Contact.EntityLogicalName, "firstname");

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)service.Execute(req);
            var contact = result.Entity.ToEntity<Contact>();
            Assert.AreEqual("Arjen", contact.FirstName);
            Assert.AreEqual(null, contact.LastName);
        }

        [Test]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_With_EntityReference()
        {
            var ctx = new XrmFakedContext
            {
                ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact))
            };

            var service = ctx.GetOrganizationService();

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            ctx.Initialize(new List<Entity> { lead });
            ctx.AddAttributeMapping(Lead.EntityLogicalName, "leadid", Contact.EntityLogicalName, "originatingleadid");

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)service.Execute(req);
            var contact = result.Entity;
            var originatingleadid = contact["originatingleadid"];
            Assert.IsInstanceOf<EntityReference>(originatingleadid);
        }

        [Test]
        public void When_Calling_InitializeFromRequest_Should_Return_Entity_Without_Id()
        {
            var ctx = new XrmFakedContext
            {
                ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact))
            };

            var service = ctx.GetOrganizationService();

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = "Arjen",
                LastName = "Stortelder"
            };

            ctx.Initialize(new List<Entity> { lead });
            ctx.AddAttributeMapping(Lead.EntityLogicalName, "firstname", Contact.EntityLogicalName, "firstname");

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)service.Execute(req);
            var contact = result.Entity.ToEntity<Contact>();
            Assert.AreEqual(Guid.Empty, contact.Id);
        }

        [Test]
        public void When_Calling_InitializeFromRequest_With_Early_Bound_Classes_Should_Return_Early_Bound_Entity()
        {
            var ctx = new XrmFakedContext();

            var service = ctx.GetOrganizationService();

            var lead = new Lead
            {
                Id = Guid.NewGuid()
            };

            // This will set ProxyTypesAssembly = true
            ctx.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference(Lead.EntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = Contact.EntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)service.Execute(req);

            Assert.IsInstanceOf<Contact>(result.Entity);
        }

        [Test]
        public void When_Calling_InitializeFromRequest_With_Late_Bound_Classes_Should_Return_Late_Bound_Entity()
        {
            var ctx = new XrmFakedContext();
            string sourceEntityLogicalName = "lead";
            string targetEntityLogicalName = "contact";

            var service = ctx.GetOrganizationService();

            var lead = new Entity
            {
                Id = Guid.NewGuid(),
                LogicalName = sourceEntityLogicalName
            };

            ctx.Initialize(new List<Entity> { lead });

            var entityReference = new EntityReference(sourceEntityLogicalName, lead.Id);
            var req = new InitializeFromRequest
            {
                EntityMoniker = entityReference,
                TargetEntityName = targetEntityLogicalName,
                TargetFieldType = TargetFieldType.All
            };

            var result = (InitializeFromResponse)service.Execute(req);

            Assert.IsInstanceOf<Entity>(result.Entity);
            Assert.AreEqual(targetEntityLogicalName, result.Entity.LogicalName);
        }
    }
}