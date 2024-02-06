using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole.Duel
{
    public class Cancel : ICommand
    {
        public Cancel (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = aliases;
            Log.Debug("Loaded Cancel subcommand.", CommandTranslation.commandTranslation.Debug, "GhostSpectator");
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
            if (!DuelParent.list.ContainsKey(commandsender))
            {
                response = translation.NoDuelRequests;
                return false;
            }
            Player opponent = DuelParent.list[commandsender];
            DuelParent.list.Remove(commandsender);
            opponent.ReceiveHint(translation.DuelRequestCancelled.Replace("%player%", commandsender.Nickname), 5);
            response = translation.DuelCancelSuccess.Replace("%player%", opponent.Nickname);
            Log.Debug($"Player {commandsender.Nickname} has cancelled a duel request with {opponent.Nickname}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Cancel");
            return true;
        }

        internal const string _command = "cancel";

        internal const string _description = "Cancel duel offer with currently challenged player.";

        internal static readonly string[] _aliases = new string[] { "cnx", "c" };

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
