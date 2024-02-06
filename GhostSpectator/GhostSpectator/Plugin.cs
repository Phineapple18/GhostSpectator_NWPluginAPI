using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.IO;

using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Helpers;
using PluginAPI.Loader;
using UnityEngine;

namespace GhostSpectator
{
    public class Plugin
    {  
        [PluginPriority(LoadPriority.Medium)]
		[PluginEntryPoint("GhostSpectator", "1.2.1", null, "Phineapple18")]
		public void OnLoad()
		{
            if (!PluginConfig.IsEnabled)
			{
                Log.Warning("Plugin GhostSpectator is disabled.", "GhostSpectator");
                return;
			}
            if (!AssemblyLoader.InstalledPlugins.Any(p => p.PluginName == "NWApiPermissionSystem"))
            {
                throw new DllNotFoundException("The NWApiPermissionSystem plugin is required for GhostSpectator to work.");
            }
            if (File.Exists(Path.Combine(Paths.GlobalPlugins.Plugins, "0Harmony.dll")) || File.Exists(Path.Combine(Paths.LocalPlugins.Plugins, "0Harmony.dll")))
            {
                Log.Warning("0Harmony should be in the dependencies folder.", pluginHandler.PluginName);
            }
            Singleton = this;
            pluginHandler = PluginHandler.Get(this);
            EventManager.RegisterEvents<EventHandlers>(this);
            this.GetSpawnPoints(CultureInfo.InvariantCulture);
            this.GetShootingAreas(CultureInfo.InvariantCulture);
            this.harmony = new Harmony($"ghostspectator.{DateTime.UtcNow.Ticks}");
            this.harmony.PatchAll();
            Log.Info($"Loaded plugin {pluginHandler.PluginName} by {pluginHandler.PluginAuthor}.", pluginHandler.PluginName);
        }

        public void GetSpawnPoints(CultureInfo dot)
        {
            foreach (string position in PluginConfig.SpawnPositions)
            {
                try
                {
                    string[] newPosition = position.Split(',');
                    spawnPositions.Add(new (float.Parse(newPosition[0], dot), float.Parse(newPosition[1], dot), float.Parse(newPosition[2], dot)));
                }
                catch (Exception)
                {
                    Log.Debug($"The spawn point {position} is not a valid position.", PluginConfig.Debug, pluginHandler.PluginName);
                    continue;
                }
            }
            if (spawnPositions.Count == 0)
            {
                spawnPositions.Add(new (9f, 1002f, 1f));
                Log.Error("All spawn positions were in wrong format, default spawn point will be used instead.", pluginHandler.PluginName);
                return;
            }
            Log.Debug($"Successfully loaded {spawnPositions.Count} Ghost spawn positions.", PluginConfig.Debug, pluginHandler.PluginName);
        }

        public void GetShootingAreas(CultureInfo dot)
        {
            foreach (var area in PluginConfig.ShootingAreas)
            {
                string[] string1 = area.Key.Split(',');
                string[] string2 = area.Value.Split(',');

                Vector3 vector1;
                Vector3 vector2;
                try
                {
                    vector1 = new (float.Parse(string1[0], dot), float.Parse(string1[1], dot), float.Parse(string1[2], dot));
                    vector2 = new (float.Parse(string2[0], dot), float.Parse(string2[1], dot), float.Parse(string2[2], dot));
                }
                catch (Exception)
                {
                    Log.Debug($"At least one of the shooting area coordinates is in invalid format ({area.Key}) ({area.Value}).", PluginConfig.Debug, pluginHandler.PluginName);
                    continue;
                }
                shootingAreas.Add(new Bounds((vector1 + vector2) * 0.5f, (vector1 - vector2).Abs()));
            }
            if (shootingAreas.Count == 0)
            {
                shootingAreas.Add(new (new Vector3(0f, 995.5f, -8f), new Vector3(20f, 1f, 8f)));
                Log.Error("All shooting areas were in wrong format, default shooting area will be used instead.", pluginHandler.PluginName);
                return;
            }
            Log.Debug($"Successfully created {shootingAreas.Count} shooting areas.", PluginConfig.Debug, pluginHandler.PluginName);
        }

        public static Plugin Singleton { get; private set; }

        public PluginHandler pluginHandler;

        [PluginConfig] public Config PluginConfig;

        internal static List<Bounds> shootingAreas = new ();

        internal static List<Vector3> spawnPositions = new();

        private Harmony harmony;
    }
}

