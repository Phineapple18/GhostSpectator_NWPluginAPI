using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlayerRoles;
using PluginAPI.Core;

namespace GhostSpectator
{
    public static class GhostSpectator
    {
		public static void Spawn(Player ply)
		{
			ply.TemporaryData.Add("IsGhostSpectator", "spawning");
			ply.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
			try
			{
				ply.GameObject.GetComponent<GhostComponent>().enabled = true;
			}
			catch (Exception)
			{
                ply.GameObject.AddComponent<GhostComponent>();
            }
            Log.Debug($"Player {ply.Nickname} was turned into Ghost.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
		}

		public static void Despawn(Player ply, bool forceToSpectator = false)
		{
            ply.GameObject.GetComponent<GhostComponent>().enabled = false;
			if (forceToSpectator)
			{
				ply.SetRole(RoleTypeId.Spectator, RoleChangeReason.RemoteAdmin);
			}
			ply.TemporaryData.Remove("IsGhostSpectator");
			Log.Debug($"Player {ply.Nickname} was despawned from Ghost.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
		}

        public static bool IsGhost(this Player player)
		{
			return player != null && player.TemporaryData.StoredData.Any(kvp => kvp.Key == "IsGhostSpectator" && (string)kvp.Value == "spawned");
		}

		public static IEnumerable<Player> List => Player.GetPlayers().Where(x => x.IsGhost());
    }
}
