using Crm;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.QualifyLeadTests
{
    public class QualifyLeadTests
    {
        [Test]
        public void Check_if_Account_was_created_after_sending_request()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var lead = new Lead()
            {
                Id = Guid.NewGuid()
            };
            context.Initialize(new[] { lead });

            var request = new QualifyLeadRequest()
            {
                CreateAccount = true,
                CreateContact = false,
                CreateOpportunity = false,
                LeadId = lead.ToEntityReference(),
                Status = new OptionSetValue((int)LeadState.Qualified)
            };

            service.Execute(request);

            var account = (from acc in context.CreateQuery<Account>()
                           where acc.OriginatingLeadId.Id == lead.Id
                           select acc).First();

            Assert.NotNull(account);
        }

        [Test]
        public void Check_if_Contact_was_created_after_sending_request()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var lead = new Lead()
            {
                Id = Guid.NewGuid()
            };
            context.Initialize(new[] { lead });

            var request = new QualifyLeadRequest()
            {
                CreateAccount = false,
                CreateContact = true,
                CreateOpportunity = false,
                LeadId = lead.ToEntityReference(),
                Status = new OptionSetValue((int)LeadState.Qualified)
            };

            service.Execute(request);

            var contact = (from con in context.CreateQuery<Contact>()
                           where con.OriginatingLeadId.Id == lead.Id
                           select con).First();

            Assert.NotNull(contact);
        }

        [Test]
        public void Check_if_Opportunity_was_created_after_sending_request()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var lead = new Lead()
            {
                Id = Guid.NewGuid()
            };
            context.Initialize(new[] { lead });

            var request = new QualifyLeadRequest()
            {
                CreateAccount = false,
                CreateContact = false,
                CreateOpportunity = true,
                LeadId = lead.ToEntityReference(),
                Status = new OptionSetValue((int)LeadState.Qualified)
            };

            service.Execute(request);

            var opportunity = (from opp in context.CreateQuery<Opportunity>()
                               where opp.OriginatingLeadId.Id == lead.Id
                               select opp).First();

            Assert.NotNull(opportunity);
        }

        [Test]
        public void Check_if_Account_was_associated_with_Opportunity()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var account = new Account()
            {
                Id = Guid.NewGuid()
            };
            var lead = new Lead()
            {
                Id = Guid.NewGuid()
            };
            context.Initialize(new List<Entity>() { account, lead });

            var request = new QualifyLeadRequest()
            {
                CreateAccount = false,
                CreateContact = false,
                CreateOpportunity = true,
                LeadId = lead.ToEntityReference(),
                Status = new OptionSetValue((int)LeadState.Qualified),
                OpportunityCustomerId = account.ToEntityReference()
            };

            service.Execute(request);

            var opportunity = (from opp in context.CreateQuery<Opportunity>()
                               where opp.OriginatingLeadId.Id == lead.Id
                               select opp).First();

            Assert.NotNull(opportunity.CustomerId);
        }

        [Test]
        public void Status_of_qualified_Lead_should_be_qualified()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var lead = new Lead()
            {
                Id = Guid.NewGuid()
            };
            context.Initialize(new List<Entity>() { lead });

            var request = new QualifyLeadRequest()
            {
                CreateAccount = false,
                CreateContact = false,
                CreateOpportunity = false,
                LeadId = lead.ToEntityReference(),
                Status = new OptionSetValue((int)LeadState.Qualified)
            };

            service.Execute(request);

            var qualifiedLead = (from l in context.CreateQuery<Lead>()
                               where l.Id == lead.Id
                               select l).Single();

            Assert.AreEqual((int)LeadState.Qualified, qualifiedLead.StatusCode.Value);
        }
    }
}
