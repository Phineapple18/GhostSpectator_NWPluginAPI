using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using MEC;
using NorthwoodLib.Pools;
using NWAPIPermissionSystem;
using PluginAPI.Core;
using UnityEngine;

namespace GhostSpectator.Commands.ClientConsole.Duel
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DuelParent : ParentCommand
    {
        public DuelParent()
        {
            translation = CommandTranslation.commandTranslation ??= CommandTranslation.PrepareTranslations();
            Command = !string.IsNullOrWhiteSpace(translation.DuelParentCommand) ? translation.DuelParentCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.DuelParentDescription) ? translation.DuelParentDescription : _description;
            Aliases = translation.DuelParentAliases;
            Log.Debug("Loaded Duel parent command.", translation.Debug, "GhostSpectator");
            this.LoadGeneratedCommands();
        }

        public sealed override void LoadGeneratedCommands()
        {
            this.RegisterCommand(new Abandon(translation.AbandonCommand, translation.AbandonDescription, translation.AbandonAliases));
            this.RegisterCommand(new Accept(translation.AcceptCommand, translation.AcceptDescription, translation.AcceptAliases));
            this.RegisterCommand(new Cancel(translation.CancelCommand, translation.CancelDescription, translation.CancelAliases));
            this.RegisterCommand(new ListDuel(translation.ListDuelCommand, translation.ListDuelDescription, translation.ListDuelAliases));
            Log.Info($"Registered {this.AllCommands.Count()} command(s) for DuelParent.", "GhostSpectator");
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Singleton == null)
            {
                response = translation.NotEnabled;
                return false;
            }
            if (arguments.IsEmpty())
            {
                StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
                stringBuilder.AppendLine($"{Description} \n{translation.Subcommands}:");
                foreach (ICommand command in this.AllCommands)
                {
                    stringBuilder.AppendLine($"- {command.Command} | {translation.Aliases}: {(command.Aliases == null || command.Aliases.IsEmpty() ? "" : string.Join(", ", command.Aliases))} | {translation.Description}: {command.Description}");
                }
                response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd(Array.Empty<char>());
                return true;
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
            Player commandsender = Player.Get(sender);
            if (!commandsender.IsGhost())
            {
                response = translation.NotGhostSelf;
                return false;
            }
            if (list.ContainsKey(commandsender))
            {
                response = translation.AlreadyPendingDuel.Replace("%player%", list[commandsender].Nickname);
                return false;
            }
            List<Player> players = Player.GetPlayers().Where(p => p != commandsender && p.Nickname == arguments.At(0) && p.IsGhost()).ToList();
            if (players.IsEmpty())
            {
                response = translation.NoGhosts;
                return false;
            }
            Player opponent = players.ElementAt(0);
            if (players.Count > 1)
            {
                for (int i = 1; i < players.Count; i++)
                {
                    if (Vector3.Distance(commandsender.Position, players[i].Position) < Vector3.Distance(commandsender.Position, opponent.Position))
                    {
                        opponent = players[i];
                    }
                }
            }
            if (opponent.GetGhostComponent().duelPartner != null)
            {
                response = translation.AlreadyActiveDuel.Replace("%player%", opponent.Nickname);
                return false;
            }
            list.Add(commandsender, opponent);
            Config config = Plugin.Singleton.PluginConfig;
            Timing.CallDelayed(config.DuelRequestTime, delegate ()
            {
                if (list.Any(kvp => kvp.Key == commandsender && kvp.Value == opponent))
                {
                    list.Remove(commandsender);
                    commandsender.ReceiveHint(translation.DuelRequestExpired.Replace("%player%", opponent.Nickname), 5);
                }
            });
            opponent.ReceiveHint(translation.DuelRequestReceived.Replace("%player", commandsender.Nickname).Replace("%time%", config.DuelRequestTime.ToString()), 7);
            response = translation.DuelParentSuccess.Replace("%player%", opponent.Nickname);
            Log.Debug($"Player {commandsender.Nickname} has challenged {opponent.Nickname} to a duel.", config.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Duel");
            return true;
        }

        internal static IEnumerator<float> PrepareDuel(Player player1, Player player2)
        {
            player1.ReceiveHint($"{translation.DuelPrepare}", 5);
            player2.ReceiveHint($"{translation.DuelPrepare}", 5);
            yield return Timing.WaitForSeconds(5f);
            int i = 5;
            while (player1.IsGhost() && player2.IsGhost())
            {
                if (i == 0)
                {
                    player1.GetGhostComponent().duelPartner = player2;
                    player2.GetGhostComponent().duelPartner = player1;
                    player1.ReceiveHint($"{translation.DuelStarted}", 5);
                    player2.ReceiveHint($"{translation.DuelStarted}", 5);
                    yield break;
                }
                player1.ReceiveHint($"{i}");
                player2.ReceiveHint($"{i}");
                i--;
                yield return Timing.WaitForSeconds(1f);
            }
            player1.ReceiveHint($"{translation.DuelAborted}", 5);
            player2.ReceiveHint($"{translation.DuelAborted}", 5);
            yield break;
        }

        internal const string _command = "duel";

        internal const string _description = "Challenge another Ghost to a duel by typing their nickname. Also parent command for Duel.";

        internal static readonly string[] _aliases = Array.Empty<string>();

        internal static Dictionary<Player, Player> list = new ();

        private static CommandTranslation translation;

        public override string Command { get; }
        public override string[] Aliases { get; }
        public override string Description { get; }
    }
}
