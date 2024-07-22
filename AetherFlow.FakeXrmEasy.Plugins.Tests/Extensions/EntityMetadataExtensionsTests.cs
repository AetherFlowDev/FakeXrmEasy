using AetherFlow.FakeXrmEasy.Plugins.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using System.Collections.Generic;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Extensions
{
    public class EntityMetadataExtensionsTests
    {
        [Test]
        public void SetSealedPropertyValue_should_update_entity_metadata()
        {
            var entityMetadata = new EntityMetadata();
            entityMetadata.SetSealedPropertyValue("PrimaryNameAttribute", "Account");
            Assert.AreEqual("Account", entityMetadata.PrimaryNameAttribute);
        }

        [Test]
        public void SetSealedPropertyValue_should_update_attribute_metadata()
        {
            var fakeAttribute = new StringAttributeMetadata();

            fakeAttribute.SetSealedPropertyValue("IsManaged", false);
            Assert.AreEqual(false, fakeAttribute.IsManaged);
        }

        [Test]
        public void SetSealedPropertyValue_should_update_manytomanyrelationship_metadata()
        {
            var fakeRelationship = new ManyToManyRelationshipMetadata();

            fakeRelationship.SetSealedPropertyValue("Entity2LogicalName", "role");
            Assert.AreEqual("role", fakeRelationship.Entity2LogicalName);
        }

        [Test]
        public void SetSealedPropertyValue_should_update_onetomanyrelationship_metadata()
        {
            var fakeRelationship = new OneToManyRelationshipMetadata();

            fakeRelationship.SetSealedPropertyValue("ReferencingEntity", "account");
            Assert.AreEqual("account", fakeRelationship.ReferencingEntity);
        }

        [Test]
        public void SetAttributeCollection_should_update_attributes()
        {
            var entityMetadata = new EntityMetadata();
            var fakeAttribute = new StringAttributeMetadata() { LogicalName = "name" };
            var fakeAttribute2 = new StringAttributeMetadata() { LogicalName = "name2" };


            entityMetadata.SetAttributeCollection(new List<AttributeMetadata>() { fakeAttribute, fakeAttribute2 });
            Assert.AreEqual(2, entityMetadata.Attributes.Length);
            Assert.AreEqual("name", entityMetadata.Attributes[0].LogicalName);
            Assert.AreEqual("name2", entityMetadata.Attributes[1].LogicalName);
        }

        [Test]
        public void SetAttribute_should_not_throw_error()
        {
            var entityMetadata = new EntityMetadata();
            var fakeAttribute = new StringAttributeMetadata() { LogicalName = "name" };


            entityMetadata.SetAttribute(fakeAttribute);
            Assert.AreEqual(1, entityMetadata.Attributes.Length);
            Assert.AreEqual("name", entityMetadata.Attributes[0].LogicalName);
        }
    }
}
