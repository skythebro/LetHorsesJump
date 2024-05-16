# LetHorsesJump for V Rising

LetHorsesJump is a mod that allows any horse you ride to jump with some other stuff added too.

---
A big thanks to the V Rising modding discord for support!
Also credit to decaprime for his VRising mod LeadAHorseToWater, I copied some of his commands and code to get horses
around the player, I will prob remove them after testing is done.
[V Rising Modding Discord](https://vrisingmods.com/discord)

Features:
- Allows any horse to jump.
- Makes all horses you ride invulnerable.
- Makes all horses able to have saddles (still in testing)


## Commands

- The following commands are copied from LeadAHorseToWater or slightly changed to fit the mod
- `.horsejump spawn (amount) (type)`
    - Spawns (amount) of horse with (type) this can be: `Regular` or `Spectral`
    - shortcut: .hj spawn
- `.horsejump whistle (name)`
    - Teleports horse with name or closest horse if name is not given
    - shortcut .hj w
- `.horsejump kill`
    - Kills horse with name or closest horse if name is not given
    - shortcut: .hj kill
- `.horsejump cull (radius) (percentage)`
    - Culls all horses within radius of `default:5f` which is 1 tile with percentage `default:1f` which is 100% 

## Installation (Manual)

* Install BepInEx
* Install Bloodstone
* (optional) Install VampireCommandFramework
* Extract [LetHorsesJump.dll](https://github.com/skythebro/LetHorsesJump/releases/tag/0.1.0) into (VRising server folder)/BepInEx/plugins

## There is no config yet, but I will add one soon for customization purposes.

### Troubleshooting
- Make sure you install the mod on the server. If you are in a singleplayer world use [ServerLaunchFix](https://v-rising.thunderstore.io/package/Mythic/ServerLaunchFix/) (when its updated)
- Check your BepInEx logs on the server to make sure the latest version of both LetHorsesJump and Bloodstone were loaded (optionally VampireCommandFramework too).

### Support
- Open an issue on [github](https://github.com/skythebro/LetHorsesJump/issues)
- Ask in the V Rising Mod Community [discord](https://vrisingmods.com/discord)

### Contributors
- skythebro/skyKDG: `@realskye` on Discord
- [decaprime](https://github.com/decaprime) for his VRising mod LeadAHorseToWater, I copied some of his commands and code to get horses around the player.
- V Rising Mod Community discord for helpful resources to mod this game and code inspiration.