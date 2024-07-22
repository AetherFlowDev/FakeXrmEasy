using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using AetherFlow.FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests
{
    public class TestGenericMessageExecutors
    {
        [Test]
        public void TestGenericMessage()
        {
            XrmFakedContext context = new XrmFakedContext();
            context.AddGenericFakeMessageExecutor("new_TestAction", new FakeMessageExecutor());
            IOrganizationService service = context.GetOrganizationService();
            OrganizationRequest request = new OrganizationRequest("new_TestAction");
            request["input"] = "testinput";
            OrganizationResponse response = service.Execute(request);
            Assert.AreEqual("testinput", response["output"]);
        }

        [Test]
        public void TestGenericMessageRemoval()
        {

            XrmFakedContext context = new XrmFakedContext();
            context.AddGenericFakeMessageExecutor("new_TestAction", new FakeMessageExecutor());
            IOrganizationService service = context.GetOrganizationService();
            OrganizationRequest request = new OrganizationRequest("new_TestAction");
            request["input"] = "testinput";
            OrganizationResponse response = service.Execute(request);
            Assert.AreEqual("testinput", response["output"]);
            context.RemoveGenericFakeMessageExecutor("new_TestAction");
            Assert.Throws(typeof(FakeXrmEasy.Plugins.PullRequestException), () => service.Execute(request));
        }
    }

    class FakeMessageExecutor : FakeMessageExecutors.IFakeMessageExecutor
    {
        public bool CanExecute(OrganizationRequest request)
        {
            return request.RequestName == "new_TestAction";
        }

        public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
        {
            OrganizationResponse response = new OrganizationResponse();
            response["output"] = request["input"];
            return response;
        }

        public Type GetResponsibleRequestType()
        {
            throw new NotImplementedException();
        }
    }
}
