﻿using Microsoft.Xrm.Sdk;
using System;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting
{
    public class ExceptionLoverPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            throw new InvalidPluginExecutionException("This is an amazing exception raised from a plugin!");
        }
    }
}