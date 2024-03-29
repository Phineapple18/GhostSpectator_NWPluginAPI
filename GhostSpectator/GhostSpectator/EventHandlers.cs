﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InventorySystem.Items.Firearms;
using MEC;
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
        [PluginEvent(ServerEventType.PlaceBlood)]
        internal bool OnPlaceBlood(PlaceBloodEvent ev)
        {
            return !ev.Player.IsGhost();
        }

        [PluginEvent(ServerEventType.PlayerCoinFlip)]
        internal bool OnPlayerCoinFlip(PlayerCoinFlipEvent ev)
        {
            return !ev.Player.IsGhost();
        }

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        internal void OnPlayerChangeRole(PlayerChangeRoleEvent ev)
        {
            if (ev.Player.IsGhost())
			{
				GhostExtensions.Despawn(ev.Player, false);
			}
		}

        [PluginEvent(ServerEventType.PlayerDamage)]
        internal bool OnPlayerDamage(PlayerDamageEvent ev)
		{
            if (!(ev.Player.IsGhost() || ev.Target.IsGhost()))
			{
                return true;
			}
            if (ev.Player.IsGhost() ^ ev.Target.IsGhost())
			{
                return false;
			}
            return ev.Target.GetGhostComponent().duelPartner == ev.Player;
        }

        [PluginEvent(ServerEventType.PlayerDamagedWindow)]
        internal bool OnPlayerDamagedWindow(PlayerDamagedWindowEvent ev)
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
                        ev.Player.ReceiveHint(config.TeleportFail, 3f);
						Log.Debug($"Ghost {ev.Player.Nickname} failed to teleport due to missing valid targets.", config.Debug, pluginName);
                    }
                    else
                    {
                        Player target = validPlayers.ElementAt(random.Next(validPlayers.Count));
                        ev.Player.Position = target.Position + Vector3.up;
                        ev.Player.ReceiveHint(config.TeleportSuccess.Replace("%player%", target.Nickname), 3f);
                        Log.Debug($"Ghost {ev.Player.Nickname} was teleported to a target {target.Nickname}.", config.Debug, pluginName);
                    }
                    return false;
                }
				if (!ev.Player.CheckPermission("gs.item"))
				{
					ev.Player.RemoveItem(ev.Item);
                }
            }
            return true;
		}

        [PluginEvent(ServerEventType.PlayerDying)]
        internal bool OnPlayerDying(PlayerDyingEvent ev)
        {
            if (ev.Attacker.IsGhost() && ev.Player.IsGhost())
            {
                ev.Attacker.GetGhostComponent().duelPartner = null;
                ev.Player.GetGhostComponent().duelPartner = null;
                ev.Attacker.Health = config.GhostHealth;
                ev.Player.Health = config.GhostHealth;
                ev.Attacker.SendBroadcast(config.DuelWon.Replace("%player%", ev.Player.Nickname), 5, Broadcast.BroadcastFlags.Normal, true);
                ev.Player.SendBroadcast(config.DuelLost.Replace("%player%", ev.Player.Nickname), 5, Broadcast.BroadcastFlags.Normal, true);
                return false;
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
                ev.Player.Position = Plugin.spawnPositions.ElementAt(random.Next(Plugin.spawnPositions.Count));
                Log.Debug($"Player {ev.Player.Nickname} left a Pocket Dimension as a Ghost.", config.Debug, pluginName);
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
                Log.Debug($"Destroyed a GhostComponent for {ev.Player.Nickname}.", config.Debug, pluginName);
            }
        }

        [PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
        internal bool OnPlayerRemoveHandcuffs(PlayerRemoveHandcuffsEvent ev)
		{
            return !ev.Player.IsGhost();
        }

        [PluginEvent(ServerEventType.PlayerShotWeapon)]
        internal bool OnPlayerShotWeapon(PlayerShotWeaponEvent ev)
		{
			if (ev.Player.IsGhost())
			{
				if (ev.Firearm.ItemTypeId == ItemType.ParticleDisruptor)
				{
					return ev.Player.CheckPermission("gs.item");
				}
				if (ev.Firearm.Status.Ammo == 0)
				{
                    uint attachments = ev.Firearm.Status.Attachments;
                    ev.Firearm.Status = new FirearmStatus(ev.Firearm.AmmoManagerModule.MaxAmmo, FirearmStatusFlags.MagazineInserted, attachments);
                }
            }
			return true;
		}

        [PluginEvent(ServerEventType.PlayerThrowItem)]
		internal bool OnPlayerThrowItem(PlayerThrowItemEvent ev)
		{
			return !ev.Player.IsGhost() || ev.Player.CheckPermission("gs.item");
		}

		[PluginEvent(ServerEventType.PlayerThrowProjectile)]
		internal bool OnPlayerThrowProjectile(PlayerThrowProjectileEvent ev)
		{
			return !ev.Thrower.IsGhost() || ev.Thrower.CheckPermission("gs.item");
        }

        [PluginEvent(ServerEventType.PlayerUseItem)]
        internal bool OnPlayerUseItem(PlayerUseItemEvent ev)
        {
            return !ev.Player.IsGhost();
        }

        [PluginEvent(ServerEventType.PlayerUsingIntercom)]
		internal bool OnPlayerUsingIntercom(PlayerUsingIntercomEvent ev)
		{
			return !ev.Player.IsGhost();
		}

        [PluginEvent(ServerEventType.RoundEnd)]
        internal void OnRoundEnd(RoundEndEvent ev)
        {
            foreach (Player ghost in GhostExtensions.List)
            {
                GhostExtensions.Despawn(ghost, false);
            }
        }

        [PluginEvent(ServerEventType.Scp049ResurrectBody)]
        internal void OnScp049ResurrectBody(Scp049ResurrectBodyEvent ev)
        {
            Timing.RunCoroutine(GhostExtensions.CorrectZombiePosition(ev.Target, ev.Body.CenterPoint.position));
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
				foreach (Player ghost in from g in GhostExtensions.List where !g.CheckPermission("gs.warhead") select g)
				{
					GhostExtensions.Despawn(ghost);
				}
				Log.Debug("Despawned all Ghosts, that don't have required permission, due to warhead detonation.", config.Debug, pluginName);
			}
		}

        private readonly Config config = Plugin.Singleton.PluginConfig;

        private readonly string pluginName = Plugin.Singleton.pluginHandler.PluginName;

        private static readonly System.Random random = new ();
	}
}
