using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole.Duel
{
    internal class ListDuel : ICommand
    {
        public ListDuel (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = aliases;
            Log.Debug("Loaded ListDuel subcommand.", CommandTranslation.commandTranslation.Debug, "GhostSpectator");
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
            response = $"{translation.DuelList}: \n- {string.Join("\n- ", from kvp in DuelParent.list where kvp.Value == commandsender select kvp.Key.Nickname)}";
            return true;
        }

        internal const string _command = "list";

        internal const string _description = "Print a list of all players, who challenged you to a duel.";

        internal static readonly string[] _aliases = new string[] { "l" };

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
