using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.Spectating;
using PluginAPI.Core;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Scp049ResurrectAbility), "ServerValidateAny")]
    internal class Scp049ResurrectPatch
	{
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex((CodeInstruction i) => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == AccessTools.PropertyGetter(typeof(PlayerRoleManager), "CurrentRole"));
            
            newInstructions.RemoveRange(index - 1, 3);
            newInstructions.Insert(index - 1, new (OpCodes.Call, AccessTools.Method(typeof(Scp049ResurrectPatch), nameof(IsDead), new Type[] { typeof(ReferenceHub) })));

            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool IsDead(ReferenceHub hub)
        {
            Player player = Player.Get(hub);
            return player.RoleBase is SpectatorRole || player.TemporaryData.Contains("IsGhostSpectator");
        }
    }
}
