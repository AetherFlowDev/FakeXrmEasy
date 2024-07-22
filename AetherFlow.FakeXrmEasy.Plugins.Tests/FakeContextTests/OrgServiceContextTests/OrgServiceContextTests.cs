using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Linq;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.OrgServiceContextTests
{
    public class OrgServiceContextTests
    {
        [Test]
        public void When_calling_context_add_and_save_changes_entity_is_added_to_the_faked_context()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            using (var ctx = new XrmServiceContext(service))
            {
                ctx.AddObject(new Account() { Name = "Test account" });
                ctx.SaveChanges();

                var account = ctx.CreateQuery<Account>()
                            .ToList()
                            .FirstOrDefault();

                Assert.AreEqual("Test account", account.Name);
            }
        }

        [Test]
        public void When_calling_context_add_and_save_changes_returns_correct_result()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            using (var ctx = new XrmServiceContext(service))
            {
                ctx.AddObject(new Account() { Name = "Test account" });
                var result = ctx.SaveChanges();

                Assert.NotNull(result);
                Assert.IsNotEmpty(result);
                Assert.AreEqual("Create", result.First().Response.ResponseName);
                Assert.IsInstanceOf<CreateResponse>(result.First().Response);
                Assert.AreNotEqual(Guid.Empty, ((CreateResponse)result.First().Response).id);
            }
        }

        [Test]
        public void When_calling_context_add_addrelated_and_save_changes_entities_are_added_to_the_faked_context()
        {
            var context = new XrmFakedContext();

            var relationship = new XrmFakedRelationship()
            {
                IntersectEntity = "accountleads",
                Entity1Attribute = "accountid",
                Entity2Attribute = "leadid",
                Entity1LogicalName = "account",
                Entity2LogicalName = "lead"
            };
            context.AddRelationship("accountleads", relationship);

            var service = context.GetOrganizationService();

            using (var ctx = new XrmServiceContext(service))
            {
                var account = new Account() { Name = "Test account" };
                ctx.AddObject(account);

                var contact = new Lead() { FirstName = "Jane", LastName = "Doe" };
                ctx.AddRelatedObject(account, new Relationship("accountleads"), contact);
                var result = ctx.SaveChanges();

                var resultaccount = ctx.CreateQuery<Account>()
                                       .ToList()
                                       .FirstOrDefault();

                Assert.NotNull(resultaccount);
                Assert.AreEqual("Test account", resultaccount.Name);

                var reaultlead = ctx.CreateQuery<Lead>()
                                    .ToList()
                                    .FirstOrDefault();

                Assert.NotNull(reaultlead);
                Assert.AreEqual("Jane", reaultlead.FirstName);
                Assert.AreEqual("Doe", reaultlead.LastName);

                var relationshipRecords = ctx.CreateQuery("accountleads")
                                             .ToList();
                Assert.IsNotEmpty(relationshipRecords);
            }
        }
    }
}