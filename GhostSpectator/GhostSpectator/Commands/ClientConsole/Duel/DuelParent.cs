using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using GhostSpectator.Extensions;
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
            translation = Translation.AccessTranslation();
            commandName = $"{Translation.pluginName}.{this.GetType().Name}";
            Command = !string.IsNullOrWhiteSpace(translation.DuelParentCommand) ? translation.DuelParentCommand : _command;
            Description = translation.DuelParentDescription;
            Aliases = translation.DuelParentAliases;
            Log.Debug($"Registered {this.Command} parent command.", translation.Debug, Translation.pluginName);
            this.LoadGeneratedCommands();
        }

        public sealed override void LoadGeneratedCommands()
        {
            this.RegisterCommand(new Accept(translation.AcceptCommand, translation.AcceptDescription, translation.AcceptAliases));
            this.RegisterCommand(new Cancel(translation.CancelCommand, translation.CancelDescription, translation.CancelAliases));
            this.RegisterCommand(new ListDuel(translation.ListduelCommand, translation.ListduelDescription, translation.ListduelAliases));
            this.RegisterCommand(new Reject(translation.RejectCommand, translation.RejectDescription, translation.RejectAliases));
            Log.Debug($"Registered {this.AllCommands.Count()} command(s) for DuelParent.", translation.Debug, Translation.pluginName);
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Singleton == null)
            {
                response = translation.NotEnabled;
                Log.Debug($"Plugin {Translation.pluginName} is not enabled.", translation.Debug, commandName);
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
                Log.Debug("Command sender is null.", Config.Debug, commandName);
                return false;
            }
            if (!sender.CheckPermission("gs.duel"))
            {
                response = translation.NoPerms;
                Log.Debug($"Player {sender.LogName} doesn't have required permission to use this command.", Config.Debug, commandName);
                return false;
            }
            Player commandsender = Player.Get(sender);
            if (!commandsender.IsGhost())
            {
                response = translation.NotGhost;
                Log.Debug($"Player {commandsender.Nickname} is not a Ghost.", Config.Debug, commandName);
                return false;
            }
            GhostComponent component = commandsender.GetGhostComponent();
            if (component.DuelPartner != null)
            {
                response = translation.ActiveDuelSelf.Replace("%playernick%", component.DuelPartner.Nickname);
                Log.Debug($"Player {commandsender.Nickname} has already active duel with {component.DuelPartner.Nickname}.", Config.Debug, commandName);
                return false;
            }
            List<Player> players = new();
            string nickname = string.Join(" ", arguments);
            var ghostList = GhostExtensions.GhostPlayerList.Where(p => p != commandsender).ToList();
            if (ghostList.Any(p => string.Equals(p.Nickname, nickname, StringComparison.OrdinalIgnoreCase)))
            {
                players = ghostList.Where(p => string.Equals(p.Nickname, nickname, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            else
            {
                players = ghostList.Where(p => p.Nickname.IndexOf(nickname, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            if (players.IsEmpty())
            {
                response = translation.NoGhosts;
                Log.Debug($"There is no Ghost, that is not {commandsender.Nickname}, with/containing provided nickname.", Config.Debug, commandName);
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
            if (opponent.GetGhostComponent().DuelPartner != null)
            {
                response = translation.ActiveDuelOther.Replace("%playernick%", opponent.Nickname);
                Log.Debug($"Player {commandsender.Nickname} can't challenge {opponent.Nickname} to duel as they already have active duel.", Config.Debug, commandName);
                return false;
            }
            if (DuelExtensions.DuelRequests.TryGetValue(commandsender, out Tuple<Player, int> previousOpponent) && previousOpponent.Item1 == opponent)
            {
                response = translation.RequestAlreadySent;
                Log.Debug($"Player {commandsender.Nickname} already sent duel request to {opponent.Nickname}.", Config.Debug, commandName);
                return false;
            }
            commandsender.RequestDuel(opponent, previousOpponent?.Item1);
            response = translation.DuelParentSuccess.Replace("%playernick%", opponent.Nickname);
            Log.Debug($"Player {commandsender.Nickname} has challenged {opponent.Nickname} to a duel.", Config.Debug, commandName);
            return true;
        }

        internal const string _command = "duel";

        internal const string _description = "Parent command. Challenge another Ghost to a duel by typing their nickname, whole or part of it. The case is ignored.";

        internal static readonly string[] _aliases = Array.Empty<string>();

        private readonly string commandName;

        private static Translation translation;

        public override string Command { get; }
        public override string Description { get; }
        public override string[] Aliases { get; }
        public bool SanitizeResponse { get; }
        private static Config Config => Plugin.Singleton.pluginConfig;
    }
}
