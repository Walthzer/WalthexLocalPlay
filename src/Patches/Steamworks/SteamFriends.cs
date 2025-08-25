using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using WalthexLocalPlay;

namespace WalthexLocalPlay.Patches.Steamworks;

[HarmonyPatch(typeof(SteamFriends))]
internal class SteamFriendsPatch
{
    [HarmonyPatch(nameof(SteamFriends.GetPersonaName))]
    [HarmonyPrefix]
    private static bool GetPersonaName_Prefix(ref string __result)
    {
        __result = WLPPlugin.steamID.m_SteamID.ToString();
        return false;
    }

    [HarmonyPatch(nameof(SteamFriends.GetFriendPersonaName))]
    [HarmonyPrefix]
    private static bool GetFriendPersonaName_Prefix(ref CSteamID steamIDFriend, ref string __result)
    {
        __result = steamIDFriend.m_SteamID.ToString();
        return false;
    }
}