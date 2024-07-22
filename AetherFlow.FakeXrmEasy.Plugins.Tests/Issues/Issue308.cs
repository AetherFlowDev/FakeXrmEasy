using System;
using System.Collections.Generic;
using System.Text;
using FakeItEasy;
using AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Issues
{
    public class Issue308
    {
        [Test]
        public void Should_post_plugin_context_to_service_endpoint()
        {
            var endpointId = Guid.NewGuid();
            var fakedContext = new XrmFakedContext();

            var fakedServiceEndpointNotificationService = fakedContext.GetFakedServiceEndpointNotificationService();

            A.CallTo(() => fakedServiceEndpointNotificationService.Execute(A<EntityReference>._, A<IExecutionContext>._))
                .Returns("response");

            var plugCtx = fakedContext.GetDefaultPluginContext();

            var fakedPlugin =
                fakedContext
                    .ExecutePluginWithConfigurations<ServiceEndpointNotificationPlugin>(plugCtx, endpointId.ToString(), null );



            A.CallTo(() => fakedServiceEndpointNotificationService.Execute(A<EntityReference>._, A<IExecutionContext>._))
                .MustHaveHappened();

        }
    }
}
