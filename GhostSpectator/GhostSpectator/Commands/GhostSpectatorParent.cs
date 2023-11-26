using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
				response = Plugin.notEnabled;
				return false;
			}
			Translation translation = Plugin.Singleton.PluginConfig.Translation;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			stringBuilder.AppendLine($"{translation.ParentDescription} {translation.Subcommands}:");

            foreach (ICommand command in this.AllCommands)
			{
                stringBuilder.AppendLine($"- {command.Command} | {translation.Aliases}: {(command.Aliases.Length == 0 ? "None" : string.Join(", ", command.Aliases))} | {translation.Description}: {translation.Descriptions().First(c => c.Key.ToLower() == $"{command.Command}description").Value}");
            }
            response = StringBuilderPool.Shared.ToStringReturn(stringBuilder).TrimEnd(Array.Empty<char>());
			return true;
		}
	}
}
