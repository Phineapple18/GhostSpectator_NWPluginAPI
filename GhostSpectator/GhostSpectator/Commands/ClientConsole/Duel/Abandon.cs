using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole.Duel
{
    internal class Abandon : ICommand
    {
        public Abandon(string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = aliases;
            Log.Debug("Loaded Abort subcommand.", CommandTranslation.commandTranslation.Debug, "GhostSpectator");
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandTranslation translation = CommandTranslation.commandTranslation;
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
            Player commandsender = Player.Get(sender);
            if (!commandsender.IsGhost())
            {
                response = translation.NotGhostSelf;
                return false;
            }
            Player opponent = commandsender.GetGhostComponent().duelPartner;
            if (opponent == null)
            {
                response = translation.NoActiveDuel;
                return false;
            }
            commandsender.GetGhostComponent().duelPartner = null;
            opponent.GetGhostComponent().duelPartner = null;
            opponent.ReceiveHint(translation.DuelAbandoned.Replace("%player%", commandsender.Nickname), 5);
            response = translation.DuelAbandonSuccess;
            Log.Debug($"Player {commandsender.Nickname} has abadndoned a duel with {opponent.Nickname}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Abandon");
            return true;
        }

        internal const string _command = "abort";

        internal const string _description = "Abandon your current duel.";

        internal static readonly string[] _aliases = new string[] { "ab" };

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
