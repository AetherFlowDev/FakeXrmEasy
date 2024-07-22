using Crm;
using FakeItEasy;
using AetherFlow.FakeXrmEasy.Plugins.Extensions;
using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.RetrieveVersionRequestTests
{
    public class RetrieveVersionRequestTests
    {
        [Test]
        public void AddsFakeVersionRequest()
        {
            var fakedContext = new XrmFakedContext();
            var fakedService = fakedContext.GetOrganizationService();

            var fakeVersionExecutor = new RetrieveVersionRequestExecutor();
            fakedContext.AddFakeMessageExecutor<RetrieveVersionRequest>(fakeVersionExecutor);

            var fakeVersionRequest = new RetrieveVersionRequest();
            var result = (RetrieveVersionResponse)fakedService.Execute(fakeVersionRequest);
            var version = result.Version;
            var versionComponents = version.Split('.');

            var majorVersion = versionComponents[0];
            var minorVersion = versionComponents[1];


            Assert.True(int.Parse(majorVersion) >= 9);

        }
    }
}
