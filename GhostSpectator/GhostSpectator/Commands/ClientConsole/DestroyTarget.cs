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

namespace GhostSpectator.Commands.ClientConsole
{
    public class DestroyTarget: ICommand, IUsageProvider
    {
        public DestroyTarget (string command, string description, string[] aliases)
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
            "NetId/\"list\""
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
            if (!uint.TryParse(arguments.At(0), out uint targetid))
            {
                response = translation.WrongFormat;
                return false;
            }
            if (!component.shootingTargets.Keys.Contains(targetid))
            {
                response = translation.NoTargetId;
                return false;
            }
            AdminToyBase target = NetworkUtils.SpawnedNetIds[targetid].GetComponent<AdminToyBase>();
            NetworkServer.Destroy(target.gameObject);
            component.shootingTargets.Remove(targetid);
            Log.Debug($"Ghost {commandsender.Nickname} destroyed a shooting target with Id {target.netId}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.DestroyTarget");
            response = translation.DestroytargetSuccess;
            return true;
        }

        internal static readonly string _command = "destroytarget";

        internal static readonly string _description = "Destroy your shooting target by typing its NetId or print a list of all your currently spawned targets.";

        internal static readonly string[] _aliases = new string[] { "dt" };
    }
}
