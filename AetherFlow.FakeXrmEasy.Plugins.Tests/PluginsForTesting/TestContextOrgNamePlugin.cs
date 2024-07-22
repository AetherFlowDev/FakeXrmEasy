using Microsoft.Xrm.Sdk;
using System;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting
{
    public class TestContextOrgNamePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            context.OutputParameters.Add("OrgName", context.OrganizationName);
        }
    }
}