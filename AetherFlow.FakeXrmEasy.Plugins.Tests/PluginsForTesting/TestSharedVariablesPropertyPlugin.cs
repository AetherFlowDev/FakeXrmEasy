using Microsoft.Xrm.Sdk;
using System;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting
{
    public class TestSharedVariablesPropertyPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.SharedVariables.Count == 0)
            {
                throw new Exception("Plugin context must have shared variables");
            }
        }
    }
}