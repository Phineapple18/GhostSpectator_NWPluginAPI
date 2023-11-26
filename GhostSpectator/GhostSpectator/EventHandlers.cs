﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NWAPIPermissionSystem;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using UnityEngine;

namespace GhostSpectator
{
    internal class EventHandlers
    {
        [PluginEvent(ServerEventType.PlayerChangeRole)]
		internal void OnPlayerChangeRole(PlayerChangeRoleEvent ev)
		{
			if (ev.Player.IsGhost())
			{
				GhostSpectator.Despawn(ev.Player);
			}
		}

		[PluginEvent(ServerEventType.PlayerDamage)]
        internal bool OnPlayerDamage(PlayerDamageEvent ev)
		{
            return !ev.Player.IsGhost();
		}

        [PluginEvent(ServerEventType.PlayerDropItem)]
        internal bool OnPlayerDropItem(PlayerDropItemEvent ev)
		{
            if (ev.Player.IsGhost() && ev.Item != null)
            {
				if (ev.Item.ItemTypeId == config.TeleportItem)
				{
                    List<Player> validPlayers = Player.GetPlayers().Where(p => p.IsAlive && !(p.IsGhost() || p.Role == RoleTypeId.Scp079 || config.RoleTeleportBlacklist.Contains(p.Role))).ToList();
                    if (validPlayers.IsEmpty())
                    {
                        ev.Player.ReceiveHint(config.Translation.TeleportFail, 3f);
						Log.Debug($"Player {ev.Player.Nickname} failed to teleport due to missing valid targets.", config.Debug, pluginName);
                    }
                    else
                    {
                        Player target = validPlayers.ElementAt(RandInt.Next(validPlayers.Count - 1));
                        ev.Player.Position = target.Position + Vector3.up;
						string message = config.Translation.TeleportSuccess.Replace("%player%", target.Nickname);
                        ev.Player.ReceiveHint(message, 3f);
                        Log.Debug($"Player {ev.Player.Nickname} teleported to {target.Nickname}.", config.Debug, pluginName);
                    }
                    return false;
                }
				return ev.Player.CheckPermission("gs.items");
            }

            return true;
		}

		[PluginEvent(ServerEventType.PlayerEnterPocketDimension)]
		internal bool OnPlayerEnterPocketDimension(PlayerEnterPocketDimensionEvent ev)
		{
			return !ev.Player.IsGhost();
		}

		[PluginEvent(ServerEventType.PlayerExitPocketDimension)]
		internal bool OnPlayerExitPocketDimension(PlayerExitPocketDimensionEvent ev)
		{
			if (ev.Player.IsGhost())
			{
				ev.Player.Position = Plugin.spawnPosition;
                Log.Debug($"Player {ev.Player.Nickname} left PD as Ghost.", config.Debug, pluginName);
                return false;
			}

			return true;
		}

        [PluginEvent(ServerEventType.PlayerLeft)]
        internal void OnPlayerLeft(PlayerLeftEvent ev)
        {
            if (ev.Player.TryGetComponent<GhostComponent>(out GhostComponent ghostComponent))
            {
                UnityEngine.Object.Destroy(ghostComponent);
                Log.Debug($"Destroyed GhostComponent for {ev.Player.Nickname}.", config.Debug, pluginName);
            }
        }

        [PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
        internal bool OnPlayerRemoveHandcuffs(PlayerRemoveHandcuffsEvent ev)
		{
            return !ev.Player.IsGhost();
        }

        [PluginEvent(ServerEventType.PlayerThrowItem)]
		internal bool OnPlayerThrowItem(PlayerThrowItemEvent ev)
		{
			return !ev.Player.IsGhost() || ev.Player.CheckPermission("gs.items");
		}

		[PluginEvent(ServerEventType.PlayerThrowProjectile)]
		internal bool OnPlayerThrowProjectile(PlayerThrowProjectileEvent ev)
		{
			return !ev.Thrower.IsGhost() || ev.Thrower.CheckPermission("gs.items");
        }

		[PluginEvent(ServerEventType.PlayerUsingIntercom)]
		internal bool OnPlayerUsingIntercom(PlayerUsingIntercomEvent ev)
		{
			return !ev.Player.IsGhost();
		}

		[PluginEvent(ServerEventType.Scp096AddingTarget)]
		internal bool OnScp096AddingTarget(Scp096AddingTargetEvent ev)
		{
			return !ev.Target.IsGhost();
		}

		[PluginEvent(ServerEventType.Scp173NewObserver)]
		internal bool OnScp173NewObserver(Scp173NewObserverEvent ev)
		{
			return !ev.Target.IsGhost();
		}

		[PluginEvent(ServerEventType.Scp173PlaySound)]
		internal bool OnScp173PlaySound(Scp173PlaySoundEvent ev)
		{
			return !ev.Player.IsGhost();
		}

		[PluginEvent(ServerEventType.Scp914ProcessPlayer)]
		internal bool OnScp914ProcessPlayer(Scp914ProcessPlayerEvent ev)
		{
			return !ev.Player.IsGhost();
		}

		[PluginEvent(ServerEventType.Scp914UpgradeInventory)]
		internal bool OnScp914UpgradeInventory(Scp914UpgradeInventoryEvent ev)
		{
			return !ev.Player.IsGhost();
		}

		[PluginEvent(ServerEventType.WarheadDetonation)]
		internal void OnWarheadDetonation()
		{
			if (config.DespawnOnDetonation)
			{
				foreach (Player ghost in from g in GhostSpectator.List where !g.CheckPermission("gs.warhead") select g)
				{
					GhostSpectator.Despawn(ghost, true);
				}
			}
		}

		private readonly Config config = Plugin.Singleton.PluginConfig;

        private readonly string pluginName = Plugin.Singleton.pluginHandler.PluginName;

        private static readonly System.Random RandInt = new System.Random();
	}
}
