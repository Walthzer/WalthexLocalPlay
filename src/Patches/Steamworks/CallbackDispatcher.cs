using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WalthexLocalPlay.Patches.Steamworks;

[HarmonyPatch(typeof(CallbackDispatcher))]
internal class CallbackDispatcherPatch
{
    //Extra layer to prevent callbacks from running twice.
    [HarmonyPrefix]
    [HarmonyPatch("RunFrame")]
    public static bool RunFrame_Prefix(bool isGameServer)
    {
        return false;
    }
}
