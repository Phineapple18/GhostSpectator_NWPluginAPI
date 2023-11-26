using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerRoles.Visibility;
using PlayerRoles.Spectating;
using PlayerRoles;
using PluginAPI.Core;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(FpcServerPositionDistributor), nameof(FpcServerPositionDistributor.WriteAll))]
    internal class VisibilityPatch
	{
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            
            int index = newInstructions.FindIndex((CodeInstruction i) => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == AccessTools.Method(typeof(VisibilityController), nameof(VisibilityController.ValidateVisibility)));
            int offset = 6;

            newInstructions.InsertRange(index + offset, new List<CodeInstruction>
            {
                new (OpCodes.Ldarg_0),
                new (OpCodes.Ldloc_S, 5),
                new (OpCodes.Ldloca_S, 7),
                new (OpCodes.Call, AccessTools.Method(typeof(VisibilityPatch), nameof(IsGhostInvisible), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool).MakeByRefType() }))
            });

            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void IsGhostInvisible(ReferenceHub receiver, ReferenceHub referenceHub, ref bool isInvisible)
        {
            Player observer = Player.Get(receiver);
            Player target = Player.Get(referenceHub);

            if (target.IsGhost())
            {
                if (observer.IsGhost() || observer.Role == RoleTypeId.Overwatch)
                {
                    isInvisible = false;
                    return;
                }
                if (observer.IsAlive)
                {
                    isInvisible = true;
                    return;
                }
                if (observer.Role == RoleTypeId.Filmmaker)
                {
                    isInvisible = !Plugin.Singleton.PluginConfig.FilmmakerSeeGhosts;
                    return;
                }
                Player spectated = Player.GetPlayers().FirstOrDefault(p => p.ReferenceHub.IsSpectatedBy(observer.ReferenceHub));
                isInvisible = !(spectated.IsGhost() || Plugin.Singleton.PluginConfig.AlwaysSeeGhosts);
            }
        }
    }
}
