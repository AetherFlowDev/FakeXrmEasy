using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.SendEmailRequestTests
{
    public class SendEmailRequestTests
    {
        [Test]
        public void When_SendEmailRequest_call_statecode_is_Completed_and_statuscode_is_Sent()
        {
            var context = new XrmFakedContext();
            context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();
            var service = context.GetOrganizationService();

            var email = new Crm.Email()
            {
                Id = Guid.NewGuid()
            };
            var emailId = service.Create(email);

            var request = new SendEmailRequest
            {
                EmailId = emailId,
                TrackingToken = "TrackingToken",
                IssueSend = true
            };
            var response = (SendEmailResponse)service.Execute(request);

            var entity = service.Retrieve("email", emailId, new ColumnSet("statecode", "statuscode"));
            Assert.AreEqual(1, entity?.GetAttributeValue<OptionSetValue>("statecode")?.Value);
            Assert.AreEqual(3, entity?.GetAttributeValue<OptionSetValue>("statuscode")?.Value);
        }
    }
}
