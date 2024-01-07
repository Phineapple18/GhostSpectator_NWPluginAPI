using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp049;
using PluginAPI.Core;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Scp049ResurrectAbility), "ServerValidateAny")]
    internal class Scp049ResurrectPatch
	{
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label checkNext = generator.DefineLabel();
            newInstructions.FindAll((CodeInstruction i) => i.opcode == OpCodes.Ldarg_0).ElementAt(3).labels.Add(checkNext);
            int index = newInstructions.FindIndex((CodeInstruction i) => i.opcode == OpCodes.Isinst);
            int offset = 1;

            newInstructions.InsertRange(index + offset, new List<CodeInstruction>
            {
                new (OpCodes.Brtrue_S, checkNext),
                new (OpCodes.Ldloc_0),
                new (OpCodes.Call, AccessTools.Method(typeof(Scp049ResurrectPatch), nameof(IsDead), new Type[] {typeof(ReferenceHub)})),
            });

            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool IsDead(ReferenceHub hub)
        {
            return Player.Get(hub).TemporaryData.Contains("IsGhostSpectator");
        }
    }
}
