using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;

namespace GhostSpectator.Commands.RemoteAdminConsole
{
	public class List : ICommand
	{
        public List (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = !aliases.IsEmpty() ? aliases : _aliases;
        }

        public string Command { get; } 

		public string[] Aliases { get; } 

		public string Description { get; } 

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = CommandTranslation.loadedTranslation.NotEnabled;
				return false;
			}
            response = $"{CommandTranslation.loadedTranslation.GhostList}:" + string.Join("\n- ", from player in GhostSpectator.List select player.Nickname);
			return true;
		}

        internal static readonly string _command = "list";

        internal static readonly string _description = "Print a list of all Ghosts.";

        internal static readonly string[] _aliases = new string[] { "l" };
    }
}
