using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole.Voicechat
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class EnableVoicechat : ICommand, IUsageProvider
    {
        public EnableVoicechat()
        {
            translation = CommandTranslation.commandTranslation;
            Command = !string.IsNullOrWhiteSpace(translation.EnableVoicechatCommand) ? translation.EnableVoicechatCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.EnableVoicechatDescription) ? translation.EnableVoicechatDescription : _description;
            Aliases = translation.EnableVoicechatAliases;
            Log.Debug("Loaded EnableVoicechat command.", translation.Debug, "GhostSpectator");
        }

        public string[] Usage { get; } = new string[]
        {
            "scp/dead/ghost/all"
        };

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
            if (!(sender.CheckPermission("gs.listen.scp") || sender.CheckPermission("gs.listen.dead") || sender.CheckPermission("gs.listen.ghost")))
            {
                response = translation.NoPerms;
                return false;
            }
            if (arguments.IsEmpty())
            {
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
            }
            if (!arguments.Any(a => availableVoicechats.Keys.Contains(a.ToLower()) || a.ToLower() == "all"))
            {
                response = translation.WrongArgument;
                return false;
            }
            Player commandsender = Player.Get(sender);
            List<string> voicechats = arguments.Any(a => a.ToLower() == "all") ? availableVoicechats.Keys.ToList() : arguments.ToList();
            StringBuilder result = new ($"{translation.EnableVoicechatSuccess}:");
            bool success = false;
            foreach (string chat in voicechats)
            {
                if (availableVoicechats.Keys.Contains(chat) && commandsender.CheckPermission($"gs.listen.{chat}") && commandsender.TemporaryData.Add(availableVoicechats[chat].Key, "1"))
                {
                    result.Append($" {availableVoicechats[chat].Value},");
                    success = true;
                }
            }
            response = success ? result.Replace(',', '.', result.Length - 1, 1).ToString() : translation.EnableVoicechatFail;
            Log.Debug($"Player {commandsender.Nickname} {(success ? "" : "un")}successfully enabled themselves voicechannel(s).", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.EnableVoicechannel");
            return success;
        }

        internal static readonly Dictionary<string, KeyValuePair<string, string>> availableVoicechats = new()
        {
            { "scp", new ("VcScpChat", "SCPs") },
            { "dead", new ("VcSpectator", "Spectators") },
            { "ghost", new ("ListenGhosts", "Ghosts") }
        };

        internal const string _command = "enablevoicechat";

        internal const string _description = "Enable listening to selected voicechat(s) as a Ghost.";

        internal static readonly string[] _aliases = new string[] { "evc" };

        private readonly CommandTranslation translation;

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
