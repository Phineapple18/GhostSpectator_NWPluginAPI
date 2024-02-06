using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using MEC;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole.Duel
{
    public class Accept : ICommand
    {
        public Accept (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = aliases;
            Log.Debug("Loaded Accept subcommand.", CommandTranslation.commandTranslation.Debug, "GhostSpectator");
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
            if (!sender.CheckPermission("gs.duel"))
            {
                response = translation.NoPerms;
                return false;
            }
            if (Warhead.IsDetonated && Plugin.Singleton.PluginConfig.DespawnOnDetonation && !sender.CheckPermission("gs.warhead"))
            {
                response = translation.AfterWarhead;
                return false;
            }
            Player commandsender = Player.Get(sender);
            if (!commandsender.IsGhost())
            {
                response = translation.NotGhostSelf;
                return false;
            }
            if (!DuelParent.list.Values.Contains(commandsender))
            {
                response = translation.NoDuelRequests;
                return false;
            }
            List<Player> playerList = (from p in DuelParent.list where p.Value == commandsender select p.Key).ToList();
            Player requester = arguments.IsEmpty() ? playerList.First() : playerList.FirstOrDefault(p => p.Nickname == arguments.At(0));
            foreach (Player player in playerList)
            {
                if (player != requester)
                {
                    player.ReceiveHint(translation.DuelRequestExpired.Replace("%player%", commandsender.Nickname), 5);
                }
                DuelParent.list.Remove(player);
            }
            Timing.RunCoroutine(DuelParent.PrepareDuel(requester, commandsender));
            response = translation.DuelAcceptSuccess.Replace("%player", requester.Nickname);
            Log.Debug($"Player {commandsender.Nickname} has accepted a duel request from {requester.Nickname}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Accept");
            return true;
        }

        internal const string _command = "accept";

        internal const string _description = "Accept duel offer from player. If you have multiple offers, first one will be accepted, unless you provide a player nickname.";

        internal static readonly string[] _aliases = new string[] { "a" };

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
