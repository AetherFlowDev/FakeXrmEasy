using Crm;
using AetherFlow.FakeXrmEasy.Plugins.Extensions;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.UpsertRequestTests
{
    public class UpsertRequestTests
    {
        [Test]
        public void Upsert_Creates_Record_When_It_Does_Not_Exist()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = "FakeXrm",
                LastName = "Easy"
            };

            var request = new UpsertRequest()
            {
                Target = contact
            };

            var response = (UpsertResponse)service.Execute(request);

            var contactCreated = context.CreateQuery<Contact>().FirstOrDefault();

            Assert.AreEqual(true, response.RecordCreated);
            Assert.NotNull(contactCreated);
        }

        [Test]
        public void Upsert_Updates_Record_When_It_Exists()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = "FakeXrm"
            };
            context.Initialize(new[] { contact });

            contact = new Contact()
            {
                Id = contact.Id,
                FirstName = "FakeXrm2",
                LastName = "Easy"
            };

            var request = new UpsertRequest()
            {
                Target = contact
            };


            var response = (UpsertResponse)service.Execute(request);
            var contactUpdated = context.CreateQuery<Contact>().FirstOrDefault();

            Assert.AreEqual(false, response.RecordCreated);
            Assert.AreEqual("FakeXrm2", contactUpdated.FirstName);
        }

        [Test]
        public void Upsert_Creates_Record_When_It_Does_Not_Exist_Using_Alternate_Key()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            context.InitializeMetadata(Assembly.GetExecutingAssembly());
            var service = context.GetOrganizationService();

            var metadata = context.GetEntityMetadataByName("contact");
            metadata.SetFieldValue("_keys", new EntityKeyMetadata[]
            {
                new EntityKeyMetadata()
                {
                    KeyAttributes = new string[]{"firstname"}
                }
            });
            context.SetEntityMetadata(metadata);
            var contact = new Contact()
            {
                FirstName = "FakeXrm",
                LastName = "Easy"
            };
            contact.KeyAttributes.Add("firstname", contact.FirstName);

            var request = new UpsertRequest()
            {
                Target = contact
            };

            var response = (UpsertResponse)service.Execute(request);

            Assert.AreEqual(true, response.RecordCreated);
        }

        [Test]
        public void Upsert_Updates_Record_When_It_Exists_Using_Alternate_Key()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            context.InitializeMetadata(Assembly.GetExecutingAssembly());
            var service = context.GetOrganizationService();


            var metadata = context.GetEntityMetadataByName("contact");
            metadata.SetFieldValue("_keys", new EntityKeyMetadata[]
            {
                new EntityKeyMetadata()
                {
                    KeyAttributes = new string[]{"firstname"}
                }
            });
            context.SetEntityMetadata(metadata);

            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                FirstName = "FakeXrm",
                LastName = "Easy"
            };
            context.Initialize(new[] { contact });

            contact = new Contact()
            {
                FirstName = "FakeXrm2",
                LastName = "Easy2"
            };

            contact.KeyAttributes.Add("firstname", "FakeXrm");

            var request = new UpsertRequest()
            {
                Target = contact
            };

            var response = (UpsertResponse)service.Execute(request);

            Assert.AreEqual(false, response.RecordCreated);
        }
    }
}
