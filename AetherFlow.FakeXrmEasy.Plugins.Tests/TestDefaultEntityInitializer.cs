using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests
{
    public class TestDefaultEntityInitializer
    {
        [Test]
        public void TestWithUnpopulatedValues()
        {
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService service = context.GetOrganizationService();

            List<Entity> initialEntities = new List<Entity>();

            Entity user = new Entity("systemuser");
            user.Id = Guid.NewGuid();
            initialEntities.Add(user);

            context.CallerId = user.ToEntityReference();

            Entity testEntity = new Entity("test");
            testEntity.Id = Guid.NewGuid();
            initialEntities.Add(testEntity);

            context.Initialize(initialEntities);
            Entity testPostCreate = service.Retrieve("test", testEntity.Id, new ColumnSet(true));
            Assert.AreEqual(user.ToEntityReference(), testPostCreate["ownerid"]);
            Assert.AreEqual(user.ToEntityReference(), testPostCreate["createdby"]);
            Assert.AreEqual(user.ToEntityReference(), testPostCreate["modifiedby"]);
        }
    }
}