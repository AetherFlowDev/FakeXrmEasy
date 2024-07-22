using System;
using System.Collections.Generic;
using System.Text;
using AetherFlow.FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Microsoft.Crm.Sdk.Messages;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.ModifyAccessRightsTests
{
    public class ModifyAccessRequestTests
    {
        /// <summary>
        /// Test that if permissions already exist that they can be modified
        /// </summary>
        [Test]
        public void Test_That_Existing_Permissions_Can_Be_Modified()
        {
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService service = context.GetOrganizationService();
            List<Entity> initialEntities = new List<Entity>();

            Entity contact = new Entity("contact");
            contact.Id = Guid.NewGuid();
            initialEntities.Add(contact);

            Entity user = new Entity("systemuser");
            user.Id = Guid.NewGuid();
            initialEntities.Add(user);

            context.Initialize(initialEntities);

            GrantAccessRequest grantRequest = new GrantAccessRequest()
            {
                Target = contact.ToEntityReference(),
                PrincipalAccess = new PrincipalAccess() { Principal = user.ToEntityReference(), AccessMask = AccessRights.ReadAccess }
            };

            service.Execute(grantRequest);

            RetrieveSharedPrincipalsAndAccessRequest getPermissions = new RetrieveSharedPrincipalsAndAccessRequest()
            {
                Target = contact.ToEntityReference(),
            };

            var permissionsResponse = (RetrieveSharedPrincipalsAndAccessResponse)service.Execute(getPermissions);

            // Make sure things are correct before I start changing things
            Assert.AreEqual(user.Id, permissionsResponse.PrincipalAccesses[0].Principal.Id);
            Assert.AreEqual(AccessRights.ReadAccess, permissionsResponse.PrincipalAccesses[0].AccessMask);

            ModifyAccessRequest modifyRequest = new ModifyAccessRequest()
            {
                Target = contact.ToEntityReference(),
                PrincipalAccess = new PrincipalAccess() { Principal = user.ToEntityReference(), AccessMask = AccessRights.ReadAccess | AccessRights.DeleteAccess }
            };

            service.Execute(modifyRequest);

            permissionsResponse = (RetrieveSharedPrincipalsAndAccessResponse)service.Execute(getPermissions);

            // Check permissions
            Assert.AreEqual(user.Id, permissionsResponse.PrincipalAccesses[0].Principal.Id);
            Assert.AreEqual(AccessRights.ReadAccess | AccessRights.DeleteAccess, permissionsResponse.PrincipalAccesses[0].AccessMask);
        }

        /// <summary>
        /// If permssions haven't been set ModifyAccessRequest actually creates the permissions
        /// </summary>
        [Test]
        public void Test_If_Permissions_Missing_Permissions_Are_Added()
        {
            XrmFakedContext context = new XrmFakedContext();
            IOrganizationService service = context.GetOrganizationService();
            List<Entity> initialEntities = new List<Entity>();

            Entity contact = new Entity("contact");
            contact.Id = Guid.NewGuid();
            initialEntities.Add(contact);

            Entity user = new Entity("systemuser");
            user.Id = Guid.NewGuid();
            initialEntities.Add(user);

            context.Initialize(initialEntities);

            ModifyAccessRequest modifyRequest = new ModifyAccessRequest()
            {
                Target = contact.ToEntityReference(),
                PrincipalAccess = new PrincipalAccess() { Principal = user.ToEntityReference(), AccessMask = AccessRights.ReadAccess | AccessRights.DeleteAccess }
            };

            service.Execute(modifyRequest);


            RetrieveSharedPrincipalsAndAccessRequest getPermissions = new RetrieveSharedPrincipalsAndAccessRequest()
            {
                Target = contact.ToEntityReference(),
            };

            var permissionsResponse = (RetrieveSharedPrincipalsAndAccessResponse)service.Execute(getPermissions);

            // Check permissions
            Assert.AreEqual(user.Id, permissionsResponse.PrincipalAccesses[0].Principal.Id);
            Assert.AreEqual(AccessRights.ReadAccess | AccessRights.DeleteAccess, permissionsResponse.PrincipalAccesses[0].AccessMask);
        }
    }
}
