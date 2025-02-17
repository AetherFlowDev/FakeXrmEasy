﻿using Crm;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.LoseOpportunityTests
{
    public class LoseOpportunityTests
    {
        [Test]
        public void Check_if_Opportunity_status_is_Lose_after_set()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var opportunity = new Opportunity()
            {
                Id = Guid.NewGuid()
            };
            context.Initialize(new[] { opportunity });

            var request = new LoseOpportunityRequest()
            {
                OpportunityClose = new OpportunityClose
                {
                    OpportunityId = new EntityReference(Opportunity.EntityLogicalName, opportunity.Id)
                },
                Status = new OptionSetValue((int)OpportunityState.Lost)
            };

            service.Execute(request);

            var opp = (from op in context.CreateQuery<Opportunity>()
                       where op.Id == opportunity.Id
                       select op).FirstOrDefault();

            Assert.AreEqual(opp.StatusCode.Value, (int)OpportunityState.Lost);
        }
    }
}