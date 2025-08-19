using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using WalthexLocalPlay.Modules;

namespace WalthexLocalPlay;

[BepInProcess("MageArena")]
[BepInDependency("com.magearena.modsync", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyGUID, PluginName, VersionString)]
//Rename this to match the name of your mod
public class WLPPlugin : BaseUnityPlugin
    internal static WLPPlugin Instance { get; private set; }
    private const string MyGUID = "com.walthzer.walthexlocalplay.dev";
    internal const string PluginName = "WalthexLocalPlay";
    private const string VersionString = "1.0.0";


    //This is related to Harmony, used to patch the Mage Arena Game
    private static Harmony? Harmony;
    //This allows you to output text into the console
    internal static new ManualLogSource Logger;
    public static string modsync = "all";

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Instance = this;
        Harmony = new(MyGUID);
        Harmony.PatchAll();

        //Last line of Plugin Logic, to indicate success!
        Logger.LogInfo($"Plugin {MyGUID} v{VersionString} is loaded!");
    }
}
