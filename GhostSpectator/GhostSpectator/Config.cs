using System;
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

        [Description("Should Debug be enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Color of Ghosts nicknames.")]
        public string GhostColor { get; set; } = "#A0A0A0";

        [Description("Message shown to Ghosts upon spawning.")]
        public string SpawnMessage { get; set; } = "<size=50><color=#A0A0A0>You are a Ghost!</color>\n<size=30>Drop the %TeleportItem% to teleport to a random player.</size>";

        [Description("Duration of spawn message.")]
        public ushort SpawnMessageDuration { get; set; } = 5;

        [Description("Item given to every Ghost, that can teleport them to alive player when dropped.")]
        public ItemType TeleportItem { get; set; } = ItemType.Lantern;

        [Description("Hint shown to Ghost, if teleport fails.")]
        public string PlayerTeleportFailMessage { get; set; } = "There is nobody you can teleport to.";

        [Description("A list of roles that Ghosts cannot be teleported to. Scp079 is already included.")]
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

        [Description("Groups, that can change themselves into Ghosts. \"none\" means a person with no server role.")]
        public List<string> GhostSelf { get; set; } = new List<string>
        {
            "admin",
            "vip",
            "none"
        };

        [Description("Groups, that can change others into Ghosts.")]
        public List<string> GhostOthers { get; set; } = new List<string>
        {
             "admin"
        };

        [Description("Groups, that can interact drop or throw items while being Ghosts.")]
        public List<string> GhostInteractItems { get; set; } = new List<string>();

        [Description("Groups, that can change into Ghosts after warhead detonation.")]
        public List<string> GhostAfterWarhead { get; set; } = new List<string>();
    }
}
