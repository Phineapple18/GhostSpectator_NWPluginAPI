using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NorthwoodLib.Pools;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole
{
	[CommandHandler(typeof(ClientCommandHandler))]
    public class GhostSpectatorParent : ParentCommand
	{
		public GhostSpectatorParent()
		{
            translation = CommandTranslation.loadedTranslation ?? CommandTranslation.PrepareTranslations();
            Command = !string.IsNullOrWhiteSpace(translation.ParentCommand) ? translation.ParentCommand : CommandTranslation._parentCommand;
            Description = !string.IsNullOrWhiteSpace(translation.ParentDescription) ? translation.ParentDescription : CommandTranslation._parentDescription;
            Aliases = !translation.ParentAliases.IsEmpty() ? translation.ParentAliases : CommandTranslation._parentAliases;
            this.LoadGeneratedCommands();
		}

		public override string Command { get; }

        public override string[] Aliases { get; }

        public override string Description { get; }

        public sealed override void LoadGeneratedCommands()
		{
            this.RegisterCommand(new CreateTarget(translation.CreatetargetCommand, translation.CreatetargetDescription, translation.CreatetargetAliases));
            this.RegisterCommand(new DestroyTarget(translation.DestroytargetCommand, translation.DestroytargetDescription, translation.DestroytargetAliases));
            this.RegisterCommand(new DisableVoicechannel(translation.DisableVoicechannelCommand, translation.DisableVoicechannelDescription, translation.DisableVoicechannelAliases));
            this.RegisterCommand(new EnableVoicechannel(translation.EnableVoicechannelCommand, translation.EnableVoicechannelDescription, translation.EnableVoicechannelAliases));
            this.RegisterCommand(new GhostMe (translation.GhostmeCommand, translation.GhostmeDescription, translation.GhostmeAliases));
            this.RegisterCommand(new GiveFirearm(translation.GivefirearmCommand, translation.GivefirearmDescription, translation.GivefirearmAliases));
            Log.Info($"Registered {this.AllCommands.Count()} command(s) for GhostSpectatorParent (Client Console).", "GhostSpectator");
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
