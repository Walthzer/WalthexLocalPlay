using HarmonyLib;
using JetBrains.Annotations;
using Steamworks;
using System;
using UnityEngine;


namespace WalthexLocalPlay.Patches.Steamworks;


[HarmonyPatch(typeof(SteamUserStats))]
internal class SteamUserStatsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamUserStats.RequestUserStats))]
    private static bool RequestUserStats_Prefix(CSteamID steamIDUser)
    {
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamUserStats.GetStat))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(int) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out})]
    private static bool GetStat_Prefix(ref string pchName, out int pData, ref bool __result)
    {

        __result = WLPPlugin.LocalUserStats.GetStat(pchName, out pData);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamUserStats.GetStat))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(float) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Out })]
    private static bool GetStat_Prefix(ref string pchName, out float pData, ref bool __result)
    {

        __result = WLPPlugin.LocalUserStats.GetStat(pchName, out pData);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamUserStats.SetStat))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(int) })]
    private static bool SetStat_Prefix(string pchName, int nData, ref bool __result)
    {

        __result = WLPPlugin.LocalUserStats.SetStat(pchName, nData);
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SteamUserStats.SetStat))]
    [HarmonyPatch(new Type[] { typeof(string), typeof(float) })]
    private static bool SetStat_Prefix(string pchName, float fData, ref bool __result)
    {

        __result = WLPPlugin.LocalUserStats.SetStat(pchName, fData);
        return false;
    }
}