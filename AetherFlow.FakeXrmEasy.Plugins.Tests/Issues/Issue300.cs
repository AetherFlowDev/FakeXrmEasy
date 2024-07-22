using Crm;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Issues
{
    public class Issue300
    {
        [Test]
        public void Should_Create_Account_With_Local_DateTime_And_Retrieve_Utc()
        {
            DateTime dateTimeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            Account account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "Goggle ltd",
                LastUsedInCampaign = dateTimeNow
            };

            var context = new XrmFakedContext();

            context.GetOrganizationService().Create(account);

            var retrievedAccount = context.CreateQuery<Account>().SingleOrDefault(p => p.Id == account.Id);

            Assert.NotNull(retrievedAccount);
            Assert.True(retrievedAccount.LastUsedInCampaign.HasValue);
            Assert.AreEqual(DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc).Kind, retrievedAccount.LastUsedInCampaign.Value.Kind);
        }

        [Test]
        public void Should_Initialize_Account_With_Local_DateTime_And_Retrieve_Utc()
        {
            DateTime dateTimeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            Account account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "Goggle ltd",
                LastUsedInCampaign = dateTimeNow
            };

            var context = new XrmFakedContext();

            context.Initialize(new List<Entity> { account });

            var retrievedAccount = context.CreateQuery<Account>().SingleOrDefault(p => p.Id == account.Id);

            Assert.NotNull(retrievedAccount);
            Assert.True(retrievedAccount.LastUsedInCampaign.HasValue);
            Assert.AreEqual(DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc).Kind, retrievedAccount.LastUsedInCampaign.Value.Kind);
        }

        [Test]
        public void Should_Update_Account_With_Local_DateTime_And_Retrieve_Utc()
        {
            DateTime dateTimeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            Account account = new Account
            {
                Id = Guid.NewGuid(),
                Name = "Goggle ltd",
                LastUsedInCampaign = dateTimeNow
            };

            var context = new XrmFakedContext();
            var orgService = context.GetOrganizationService();
            orgService.Create(account);

            account.LastUsedInCampaign = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
            orgService.Update(account);

            var retrievedAccount = context.CreateQuery<Account>().SingleOrDefault(p => p.Id == account.Id);

            Assert.NotNull(retrievedAccount);
            Assert.True(retrievedAccount.LastUsedInCampaign.HasValue);
            Assert.AreEqual(DateTime.SpecifyKind(dateTimeNow, DateTimeKind.Utc).Kind, retrievedAccount.LastUsedInCampaign.Value.Kind);
        }
    }
}