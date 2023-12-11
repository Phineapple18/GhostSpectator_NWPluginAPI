using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole
{
    internal class DisableVoicechannel : ICommand, IUsageProvider
    {
        public DisableVoicechannel(string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = !aliases.IsEmpty() ? aliases : _aliases;
        }

        public string Command { get; }

        public string[] Aliases { get; }

        public string Description { get; }

        public string[] Usage { get; } = new string[]
        {
            "\"scp\"/\"dead\"/\"ghost\"/\"all\""
        };

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
            if (!(sender.CheckPermission("gs.listen.scp") || sender.CheckPermission("gs.listen.dead") || sender.CheckPermission("gs.listen.ghost")))
            {
                response = translation.NoPerms;
                return false;
            }
            if (!Round.IsRoundStarted)
            {
                response = translation.BeforeRound;
                return false;
            }
            if (arguments.IsEmpty())
            {
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
            }
            if (!arguments.Any(a => chats.Keys.Contains(a.ToLower()) || a.ToLower() == "all"))
            {
                response = translation.WrongArgument;
                return false;
            }
            Player commandsender = Player.Get(sender);
            List<string> voicechannels = arguments.Any(a => a.ToLower() == "all") ? chats.Keys.ToList() : arguments.ToList();
            StringBuilder result = new ($"{translation.DisableVoicechatSuccess}:");
            bool success = false;
            foreach (string argument in voicechannels)
            {
                if (chats.Keys.Contains(argument) && commandsender.CheckPermission($"gs.listen.{argument}") && commandsender.TemporaryData.Remove(chats[argument].Key))
                {
                    result.Append($" {chats[argument].Value},");
                    success = true;
                }
            }
            response = success ? result.Replace(',', '.', result.Length - 1, 1).ToString() : translation.DisableVoicechatFail;
            return success;
        }

        internal static readonly string _command = "disablevoicechannel";

        internal static readonly string _description = "Disable listening to Ghosts, SCP or Spectator chat. You can provide multiple arguments or just type \"all\".";

        internal static readonly string[] _aliases = new string[] { "dvc" };

        private readonly Dictionary<string, KeyValuePair<string, string>> chats = new()
        {
            { "scp", new ("VcScpChat", "SCPs") },
            { "dead", new ("VcSpectator", "Spectators") },
            { "ghost", new ("ListenGhosts", "Ghosts") }
        };
    }
}
