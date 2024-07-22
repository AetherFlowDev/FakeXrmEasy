using AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.BasePluginTests
{
    public class BasePluginTests
    {
        [Test]
        public void My_First_Test()
        {
            var context = new XrmFakedContext();

            var account = new Entity("account");
            account["name"] = "Hello World";
            account["address1_postcode"] = "1234";

            ParameterCollection inputParameters = new ParameterCollection();
            inputParameters.Add("Target", account);

            var pluginCtx = context.GetDefaultPluginContext();
            pluginCtx.Stage = 20;
            pluginCtx.MessageName = "Create";
            pluginCtx.InputParameters = inputParameters;

            Assert.DoesNotThrow(() => context.ExecutePluginWithConfigurations<AccountSetTerritories>(pluginCtx, null, null));
        }
    }
}