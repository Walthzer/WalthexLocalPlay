using HarmonyLib;
using UnityEngine;

namespace WalthexLocalPlay.Patches.Managers;

//Remove dependance on steamAPI
[HarmonyPatch(typeof(BootstrapManager))]
internal class BootstrapManagerPatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    private static void Awake_PostFix(BootstrapManager __instance)
    {
        __instance.GoToMenu();
    }
}