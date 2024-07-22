using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.CloseQuoteRequestTests
{
    public class CloseQuoteRequestTests
    {
        [Test]
        public void When_can_execute_is_called_with_an_invalid_request_result_is_false()
        {
            var executor = new CloseQuoteRequestExecutor();
            var anotherRequest = new RetrieveMultipleRequest();
            Assert.False(executor.CanExecute(anotherRequest));
        }

        [Test]
        public void Should_Change_Status_When_Closing()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var quote = new Entity
            {
                LogicalName = "quote",
                Id = Guid.NewGuid(),
                Attributes = new AttributeCollection
                {
                    {"statuscode", new OptionSetValue(0)}
                }
            };

            context.Initialize(new[]
            {
                quote
            });

            var executor = new CloseQuoteRequestExecutor();

            var req = new CloseQuoteRequest
            {
                QuoteClose = new Entity
                {
                    Attributes = new AttributeCollection
                    {
                        { "quoteid", quote.ToEntityReference() }
                    }
                },
                Status = new OptionSetValue(1)
            };

            executor.Execute(req, context);

            quote = service.Retrieve("quote", quote.Id, new ColumnSet(true));

            Assert.AreEqual(new OptionSetValue(1), quote.GetAttributeValue<OptionSetValue>("statuscode"));
        }
    }
}