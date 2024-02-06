using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminToys;
using CustomPlayerEffects;
using GhostSpectator.Commands;
using GhostSpectator.Commands.ClientConsole.Duel;
using InventorySystem.Items;
using MEC;
using Mirror;
using NWAPIPermissionSystem;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using UnityEngine;
using Utils.Networking;

namespace GhostSpectator
{
    [DisallowMultipleComponent]
    public class GhostComponent : MonoBehaviour, IInteractionBlocker
    {
        public void Awake()
        {
            player = Player.Get(ReferenceHub.GetHub(base.transform.root.gameObject));
            Log.Debug($"Created a GhostComponent for player {player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public void OnEnable()
        {
            Config config = Plugin.Singleton.PluginConfig;

            player.ReferenceHub.roleManager.ServerSetRole(this.RoleType, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.AssignInventory);
            player.PlayerInfo.IsRoleHidden = true;
            player.PlayerInfo.IsNicknameHidden = true;

            player.Position = Plugin.spawnPositions.ElementAt(new System.Random().Next(Plugin.spawnPositions.Count));
            player.Health = this.Health;        
            player.EffectsManager.EnableEffect<Scp207>();
            Timing.CallDelayed(0.1f, () => player.EffectsManager.EnableEffect<Ghostly>());
            if (player.CheckPermission("gs.noclip"))
            {
                FpcNoclip.PermitPlayer(player.ReferenceHub);
            }

            player.ReferenceHub.interCoordinator.AddBlocker(this);
            player.AddItem(config.TeleportItem);
            if (!string.IsNullOrWhiteSpace(config.SpawnMessage))
            {
                string message = config.SpawnMessage.Replace("%colour%", config.GhostColor).Replace("%TeleportItem%", config.TeleportItem.ToString());
                player.SendBroadcast(message, config.SpawnMessageDuration, Broadcast.BroadcastFlags.Normal, true);
            }
            player.TemporaryData.StoredData["IsGhostSpectator"] = "spawned";
            Log.Debug($"Enabled a GhostComponent for player {player.Nickname}.", config.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public void Update()
        {
            player.CustomInfo = $"<color={Plugin.Singleton.PluginConfig.GhostColor}>{player.DisplayNickname.Replace("#855439", "#944710")}\n{Plugin.Singleton.PluginConfig.GhostNickname}</color>";
        }

        public void OnDisable()
        {
            player.TemporaryData.StoredData["IsGhostSpectator"] = "despawning";
            player.PlayerInfo.IsRoleHidden = false;
            player.PlayerInfo.IsNicknameHidden = false;
            player.CustomInfo = string.Empty;

            player.ClearInventory();
            player.EffectsManager.DisableAllEffects();
            player.IsGodModeEnabled = false;
            player.Health = player.MaxHealth;
            if (player.CheckPermission("gs.noclip"))
            {
                FpcNoclip.UnpermitPlayer(player.ReferenceHub);
            }

            if (duelPartner != null)
            {
                duelPartner.ReceiveHint(CommandTranslation.commandTranslation.DuelAborted, 5);
                duelPartner.GetGhostComponent().duelPartner = null;
            }
            duelPartner = null;
            DuelParent.list.Remove(player);
            foreach (var toy in this.shootingTargets)
            {
                if (NetworkUtils.SpawnedNetIds.TryGetValue(toy.Key, out NetworkIdentity networkIdentity) && networkIdentity.TryGetComponent<AdminToyBase>(out AdminToyBase target))
                {
                    NetworkServer.Destroy(target.gameObject);
                }
                this.shootingTargets.Remove(toy.Key);
            }
            Log.Debug($"Disabled a GhostComponent for player {player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public BlockedInteraction BlockedInteractions => BlockedInteraction.BeDisarmed | BlockedInteraction.GeneralInteractions | BlockedInteraction.GrabItems;

        public bool CanBeCleared => !base.enabled;

        internal float Health => Plugin.Singleton.PluginConfig.GhostHealth;

        private RoleTypeId RoleType => RoleTypeId.Tutorial;

        internal Player duelPartner;   

        internal Dictionary<uint, AdminToyBase> shootingTargets = new ();

        private Player player;
    }
}
