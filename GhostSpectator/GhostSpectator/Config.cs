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

        [Description("Duration of message shown upon Ghost spawn.")]
        public ushort SpawnMessageDuration { get; set; } = 5;

        [Description("Ghost spawn position.")]
        public string SpawnPoint { get; set; } = "9, 1002, 1";

        [Description("Item given to every Ghost, that can teleport them to alive player when dropped.")]
        public ItemType TeleportItem { get; set; } = ItemType.Lantern;

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

        [Description("Translations. Don't translate words put between two \'%\'.")]
        public Translation Translation { get; set; } = new Translation();
    }
}
