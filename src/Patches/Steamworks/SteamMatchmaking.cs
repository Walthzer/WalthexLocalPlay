using HarmonyLib;
using JetBrains.Annotations;
using Steamworks;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using WalthexLocalPlay.Modules.LocalMatchmaking;

namespace WalthexLocalPlay.Patches.Steamworks;


[HarmonyPatch(typeof(SteamMatchmaking))]
public class SteamMatchmakingPatch
{

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.CreateLobby))]
    public static bool CreateLobby(ELobbyType eLobbyType, int cMaxMembers, ref SteamAPICall_t __result)
    {
        WLPPlugin.LocalMatchmaking.CreateLobby(eLobbyType, cMaxMembers);
        __result = new SteamAPICall_t(0);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.JoinLobby))]
    public static bool JoinLobby(CSteamID steamIDLobby, ref SteamAPICall_t __result)
    {
        __result = new SteamAPICall_t(0);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.LeaveLobby))]
    public static bool LeaveLobby(CSteamID steamIDLobby)
    {
        WLPPlugin.LocalMatchmaking.LeaveLobby(steamIDLobby);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.RequestLobbyList))]
    public static bool RequestLobbyList(ref SteamAPICall_t __result)
    {
        WLPPlugin.LocalMatchmaking.RequestLobbyList();
        __result = new SteamAPICall_t(0);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetLobbyByIndex))]
    public static bool GetLobbyByIndex(int iLobby, ref CSteamID __result)
    {
        __result = WLPPlugin.LocalMatchmaking.GetLobbyByIndex(iLobby);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetLobbyDataCount))]
    public static bool GetLobbyDataCount(CSteamID steamIDLobby, ref int __result)
    {
        __result = WLPPlugin.LocalMatchmaking.GetLobbyDataCount(steamIDLobby);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.RequestLobbyData))]
    public static bool RequestLobbyData(CSteamID steamIDLobby, ref bool __result)
    {
        __result = WLPPlugin.LocalMatchmaking.RequestLobbyData(steamIDLobby);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetLobbyData))]
    public static bool GetLobbyData(CSteamID steamIDLobby, string pchKey, ref string __result)
    {
        __result = WLPPlugin.LocalMatchmaking.GetLobbyData(steamIDLobby, pchKey);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.SetLobbyData))]
    public static bool SetLobbyData(CSteamID steamIDLobby, string pchKey, string pchValue, ref bool __result)
    {
        __result = WLPPlugin.LocalMatchmaking.SetLobbyData(steamIDLobby, pchKey, pchValue);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetLobbyOwner))]
    public static bool GetLobbyOwner(CSteamID steamIDLobby, ref CSteamID __result)
    {
        __result = WLPPlugin.LocalMatchmaking.GetLobbyOwner(steamIDLobby);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetNumLobbyMembers))]
    public static bool GetNumLobbyMembers(CSteamID steamIDLobby, ref int __result)
    {
        __result = WLPPlugin.LocalMatchmaking.GetNumLobbyMembers(steamIDLobby);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.SetLobbyMemberLimit))]
    public static bool SetLobbyMemberLimit(CSteamID steamIDLobby, int cMaxMembers, ref bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetLobbyMemberLimit))]
    public static bool GetLobbyMemberLimit(CSteamID steamIDLobby, ref int __result)
    {
        __result = 8;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.SetLobbyType))]
    public static bool SetLobbyType(CSteamID steamIDLobby, ELobbyType eLobbyType, ref bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.SetLobbyJoinable))]
    public static bool SetLobbyJoinable(CSteamID steamIDLobby, bool bLobbyJoinable, ref bool __result)
    {
        __result =  true;
        return false;
    }

    // TODO: Implement
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.SendLobbyChatMsg))]
    public static bool SendLobbyChatMsg(ref CSteamID steamIDLobby, ref byte[] pvMsgBody, ref int cubMsgBody, ref bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.GetLobbyChatEntry))]
    public static bool GetLobbyChatEntry(CSteamID steamIDLobby, int iChatID, out CSteamID pSteamIDUser, byte[] pvData, int cubData, out EChatEntryType peChatEntryType, ref int __result)
    {
        pSteamIDUser = new CSteamID(0);
        peChatEntryType = EChatEntryType.k_EChatEntryTypeChatMsg;
        __result = 0;
        return false;
    }

    //------------ Disabled Functions ------------

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.DeleteLobbyData))]
    public static bool DeleteLobbyData(ref CSteamID steamIDLobby, string pchKey, ref bool __result)
    {
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.SetLinkedLobby))]
    public static bool SetLinkedLobby(CSteamID steamIDLobby, CSteamID steamIDLobbyDependent)
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListStringFilter))]
    public static bool AddRequestLobbyListStringFilter_Prefix(string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType)
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListNumericalFilter))]
    public static bool AddRequestLobbyListNumericalFilter_Prefix(string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType)
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListNearValueFilter))]
    public static bool AddRequestLobbyListNearValueFilter_Prefix(string pchKeyToMatch, int nValueToBeCloseTo)
    {
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable))]
    public static bool AddRequestLobbyListFilterSlotsAvailable_Prefix(int nSlotsAvailable)
    {
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListDistanceFilter))]
    public static bool AddRequestLobbyListDistanceFilter_Prefix(ELobbyDistanceFilter eLobbyDistanceFilter)
    {
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListResultCountFilter))]
    public static bool AddRequestLobbyListResultCountFilter_Prefix(int cMaxResults)
    {
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter))]
    public static bool AddRequestLobbyListCompatibleMembersFilter_Prefix(CSteamID steamIDLobby)
    {
        return false;
    }
}