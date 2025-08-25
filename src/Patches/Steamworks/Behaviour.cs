using HarmonyLib;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

namespace WalthexLocalPlay.Patches.Heathen;

//Remove dependance on steamAPI
[HarmonyPatch(typeof(SteamworksBehaviour))]
internal class BehaviourPatch
{
    [HarmonyPatch("OnEnable")]
    [HarmonyPrefix]
    private static bool OnEnable_Prefix()
    {
        return false;
    }

    [HarmonyPatch("HandleInitialization")]
    [HarmonyPrefix]
    private static bool HandleInitialization_Prefix()
    {
        return false;
    }
}