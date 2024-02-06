using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminToys;
using CommandSystem;
using Mirror;
using NWAPIPermissionSystem;
using PluginAPI.Core;
using Utils.Networking;

namespace GhostSpectator.Commands.ClientConsole.ShootingTarget
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DestroyTarget: ICommand, IUsageProvider
    {
        public DestroyTarget ()
        {
            translation = CommandTranslation.commandTranslation;
            Command = !string.IsNullOrWhiteSpace(translation.DestroytargetCommand) ? translation.DestroytargetCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.DestroytargetDescription) ? translation.DestroytargetDescription : _description;
            Aliases = translation.DestroytargetAliases;
            Log.Debug("Loaded DestroyTarget command.", translation.Debug, "GhostSpectator");
        }

        public string[] Usage { get; } = new string[]
        {
            "[Net ID]/list"
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
            GhostComponent component = commandsender.GetGhostComponent();
            if (component.shootingTargets.Count == 0)
            {
                response = translation.NoTargets;
                return false;
            }
            if (arguments.IsEmpty())
            {
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
            }
            if (arguments.At(0).ToLower() == "list")
            {
                response = $"{translation.TargetList.Replace("%num%", component.shootingTargets.Count().ToString())}:\n- {string.Join("\n- ", from id in component.shootingTargets select id.Key)}";
                return false;
            }
            if (!uint.TryParse(arguments.At(0), out uint targetId))
            {
                response = translation.WrongFormat;
                return false;
            }
            if (!component.shootingTargets.Keys.Contains(targetId))
            {
                response = translation.NoTargetId;
                return false;
            }
            AdminToyBase target = NetworkUtils.SpawnedNetIds[targetId].GetComponent<AdminToyBase>();
            NetworkServer.Destroy(target.gameObject);
            component.shootingTargets.Remove(targetId);
            response = translation.DestroytargetSuccess;
            Log.Debug($"Player {commandsender.Nickname} destroyed a {target.CommandName} with Id {target.netId}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.DestroyTarget");
            return true;
        }

        internal const string _command = "destroytarget";

        internal const string _description = "Destroy your shooting target or print a list of your targets.";

        internal static readonly string[] _aliases = new string[] { "dstg", "dtg" };

        private readonly CommandTranslation translation;

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
