using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace WalthexLocalPlay.Patches.Steamworks;


[HarmonyPatch(typeof(SteamUser))]
internal class SteamUserPatch
{
    [HarmonyPatch(nameof(SteamUser.GetSteamID))]
    [HarmonyPrefix]
    private static bool Prefix(ref CSteamID __result)
    {
        __result = WLPPlugin.steamID;
        return false;
    }
}