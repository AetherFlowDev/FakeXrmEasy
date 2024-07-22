using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using AetherFlow.FakeXrmEasy.Plugins.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Extensions
{
    public class QueryExpressionExtensionsTests
    {
        [Test]
        public void TestClone()
        {
            QueryExpression query = new QueryExpression("entity");
            LinkEntity link = new LinkEntity("entity", "second", "secondid", "secondid", JoinOperator.Inner);
            link.EntityAlias = "second";
            link.LinkCriteria.AddCondition("filter", ConditionOperator.Equal, true);
            query.LinkEntities.Add(link);

            QueryExpression cloned = query.Clone();
            cloned.LinkEntities[0].LinkCriteria.Conditions[0].AttributeName = "otherfield";

            cloned.LinkEntities[0].LinkCriteria.Conditions[0].AttributeName = "link.field";
            Assert.AreEqual("entity", query.EntityName);
            Assert.AreEqual("filter", query.LinkEntities[0].LinkCriteria.Conditions[0].AttributeName );
        }
    }
}
