using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CommandSystem;
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
            if (!sender.CheckPermission("gs.spawn.others"))
			{
				response = translation.NoPerms;
				return false;
			}
            if (arguments.IsEmpty())
			{
				response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
				return false;
			}
            int index = bool.TryParse(arguments.At(0), out bool tutorial) ? 1 : 0;
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
            StringBuilder success = new ($"{translation.DepawnSuccess}: ");
            StringBuilder failure = new ($"{translation.DepawnFail}: ");
            foreach (ReferenceHub hub in validHubs)
            {
                Player player = Player.Get(hub);
                if (player.IsServer)
                {
                    continue;
                }
                if (!player.IsGhost())
                {
                    failure.Append($"{player.Nickname}|");
                    continue;
                }
                GhostSpectator.Despawn(player, !tutorial);
                success.Append($"{player.Nickname}|");
            }
            success.Replace("%num%", success.ToString().Count(c => c.Equals('|')).ToString());
            failure.Replace("%num%", failure.ToString().Count(c => c.Equals('|')).ToString());

			response = success.AppendLine().Append(failure).ToString();
            string debug = Regex.Replace(response, "<.*?>", "").Replace(Environment.NewLine, "| ");
            Log.Debug($"{debug}", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Despawn");
            return true;
		}

        internal static readonly string _command = "despawn";

        internal static readonly string _description = "Despawn selected player(s) from Ghost to Tutorial (true) or Spectator (false = default option).";

        internal static readonly string[] _aliases = new string[] { "d" };
    }
}
