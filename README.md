# GhostSpectator_NWAPI
Plugin, that lets players change into Ghosts: Tutorials, that are undetectable to alive player and have no influence on course of game. Ghosts can teleport to random alive player by dropping item declared in Config (Lantern by default).
- Ghosts are always visible to each other, Spectators spactating a Ghost and Overwatchers. Depending on Config, Ghosts can be visible to Spectators spectating a non-Ghost player and Filmmakers.
- Ghosts can't pick up or use items and interact with objects.
- Ghosts can pass through most of the doors.

## Config
|Name|Type|Default value|Description|
|---|---|---|---|
|is_enabled|bool|true|Is plugin enabled?|
|debug|bool|false|Should Debug be enabled?|
|ghost_color|string|'#A0A0A0'|Color of Ghosts nicknames.|
|spawn_message_duration|ushort|5|Duration of spawn message.|
|spawn_point|string|9, 1002, 1|Ghost spawn position.|
|teleport_item|ItemType|Lantern|Item given to every Ghost, that can teleport them to alive player when dropped.|
|role_teleport_blacklist|List\<RoleTypeId\>|- Tutorial|A list of roles that Ghosts cannot be teleported to. Scp079 is already included.|
|despawn_on_detonation|bool|true|Should Ghosts be despawned and not allowed to spawn after warhead detonation?|
|always_see_ghosts|bool|false|Should Spectators be able to see Ghosts, if spectated player is not a Ghost?|
|filmmaker_see_ghosts|bool|false|Should Filmmakers be able to see Ghosts?|
|translation|Translation|new Translation()|Translations. Don't translate words put between two \'%\'.|

**Translation** allows person to translate **Ghost nickname, shown messages, command descriptions and outputs.**

## Commands
- GhostParent - Parent command for GhostSpectator.
- Spawn - Spawn selected player(s) as a Ghost.
- Despawn - Despawn selected player(s) from Ghost to Tutorial (true) or Spectator (false = default option).
- GhostMe - Change yourself to Ghost from Spectator or vice versa.
- List - Print list of all Ghosts.

## Permissions
- gs.items - allows Ghost to drop or throw items
- gs.noclip - allows Ghost to have noclip enabled 
- gs.spawn.self - allows player to (de)spawn themselves to and from Ghost
- gs.spawn.others - allows player to (de)spawn any player to and from Ghost
- gs.warhead - allows player to (de)spawn themselves or others to and from Ghost after warhead detonation

## Required dependencies
- [NWAPIPermissionSystem](https://github.com/CedModV2/NWAPIPermissionSystem) by ced777ric
- [Harmony](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0) by pardeike - attached to release
- AssemblyPublicized - attached to release

## Credits
- Original plugin creator: [Thundermaker300](https://github.com/Thundermaker300)
