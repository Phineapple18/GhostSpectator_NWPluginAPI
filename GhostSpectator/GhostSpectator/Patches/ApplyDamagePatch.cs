using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection.Emit;

using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using PluginAPI.Core;
using System.Reflection;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(StandardDamageHandler), nameof(StandardDamageHandler.ApplyDamage))]
    internal class ApplyDamagePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label skipLabel = generator.DefineLabel();
            newInstructions.FindAll((CodeInstruction i) => i.opcode == OpCodes.Ldarg_1).ElementAt(4).labels.Add(skipLabel);
            int index = newInstructions.FindIndex((CodeInstruction i) => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == AccessTools.Method(typeof(StandardDamageHandler), "ProcessDamage"));

            newInstructions.InsertRange(index, new List<CodeInstruction>
            {
                new (OpCodes.Call, AccessTools.Method(typeof(ApplyDamagePatch), nameof(SkipProcessing), new[] { typeof(StandardDamageHandler), typeof(ReferenceHub) })),
                new (OpCodes.Brtrue, skipLabel),
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldarg_1)
            });

            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool SkipProcessing(StandardDamageHandler sdh, ReferenceHub ply)
        {
            if (sdh is not AttackerDamageHandler adh || Server.FriendlyFire)
            {
                return false;
            }
            if (!(adh.Attacker.Hub.IsGhost() && ply.IsGhost()))
            {
                return false;
            }
            Player attacker = Player.Get(adh.Attacker.Hub);
            Player target = Player.Get(ply);
            return attacker.GetGhostComponent().duelPartner == target && target.GetGhostComponent().duelPartner == attacker;
        }
    }
}
