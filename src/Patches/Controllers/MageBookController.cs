using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

namespace ModTemplate.Patches.Controllers;


//This patch casts Magic Missle everytime you equip the Mage Book
[HarmonyPatch(typeof(MageBookController))]
internal class MageBookControllerPatch
{
    [HarmonyPatch(nameof(MageBookController.ItemInit))]
    [HarmonyPrefix]
    private static void PostFix(MageBookController __instance)
    {
        //Get player based on inventory:
        GameObject player = GameObject.FindWithTag("Player");
        __instance.CastWard(player, 1);
        ModTemplatePlugin.Logger.LogMessage("Mage Book has been equipped!");
    }
}