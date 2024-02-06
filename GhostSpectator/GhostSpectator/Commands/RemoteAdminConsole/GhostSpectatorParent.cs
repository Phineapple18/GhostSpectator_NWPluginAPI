using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NorthwoodLib.Pools;
using PluginAPI.Core;

namespace GhostSpectator.Commands.RemoteAdminConsole
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GhostSpectatorParent : ParentCommand
	{
		public GhostSpectatorParent()
		{
            translation = CommandTranslation.commandTranslation ??= CommandTranslation.PrepareTranslations();
            Command = !string.IsNullOrWhiteSpace(translation.GhostSpectatorParentCommand) ? translation.GhostSpectatorParentCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.GhostSpectatorParentDescription) ? translation.GhostSpectatorParentDescription : _description;
            Aliases = translation.GhostSpectatorParentAliases;
            Log.Debug("Loaded GhostSpectator parent command.", translation.Debug, "GhostSpectator");
            this.LoadGeneratedCommands();
		}

		public sealed override void LoadGeneratedCommands()
		{
            this.RegisterCommand(new Despawn(translation.DespawnCommand, translation.DespawnDescription, translation.DespawnAliases));
            this.RegisterCommand(new List(translation.ListCommand, translation.ListDescription, translation.ListAliases));
            this.RegisterCommand(new Spawn(translation.SpawnCommand, translation.SpawnDescription, translation.SpawnAliases));
            Log.Info($"Registered {this.AllCommands.Count()} command(s) for GhostSpectatorParent.", "GhostSpectator");
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = translation.NotEnabled;
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
            stringBuilder.AppendLine($"{Description} {translation.Subcommands}:");
            foreach (ICommand command in this.AllCommands)
			{
                stringBuilder.AppendLine($"- {command.Command} | {translation.Aliases}: {(command.Aliases == null || command.Aliases.IsEmpty() ? "" : string.Join(", ", command.Aliases))} | {translation.Description}: {command.Description}");
            }
            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd(Array.Empty<char>());
			return true;
		}

        internal const string _command = "ghostspectator";

        internal const string _description = "Parent command for GhostSpectator.";

        internal static readonly string[] _aliases = new string[] { "ghost", "gsp", "gs" };

        private readonly CommandTranslation translation;

        public override string Command { get; }
        public override string[] Aliases { get; }
        public override string Description { get; }
    }
}
