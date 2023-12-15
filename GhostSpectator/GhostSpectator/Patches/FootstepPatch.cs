using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PluginAPI.Core;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(AnimatedCharacterModel), "PlayFootstep")]

    internal class FootstepPatch
    {
        internal static bool Prefix(AnimatedCharacterModel __instance)
        {
            return !Player.Get(__instance.OwnerHub).IsGhost();
        }
    }
}
