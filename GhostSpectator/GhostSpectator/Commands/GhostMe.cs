using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;

namespace GhostSpectator.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class GhostMe : ICommand
	{
		public string Command { get; } = "ghostme";

		public string[] Aliases { get; } = new string[]
		{
			"gme"
		};

		public string Description { get; } = "Change yourself to Ghost from Spectator or vice versa by typing this command.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = "GhostSpectator is not enabled.";
				return false;
			}
			Player commandsender = Player.Get(sender);
			var config = Plugin.Singleton.PluginConfig;
			if (commandsender == null || !config.GhostSelf.Contains(commandsender.GetGroup()))
			{
				response = "You don't have permission to use that command.";
				return false;
			}
			if (commandsender.IsGhost())
			{
				GhostSpectator.Despawn(commandsender, true);
				response = "You have changed yourself to Spectator.";
				return true;
			}
			if (!Round.IsRoundStarted)
			{
				response = "You can't turn into Ghost before round start.";
				return false;
			}
			if (Warhead.IsDetonated && config.DespawnOnDetonation && !config.GhostAfterWarhead.Contains(commandsender.GetGroup()))
			{
				response = "You can't turn into Ghost after warhead detonation.";
				return false;
			}
			if (commandsender.Role == RoleTypeId.Spectator)
			{
				GhostSpectator.Spawn(commandsender);
				response = "You have changed yourself to Ghost.";
				return true;
			}
			response = "You can't use that command, if you are neither dead nor Ghost.";
			return false;
		}
	}
}
