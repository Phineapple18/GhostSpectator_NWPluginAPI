using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal class BulletPatch
    {
        private static bool Prefix(StandardHitregBase __instance, Ray ray, RaycastHit hit)
        {
            return !Player.Get(__instance.Hub).IsGhost();
        }
    }
}
