using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminToys;
using CommandSystem;
using Mirror;
using NWAPIPermissionSystem;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using Utils.Networking;

namespace GhostSpectator.Commands.ClientConsole.ShootingTarget
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class CreateTarget : ICommand, IUsageProvider
    {
        public CreateTarget ()
        {
            translation = CommandTranslation.commandTranslation;
            Command = !string.IsNullOrWhiteSpace(translation.CreatetargetCommand) ? translation.CreatetargetCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.CreatetargetDescription) ? translation.CreatetargetDescription : _description;
            Aliases = translation.CreatetargetAliases;
            Log.Debug("Loaded CreateTarget command.", translation.Debug, "GhostSpectator");
        }

        public string[] Usage { get; } = new string[]
        {
            "dboy/sport/bin"
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Singleton == null)
            {
                response = translation.NotEnabled;
                return false;
            }
            if (sender == null)
            {
                response = translation.SenderNull;
                return false;
            }
            if (!sender.CheckPermission("gs.target"))
            {
                response = translation.NoPerms;
                return false;
            }
            Player commandsender = Player.Get(sender);
            if (!commandsender.IsGhost())
            {
                response = translation.NotGhostSelf;
                return false;
            }
            if (Plugin.Singleton.PluginConfig.TargetLimit <= 0)
            {
                response = translation.TargetNotAllowed;
                return false;
            }
            if (!Plugin.shootingAreas.Any(a => a.Contains(commandsender.Position)))
            {
                response = translation.WrongArea;
                return false;
            }
            IFpcRole fpcRole = commandsender.RoleBase as IFpcRole;
            if (!fpcRole.FpcModule.IsGrounded)
            {
                response = translation.NotGrounded;
                return false;
            }
            if (arguments.IsEmpty())
            {
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
            }
            AdminToyBase targetBase = null;
            AdminToyBase target = null;
            try
            {
                NetworkClient.prefabs.Values.First(a => a.TryGetComponent<AdminToyBase>(out targetBase) && targetBase.CommandName == this.targetNames[arguments.At(0)]);
            }
            catch (Exception)
            {
                response = translation.WrongArgument;
                return false;
            }
            GhostComponent component = commandsender.GetGhostComponent();
            if (component.shootingTargets.Count >= Plugin.Singleton.PluginConfig.TargetLimit)
            {
                var targetToRemove = component.shootingTargets.ElementAt(0);
                if (NetworkUtils.SpawnedNetIds.TryGetValue(targetToRemove.Key, out NetworkIdentity networkIdentity) && networkIdentity.TryGetComponent<AdminToyBase>(out AdminToyBase adminToy))
                {
                    NetworkServer.Destroy(adminToy.gameObject);
                }
                component.shootingTargets.Remove(targetToRemove.Key);
            }
            target = UnityEngine.Object.Instantiate<AdminToyBase>(targetBase);
            target.OnSpawned(commandsender.ReferenceHub, arguments);
            component.shootingTargets.Add(target.netId, target); 
            response = translation.CreatetargetSuccess.Replace("%id%", target.netId.ToString());
            Log.Debug($"Player {commandsender.Nickname} created a {target.CommandName} with Id {target.netId}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.CreateTarget");
            return true;
        }

        internal const string _command = "createtarget";

        internal const string _description = "Create a shooting target.";

        internal static readonly string[] _aliases = new string[] { "cstg", "ctg" };

        private readonly Dictionary<string, string> targetNames = new ()
        {
            { "dboy", "TargetDBoy" },
            { "sport", "TargetSport" },
            { "bin", "TargetBinary" }
        };

        private readonly CommandTranslation translation;

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
