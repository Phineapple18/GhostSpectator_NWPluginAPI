using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomPlayerEffects;
using InventorySystem.Items;
using MEC;
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
        }

        public void OnEnable()
        {
            _player.TemporaryData.StoredData["IsGhostSpectator"] = "spawned";
            _player.PlayerInfo.IsRoleHidden = true;
            _player.PlayerInfo.IsNicknameHidden = true;
            _player.CustomInfo = $"<color={Plugin.Singleton.PluginConfig.GhostColor}>{_player.Nickname}\nGHOST</color>";

            _player.IsGodModeEnabled = true;
            _player.Health = 64057f;
            _player.ReferenceHub.interCoordinator.AddBlocker(this);
            _player.EffectsManager.EnableEffect<Scp207>();
            Timing.CallDelayed(0.1f, () => _player.EffectsManager.EnableEffect<Ghostly>());

            _player.AddItem(Plugin.Singleton.PluginConfig.TeleportItem);
            if (!string.IsNullOrWhiteSpace(Plugin.Singleton.PluginConfig.SpawnMessage))
            {
                string message = Plugin.Singleton.PluginConfig.SpawnMessage.Replace("%TeleportItem%", Plugin.Singleton.PluginConfig.TeleportItem.ToString());
                _player.SendBroadcast(message, Plugin.Singleton.PluginConfig.SpawnMessageDuration, Broadcast.BroadcastFlags.Normal, true);
            }
        }

        public void OnDisable()
        {
            _player.TemporaryData.StoredData["IsGhostSpectator"] = "despawning";
            _player.PlayerInfo.IsRoleHidden = false;
            _player.PlayerInfo.IsNicknameHidden = false;
            _player.CustomInfo = string.Empty;

            _player.EffectsManager.DisableAllEffects();
            _player.IsGodModeEnabled = false;
            _player.Health = 100f;
            _player.ClearInventory();
        }

        public BlockedInteraction BlockedInteractions => BlockedInteraction.BeDisarmed | BlockedInteraction.GeneralInteractions | BlockedInteraction.GrabItems | BlockedInteraction.ItemPrimaryAction | BlockedInteraction.ItemUsage;

        public bool CanBeCleared => !base.enabled;

        private Player _player;
    }
}
