﻿using AetherFlow.FakeXrmEasy.Plugins.Tests.CodeActivitiesForTesting;
using AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Tracing
{
    public class XrmFakeTracingExample
    {
        [Test]
        public void Example_about_retrieving_traces_written_by_plugin()
        {
            var fakedContext = new XrmFakedContext();

            var guid1 = Guid.NewGuid();
            var target = new Entity("account") { Id = guid1 };

            //Execute our plugin against a target that doesn't contains the accountnumber attribute
            var fakedPlugin = fakedContext.ExecutePluginWithTarget<AccountNumberPlugin>(target);

            //Get tracing service
            var fakeTracingService = fakedContext.GetFakeTracingService();
            var log = fakeTracingService.DumpTrace();

            //Assert that the target contains a new attribute
            Assert.AreEqual(log, "Contains target\r\nIs Account\r\n");
        }

        [Test]
        public void Should_retrieve_traces_from_a_code_activity()
        {
            var fakedContext = new XrmFakedContext();

            //Inputs
            var inputs = new Dictionary<string, object>() {
                { "firstSummand", 2 },
                { "secondSummand", 3 }
            };

            var result = fakedContext.ExecuteCodeActivity<AddActivity>(inputs);

            //Get tracing service
            var fakeTracingService = fakedContext.GetFakeTracingService();
            var log = fakeTracingService.DumpTrace();

            //Assert that the target contains a new attribute
            Assert.AreEqual(log, "{ title: \"Complex message\", description: \"That could brake Formatter if not treated carefully\" }" + System.Environment.NewLine);
        }

        [Test]
        public void The_TracingService_Should_Be_Retrievable_Without_Calling_Execute_Before()
        {
            var fakedContext = new XrmFakedContext();

            //Get tracing service
            var fakeTracingService = fakedContext.GetFakeTracingService();

            Assert.NotNull(fakeTracingService);
        }

        [Test]
        public void Retrieving_The_TracingService_Twice_Should_Return_The_Same_Instance()
        {
            var fakedContext = new XrmFakedContext();

            //Get tracing service
            var fakeTracingService1 = fakedContext.GetFakeTracingService();
            fakeTracingService1.Trace("foobar");

            var fakeTracingService2 = fakedContext.GetFakeTracingService();

            Assert.NotNull(fakeTracingService1);
            Assert.NotNull(fakeTracingService2);

            Assert.IsTrue(fakeTracingService2.DumpTrace().Contains("foobar"));
        }
    }
}