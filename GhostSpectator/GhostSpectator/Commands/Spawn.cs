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

namespace GhostSpectator.Commands
{
	public class Spawn : ICommand, IUsageProvider
	{
		public string Command { get; } = "spawn";

		public string[] Aliases { get; } = new string[]
		{
			"s"
		};

		public string Description { get; } = "Spawn selected player(s) as a Ghost.";

		public string[] Usage { get; } = new string[]
		{
			"%player% or all"
		};

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (Plugin.Singleton == null)
			{
				response = Plugin.notEnabled;
				return false;
			}
            Config config = Plugin.Singleton.PluginConfig;
            if (!sender.CheckPermission("gs.spawn.others"))
            {
                response = config.Translation.NoPerms;
                return false;
            }
            if (!Round.IsRoundStarted)
			{
				response = config.Translation.BeforeRound;
				return false;
			}
			if (Warhead.IsDetonated && config.DespawnOnDetonation && !sender.CheckPermission("gs.warhead"))
			{
				response = config.Translation.AfterWarhead;
				return false;
			}
			if (arguments.IsEmpty())
			{
                response = $"{config.Translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
			}
			List<ReferenceHub> validHubs = arguments.At(0).ToLower() == "all" ? ReferenceHub.AllHubs.ToList() : RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out string[] array);
			if (validHubs.IsEmpty())
			{
                response = config.Translation.NoPlayers;
                return false;
            }
            if (validHubs.Count == 1 && validHubs[0].isLocalPlayer)
            {
                response = config.Translation.DedicatedServer;
                return false;
            }
            StringBuilder success = new ($"{config.Translation.SpawnSuccess}: ");
            StringBuilder failure = new ($"{config.Translation.SpawnFail}: ");
            foreach (var hub in validHubs)
			{
				Player player = Player.Get(hub);
				if (player.IsServer)
				{
					continue;
				}
                if (player.IsGhost())
                {
                    failure.Append($"{player.Nickname}|");
                    continue;
                }
                GhostSpectator.Spawn(player);
                success.Append($"{player.Nickname}|");
            }
            success.Replace("%num%", success.ToString().Count(c => c.Equals('|')).ToString());
            failure.Replace("%num%", failure.ToString().Count(c => c.Equals('|')).ToString());

            response = success.AppendLine().Append(failure).ToString();
            string debug = Regex.Replace(response, "<.*?>", "").Replace(Environment.NewLine, "| ");
            Log.Debug($"{debug}", config.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.Spawn");
            return true;
		}
	}
}
