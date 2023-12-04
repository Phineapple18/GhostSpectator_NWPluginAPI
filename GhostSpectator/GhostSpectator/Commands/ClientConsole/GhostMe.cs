using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NWAPIPermissionSystem;
using PlayerRoles;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole
{
    public class GhostMe : ICommand
	{
        public GhostMe (string command, string description, string[] aliases) 
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
            if (!sender.CheckPermission("gs.spawn.self"))
			{
				response = translation.NoPerms;
                return false;
            }
            if (!Round.IsRoundStarted)
            {
                response = translation.BeforeRound;
                return false;
            }
            if (Warhead.IsDetonated && Plugin.Singleton.PluginConfig.DespawnOnDetonation && !sender.CheckPermission("gs.warhead"))
            {
                response = translation.AfterWarhead;
                return false;
            }
            Player commandsender = Player.Get(sender);
            if (commandsender.IsGhost())
			{
				GhostSpectator.Despawn(commandsender, true);
                response = translation.SelfSpec;
                return true;
			}
			if (commandsender.Role == RoleTypeId.Spectator)
			{
				GhostSpectator.Spawn(commandsender);
                response = translation.SelfGhost;
                return true;
			}
            response = translation.SelfFail;
            return false;
		}

        internal static readonly string _command = "ghostme";

        internal static readonly string _description = "Change yourself to Ghost from Spectator or vice versa.";

        internal static readonly string[] _aliases = new string[] { "gme", "me" };
    }
}
