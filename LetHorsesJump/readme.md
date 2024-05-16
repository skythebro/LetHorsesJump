# LetHorsesJump for V Rising
LetHorsesJump is a mod that allows any horse you ride to jump.

---
A big thanks to the V Rising modding discord for support!
Also credit to decaprime for his Vrising mod LeadAHorseToWater, I coppied some of his commands and code to get horses around the player.
[V Rising Modding Discord](https://vrisingmods.com/discord)

## Commands

### State Commands
- `.state player (name)`
    - Dumps the state of a player (User/Char/Team/Progression)
    - shortcut: .s p
- `.state clan`
    - Spits out Clan Info
    - shortcut .s c
- `.state inventory`
    - retrieves inventory state
    - shortcut: .s i
- `.state door`
    - retrieves door state
- `.state ownedby (player name)`
    - Outputs state of entities owned by the player
      -shortcut: .s o
- `.state entity (entityID#) (version)`
    - spits out enitiy info of the entity specified by ID and Version
- `.state prefab (guid)`
    - spits out prefab info- singular if GUID specified, otherwise all prefabs to a file each with components attached within
- `.state teams`
    - checks all the team data
    - shortcut: .s t
- `.state nearby (radius)`
    - gets nearby entities based on range. Careful there are a lot even very close to you.
    - shortcut: .s n
- `.state tilemodels (radius)`
    - gets nearby tilemodel entities
    - shortcut: .s tm
- `.state rooms (radius)`
    - gets nearby rooms
    - shorcut: .s r
- `.state castleterritory (index#)`
    - outputs a particular or all castle territories.
    - shorcut: .s ct
- `.state mapzones`
    - outputs map zones
    - shorcut: .s mz
- `.state worldregionpolygon`
    - outputs all world region polygons
    - shorcut: .s wrp
- `.state SetPasteBinKeysNoLog`
    - sets the Pastebin API, userkey, and optional folder keys (This is kind of legacy from a foray into use on lives, but pastebin has limits and the like. I'm leaving it, but its not needed)




### Entity Commands
- `.entity teleport (entityID) (version)`
    - teleport to the specified entity
    - shortcut: .e tp
- `.entity despawn (entityID) (version)`
    - despawn the specified entity
    - shortcut: .e d
- `.entity destroy (entityID) (Version)`
    - Destroy the specified entity
    - shortcut: .e del

### Dump Commands
- `.dump prefabs`
    - dumps all prefabs to a file as prefabGuids for a Prefabs.cs file
    - shortcut: .s dp
- `.dump types`
    - Dumps all ECS component types to file (for usage in ComponentExtractors.tt)
    - shorcut: .s dt
- `.dump prefabjsons`
    - dumps all prefab names and IDs to JSON files, grouped by prefix
    - shortcut: .s dpj




## Eventual To-Do/Possible features
- I add as I require. 