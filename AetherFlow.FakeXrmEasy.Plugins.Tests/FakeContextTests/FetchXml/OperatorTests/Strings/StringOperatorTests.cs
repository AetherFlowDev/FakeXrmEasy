using System;
using System.Collections.Generic;
using System.Text;

using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Reflection;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.FetchXml.OperatorTests.Strings
{
    public class StringOperatorTests
    {
        [Test]
        public void FetchXml_Operator_Lt_Translation()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='contactid' />
                                        <filter type='and'>
                                            <condition attribute='nickname' operator='lt' value='Bob' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var ct = new Contact();

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Criteria != null);
            Assert.AreEqual(1, query.Criteria.Conditions.Count);
            Assert.AreEqual("nickname", query.Criteria.Conditions[0].AttributeName);
            Assert.AreEqual(ConditionOperator.LessThan, query.Criteria.Conditions[0].Operator);
            Assert.AreEqual("Bob", query.Criteria.Conditions[0].Values[0]);
        }

        [Test]
        public void FetchXml_Operator_Lt_Execution()
        {
            var ctx = new XrmFakedContext();

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='nickname' />
                                        <filter type='and'>
                                            <condition attribute='nickname' operator='lt' value='C' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var ct1 = new Contact() { Id = Guid.NewGuid(), NickName = "Alice" };
            var ct2 = new Contact() { Id = Guid.NewGuid(), NickName = "Bob" };
            var ct3 = new Contact() { Id = Guid.NewGuid(), NickName = "Nati" };
            ctx.Initialize(new[] { ct1, ct2, ct3 });
            var service = ctx.GetOrganizationService();

            var collection = service.RetrieveMultiple(new FetchExpression(fetchXml));

            Assert.AreEqual(2, collection.Entities.Count);
            Assert.AreEqual("Alice", collection.Entities[0]["nickname"]);
            Assert.AreEqual("Bob", collection.Entities[1]["nickname"]);
        }

        [Test]
        public void FetchXml_Operator_Gt_Translation()
        {
            var ctx = new XrmFakedContext();
            ctx.ProxyTypesAssembly = Assembly.GetAssembly(typeof(Contact));

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='fullname' />
                                    <attribute name='contactid' />
                                        <filter type='and'>
                                            <condition attribute='nickname' operator='gt' value='Bob' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var ct = new Contact();

            var query = XrmFakedContext.TranslateFetchXmlToQueryExpression(ctx, fetchXml);

            Assert.True(query.Criteria != null);
            Assert.AreEqual(1, query.Criteria.Conditions.Count);
            Assert.AreEqual("nickname", query.Criteria.Conditions[0].AttributeName);
            Assert.AreEqual(ConditionOperator.GreaterThan, query.Criteria.Conditions[0].Operator);
            Assert.AreEqual("Bob", query.Criteria.Conditions[0].Values[0]);
        }

        [Test]
        public void FetchXml_Operator_Gt_Execution()
        {
            var ctx = new XrmFakedContext();

            var fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='contact'>
                                    <attribute name='nickname' />
                                        <filter type='and'>
                                            <condition attribute='nickname' operator='gt' value='Alice' />
                                        </filter>
                                  </entity>
                            </fetch>";

            var ct1 = new Contact() { Id = Guid.NewGuid(), NickName = "Alice" };
            var ct2 = new Contact() { Id = Guid.NewGuid(), NickName = "Bob" };
            var ct3 = new Contact() { Id = Guid.NewGuid(), NickName = "Nati" };
            ctx.Initialize(new[] { ct1, ct2, ct3 });
            var service = ctx.GetOrganizationService();

            var collection = service.RetrieveMultiple(new FetchExpression(fetchXml));

            Assert.AreEqual(2, collection.Entities.Count);
            Assert.AreEqual("Bob", collection.Entities[0]["nickname"]);
            Assert.AreEqual("Nati", collection.Entities[1]["nickname"]);
        }
    }
}
