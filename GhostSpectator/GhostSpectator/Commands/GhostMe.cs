using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NWAPIPermissionSystem;
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

		public string Description { get; } = "Change yourself to Ghost from Spectator or vice versa.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
                response = Plugin.notEnabled;
                return false;
			}
            Config config = Plugin.Singleton.PluginConfig;
            if (!sender.CheckPermission("gs.spawn.self"))
			{
				response = config.Translation.NoPerms;
                return false;
            }
			Player commandsender = Player.Get(sender);	
			if (commandsender == null)
			{
				response = config.Translation.SenderNull;
                return false;
            }
            if (!Round.IsRoundStarted)
            {
                response = config.Translation.BeforeRound;
                return false;
            }
            if (Warhead.IsDetonated && config.DespawnOnDetonation && !sender.CheckPermission("gs.warhead"))
            {
                response = config.Translation.AfterWarhead;
                return false;
            }
            if (commandsender.IsGhost())
			{
				GhostSpectator.Despawn(commandsender, true);
                response = config.Translation.SelfSpec;
                return true;
			}
			if (commandsender.Role == RoleTypeId.Spectator)
			{
				GhostSpectator.Spawn(commandsender);
                response = config.Translation.SelfGhost;
                return true;
			}
            response = config.Translation.SelfFail;
            return false;
		}
	}
}
