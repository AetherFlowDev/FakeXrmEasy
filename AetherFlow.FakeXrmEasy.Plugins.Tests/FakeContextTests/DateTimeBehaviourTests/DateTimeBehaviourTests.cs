﻿using Crm;
using AetherFlow.FakeXrmEasy.Plugins.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests
{
    public class DateTimeBehaviourTests
    {
        [Test]
        public void When_RetrieveMultiple_with_DateTime_Field_Behaviour_set_to_DateOnly_result_is_Time_Part_is_Zero()
        {
            var ctx = new XrmFakedContext
            {
                DateBehaviour = new Dictionary<string, Dictionary<string, DateTimeAttributeBehavior>>
                {
                    {
                        "contact", new Dictionary<string, DateTimeAttributeBehavior>
                        {
                            { "birthdate", DateTimeAttributeBehavior.DateOnly }
                        }
                    }
                }
            };

            ctx.Initialize(new List<Entity>
            {
                new Contact
                {
                    Id = Guid.NewGuid(),
                    ["createdon"] = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc),
                    BirthDate = new DateTime(2000, 1, 1, 23, 0, 0, DateTimeKind.Utc)
                }
            });

            var service = ctx.GetOrganizationService();

            var query = new QueryExpression(Contact.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("createdon", "birthdate")
            };

            var entity = service.RetrieveMultiple(query).Entities.Cast<Contact>().First();

            Assert.AreEqual(new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc), entity.CreatedOn);
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), entity.BirthDate);
        }

        [Test]
        public void When_RetrieveMultiple_with_DateTime_Field_Behaviour_set_to_UserLocal_result_is_Time_Part_is_Kept()
        {
            var ctx = new XrmFakedContext
            {
                DateBehaviour = new Dictionary<string, Dictionary<string, DateTimeAttributeBehavior>>
                {
                    {
                        "contact", new Dictionary<string, DateTimeAttributeBehavior>
                        {
                            { "birthdate", DateTimeAttributeBehavior.UserLocal }
                        }
                    }
                }
            };

            ctx.Initialize(new List<Entity>
            {
                new Contact
                {
                    Id = Guid.NewGuid(),
                    ["createdon"] = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc),
                    BirthDate = new DateTime(2000, 1, 1, 23, 0, 0, DateTimeKind.Utc)
                }
            });

            var service = ctx.GetOrganizationService();

            var query = new QueryExpression(Contact.EntityLogicalName)
            {
                ColumnSet = new ColumnSet("createdon", "birthdate")
            };

            var entity = service.RetrieveMultiple(query).Entities.Cast<Contact>().First();

            Assert.AreEqual(new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc), entity.CreatedOn);
            Assert.AreEqual(new DateTime(2000, 1, 1, 23, 0, 0, DateTimeKind.Utc), entity.BirthDate);
        }

        [Test]
        public void When_Retrieve_with_DateTime_Field_Behaviour_set_to_DateOnly_result_is_Time_Part_is_Zero()
        {
            var ctx = new XrmFakedContext
            {
                DateBehaviour = new Dictionary<string, Dictionary<string, DateTimeAttributeBehavior>>
                {
                    {
                        "contact", new Dictionary<string, DateTimeAttributeBehavior>
                        {
                            { "birthdate", DateTimeAttributeBehavior.DateOnly }
                        }
                    }
                }
            };

            var id = Guid.NewGuid();

            ctx.Initialize(new List<Entity>
            {
                new Contact
                {
                    Id = id,
                    ["createdon"] = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc),
                    BirthDate = new DateTime(2000, 1, 1, 23, 0, 0, DateTimeKind.Utc)
                }
            });

            var service = ctx.GetOrganizationService();

            var entity = service.Retrieve("contact", id, new ColumnSet("createdon", "birthdate")).ToEntity<Contact>();

            Assert.AreEqual(new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc), entity.CreatedOn);
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), entity.BirthDate);
        }

        [Test]
        public void When_Retrieve_with_DateTime_Field_Behaviour_set_to_UserLocal_result_is_Time_Part_is_Kept()
        {
            var ctx = new XrmFakedContext
            {
                DateBehaviour = new Dictionary<string, Dictionary<string, DateTimeAttributeBehavior>>
                {
                    {
                        "contact", new Dictionary<string, DateTimeAttributeBehavior>
                        {
                            { "birthdate", DateTimeAttributeBehavior.UserLocal }
                        }
                    }
                }
            };

            var id = Guid.NewGuid();

            ctx.Initialize(new List<Entity>
            {
                new Contact
                {
                    Id = id,
                    ["createdon"] = new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc),
                    BirthDate = new DateTime(2000, 1, 1, 23, 0, 0, DateTimeKind.Utc)
                }
            });

            var service = ctx.GetOrganizationService();

            var entity = service.Retrieve("contact", id, new ColumnSet("createdon", "birthdate")).ToEntity<Contact>();

            Assert.AreEqual(new DateTime(2017, 1, 1, 1, 0, 0, DateTimeKind.Utc), entity.CreatedOn);
            Assert.AreEqual(new DateTime(2000, 1, 1, 23, 0, 0, DateTimeKind.Utc), entity.BirthDate);
        }
    }
}