using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PluginAPI.Core;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(TeslaGate), nameof(TeslaGate.IsInIdleRange), new Type[] { typeof(ReferenceHub) })]
    internal class TeslaIdleRangePatch
    {
        internal static void Postfix(TeslaGate __instance, ReferenceHub player, ref bool __result)
        {
            if (Player.Get(player).IsGhost())
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(TeslaGate), nameof(TeslaGate.PlayerInRange))]
    internal class TeslaPlayerInRangePatch
    {
        internal static void Postfix(TeslaGate __instance, ReferenceHub player, ref bool __result)
        {
            if (Player.Get(player).IsGhost())
            {
                __result = false;
            }
        }
    }
}
