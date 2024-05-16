using BepInEx;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;

namespace LetHorsesJump;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.Bloodstone")]
[BepInDependency("gg.deca.VampireCommandFramework", BepInDependency.DependencyFlags.SoftDependency)]
[Reloadable]
public class Plugin : BasePlugin, IRunOnInitialized
{
    Harmony _harmony;
    
    public static ManualLogSource LogInstance { get; private set; }

    public override void Load()
    {
        LogInstance = Log;
        
        if (!VWorld.IsServer)
        {
            Log.LogWarning("This plugin is a server-only plugin!");
        }
    }
    
    public void OnGameInitialized()
    {
        if (VWorld.IsClient)
        {
            return;
        }

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");
        
        Log.LogInfo("Trying to find VCF:");
        if (Commands.Enabled)
        {
            Commands.Register();
        }
        else
        {
            Log.LogError("This mod has commands, you need to install VampireCommandFramework to use them, find whereever you get mods or : https://a.deca.gg/vcf .");
        }
    }


    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}