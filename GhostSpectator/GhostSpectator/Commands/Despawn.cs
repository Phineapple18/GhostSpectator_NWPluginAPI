using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands
{
	public class Despawn : ICommand, IUsageProvider
	{
		public string Command { get; } = "despawn";

		public string[] Aliases { get; } = new string[]
		{
			"d"
		};

		public string Description { get; } = "Despawn selected player from Ghost to spectator (true and default) or just Tutorial (false).";

		public string[] Usage { get; } = new string[]
		{
			"PlayerID",
			"true/false"
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
			if (arguments.IsEmpty())
			{
				response = $"Usage: {Usage[0]} {Usage[1]}.";
				return false;
			}
			if (!int.TryParse(arguments.At(0), out int id))
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
            if (!player.IsGhost())
			{
				response = "Player is not a Ghost.";
				return false;
			}
			GhostSpectator.Despawn(player, arguments.Count == 1 || !bool.TryParse(arguments.At(1), out bool toSpec) || toSpec);
			response = $"Player {player.Nickname} was despawned from Ghost.";
			Log.Debug($"{response} by {sender.LogName}.", config.Debug, Plugin.Singleton.pluginHandler.PluginName);
			return true;
		}
	}
}
