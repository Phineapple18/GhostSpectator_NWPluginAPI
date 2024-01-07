using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CommandSystem;
using Discord;
using NorthwoodLib.Pools;
using NWAPIPermissionSystem;
using PluginAPI.Core;
using Utils;

namespace GhostSpectator.Commands.RemoteAdminConsole
{
    public class Spawn : ICommand, IUsageProvider
	{
        public Spawn (string command, string description, string[] aliases)
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
			"%player%/\"all\""
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
            if (!Round.IsRoundStarted)
			{
				response = translation.BeforeRound;
				return false;
			}
			if (Warhead.IsDetonated && Plugin.Singleton.PluginConfig.DespawnOnDetonation && !sender.CheckPermission("gs.warhead"))
			{
				response = translation.AfterWarhead;
				return false;
			}
			if (arguments.IsEmpty())
			{
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
			}
			List<ReferenceHub> validHubs = arguments.At(0).ToLower() == "all" ? ReferenceHub.AllHubs.ToList() : RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out string[] array);
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
            StringBuilder success = StringBuilderPool.Shared.Rent(); 
            StringBuilder failure = StringBuilderPool.Shared.Rent();
            success.AppendLine($"{translation.SpawnSuccess}:");
            failure.AppendLine($"{translation.SpawnFail}:");
            int numS = 0;
            int numF = 0;
            foreach (ReferenceHub hub in validHubs)
			{
				Player player = Player.Get(hub);
                if (player.IsServer || player.IsGhost())
                {
                    failure.AppendLine($"- {player.Nickname}");
                    numF++;
                    continue;
                }
                GhostExtensions.Spawn(player);
                success.AppendLine($"- {player.Nickname}");
                numS++;
            }
            success.Replace("%num%", numS.ToString());
            failure.Replace("%num%", numF.ToString());

            StringBuilder result = numS == 0 ? failure : numF == 0 ? success : success.Append(failure);
            response = StringBuilderPool.Shared.ToStringReturn(result).TrimEnd(Array.Empty<char>());
            string debug = Regex.Replace(response, "<.*?>", "").Replace("- ", "").Replace(Environment.NewLine, ". ");
            Log.Debug($"{debug}", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Spawn");
            return true;
		}

        internal static readonly string _command = "spawn";

        internal static readonly string _description = "Spawn selected player(s) as a Ghost.";

        internal static readonly string[] _aliases = new string[] { "s" };
    }
}
