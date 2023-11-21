using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands
{
	public class Spawn : ICommand, IUsageProvider
	{
		public string Command { get; } = "spawn";

		public string[] Aliases { get; } = new string[]
		{
			"s"
		};

		public string Description { get; } = "Spawns selected player as a Ghost.";

		public string[] Usage { get; } = new string[]
		{
			"PlayerID"
		};

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = "GhostSpectator is not enabled.";
				return false;
			}
			Player commandsender = Player.Get(sender);
			var config = Plugin.Singleton.PluginConfig;
			if (commandsender == null || !config.GhostOthers.Contains(commandsender.GetGroup()))
			{
				response = "You don't have permission to use that command.";
				return false;
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
			if (arguments.IsEmpty())
			{
				response = $"Usage: {Usage[0]}.";
				return false;
			}
			if (!int.TryParse(arguments.At(0).ToString(), out int id))
			{
				response = "Player ID must be a number.";
				return false;
			}
			if (!Player.TryGet(id, out Player player))
			{
				response = $"Player with ID {id} not found.";
				return false;
			}
            if (player == null)
            {
                response = "Player is null.";
                return false;
            }
            if (player.IsGhost())
			{
				response = "Player is already Ghost.";
				return false;
			}
			GhostSpectator.Spawn(player);
			response = $"Player {player.Nickname} was turned into Ghost.";
			Log.Debug($"{response} by {sender.LogName}.", config.Debug, Plugin.Singleton.pluginHandler.PluginName);
			return true;
		}
	}
}
