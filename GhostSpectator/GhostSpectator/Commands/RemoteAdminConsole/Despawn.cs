using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CommandSystem;
using NorthwoodLib.Pools;
using NWAPIPermissionSystem;
using PluginAPI.Core;
using Utils;

namespace GhostSpectator.Commands.RemoteAdminConsole
{
	public class Despawn : ICommand, IUsageProvider
	{
        public Despawn (string command, string description, string[] aliases)
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
            "(\"true/false\")",
            "%player%\"all\""
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
            if (!sender.CheckPermission("gs.spawn.player"))
			{
				response = translation.NoPerms;
				return false;
			}
            if (arguments.IsEmpty())
			{
				response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
				return false;
			}
            int index = bool.TryParse(arguments.At(0), out bool toTutorial) ? 1 : 0;
            List<ReferenceHub> validHubs = arguments.At(index).ToLower() == "all" ? ReferenceHub.AllHubs.ToList() : RAUtils.ProcessPlayerIdOrNamesList(arguments, index, out string[] array);
            if (validHubs.IsEmpty())
            {
                response = translation.NoPlayers;
                return false;
            }
			if (validHubs.Count == 1 && validHubs[0].isLocalPlayer)
			{
                response = translation.DedicatedServer;
                return false;
            }
            //validHubs.Remove(validHubs.FirstOrDefault(h => h.isLocalPlayer));
            StringBuilder success = StringBuilderPool.Shared.Rent();
            StringBuilder failure = StringBuilderPool.Shared.Rent();
            success.AppendLine($"{translation.SpawnSuccess}:");
            failure.AppendLine($"{translation.SpawnFail}:");
            int numS = 0;
            int numF = 0;
            foreach (ReferenceHub hub in validHubs)
            {
                Player player = Player.Get(hub);
                if (player.IsServer || !player.IsGhost())
                {
                    failure.AppendLine($"- {player.Nickname}");
                    numF++;
                    continue;
                }
                GhostExtensions.Despawn(player, !toTutorial);
                success.AppendLine($"- {player.Nickname}");
                numS++;
            }
            success.Replace("%num%", numS.ToString());
            failure.Replace("%num%", numF.ToString());

            StringBuilder result = numS == 0 ? failure : numF == 0 ? success : success.Append(failure);
            response = StringBuilderPool.Shared.ToStringReturn(result).TrimEnd(Array.Empty<char>());
            string debug = Regex.Replace(response, "<.*?>", "").Replace("- ", "").Replace(Environment.NewLine, ". ");
            Log.Debug($"{debug}", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Despawn");
            return true;
		}

        internal static readonly string _command = "despawn";

        internal static readonly string _description = "Despawn selected player(s) from Ghost to Tutorial (true) or Spectator (false = default option).";

        internal static readonly string[] _aliases = new string[] { "d" };
    }
}
