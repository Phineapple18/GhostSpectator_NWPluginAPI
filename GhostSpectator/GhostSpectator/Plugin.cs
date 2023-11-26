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
using UnityEngine;

namespace GhostSpectator
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }

		public PluginHandler pluginHandler;

		[PluginPriority(LoadPriority.Medium)]
		[PluginEntryPoint("GhostSpectator", "1.0.2", null, "Phineapple_18")]
		public void OnLoad()
		{
			if (!PluginConfig.IsEnabled)
			{
				notEnabled = PluginConfig.Translation.NotEnabled;
                return;
			}
			Singleton = this;
            pluginHandler = PluginHandler.Get(this);
            EventManager.RegisterEvents<EventHandlers>(this);
            this.harmony = new Harmony(string.Format("ghostspectator.{0}", DateTime.UtcNow.Ticks));
            this.harmony.PatchAll();
            try
            {
                string[] newPosition = PluginConfig.SpawnPoint.Split(',');
                spawnPosition = new(float.Parse(newPosition[0]), float.Parse(newPosition[1]), float.Parse(newPosition[2]));
            }
            catch (Exception)
            {
                Log.Error("The SpawnPosition is in wrong format in Config file, default SpawnPosition will be used instead.", pluginHandler.PluginName);
            }
            Log.Info($"Loaded plugin {pluginHandler.PluginName} by {pluginHandler.PluginAuthor}.");
		}

        [PluginConfig] public Config PluginConfig;

        internal Harmony harmony;

        internal static string notEnabled;

        internal static Vector3 spawnPosition = new (9f, 1002f, 1f);
    }
}

