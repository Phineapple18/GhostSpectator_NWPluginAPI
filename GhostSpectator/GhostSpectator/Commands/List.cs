using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;

namespace GhostSpectator.Commands
{
	public class List : ICommand
	{
		public string Command { get; } = "list";

		public string[] Aliases { get; } = new string[]
		{
			"l"
		};

		public string Description { get; } = "Get list of all Ghosts.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = "GhostSpectator is not enabled.";
				return false;
			}
			if (GhostSpectator.List.IsEmpty())
			{
				response = "There are no Ghosts.";
				return true;
			}
			response = "List of Ghosts:" + string.Join("\n- ", from player in GhostSpectator.List select player.Nickname);
			return true;
		}
	}
}
