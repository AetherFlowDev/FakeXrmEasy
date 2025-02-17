﻿using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace AetherFlow.FakeXrmEasy.Plugins.Services
{
    public class InvoiceInitializerService : IEntityInitializerService
    {
        public const string EntityLogicalName = "invoice";

        public Entity Initialize(Entity e, Guid gCallerId, XrmFakedContext ctx, bool isManyToManyRelationshipEntity = false)
        {
            if (string.IsNullOrEmpty(e.GetAttributeValue<string>("invoicenumber")))
            {
                //first AetherFlow.FakeXrmEasy.Plugins auto-numbering emulation
                e["invoicenumber"] = "INV-" + DateTime.Now.Ticks;
            }

            return e;
        }

        public Entity Initialize(Entity e, XrmFakedContext ctx, bool isManyToManyRelationshipEntity = false)
        {
            return this.Initialize(e, Guid.NewGuid(), ctx, isManyToManyRelationshipEntity);
        }
    }
}
