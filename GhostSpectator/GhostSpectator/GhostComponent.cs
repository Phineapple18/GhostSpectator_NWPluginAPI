using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using InventorySystem.Items;
using MEC;
using NWAPIPermissionSystem;
using PluginAPI.Core;
using UnityEngine;

namespace GhostSpectator
{
    [DisallowMultipleComponent]
    public class GhostComponent : MonoBehaviour, IInteractionBlocker
    {
        public void Awake()
        {
            this._player = Player.Get(ReferenceHub.GetHub(base.transform.root.gameObject));
            Log.Debug($"Created GhostComponent for {_player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public void OnEnable()
        {
            Config config = Plugin.Singleton.PluginConfig;
            _player.Position = Plugin.spawnPosition;
            _player.TemporaryData.StoredData["IsGhostSpectator"] = "spawned";
            _player.PlayerInfo.IsRoleHidden = true;
            _player.PlayerInfo.IsNicknameHidden = true;
            _player.CustomInfo = $"<color={config.GhostColor}>{_player.Nickname}\n{config.Translation.Ghost}</color>";

            _player.IsGodModeEnabled = true;  
            _player.Health = 64057f;
            _player.ReferenceHub.interCoordinator.AddBlocker(this);
            _player.EffectsManager.EnableEffect<Scp207>();
            Timing.CallDelayed(0.1f, delegate () 
            {
                _player.EffectsManager.EnableEffect<Ghostly>();
                _player.IsNoclipEnabled = _player.CheckPermission("gs.noclip");
            });

            _player.AddItem(config.TeleportItem);
            if (!string.IsNullOrWhiteSpace(config.Translation.SpawnMessage))
            {
                string message = config.Translation.SpawnMessage.Replace("%colour%", config.GhostColor).Replace("%TeleportItem%", config.TeleportItem.ToString());
                _player.SendBroadcast(message, config.SpawnMessageDuration, Broadcast.BroadcastFlags.Normal, true);
            }
            Log.Debug($"Enabled GhostComponent for {_player.Nickname}.", config.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public void OnDisable()
        {
            _player.TemporaryData.StoredData["IsGhostSpectator"] = "despawning";
            _player.PlayerInfo.IsRoleHidden = false;
            _player.PlayerInfo.IsNicknameHidden = false;
            _player.CustomInfo = string.Empty;

            _player.EffectsManager.DisableAllEffects();
            _player.IsGodModeEnabled = false;
            _player.IsNoclipEnabled = false;
            _player.Health = 100f;
            _player.ClearInventory();
            Log.Debug($"Disabled GhostComponent for {_player.Nickname}.", Plugin.Singleton.PluginConfig.Debug, Plugin.Singleton.pluginHandler.PluginName);
        }

        public BlockedInteraction BlockedInteractions => BlockedInteraction.BeDisarmed | BlockedInteraction.GeneralInteractions | BlockedInteraction.GrabItems | BlockedInteraction.ItemPrimaryAction | BlockedInteraction.ItemUsage;

        public bool CanBeCleared => !base.enabled;

        private Player _player;
    }
}
