using HarmonyLib;
using UnityEngine;

namespace WalthexLocalPlay.Patches.Managers;

//Remove dependance on steamAPI
[HarmonyPatch(typeof(MainMenuManager))]
internal class MainMenuManagerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    private static void Start_PostFix(MainMenuManager __instance)
    {
        Screen.SetResolution(800*2, 600*2, false);
    }
}