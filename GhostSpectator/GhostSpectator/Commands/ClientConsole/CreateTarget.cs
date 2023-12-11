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

namespace GhostSpectator.Commands.ClientConsole
{
    public class CreateTarget : ICommand, IUsageProvider
    {
        public CreateTarget (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = !aliases.IsEmpty() ? aliases : _aliases;
        }

        public string Command { get; }

        public string[] Aliases { get; }

        public string Description { get; }

        public string[] Usage { get; } = new string[]
        {
            "\"dboy\"/\"sport\"/\"bin\""
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandTranslation translation = CommandTranslation.loadedTranslation;
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
                response = translation.NotGhost;
                return false;
            }
            GhostComponent component = commandsender.GetComponent<GhostComponent>();
            if (Plugin.Singleton.PluginConfig.TargetLimit <= 0)
            {
                response = translation.TargetNotAllowed;
                return false;
            }
            IFpcRole fpcRole = commandsender.RoleBase as IFpcRole;
            if (!fpcRole.FpcModule.IsGrounded)
            {
                response = translation.NotGrounded;
                return false;
            }
            if (!Plugin.shootingAreas.Any(a => a.Contains(commandsender.Position)))
            {
                response = translation.WrongArea;
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
            Log.Debug($"Ghost {commandsender.Nickname} created a shooting target with Id {target.netId}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.CreateTarget");
            response = translation.CreatetargetSuccess.Replace("%id%", target.netId.ToString());
            return true;
        }

        internal static readonly string _command = "createtarget";

        internal static readonly string _description = "Create a shooting target.";

        internal static readonly string[] _aliases = new string[] { "ct" };

        private readonly Dictionary<string, string> targetNames = new Dictionary<string, string>()
        {
            { "dboy", "TargetDBoy" },
            { "sport", "TargetSport" },
            { "bin", "TargetBinary" }
        };
    }
}
