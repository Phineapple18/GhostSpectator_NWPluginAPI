using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.Voice;
using PluginAPI.Core;
using System.Reflection;
using System.Reflection.Emit;
using VoiceChat;
using VoiceChat.Networking;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public  class VoiceChannelPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex((CodeInstruction i) => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == AccessTools.Method(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ValidateReceive)));
            int offset = 2;

            newInstructions.InsertRange(index + offset, new List<CodeInstruction>
            {
                new (OpCodes.Ldloc_3),
                new (OpCodes.Ldarg_1),
                new (OpCodes.Ldfld, AccessTools.Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
                new (OpCodes.Ldloca_S, 5),
                new (OpCodes.Call, AccessTools.Method(typeof(VoiceChannelPatch), nameof(OverrideVoicechannel), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(VoiceChatChannel).MakeByRefType() }))
            });

            for (int i = 0; i < newInstructions.Count; i++)
            {
                yield return newInstructions[i];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void OverrideVoicechannel(ReferenceHub hub1, ReferenceHub hub2, ref VoiceChatChannel voiceChatChannel2)
        {
            Player receiver = Player.Get(hub1);
            if (hub1 == hub2 || Round.IsRoundEnded || !receiver.IsGhost())
            {
                return;
            }
            Player sender = Player.Get(hub2);
            if (receiver.TemporaryData.StoredData.Keys.Any(s => s == $"Vc{sender.VoiceChannel}"))
            {
                voiceChatChannel2 = sender.VoiceChannel;
                return;
            }
            if (sender.IsGhost() && receiver.TemporaryData.Contains("ListenGhosts") && voiceChatChannel2 != VoiceChatChannel.Proximity)
            {
                voiceChatChannel2 = VoiceChatChannel.RoundSummary;
                return;
            }
        }
    }
}
