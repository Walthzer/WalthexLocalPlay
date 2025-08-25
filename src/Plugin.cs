using BepInEx;
using BepInEx.Logging;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using HarmonyLib;
using Steamworks;
using System.Collections;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using WalthexLocalPlay.Modules;
using WalthexLocalPlay.Modules.LocalMatchmaking;

namespace WalthexLocalPlay;

[BepInProcess("MageArena")]
[BepInPlugin(MyGUID, PluginName, VersionString)]
//Rename this to match the name of your mod
public class WLPPlugin : BaseUnityPlugin { 
    internal static WLPPlugin Instance { get; private set; }
    private const string MyGUID = "com.walthzer.walthexlocalplay.dev";
    internal const string PluginName = "WalthexLocalPlay";
    private const string VersionString = "1.0.0";


    //This is related to Harmony, used to patch the Mage Arena Game
    private Harmony? Harmony;
    //This allows you to output text into the console
    public static new ManualLogSource Logger;

    //FishySteamworks override
    public static Tugboat Tugboat = new Tugboat();
    private static bool TugboatHooked = false;


    //Steam overrides
    //Instead of making these classes static, refer to them via the WLPPlugin class
    public static CSteamID steamID = new CSteamID(76561197960287930);
    public static LocalUserStats LocalUserStats = new LocalUserStats();
    public static LocalDispatcher LocalDispatcher = new LocalDispatcher();
    public static LocalMatchmaking LocalMatchmaking;
    
    

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Instance = this;
        Harmony = new(MyGUID);
        Harmony.PatchAll();

        LocalMatchmaking = new LocalMatchmaking(IPAddress.Parse("127.0.0.1"), new PortRange(50000, 50002));

        //Last line of Plugin Logic, to indicate success!
        Logger.LogInfo($"Plugin {MyGUID} v{VersionString} is loaded!");
    }

    private void Update()
    {
        if (!TugboatHooked)
        {
            FishySteamworks.FishySteamworks fishy = Object.FindFirstObjectByType<global::FishySteamworks.FishySteamworks>();
            if (fishy != null)
            {
                //Hook events
                Tugboat.OnClientConnectionState += fishy.HandleClientConnectionState;
                Tugboat.OnServerConnectionState += fishy.HandleServerConnectionState;
                Tugboat.OnClientReceivedData += fishy.HandleClientReceivedDataArgs;
                Tugboat.OnServerReceivedData += fishy.HandleServerReceivedDataArgs;
                Tugboat.OnRemoteConnectionState += fishy.HandleRemoteConnectionState;
                TugboatHooked = true;
            }
        }

        LocalDispatcher.RunFrame();
    }

    public void EnterLobbyWhenServerOK(CSteamID lobbyID)
    {
        StartCoroutine(WaitUntilServerStarted(lobbyID));
    }
    public IEnumerator WaitUntilServerStarted(CSteamID lobbyID)
    {
        NetworkManager manager = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
        yield return new WaitUntil(() => manager.IsServerStarted);
        LocalMatchmaking.InvokeLobbyEntered(EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess, lobbyID);
    }
}
