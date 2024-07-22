using Microsoft.Crm.Sdk.Messages;
using System;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.PublishXml
{
    public class PublishXmlRequestTests
    {
        [Test]
        public void When_calling_publish_xml_exception_is_raised_if_parameter_xml_is_blank()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            var req = new PublishXmlRequest()
            {
                ParameterXml = ""
            };

            Assert.Throws<Exception>(() => service.Execute(req));
        }

        [Test]
        public void When_calling_publish_xml_no_exception_is_raised()
        {
            var ctx = new XrmFakedContext();
            var service = ctx.GetOrganizationService();

            var req = new PublishXmlRequest()
            {
                ParameterXml = "<somexml></somexml>"
            };

            Assert.DoesNotThrow(() => service.Execute(req));
        }
    }
}