using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminToys;
using CustomPlayerEffects;
using InventorySystem.Items;
using MEC;
using Mirror;
using NWAPIPermissionSystem;
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
            _player = Player.Get(ReferenceHub.GetHub(base.transform.root.gameObject));
            Log.Debug($"Created GhostComponent for {_player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public void OnEnable()
        {
            Config config = Plugin.Singleton.PluginConfig;

            _player.TemporaryData.StoredData["IsGhostSpectator"] = "spawned";
            _player.PlayerInfo.IsRoleHidden = true;
            _player.PlayerInfo.IsNicknameHidden = true;            
            _player.CustomInfo = $"<color={config.GhostColor}>{_player.Nickname}\n{config.GhostNickname}</color>";

            _player.Position = Plugin.spawnPositions.ElementAt(new System.Random().Next(Plugin.spawnPositions.Count));
            _player.IsGodModeEnabled = true;  
            _player.Health = 64057f;        
            _player.EffectsManager.EnableEffect<Scp207>();
            Timing.CallDelayed(0.1f, () => _player.EffectsManager.EnableEffect<Ghostly>());
            if (_player.CheckPermission("gs.noclip") && !FpcNoclip.IsPermitted(_player.ReferenceHub))
            {
                FpcNoclip.PermitPlayer(_player.ReferenceHub);
            }

            _player.ReferenceHub.interCoordinator.AddBlocker(this);
            _player.AddItem(config.TeleportItem);
            if (!string.IsNullOrWhiteSpace(config.Spawnmessage))
            {
                string message = config.Spawnmessage.Replace("%colour%", config.GhostColor).Replace("%TeleportItem%", config.TeleportItem.ToString());
                _player.SendBroadcast(message, config.SpawnmessageDuration, Broadcast.BroadcastFlags.Normal, true);
            }
            Log.Debug($"Enabled GhostComponent for {_player.Nickname}.", config.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public void OnDisable()
        {
            _player.TemporaryData.StoredData["IsGhostSpectator"] = "despawning";
            _player.PlayerInfo.IsRoleHidden = false;
            _player.PlayerInfo.IsNicknameHidden = false;
            _player.CustomInfo = string.Empty;

            _player.ClearInventory();
            _player.EffectsManager.DisableAllEffects();
            _player.IsGodModeEnabled = false;
            _player.Health = 100f;
            if (_player.CheckPermission("gs.noclip") && FpcNoclip.IsPermitted(_player.ReferenceHub))
            {
                FpcNoclip.UnpermitPlayer(_player.ReferenceHub);
            }

            foreach (var toy in this.shootingTargets)
            {
                if (NetworkUtils.SpawnedNetIds.TryGetValue(toy.Key, out NetworkIdentity networkIdentity) && networkIdentity.TryGetComponent<AdminToyBase>(out AdminToyBase target))
                {
                    NetworkServer.Destroy(target.gameObject);
                }
                this.shootingTargets.Remove(toy.Key);
            }
            Log.Debug($"Disabled GhostComponent for {_player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public BlockedInteraction BlockedInteractions => BlockedInteraction.BeDisarmed | BlockedInteraction.GeneralInteractions | BlockedInteraction.GrabItems;

        public bool CanBeCleared => !base.enabled;

        private Player _player;

        internal Dictionary<uint, AdminToyBase> shootingTargets = new ();
    }
}
