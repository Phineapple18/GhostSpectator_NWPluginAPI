using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MEC;
using PlayerRoles;
using PluginAPI.Core;
using UnityEngine;

namespace GhostSpectator
{
    public static class GhostExtensions
    {
		public static void Spawn(Player ply)
		{
            ply.TemporaryData.Add("IsGhostSpectator", "spawning");
            try
			{
                ply.GetGhostComponent().enabled = true;
            }
			catch (Exception)
			{
                ply.GameObject.AddComponent<GhostComponent>();
            }
            Log.Debug($"Player {ply.Nickname} was turned into a Ghost.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
		}

		public static void Despawn(Player ply, bool forceToSpectator = true)
		{
            ply.GetGhostComponent().enabled = false;
            if (forceToSpectator)
			{
				ply.SetRole(RoleTypeId.Spectator, RoleChangeReason.RemoteAdmin);
			}
			ply.TemporaryData.Remove("IsGhostSpectator");
			Log.Debug($"Player {ply.Nickname} is no longer a Ghost{(forceToSpectator ? " and was forced to Spectator" : "")}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
		}

        public static bool IsGhost(this ReferenceHub hub)
        {
			return Player.Get(hub).IsGhost();
        }

        public static bool IsGhost(this Player player)
		{
			return player != null && player.TemporaryData.StoredData.Any(kvp => kvp.Key == "IsGhostSpectator" && (string)kvp.Value == "spawned");
		}

        public static IEnumerable<Player> List => Player.GetPlayers().Where(x => x.IsGhost());

        internal static IEnumerator<float> CorrectZombiePosition(Player player, Vector3 position)
        {
            Timing.WaitUntilTrue(() => player.Role == RoleTypeId.Scp0492);
            Timing.CallDelayed(0.1f, () => player.Position = position + Vector3.up);
			Log.Debug($"Finished correcting zombie position for player {player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
            yield break;
        }     

        public static GhostComponent GetGhostComponent(this Player player)
        {
            return player.GameObject.GetComponent<GhostComponent>();
        }
    }
}
