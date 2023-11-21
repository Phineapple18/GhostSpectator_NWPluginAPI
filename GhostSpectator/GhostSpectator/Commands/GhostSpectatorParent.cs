using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using NorthwoodLib.Pools;

namespace GhostSpectator.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GhostSpectatorParent : ParentCommand
	{
		public GhostSpectatorParent()
		{
			this.LoadGeneratedCommands();
		}

		public override string Command { get; } = "ghostspectator";

		public override string[] Aliases { get; } = new string[]
		{
			"gs"
		};

		public override string Description { get; } = "Parent command for GhostSpectator.";

		public sealed override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new Despawn());
			this.RegisterCommand(new GhostMe());
			this.RegisterCommand(new Spawn());
			this.RegisterCommand(new List());
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = "GhostSpectator is not enabled.";
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			stringBuilder.AppendLine(Description + " Available subcommands:");
			foreach (ICommand command in this.AllCommands)
			{
				stringBuilder.AppendLine($"- {command.Command} | Aliases: {(command.Aliases.Length == 0 ? "None" : string.Join(", ", command.Aliases))} | Description: {command.Description}");
			}
			response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd(Array.Empty<char>());
			return true;
		}
	}
}
