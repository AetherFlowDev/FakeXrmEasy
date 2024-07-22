using Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.Issues
{
    public class Issue512: FakeXrmEasyTestsBase
    {
        private readonly Entity _privilegeObjectTypeCode;
        private readonly Entity _privilege;
        
        public Issue512()
        {
            _privilege = new Entity("privilege")
            {
                Id = Guid.NewGuid()
            };

            _privilegeObjectTypeCode = new Entity("privilegeobjecttypecodes")
            {
                Id = Guid.NewGuid(),
                ["objecttypecode"] = "contact",
                ["privilegeid"] = _privilege.ToEntityReference()
            };
        }

        [Test]
        public void Should_retrieve_privilege_type_code_entity()
        {
            _context.Initialize(new List<Entity>()
            {
                _privilegeObjectTypeCode, _privilege
            });

            using(var svcCtx = new XrmServiceContext(_service))
            {
                var privilegeObjectTypeCodes = (from privilegeObjectTypeCode in svcCtx.CreateQuery("privilegeobjecttypecodes")
                                                where (string)privilegeObjectTypeCode["objecttypecode"] == "contact"
                                                select privilegeObjectTypeCode).ToList();

                Assert.AreEqual(1, privilegeObjectTypeCodes.Count);
                Assert.AreEqual(_privilege.Id, ((EntityReference)privilegeObjectTypeCodes[0]["privilegeid"]).Id);
            }
        }
    }
}
