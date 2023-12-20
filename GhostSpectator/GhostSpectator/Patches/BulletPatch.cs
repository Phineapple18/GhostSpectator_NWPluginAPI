using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using NorthwoodLib.Pools;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(StandardHitregBase), "PlaceBulletholeDecal")]
    internal class BulletPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex((CodeInstruction i) => i.opcode == OpCodes.Ldarga_S);

            newInstructions.InsertRange(index, new List<CodeInstruction>
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(StandardHitregBase), "Hub")),
                new (OpCodes.Call, AccessTools.Method(typeof(GhostSpectator), nameof(GhostSpectator.IsGhost), new[] { typeof(ReferenceHub)})),
                new (OpCodes.Brtrue, returnLabel)
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);
            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
