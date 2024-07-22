﻿using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests
{
    public class FakeContextMockTests
    {
        [Test]
        public void Should_Execute_Mock_For_OrganizationRequests()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var e = new Entity("Contact") { Id = Guid.NewGuid() };
            context.Initialize(new[] { e });
            context.AddExecutionMock<RetrieveEntityRequest>(RetrieveEntityMock);

            var inputs = new ParameterCollection
            {
                {"Target", e }
            };

            context.ExecutePluginWith<CustomMockPlugin>(inputs, new ParameterCollection(), new EntityImageCollection(), new EntityImageCollection());

            Assert.AreEqual("Successful", (string)e["response"]);
            Assert.DoesNotThrow(() => context.RemoveExecutionMock<RetrieveEntityRequest>());
        }

        public OrganizationResponse RetrieveEntityMock(OrganizationRequest req)
        {
            return new RetrieveEntityResponse { ResponseName = "Successful" };
        }

        public OrganizationResponse AnotherRetrieveEntityMock(OrganizationRequest req)
        {
            return new RetrieveEntityResponse { ResponseName = "Another" };
        }

        [Test]
        public void Should_Override_Execution_Mock()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var e = new Entity("Contact") { Id = Guid.NewGuid() };
            context.Initialize(new[] { e });
            context.AddExecutionMock<RetrieveEntityRequest>(RetrieveEntityMock);
            context.AddExecutionMock<RetrieveEntityRequest>(AnotherRetrieveEntityMock);

            var inputs = new ParameterCollection
            {
                {"Target", e }
            };

            context.ExecutePluginWith<CustomMockPlugin>(inputs, new ParameterCollection(), new EntityImageCollection(), new EntityImageCollection());

            Assert.AreEqual("Another", (string)e["response"]);
            Assert.DoesNotThrow(() => context.RemoveExecutionMock<RetrieveEntityRequest>());
        }

        [Test]
        public void Should_Override_FakeMessageExecutor()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var e = new Entity("Contact") { Id = Guid.NewGuid() };
            context.Initialize(new[] { e });
            context.AddFakeMessageExecutor<RetrieveEntityRequest>(new FakeRetrieveEntityRequestExecutor());

            var inputs = new ParameterCollection
            {
                {"Target", e }
            };

            context.ExecutePluginWith<CustomMockPlugin>(inputs, new ParameterCollection(), new EntityImageCollection(), new EntityImageCollection());

            Assert.AreEqual("Successful", (string)e["response"]);
            Assert.DoesNotThrow(() => context.RemoveFakeMessageExecutor<RetrieveEntityRequest>());
        }

        protected class FakeRetrieveEntityRequestExecutor : IFakeMessageExecutor
        {
            public bool CanExecute(OrganizationRequest request)
            {
                return request is RetrieveEntityRequest;
            }

            public Type GetResponsibleRequestType()
            {
                return typeof(RetrieveEntityRequest);
            }

            public OrganizationResponse Execute(OrganizationRequest request, XrmFakedContext ctx)
            {
                return new RetrieveEntityResponse { ResponseName = "Successful" };
            }
        }
    }
}