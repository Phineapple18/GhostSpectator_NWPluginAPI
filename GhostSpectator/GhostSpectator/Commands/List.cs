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

		public string Description { get; } = "Print list of all Ghosts.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = Plugin.notEnabled;
				return false;
			}
			response = $"{Plugin.Singleton.PluginConfig.Translation.GhostList}:" + string.Join("\n- ", from player in GhostSpectator.List select player.Nickname);
			return true;
		}
	}
}
