using PluginAPI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator
{
    public class Translation
    {
        public string Ghost { get; set; } = "GHOST";

        public string SpawnMessage { get; set; } = "<size=50><color=%colour%>You are a Ghost!</color>\n<size=30>Drop the %TeleportItem% to teleport to a random player.</size>";

        public string TeleportSuccess { get; set; } = "You were teleported to <color=green>%player%</color>.";

        public string TeleportFail { get; set; } = "There is nobody you can teleport to.";

        public string NotEnabled { get; set; } = "GhostSpectator is not enabled.";

        public string Aliases { get; set; } = "Aliases";

        public string Description { get; set; } = "Description";

        public string Usage { get; set; } = "Usage";

        public string Subcommands { get; set; } = "Available subcommands";

        public string ParentDescription { get; set; } = "Parent command for GhostSpectator.";

        public string SpawnDescription { get; set; } = "Spawn selected player(s) as a Ghost.";

        public string DespawnDescription { get; set; } = "Despawn selected player(s) from Ghost to Tutorial (true) or Spectator (false = default option).";

        public string GhostMeDescription { get; set; } = "Change yourself to Ghost from Spectator or vice versa by typing this command.";

        public string ListDescription { get; set; } = "Print list of all Ghosts.";

        public string NoPerms { get; set; } = "You don't have permission to use that command.";    

        public string BeforeRound { get; set; } = "You can't use that command before round start.";   

        public string AfterWarhead { get; set; } = "You can't use that command after warhead detonation.";            

        public string NoPlayers { get; set; } = "Provided player(s) doesn't exist.";      

        public string DedicatedServer { get; set; } = "You can't use that command on Dedicated Server.";

        public string SenderNull { get; set; } = "Command sender is null.";

        public string GhostList { get; set; } = "List of Ghosts";

        public string SpawnSuccess { get; set; } = "Succesfully turned <color=green>%num%</color> existing player(s) into Ghosts";     

        public string SpawnFail { get; set; } = "Command failed for <color=red>%num%</color> existing player(s) (already a Ghost)";    

        public string DepawnSuccess { get; set; } = "Succesfully despawned <color=green>%num%</color> existing player(s) from Ghosts";      

        public string DepawnFail { get; set; } = "Command failed for <color=red>%num%</color> existing player(s) (not a Ghost)";      

        public string SelfGhost { get; set; } = "You have changed yourself to Ghost.";

        public string SelfSpec { get; set; } = "You have changed yourself to Spectator.";

        public string SelfFail { get; set; } = "You can't use this command, if you are neither dead nor Ghost.";

        public Dictionary<string, string> Descriptions()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                { nameof(DespawnDescription), DespawnDescription },
                { nameof(SpawnDescription), SpawnDescription },
                { nameof(ParentDescription), ParentDescription },
                { nameof(GhostMeDescription), GhostMeDescription },
                { nameof(ListDescription), ListDescription }
            };
            return dict;
        }
    }
}
