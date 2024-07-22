using AetherFlow.FakeXrmEasy.Plugins.FakeMessageExecutors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Crm;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests.RemoveUserFromRecordTeamRequestTests
{
    public class RemoveUserFromRecordTeamRequestTests
    {
        [Test]
        public void When_can_execute_is_called_with_an_invalid_request_result_is_false()
        {
            var executor = new RemoveUserFromRecordTeamRequestExecutor();
            var anotherRequest = new AddToQueueRequest();
            Assert.False(executor.CanExecute(anotherRequest));
        }

        [Test]
        public void When_a_request_is_called_User_Is_Removed_From_Record_Team()
        {
            var context = new XrmFakedContext();

            var teamTemplate = new TeamTemplate
            {
                Id = Guid.NewGuid(),
                DefaultAccessRightsMask = (int)AccessRights.ReadAccess
            };

            var team = new Team
            {
                Id = Guid.NewGuid(),
                TeamTemplateId = teamTemplate.ToEntityReference()
            };

            var user = new SystemUser
            {
                Id = Guid.NewGuid()
            };

            var teamMembership = new TeamMembership
            {
                Id = Guid.NewGuid(),
                ["systemuserid"] = user.Id,
                ["teamid"] = team.Id
            };

            var account = new Account
            {
                Id = Guid.NewGuid()
            };

            context.Initialize(new Entity[]
            {
                teamTemplate, team, teamMembership, user, account
            });

            var executor = new RemoveUserFromRecordTeamRequestExecutor();

            var req = new RemoveUserFromRecordTeamRequest
            {
                Record = account.ToEntityReference(),
                SystemUserId = user.Id,
                TeamTemplateId = teamTemplate.Id
            };

            context.AccessRightsRepository.GrantAccessTo(account.ToEntityReference(), new PrincipalAccess
            {
                Principal = user.ToEntityReference(),
                AccessMask = AccessRights.ReadAccess
            });

            executor.Execute(req, context);

            var retrievedTeamMembership = context.CreateQuery<TeamMembership>().FirstOrDefault(p => p.SystemUserId == user.Id && p.TeamId == team.Id);
            Assert.Null(retrievedTeamMembership);

            var response = context.AccessRightsRepository.RetrievePrincipalAccess(account.ToEntityReference(),
                user.ToEntityReference());
            Assert.AreEqual(AccessRights.None, response.AccessRights);

        }
    }
}