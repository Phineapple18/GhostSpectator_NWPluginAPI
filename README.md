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
|ghost_color|string|A0A0A0|Color of Ghosts nicknames.|
|spawn_message|string|<size=50><color=#A0A0A0>You are a Ghost!</color>\n<size=30>Drop the %TeleportItem% to teleport to a random player.</size>|Message shown to Ghosts upon spawning.|
|spawn_message_duration|ushort|5|Duration of spawn message.|
|teleport_item|ItemType|Lantern|Item given to every Ghost, that can teleport them to alive player when dropped.|
|player_teleport_fail_message|string|There is nobody you can teleport to.|Hint shown to Ghost, if teleport fails.|
|role_teleport_blacklist|List\<RoleTypeId\>|- Tutorial|A list of roles that Ghosts cannot be teleported to. Scp079 is already included.|
|despawn_on_detonation|bool|true|Should Ghosts be despawned and not allowed to spawn after warhead detonation?|
|always_see_ghosts|bool|false|Should Spectators be able to see Ghosts, if spectated player is not a Ghost?|
|filmmaker_see_ghosts|bool|false|Should Filmmakers be able to see Ghosts?|
|ghost_self|List\<string\>|- admin</br>- vip</br>- none|Groups, that can change themselves into Ghosts. \"none\" means a person with no server role.|
|ghost_others|List\<string\>|-admin|Groups, that can change others into Ghosts.|
|ghost_interact_items|List\<string\>|[]|Groups, that can interact drop or throw items while being Ghosts.|
|ghost_after_warhead|List\<string\>|[]|Groups, that can change into Ghosts after warhead detonation.|

## Credits
Original plugin: [Thundermaker300](https://github.com/Thundermaker300)
