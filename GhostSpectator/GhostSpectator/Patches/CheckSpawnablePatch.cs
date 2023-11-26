using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PluginAPI.Core;
using Respawning;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.CheckSpawnable))]
    internal class CheckSpawnablePatch
    {
        internal static void Postfix(RespawnManager __instance, ReferenceHub ply, ref bool __result)
        {
            if (Player.Get(ply).IsGhost())
            {
                __result = true;
            }
        }
    }
}
