﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

using PlayerRoles;

namespace GhostSpectator
{
    public class Config
    {
        [Description("Is plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Is debug enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Nickname of a Ghost, that is displayed in place of a role.")]
        public string GhostNickname { get; set; } = "GHOST";

        [Description("Color of a Ghost nickname.")]
        public string GhostColor { get; set; } = "#A0A0A0";

        [Description("Max Ghost health.")]
        public float GhostHealth { get; set; } = 150f;

        [Description("Broadcast sent to a Ghost upon spawn.")]
        public string SpawnMessage { get; set; } = "<size=50><color=%colour%>You are a Ghost!</color>\n<size=30>Drop the %TeleportItem% to teleport to a random player.</size>";

        [Description("Duration of a spawn message.")]
        public ushort SpawnMessageDuration { get; set; } = 5;

        [Description("Ghost spawn positions.")]
        public List<string> SpawnPositions { get; set; } = new List<string> { "9, 1002, 1" };

        [Description("Item given to every Ghost, that can teleport them to an alive player when dropped.")]
        public ItemType TeleportItem { get; set; } = ItemType.Lantern;

        [Description("Hint shown to a Ghost, if teleport succeeds.")]
        public string TeleportSuccess { get; set; } = "You were teleported to <color=green>%player%</color>.";

        [Description("Hint shown to a Ghost, if teleport fails.")]
        public string TeleportFail { get; set; } = "There is nobody you can teleport to.";

        [Description("List of roles that Ghosts cannot be teleported to. Scp079 is already included.")]
        public List<RoleTypeId> RoleTeleportBlacklist { get; set; } = new List<RoleTypeId>
        {
            RoleTypeId.Tutorial
        };

        [Description("Should Ghosts be despawned and not allowed to spawn after warhead detonation?")]
        public bool DespawnOnDetonation { get; set; } = true;

        [Description("Should Spectators be able to see Ghosts, if spectated player is not a Ghost?")]
        public bool AlwaysSeeGhosts { get; set; } = false;

        [Description("Should Filmmakers be able to see Ghosts?")]
        public bool FilmmakerSeeGhosts { get; set; } = false;

        [Description("How many shooting targets can one Ghost have spawned?")]
        public int TargetLimit { get; set; } = 1;

        [Description("Areas, where Ghosts can spawn a shooting target. For each area, provide a pair of positions, their coordinates will be used as a perimeter along every axis.")]
        public Dictionary<string, string> ShootingAreas { get; set; } = new Dictionary<string, string>()
        {
            { "10, 995, -12", "-10, 996, -4" },
            { "68, 983, -36", "142, 985, -12"}
        };

        [Description("Minimum distance between the Ghosts, that will make them hear eachother via RoundSummary channel instead of Proximity, if they have enabled listening to Ghosts.")]
        public float HearDistance { get; set; } = 10f;

        [Description("Time, after which the duel offer will expire.")]
        public float DuelRequestTime { get; set; } = 10f;

        [Description("Broadcast sent to a Ghost, who won a duel.")]
        public string DuelWon { get; set; } = "You have won the duel against <color=red>%player%</color>.";

        [Description("Broadcast sent to a Ghost, who lost a duel.")]
        public string DuelLost { get; set; } = "You have lost the duel against <color=red>%player%</color>.";
    }
}
