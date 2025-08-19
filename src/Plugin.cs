using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using ModTemplate.Modules;

//Rename this to match the name of your mod, This needs to match the RootNamespace in the `.csproj` so edit that as well.
// e.g. <RootNamespace>ModTemplate</RootNamespace>
namespace ModTemplate;

// Ensure that BepInEx only loads your mod DLL into Mage Arena
[BepInProcess("MageArena")]
//This creates a dependency onto the ModSync mod, but only for BepInEX! Thunderstore only reads dependencies from the `manifest.json `
//Do not forget to include any dependecies there as well.
[BepInDependency("com.magearena.modsync", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyGUID, PluginName, VersionString)]
//Rename this to match the name of your mod
public class ModTemplatePlugin : BaseUnityPlugin
{
    //Reanme this to match the name used for the class above!
    internal static ModTemplatePlugin Instance { get; private set; }

    //This is the GUID or the unqiue identifier of your mod. Like a fingerprint!
    //It is what is used by BepInEx to find and use dependencies.
    //Use something like com.<name of auther>.<name of mod>.<dev or release> to ensure the GUID of your mod is always unique.
    private const string MyGUID = "com.walthzer.ModTemplate.dev";

    //Change this to the name of your mod
    internal const string PluginName = "ModTemplate";

    //Update this to match the version of your mod.
    //The format used is semantic versioning, see here for details https://semver.org/
    private const string VersionString = "1.0.0";


    //This is related to Harmony, used to patch the Mage Arena Game
    private static Harmony? Harmony;
    //This allows you to output text into the console
    internal static new ManualLogSource Logger;

    //Choose one of the `modync` values and remove the other two.
    //Most mods will use "all"
    public static string modsync = "all"; // Requires matching on both host and client - This is what is considered when syncing lobbies
    //public static string modsync = "host"; // Only required on host - These do not count for syncing lobbies
    //public static string modsync = "client"; // Only required on client - These do not count for syncing lobbies

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Instance = this;
        Harmony = new(MyGUID);
        Harmony.PatchAll();

        //Some example logic, you can delete this
        //Static method example
        Logger.LogDebug(ExampleModule.hello_world());


        //Last line of Plugin Logic, to indicate success!
        Logger.LogInfo($"Plugin {MyGUID} v{VersionString} is loaded!");
    }
}
