using System;
using System.Collections.Generic;
using System.Globalization;
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

        [PluginConfig] public Config PluginConfig;

        [PluginPriority(LoadPriority.Medium)]
		[PluginEntryPoint("GhostSpectator", "1.1.0", null, "Phineapple18")]
		public void OnLoad()
		{
			if (!PluginConfig.IsEnabled)
			{
                return;
			}
			Singleton = this;
            pluginHandler = PluginHandler.Get(this);
            EventManager.RegisterEvents<EventHandlers>(this);
            this.GetSpawnPoints(CultureInfo.InvariantCulture);
            this.GetShootingAreas(CultureInfo.InvariantCulture);
            this.harmony = new Harmony(string.Format("ghostspectator.{0}", DateTime.UtcNow.Ticks));
            this.harmony.PatchAll();
            Log.Info($"Loaded plugin {pluginHandler.PluginName} by {pluginHandler.PluginAuthor}.", pluginHandler.PluginName);
        }

        public void GetSpawnPoints(CultureInfo dot)
        {
            foreach (string position in PluginConfig.Spawnpositions)
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
                Log.Warning("All spawn positions were in wrong format, default spawn point will be used instead.", pluginHandler.PluginName);
            }
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
                    Log.Debug($"At least one of the coordinates is in invalid format ({string1}) ({string2}).", PluginConfig.Debug, pluginHandler.PluginName);
                    continue;
                }
                shootingAreas.Add(new Bounds((vector1 + vector2) * 0.5f, (vector1 - vector2).Abs()));
            }
            if (shootingAreas.Count == 0)
            {
                shootingAreas.Add(new (new Vector3(0f, 995.5f, -8f), new Vector3(20f, 1f, 8f)));
                Log.Warning("All shooting areas were in wrong format, default shooting area will be used instead.", pluginHandler.PluginName);
            }
        }

        internal Harmony harmony;

        internal static List<Vector3> spawnPositions = new ();

        internal static List<Bounds> shootingAreas = new ();
    }
}

