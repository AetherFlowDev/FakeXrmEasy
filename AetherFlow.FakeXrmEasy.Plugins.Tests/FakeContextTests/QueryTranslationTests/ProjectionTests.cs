using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.QueryTranslationTests
{
    public class ProjectionTests
    {
        private XrmFakedContext FakeContext;
        private IOrganizationService Service;
        private Account Account;

        [SetUp]
        public void SetUp()
        {
            FakeContext = new XrmFakedContext();
            Service = FakeContext.GetOrganizationService();
            Account = new Account()
            {
                Id = Guid.NewGuid(),
                Name = "Some name"
            };
        }

        [Test]
        public void Should_return_primary_key_attribute_even_if_not_specified_in_column_set()
        {
            FakeContext.Initialize(Account);
            var account = Service.Retrieve(Account.EntityLogicalName, Account.Id, new ColumnSet(new string[] { "name" }));
            Assert.True(account.Attributes.ContainsKey("accountid"));
        }

        [Test]
        public void Should_return_primary_key_attribute_when_retrieving_using_all_columns()
        {
            FakeContext.Initialize(Account);
            var account = Service.Retrieve(Account.EntityLogicalName, Account.Id, new ColumnSet(true));
            Assert.True(account.Attributes.ContainsKey("accountid"));
        }
    }
}
