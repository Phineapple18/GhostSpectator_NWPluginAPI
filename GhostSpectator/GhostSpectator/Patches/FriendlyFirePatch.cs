using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using PlayerStatsSystem;

namespace GhostSpectator.Patches
{
    [HarmonyPatch("FriendlyFireHandler", "IsFriendlyFire")]
    internal class FriendlyFirePatch
    {
        internal static void Postfix(ReferenceHub damagedPlayer, DamageHandlerBase handler, ref bool __result)
        {
            AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
            if (__result == true && attackerDamageHandler.Attacker.Hub.IsGhost() && damagedPlayer.IsGhost())
            {
                __result = false;
            }
        }
    }
}
