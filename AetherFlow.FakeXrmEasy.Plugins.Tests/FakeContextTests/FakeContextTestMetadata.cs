﻿using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests
{
    public class FakeContextTestMetadata
    {
        [Test]
        public void Should_throw_exception_if_null_was_used_to_initialise()
        {
            var ctx = new XrmFakedContext();
            Assert.Throws<Exception>(() => ctx.InitializeMetadata(entityMetadataList: null));
        }

        [Test]
        public void Should_throw_exception_if_logical_name_is_null_during_initialisation()
        {
            var ctx = new XrmFakedContext();
            var entityMetadata = new EntityMetadata()
            {

            };
            Assert.Throws<Exception>(() =>
                ctx.InitializeMetadata(new List<EntityMetadata>() {
                    entityMetadata
                }));
        }

        [Test]
        public void Should_throw_exception_if_logical_name_is_empty_during_initialisation()
        {
            var ctx = new XrmFakedContext();
            var entityMetadata = new EntityMetadata()
            {
                LogicalName = ""
            };
            Assert.Throws<Exception>(() =>
                ctx.InitializeMetadata(new List<EntityMetadata>() {
                    entityMetadata
                }));
        }

        [Test]
        public void Should_throw_exception_if_entity_name_is_duplicated_during_initialisation()
        {
            var ctx = new XrmFakedContext();
            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            Assert.Throws<Exception>(() =>
                ctx.InitializeMetadata(new List<EntityMetadata>() {
                    entityMetadata,
                    entityMetadata
                }));
        }

        [Test]
        public void Should_contain_one_entity_metadata_after_initialisation()
        {
            var ctx = new XrmFakedContext();
            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            ctx.InitializeMetadata(new List<EntityMetadata>() { entityMetadata });

            var metadatas = ctx.CreateMetadataQuery().ToList();
            Assert.True(metadatas.Count == 1);
            Assert.AreEqual("account", metadatas[0].LogicalName);
        }

        [Test]
        public void Should_store_a_clone_after_initialisation()
        {
            var ctx = new XrmFakedContext();
            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            ctx.InitializeMetadata(new List<EntityMetadata>() { entityMetadata });

            var metadatas = ctx.CreateMetadataQuery().ToList();
            Assert.True(metadatas[0] != entityMetadata);
        }

        [Test]
        public void Should_return_a_clone_when_querying_entity_metadatas()
        {
            var ctx = new XrmFakedContext();
            var entityMetadata = new EntityMetadata()
            {
                LogicalName = "account"
            };
            ctx.InitializeMetadata(new List<EntityMetadata>() { entityMetadata });

            var metadata1 = ctx.CreateMetadataQuery().FirstOrDefault();
            var metadata2 = ctx.CreateMetadataQuery().FirstOrDefault();
            Assert.True(metadata1 != metadata2);
        }

        [Test]
        public void Should_initialize_metadata_from_early_bound_assembly()
        {
            var ctx = new XrmFakedContext();
            ctx.InitializeMetadata(typeof(Crm.Account).Assembly);

            var accountMetadata = ctx.CreateMetadataQuery().Where(x => x.LogicalName == "account").FirstOrDefault();

            Assert.NotNull(accountMetadata);

            var accountid = accountMetadata.Attributes.FirstOrDefault(x => x.LogicalName == "accountid");


            Assert.AreEqual("accountid", accountMetadata.PrimaryIdAttribute);
            Assert.NotNull(accountid);
            Assert.AreEqual(AttributeTypeCode.Uniqueidentifier, accountid.AttributeType);
        }
    }
}
