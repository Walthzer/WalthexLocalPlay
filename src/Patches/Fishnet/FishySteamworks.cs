
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace WalthexLocalPlay.Patches.Fishnet;

[HarmonyPatch(typeof(FishySteamworks.FishySteamworks))]
internal class FishySteamworksPatches
{

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.Initialize))]
    public static bool Initialize(NetworkManager networkManager, int transportIndex)
    {
        WLPPlugin.Tugboat.Initialize(networkManager, transportIndex);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("OnDestroy")]
    private static bool OnDestroy()
    {
        WLPPlugin.Tugboat.Shutdown();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("Update")]
    private static bool Update()
    {
        //Tugboat hooks into the NetworkManagers update loop instead
        //this._clientHost.CheckSetStarted();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.GetConnectionAddress))]
    public static bool GetConnectionAddress(int connectionId, ref string __result)
    {
        __result = WLPPlugin.Tugboat.GetConnectionAddress(connectionId);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.GetConnectionState))]
    [HarmonyPatch(new Type[] { typeof(bool) }, new ArgumentType[] { ArgumentType.Normal })]
    public static bool GetConnectionState(bool server, ref LocalConnectionState __result)
    {
        __result = WLPPlugin.Tugboat.GetConnectionState(server);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.GetConnectionState))]
    [HarmonyPatch(new Type[] { typeof(int) }, new ArgumentType[] { ArgumentType.Normal })]
    public static bool GetConnectionState(int connectionId, ref RemoteConnectionState __result)
    {
        __result = WLPPlugin.Tugboat.GetConnectionState(connectionId);
        return false;
    }


    /*[HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.HandleClientConnectionState))]
    public static bool HandleClientConnectionState(ClientConnectionStateArgs connectionStateArgs)
    {
        WLPPlugin.Tugboat.HandleClientConnectionState(connectionStateArgs);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.HandleServerConnectionState))]
    public static bool HandleServerConnectionState(ServerConnectionStateArgs connectionStateArgs)
    {
        WLPPlugin.Tugboat.HandleServerConnectionState(connectionStateArgs);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.HandleRemoteConnectionState))]
    public static bool HandleRemoteConnectionState(RemoteConnectionStateArgs connectionStateArgs)
    {
        WLPPlugin.Tugboat.HandleRemoteConnectionState(connectionStateArgs);
        return false;
    }*/

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.IterateIncoming))]
    public static bool IterateIncoming(bool server)
    {
        WLPPlugin.Tugboat.IterateIncoming(server);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.IterateOutgoing))]
    public static bool IterateOutgoing(bool server)
    {
        WLPPlugin.Tugboat.IterateOutgoing(server);
        return false;
    }


    /*[HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.HandleClientReceivedDataArgs))]
    public static bool HandleClientReceivedDataArgs(ClientReceivedDataArgs receivedDataArgs)
    {
        WLPPlugin.Tugboat.HandleClientReceivedDataArgs(receivedDataArgs);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.HandleServerReceivedDataArgs))]
    public static bool HandleServerReceivedDataArgs(ServerReceivedDataArgs receivedDataArgs)
    {
        WLPPlugin.Tugboat.HandleServerReceivedDataArgs(receivedDataArgs);
        return false;
    }*/

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.SendToServer))]
    public static bool SendToServer(byte channelId, ArraySegment<byte> segment)
    {
        WLPPlugin.Tugboat.SendToServer(channelId, segment);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.SendToClient))]
    public static bool SendToClient(byte channelId, ArraySegment<byte> segment, int connectionId)
    {
        WLPPlugin.Tugboat.SendToClient(channelId, segment, connectionId);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.GetMaximumClients))]
    public static bool GetMaximumClients(ref int __result)
    {
        __result = WLPPlugin.Tugboat.GetMaximumClients();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.SetMaximumClients))]
    public static bool SetMaximumClients(int value)
    {
        WLPPlugin.Tugboat.SetMaximumClients(value);
        return false;
    }

    //FishySteamworks uses SteamID's as address. Bounce this and set address in LocalMatchmaking
    //Might hook into this to force server/client lobby port into Tugboat
    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.SetClientAddress))]
    public static bool SetClientAddress(string address)
    {
        WLPPlugin.Tugboat.SetClientAddress("127.0.0.1");
        if (WLPPlugin.LocalMatchmaking.isServer())
        {
            lock (WLPPlugin.LocalMatchmaking.m_server)
            {
                WLPPlugin.Tugboat.SetPort(Convert.ToUInt16(WLPPlugin.LocalMatchmaking.m_server.m_ports.udp));
            }
        }
        //Handle client 
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.SetServerBindAddress))]
    public static bool SetServerBindAddress(string address, IPAddressType addressType)
    {
        WLPPlugin.Tugboat.SetServerBindAddress("127.0.0.1", IPAddressType.IPv4);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.SetPort))]
    public static bool SetPort(ushort port)
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.StartConnection))]
    public static bool StartConnection(bool server, ref bool __result)
    {
        __result = WLPPlugin.Tugboat.StartConnection(server);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.StopConnection))]
    [HarmonyPatch(new Type[] { typeof(bool) }, new ArgumentType[] { ArgumentType.Normal })]
    public static bool StopConnection(bool server, ref bool __result)
    {
        __result = WLPPlugin.Tugboat.StopConnection(server);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.StopConnection))]
    [HarmonyPatch(new Type[] { typeof(int), typeof(bool) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal})]
    public static bool StopConnection(int connectionId, bool immediately, ref bool __result)
    {
        __result = WLPPlugin.Tugboat.StopConnection(connectionId, immediately);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.Shutdown))]
    public static bool Shutdown()
    {
        WLPPlugin.Tugboat.Shutdown();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FishySteamworks.FishySteamworks.GetMTU))]
    public static bool GetMTU(byte channel, ref int __result)
    {
        __result = WLPPlugin.Tugboat.GetMTU(channel);
        return false;
    }
}
