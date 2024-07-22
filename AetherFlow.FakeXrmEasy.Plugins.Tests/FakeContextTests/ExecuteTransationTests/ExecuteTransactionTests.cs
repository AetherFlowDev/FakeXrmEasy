using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.ExecuteTransationTests
{
    public class ExecuteTransactionTests
    {
        [Test]
        public void When_can_execute_is_called_with_an_invalid_request_result_is_false()
        {
            var executor = new ExecuteTransactionExecutor();
            var anotherRequest = new RetrieveMultipleRequest();
            Assert.False(executor.CanExecute(anotherRequest));
        }

        [Test]
        public void When_execute_is_called_all_requests_are_executed()
        {
            var context = new XrmFakedContext();
            var executor = new ExecuteTransactionExecutor();
            var req = new ExecuteTransactionRequest()
            {
                Requests = new OrganizationRequestCollection()
                {
                    new CreateRequest() { Target = new Entity("contact") },
                    new CreateRequest() { Target = new Entity("contact") },
                    new CreateRequest() { Target = new Entity("contact") }
                }
            };

            var response = executor.Execute(req, context) as ExecuteTransactionResponse;
            var contacts = context.CreateQuery("contact").ToList();
            Assert.AreEqual(0, response.Responses.Count);
            Assert.AreEqual(3, contacts.Count);
        }

        [Test]
        public void When_execute_is_called_all_requests_are_executed_with_responses()
        {
            var context = new XrmFakedContext();
            var executor = new ExecuteTransactionExecutor();
            var req = new ExecuteTransactionRequest()
            {
                ReturnResponses = true,
                Requests = new OrganizationRequestCollection()
                {
                    new CreateRequest() { Target = new Entity("contact") },
                    new CreateRequest() { Target = new Entity("contact") },
                    new CreateRequest() { Target = new Entity("contact") }
                }
            };

            var response = executor.Execute(req, context) as ExecuteTransactionResponse;
            var contacts = context.CreateQuery("contact").ToList();
            Assert.AreEqual(3, response.Responses.Count);
            Assert.AreEqual(3, contacts.Count);
        }
    }
}