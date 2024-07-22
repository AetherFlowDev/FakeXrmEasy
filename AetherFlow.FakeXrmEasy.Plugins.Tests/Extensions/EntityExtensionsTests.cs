using AetherFlow.FakeXrmEasy.Plugins.Extensions;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Extensions
{
    public class EntityExtensionsTests
    {
        [Test]
        public void SetValueIfEmpty_should_not_override_existing_values()
        {
            var e = new Entity("account");
            e["name"] = "Some name";

            e.SetValueIfEmpty("name", "another name");
            Assert.AreEqual(e["name"].ToString(), "Some name");
        }

        [Test]
        public void SetValueIfEmpty_should_override_if_null()
        {
            var e = new Entity("account");
            e["name"] = null;

            e.SetValueIfEmpty("name", "new name");
            Assert.AreEqual(e["name"].ToString(), "new name");
        }

        [Test]
        public void SetValueIfEmpty_should_override_if_doesnt_contains_key()
        {
            var e = new Entity("account");

            e.SetValueIfEmpty("name", "new name");
            Assert.AreEqual(e["name"].ToString(), "new name");
        }

        [Test]
        public void CloneAttribute_should_support_enumerable_of_entity()
        {
            IEnumerable<Entity> activityParties = Enumerable.Range(1, 2).Select(index =>
            {
                var activityParty = new Entity("activityparty");
                activityParty["partyid"] = new EntityReference("contact", Guid.NewGuid());

                return activityParty;
            }).ToArray();

            var e = new Entity("email");
            e["to"] = activityParties;

            var clone = EntityExtensions.CloneAttribute(e["to"]) as IEnumerable<Entity>;

            Assert.NotNull(clone);
            Assert.AreEqual(2, clone.Count());

            // RA TODO: This
            //Assert.AreEqual(activityParties, clone, new ActivityPartyComparer());
        }

         //Enity images aren't supported in versions prior to 2013, so no need to support byte arrays as attributes

        [Test]
        public void CloneAttribute_should_support_byte_array()
        {
            var random = new Random();
            byte[] image = new byte[2000];
            random.NextBytes(image);

            var e = new Entity("account");
            e["entityimage"] = image;

            var clone = EntityExtensions.CloneAttribute(e["entityimage"]) as byte[];

            Assert.NotNull(clone);
            Assert.AreEqual(2000, clone.Length);
            Assert.AreEqual(image, clone);
        }

        #region Private Helper Classes

        private class ActivityPartyComparer : EqualityComparer<Entity>
        {
            public override bool Equals(Entity x, Entity y)
            {
                var partyId_X = x["partyid"] as EntityReference;
                var partyId_Y = y["partyid"] as EntityReference;

                if (partyId_X?.LogicalName != partyId_Y?.LogicalName) return false;
                if (partyId_X?.Id != partyId_Y?.Id) return false;

                return true;
            }

            public override int GetHashCode(Entity obj)
            {
                return obj.LogicalName.GetHashCode() * obj["partyid"].GetHashCode();
            }
        }

        #endregion Private Helper Classes
    }
}