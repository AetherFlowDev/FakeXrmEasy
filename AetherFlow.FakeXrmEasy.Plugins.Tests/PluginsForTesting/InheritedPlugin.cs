﻿using Microsoft.Xrm.Sdk;
using System;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.PluginsForTesting
{
    public abstract class PluginBase : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
        }
    }

    public class MyPlugin : PluginBase
    {
    }
}