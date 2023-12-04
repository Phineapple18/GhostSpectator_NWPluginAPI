using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NorthwoodLib.Pools;

namespace GhostSpectator.Commands.RemoteAdminConsole
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GhostSpectatorParent : ParentCommand
	{
		public GhostSpectatorParent()
		{
            translation = CommandTranslation.PrepareTranslations();
            Command = !string.IsNullOrWhiteSpace(translation.ParentCommand) ? translation.ParentCommand : CommandTranslation._parentCommand;
            Description = !string.IsNullOrWhiteSpace(translation.ParentDescription) ? translation.ParentDescription: CommandTranslation._parentDescription;
            Aliases = !translation.ParentAliases.IsEmpty() ? translation.ParentAliases : CommandTranslation._parentAliases;
            this.LoadGeneratedCommands();
		}

		public override string Command { get; }

		public override string[] Aliases { get; }

        public override string Description { get; }

		public sealed override void LoadGeneratedCommands()
		{
            this.RegisterCommand(new Despawn(translation.DespawnCommand, translation.DespawnDescription, translation.DespawnAliases));
            this.RegisterCommand(new List(translation.ListCommand, translation.ListDescription, translation.ListAliases));
            this.RegisterCommand(new Spawn(translation.SpawnCommand, translation.SpawnDescription, translation.SpawnAliases));
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
				stringBuilder.AppendLine($"- {command.Command} | {translation.Aliases}: {(command.Aliases.Length == 0 ? "" : string.Join(", ", command.Aliases))} | {translation.Description}: {command.Description}");
            }
            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd(Array.Empty<char>());
			return true;
		}

        private readonly CommandTranslation translation;
    }
}
