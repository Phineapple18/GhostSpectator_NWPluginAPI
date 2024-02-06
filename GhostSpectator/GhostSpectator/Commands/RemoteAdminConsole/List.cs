using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.RemoteAdminConsole
{
	public class List : ICommand
	{
        public List (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = aliases;
            Log.Debug("Loaded List subcommand.", CommandTranslation.commandTranslation.Debug, "GhostSpectator");
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = CommandTranslation.commandTranslation.NotEnabled;
				return false;
			}
            if (!sender.CheckPermission("gs.list"))
            {
                response = CommandTranslation.commandTranslation.NoPerms;
                return false;
            }
            response = $"{CommandTranslation.commandTranslation.GhostList.Replace("%num%", GhostExtensions.List.Count().ToString())}:\n- {string.Join("\n- ", from player in GhostExtensions.List select player.Nickname)}";
            return true;
		}

        internal const string _command = "list";

        internal const string _description = "Print a list of all Ghosts.";

        internal static readonly string[] _aliases = new string[] { "l" };

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
