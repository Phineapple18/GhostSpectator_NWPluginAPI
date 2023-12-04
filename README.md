# GhostSpectator_NWAPI
Plugin, that lets players change into Ghosts: Tutorials, that are undetectable to an alive player and have no influence on course of the game.

## Features
- Ghosts can teleport to a random alive player by dropping item declared in config (Lantern by default).
- Ghosts are always visible to each other, Spectators spactating a Ghost and Overwatchers. Depending on config, Ghosts can be visible to Spectators spectating a non-Ghost player and Filmmakers.
- Ghosts can't pick up or use items and interact with objects (partially except shooting targets).
- Ghosts can pass through most of the doors.
- Depending on permission, Ghosts can noclip, spawn themselves a shooting target and give themselves a firearm. A firearm will be automatically refilled, when emptied.

## Required plugins and dependencies (1.0.3): 
- [NWAPIPermissionSystem](https://github.com/CedModV2/NWAPIPermissionSystem/releases/tag/0.0.6) by ced777ric
- [Harmony 2.2.2.0](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0) by pardeike (dependencies)
- AssemblyCSharpPublicized - attached to release (dependencies)

## Config
|Name|Type|Default value|Description|
|---|---|---|---|
|is_enabled|bool|true|Is plugin enabled?|
|debug|bool|false|Is Debug enabled?|
|ghost_nickname|string|GHOST|Nickname of Ghost, that is displayed in place of a role.|
|ghost_color|string|'#A0A0A0'|Color of a Ghost nickname.|
|spawnmessage|string|<size=50><color=%colour%>You are a Ghost!</color>\n<size=30>Drop the %TeleportItem% to teleport to a random player.</size>|Broadcast shown to a Ghost upon spawn.|
|spawnmessage_duration|ushort|5|Duration of a spawn message.|
|spawnpositions|List\<string\>|- 9, 1002, 1|Ghost spawn positions.|
|teleport_item|ItemType|Lantern|Item given to every Ghost, that can teleport them to alive player when dropped.|
|teleport_success|string|You were teleported to <color=green>%player%</color>.|Hint shown to a Ghost, if teleport succeeds.|
|teleport_fail|string|There is nobody you can teleport to.|Hint shown to a Ghost, if teleport fails.|
|role_teleport_blacklist|List\<RoleTypeId\>|- Tutorial|A list of roles that Ghosts cannot be teleported to. Scp079 is already included.|
|despawn_on_detonation|bool|true|Should Ghosts be despawned and not allowed to spawn after warhead detonation?|
|always_see_ghosts|bool|false|Should Spectators be able to see Ghosts, if spectated player is not a Ghost?|
|filmmaker_see_ghosts|bool|false|Should Filmmakers be able to see Ghosts?|
|target_limit|int|1|How many shooting targets can one Ghost have spawned?|
|shooting_areas|Dictionary\<string, string\>| 10, 995, -12: -10, 996, -4<br/> 68, 983, -36: 142, 985, -12|Areas, where Ghosts can spawn a shooting target. For each area, provide a pair of positions, their coordinates will be used as perimeter along every axis.|

## Command translations
Translations for commands are in separate file "commandtranslation.yml" in the same folder as config file. You can translate a command, its description and aliases, and all command responses. 

## Commands
- ghostspectator (CC/RAC)- Parent command for GhostSpectator.
- createtarget (CC) - Create a shooting target.
- despawn (RAC) - Despawn selected player(s) from Ghost to Tutorial (true) or Spectator (false = default option).
- destroytarget (CC) - Destroy your shooting target by typing its NetId or print a list of all your currently spawned targets.
- ghostme (CC) - Change yourself to Ghost from Spectator or vice versa.
- givegun (CC) - Give yourself a firearm. 
- list (RAC) - Print list of all Ghosts.
- spawn (RAC) - Spawn selected player(s) as a Ghost.

## Permissions
- gs.firearm - allows a Ghost to give themselves a firearm
- gs.items - allows a Ghost to drop or throw items
- gs.noclip - allows a Ghost to have noclip enabled
- gs.spawn.others - allows a player to (de)spawn any player to and from Ghost
- gs.spawn.self - allows a player to (de)spawn themselves to and from Ghost
- gs.target - allows a player to (de)spawn their shooting target
- gs.warhead - allows a player to (de)spawn themselves or others to and from Ghost after warhead detonation

## Credits
- Original plugin creator: [Thundermaker300](https://github.com/Thundermaker300)
