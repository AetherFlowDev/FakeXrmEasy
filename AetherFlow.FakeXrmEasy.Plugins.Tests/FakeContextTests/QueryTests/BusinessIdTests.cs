using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.QueryTests
{
    public class EqualBusinessIdTests
    {
        [Test]
        public void FetchXml_Operator_EqualBusinessId_Translation()
        {
            XrmFakedContext _context = new XrmFakedContext();

            string _fetchXml =
            @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                <entity name='resource'>
                    <attribute name='name'/>
                    <attribute name='isdisables'/>
                    <filter type = 'and'>
                        <condition attribute='businessunitid' operator='eq-businessid' />
                    </filter>
                </entity>
            </fetch>";

            QueryExpression _query = XrmFakedContext.TranslateFetchXmlToQueryExpression(_context, _fetchXml);

            Assert.True(_query != null);
            Assert.AreEqual(1, _query.Criteria.Conditions.Count);
            Assert.AreEqual("businessunitid", _query.Criteria.Conditions[0].AttributeName);
            Assert.AreEqual(ConditionOperator.EqualBusinessId, _query.Criteria.Conditions[0].Operator);
        }

        [Test]
        public void FetchXml_Operator_EqualBusinessId_Execution()
        {
            XrmFakedContext _context = new XrmFakedContext();

            string _fetchXml =
            @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                <entity name='resource'>
                    <attribute name='name'/>
                    <filter type = 'and'>
                        <condition attribute='businessunitid' operator='eq-businessid' />
                    </filter>
                </entity>
            </fetch>";

            string _resource1Id = "8AE5C98B-46E6-41B8-8326-279B29951A73";
            string _resource2Id = "DF328B18-45D9-4C30-B86A-A54D5CA2F04D";
            string _business1Id = "262DAFB9-9FC9-4006-9881-A680120B40C3";
            string _business2Id = "8BFC5749-8630-4CA0-A80F-9BDADE5F2BDB";

            List<Resource> _entities = new List<Resource>()
            {
                new Resource() { Id = Guid.Parse(_resource1Id), BusinessUnitId = new EntityReference("resource", Guid.Parse(_business1Id)) },
                new Resource() { Id = Guid.Parse(_resource2Id), BusinessUnitId = new EntityReference("resource", Guid.Parse(_business2Id)) }
            };

            _context.BusinessUnitId = new EntityReference("businessunit", Guid.Parse(_business2Id));
            XrmFakedWorkflowContext _workflowContext = _context.GetDefaultWorkflowContext();

            IOrganizationService _service = _context.GetOrganizationService();

            _context.Initialize(_entities);

            EntityCollection _collection = _service.RetrieveMultiple(new FetchExpression(_fetchXml));

            Assert.NotNull(_collection);
            Assert.AreEqual(1, _collection.Entities.Count);
            Assert.AreEqual(Guid.Parse(_resource2Id), _collection.Entities[0].Id);
        }

    }
}
