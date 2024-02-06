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
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GhostMe : ICommand
	{
        public GhostMe () 
        {
            translation = CommandTranslation.commandTranslation;
            Command = !string.IsNullOrWhiteSpace(translation.GhostmeCommand) ? translation.GhostmeCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.GhostmeDescription) ? translation.GhostmeDescription : _description;
            Aliases = translation.GhostmeAliases;
            Log.Debug("Loaded GhostMe command.", translation.Debug, "GhostSpectator");
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
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
				GhostExtensions.Despawn(commandsender);
                response = translation.SelfSpec;
                Log.Debug($"Player {commandsender.Nickname} turned himself to Spectator by command.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.GhostMe");
                return true;
			}
			if (commandsender.Role == RoleTypeId.Spectator)
			{
				GhostExtensions.Spawn(commandsender);
                response = translation.SelfGhost;
                Log.Debug($"Player {commandsender.Nickname} turned himself to a Ghost by command.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.GhostMe");
                return true;
			}
            response = translation.SelfFail;
            return false;
		}

        internal const string _command = "ghostme";

        internal const string _description = "Change yourself to a Ghost from Spectator or vice versa.";

        internal static readonly string[] _aliases = new string[] { "gme", "me" };

        private readonly CommandTranslation translation;

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
