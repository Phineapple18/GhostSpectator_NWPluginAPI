# GhostSpectator_NWAPI
Plugin, that allows players to change into Ghosts: Tutorials, that are undetectable to alive players and have no influence on a course of the game. Ghosts can teleport to a random player, spawn a shooting target, give themselves firearms or listen to chats of certain groups.

![Logo](https://github.com/Phineapple18/GhostSpectator_NWPluginAPI/blob/main/Images/GS_Thumbnail.png)

## Features
- Ghosts can teleport to a random alive player by dropping an item declared in the config (a Lantern by default).
- Ghosts are always visible to each other, Spectators spactating a Ghost and Overwatchers. Depending on the config, Ghosts can be visible to Spectators spectating a non-Ghost player and Filmmakers.
- Ghosts can't pick up or use items and interact with objects (partially except the shooting targets).
- Ghosts can pass through most of the doors.
- Depending on the assigned permissions, Ghosts can:
  * noclip
  * spawn themselves a shooting target and give themselves a firearm - a firearm will be automatically refilled, when emptied
  * listen to SCP and Spectators chats
  * listen to other Ghosts via RoundSummary chat, if they are not in the Proximity chat or far enough (distance can be set in the config)
  * challenge other Ghosts to a duel

## Required plugins and dependencies (1.2.1): 
- [NWAPIPermissionSystem](https://github.com/CedModV2/NWAPIPermissionSystem/releases/tag/0.0.6) by ced777ric - plugin
- [Harmony 2.2.2.0](https://github.com/pardeike/Harmony/releases/tag/v2.2.2.0) by pardeike - dependency (attached to one of the previous releases)

## Config
|Name|Type|Default value|Description|
|---|---|---|---|
|is_enabled|bool|true|Is plugin enabled?|
|debug|bool|false|Is Debug enabled?|
|ghost_nickname|string|GHOST|Nickname of a Ghost, that is displayed in place of a role.|
|ghost_color|string|'#A0A0A0'|Color of a Ghost nickname.|
|ghost_health|float|150|Max Ghost health.|
|spawnmessage|string|<size=50><color=%colour%>You are a Ghost!\</color>\n<size=30>Drop the %TeleportItem% to teleport to a random player.\</size>|Broadcast sent to a Ghost upon spawn.|
|spawnmessage_duration|ushort|5|Duration of a spawn message.|
|spawnpositions|List\<string>|- 9, 1002, 1|Ghost spawn positions.|
|teleport_item|ItemType|Lantern|Item given to every Ghost, that can teleport them to an alive player when dropped.|
|teleport_success|string|You were teleported to <color=green>%player%\</color>.|Hint shown to a Ghost, if teleport succeeds.|
|teleport_fail|string|There is nobody you can teleport to.|Hint shown to a Ghost, if teleport fails.|
|role_teleport_blacklist|List\<RoleTypeId\>|- Tutorial|A list of roles that Ghosts cannot be teleported to. Scp079 is already included.|
|despawn_on_detonation|bool|true|Should Ghosts be despawned and not allowed to spawn after warhead detonation?|
|always_see_ghosts|bool|false|Should Spectators be able to see Ghosts, if spectated player is not a Ghost?|
|filmmaker_see_ghosts|bool|false|Should Filmmakers be able to see Ghosts?|
|target_limit|int|1|How many shooting targets can one Ghost have spawned?|
|shooting_areas|Dictionary\<string, string>| 10, 995, -12: -10, 996, -4<br/> 68, 983, -36: 142, 985, -12|Areas, where Ghosts can spawn a shooting target. For each area, provide a pair of positions, their coordinates will be used as a perimeter along every axis.|
|hear_distance|float|10f|Minimum distance between the Ghosts, that will make them hear eachother via RoundSummary channel instead of Proximity, if they have enabled listening to Ghosts.|
|duel_request_time|float|10f|Time, after which the duel offer will expire.|
|duel_won|string|You have won the duel against <color=red>%player%\</color>.|Hint shown to a Ghost, who won a duel.|
|duel_lost|string|You have lost the duel against <color=red>%player%\</color>.|Hint shown to a Ghost, who lost a duel.|

## Command translations
Translations for commands are in separate file "commandtranslation.yml", in the same folder as the config file. You can translate a command, its description and aliases, and all command responses. 

## Remote Admin Commands
- ghostspectator - Parent command for GhostSpectator. Subcommands: despawn, list, spawn.
- despawn - Despawn selected player(s) from Ghost to Tutorial (true) or Spectator (false = default option).
- list - Print list of all Ghosts.
- spawn - Spawn selected player(s) as a Ghost.

## Client Console Commands
- duel - Challenge another Ghost to a duel by typing their nickname. Also parent command for Duel. Subcommands: abandon, accept, cancel, list.
- abandon - Abandon your current duel.
- accept - Accept duel offer from player. If you have multiple offers, first one will be accepted, unless you provide a player nickname.
- cancel - Cancel duel offer with currently challenged player.
- list (duel) - Print a list of all players, who challenged you to a duel.
- createtarget - Create a shooting target.
- destroytarget - Destroy your shooting target or print a list of your targets.
- disablevoicechat - Disable listening to selected voicechat(s) as a Ghost.
- enablevoicechat - Enable listening to selected voicechat(s) as a Ghost.
- ghostme - Change yourself to a Ghost from Spectator or vice versa.
- givefirearm - Give yourself a firearm or print a list of available firearms.

## Permissions
- gs.duel - allows a Ghost to challenge another Ghost to a duel
- gs.firearm - allows a Ghost to give themselves a firearm
- gs.item - allows a Ghost to drop and throw items and use Particle Disruptor
- gs.listen.dead - allows a Ghost to hear Spectators
- gs.listen.ghost - allows a Ghost to hear other Ghosts, if they are not in proximity
- gs.listen.scp - allows a Ghost to hear SCPs
- gs.noclip - allows a Ghost to have noclip enabled
- gs.spawn.player - allows a player to (de)spawn any player to and from Ghost
- gs.spawn.self - allows a player to (de)spawn themselves to and from Ghost
- gs.target - allows a player to (de)spawn their shooting target
- gs.warhead - allows a player to (de)spawn themselves and others to and from Ghost after warhead detonation

## Credits
- Original plugin creator: [Thundermaker300](https://github.com/Thundermaker300)
