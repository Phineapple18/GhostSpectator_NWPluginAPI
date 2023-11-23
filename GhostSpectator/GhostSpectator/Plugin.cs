using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace GhostSpectator
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }

		public PluginHandler pluginHandler;

		[PluginPriority(LoadPriority.Medium)]
		[PluginEntryPoint("GhostSpectator", "1.0.1", null, "Phineapple_18")]
		public void OnLoad()
		{
			if (!PluginConfig.IsEnabled)
			{
				return;
			}
			Singleton = this;
			pluginHandler = PluginHandler.Get(this);
			EventManager.RegisterEvents<EventHandlers>(this);
			this.harmony = new Harmony(string.Format("ghostspectator.{0}", DateTime.UtcNow.Ticks));
			this.harmony.PatchAll();
            Log.Info($"Loaded plugin {pluginHandler.PluginName} by {pluginHandler.PluginAuthor}.");
		}

        [PluginConfig] public Config PluginConfig;

		internal Harmony harmony;
	}
}

