using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PlayerRoles.FirstPersonControl.Thirdperson;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(AnimatedCharacterModel), "PlayFootstep")]
    internal class FootstepPatch
    {
        internal static bool Prefix(AnimatedCharacterModel __instance)
        {
            return !__instance.OwnerHub.IsGhost();
        }
    }
}
