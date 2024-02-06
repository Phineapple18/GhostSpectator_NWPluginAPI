using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.IO;

using GhostSpectator.Commands.ClientConsole;
using GhostSpectator.Commands.ClientConsole.Duel;
using GhostSpectator.Commands.ClientConsole.ShootingTarget;
using GhostSpectator.Commands.ClientConsole.Voicechat;
using GhostSpectator.Commands.RemoteAdminConsole;
using PluginAPI.Core;
using PluginAPI.Helpers;
using Serialization;

namespace GhostSpectator.Commands
{
    public class CommandTranslation
    {
        [Description("Should debug for command loading be enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Translations for GhostSpectator parent command and its subcommands. Make sure not to duplicate commands or aliases.")]
        public string GhostSpectatorParentCommand { get; set; } = GhostSpectatorParent._command;

        public string GhostSpectatorParentDescription { get; set; } = GhostSpectatorParent._description;

        public string[] GhostSpectatorParentAliases { get; set; } = GhostSpectatorParent._aliases;

        public string DespawnCommand { get; set; } = Despawn._command;

        public string DespawnDescription { get; set; } = Despawn._description;

        public string[] DespawnAliases { get; set; } = Despawn._aliases;

        public string ListCommand { get; set; } = List._command;

        public string ListDescription { get; set; } = List._description;

        public string[] ListAliases { get; set; } = List._aliases;

        public string SpawnCommand { get; set; } = Spawn._command;

        public string SpawnDescription { get; set; } = Spawn._description;

        public string[] SpawnAliases { get; set; } = Spawn._aliases;

        [Description("Translations for Duel parent command and its subcommands. Make sure not to duplicate commands or aliases.")]

        public string DuelParentCommand { get; set; } = DuelParent._command;

        public string DuelParentDescription { get; set; } = DuelParent._description;

        public string[] DuelParentAliases { get; set; } = DuelParent._aliases;

        public string AbandonCommand { get; set; } = Abandon._command;

        public string AbandonDescription { get; set; } = Abandon._description;

        public string[] AbandonAliases { get; set; } = Abandon._aliases;

        public string AcceptCommand { get; set; } = Accept._command;

        public string AcceptDescription { get; set; } = Accept._description;

        public string[] AcceptAliases { get; set; } = Accept._aliases;

        public string CancelCommand { get; set; } = Cancel._command;

        public string CancelDescription { get; set; } = Cancel._description;

        public string[] CancelAliases { get; set; } = Cancel._aliases;

        public string ListDuelCommand { get; set; } = ListDuel._command;

        public string ListDuelDescription { get; set; } = ListDuel._description;

        public string[] ListDuelAliases { get; set; } = ListDuel._aliases;

        [Description("Translations for unassigned client commands. Make sure not to duplicate commands or aliases.")]

        public string CreatetargetCommand { get; set; } = CreateTarget._command;

        public string CreatetargetDescription { get; set; } = CreateTarget._description;

        public string[] CreatetargetAliases { get; set; } = CreateTarget._aliases;        

        public string DestroytargetCommand { get; set; } = DestroyTarget._command;

        public string DestroytargetDescription { get; set; } = DestroyTarget._description;

        public string[] DestroytargetAliases { get; set; } = DestroyTarget._aliases;

        public string DisableVoicechatCommand { get; set; } = DisableVoicechat._command;

        public string DisableVoicechatDescription { get; set; } = DisableVoicechat._description;

        public string[] DisableVoicechatAliases { get; set; } = DisableVoicechat._aliases;

        public string EnableVoicechatCommand { get; set; } = EnableVoicechat._command;

        public string EnableVoicechatDescription { get; set; } = EnableVoicechat._description;

        public string[] EnableVoicechatAliases { get; set; } = EnableVoicechat._aliases;

        public string GhostmeCommand { get; set; } = GhostMe._command;

        public string GhostmeDescription { get; set; } = GhostMe._description;

        public string[] GhostmeAliases { get; set; } = GhostMe._aliases;

        public string GivefirearmCommand { get; set; } = GiveFirearm._command;

        public string GivefirearmDescription { get; set; } = GiveFirearm._description;

        public string[] GivefirearmAliases { get; set; } = GiveFirearm._aliases;        

        [Description("Translations for command interface.")]
        public string Aliases { get; set; } = "Aliases";

        public string Description { get; set; } = "Description";

        public string Subcommands { get; set; } = "Available subcommands";     

        public string Usage { get; set; } = "Usage";

        [Description("Translations for assigned command responses. Don't translate words put between two '%'.")]
        public string CreatetargetSuccess { get; set; } = "You have spawned a shooting target with id %id%.";

        public string DespawnSuccess { get; set; } = "Succesfully despawned <color=green>%num%</color> existing player(s) from Ghosts";

        public string DespawnFail { get; set; } = "Command failed for <color=red>%num%</color> existing player(s) (not a Ghost)";

        public string DestroytargetSuccess { get; set; } = "You have despawned your shooting target.";

        public string DisableVoicechatFail { get; set; } = "You have either already disabled the provided voicechat(s) or don't have permission to have them disabled.";

        public string DisableVoicechatSuccess { get; set; } = "Successfully disabled listening to following groups";

        public string DuelParentSuccess { get; set; } = "You have challenged %player% to a duel.";

        public string EnableVoicechatFail { get; set; } = "You have either already enabled the provided voicechat(s) or don't have permission to have them enabled.";

        public string EnableVoicechatSuccess { get; set; } = "Successfully enabled listening to following groups";

        public string GivefirearmSuccess { get; set; } = "You have given yourself %itemType%. If you want to get rid of it, simply drop the firearm.";

        public string DuelAbandonSuccess { get; set; } = "You have abandoned your duel with %player%.";

        public string DuelAcceptSuccess { get; set; } = "You have accepted a duel request from %player%.";

        public string DuelCancelSuccess { get; set; } = "You have cancelled a duel request with %player%.";

        public string DuelList { get; set; } = "You were challenged to a duel by following players";

        public string FirearmList { get; set; } = "List of firearms";

        public string GhostList { get; set; } = "List of Ghosts <color=red>(%num%)</color>";

        public string SelfGhost { get; set; } = "You have changed yourself to a Ghost.";

        public string SelfSpec { get; set; } = "You have changed yourself to Spectator.";

        public string SelfFail { get; set; } = "You can't use this command, if you are neither dead nor a Ghost.";

        public string SpawnSuccess { get; set; } = "Succesfully turned <color=green>%num%</color> existing player(s) into Ghosts";

        public string SpawnFail { get; set; } = "Command failed for <color=red>%num%</color> existing player(s) (already a Ghost)";

        public string TargetList { get; set; } = "List of spawned targets <color=red>(%num%)</color>";

        [Description("Translations for command responses. Don't translate words put between two '%'.")]
        public string AfterWarhead { get; set; } = "You can't use that command after warhead detonation.";

        public string AlreadyActiveDuel { get; set; } = "Player %player% already has active duel.";

        public string AlreadyPendingDuel { get; set; } = "You already have pending duel request with %player%.";

        public string BeforeRound { get; set; } = "You can't use that command before round start.";

        public string DedicatedServer { get; set; } = "You can't use that command on Dedicated Server.";

        public string DuelAbandoned { get; set; } = "Player <color>%player%</color> has abandoned a duel with you.";

        public string DuelAborted { get; set; } = "Your duel has been aborted.";

        public string DuelPrepare { get; set; } = "Prepare for duel!";

        public string DuelRequestCancelled { get; set; } = "Player <color=red>%player%</color> has cancelled a duel request with you.";

        public string DuelRequestExpired { get; set; } = "Your duel request for <color=red>%player%</color> has expired.";

        public string DuelRequestReceived { get; set; } = "Player <color=red>%player%</color> has challenged you to a duel! \nType <color=yellow>.duel a (PlayerNickname)</color> within %time%s to accept the request or let it expire to reject.";

        public string DuelStarted { get; set; } = "Duel has started!";

        public string NoActiveDuel { get; set; } = "You don't have any active duel.";

        public string NoDuelRequests { get; set; } = "You don't have any pending duel requests.";

        public string NoGhosts { get; set; } = "No Ghost with the provided nickname, that is not you, was found.";

        public string NoPerms { get; set; } = "You don't have permission to use that command.";

        public string NoPlayers { get; set; } = "Provided player(s) doesn't exist.";

        public string NoTargets { get; set; } = "You don't have any spawned shooting targets.";

        public string NoTargetId { get; set; } = "You don't have any spawned shooting target with this id.";

        public string NotEnabled { get; set; } = "GhostSpectator is not enabled.";

        public string NotGhostOther { get; set; } = "The player you are trying to us this command on, is not a Ghost.";

        public string NotGhostSelf { get; set; } = "You can only use this command, if you are a Ghost.";

        public string NotGrounded { get; set; } = "You must be standing on the ground to spawn shooting target.";

        public string NotItemtype { get; set; } = "Provided argument is not an ItemType.";

        public string OnlyFirearm { get; set; } = "You can only give yourself a firearm.";

        public string SenderNull { get; set; } = "Commandsender is null.";   

        public string TargetNotAllowed { get; set; } = "You have reached the maximum allowed number of spawned shooting targets.";

        public string WrongArea { get; set; } = "You can spawn shooting targets only in a designated area.";      

        public string WrongArgument { get; set; } = "Provided argument(s) don't exist.";

        public string WrongFormat { get; set; } = "Argument must be an id of your spawned shooting target.";        

        internal static CommandTranslation PrepareTranslations()
        {
            string directoryPath = File.Exists(globalDll) ? globalTranslation : localTranslation;
            string filePath = Path.Combine(directoryPath, fileName);
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(directoryPath);             
                File.WriteAllText(filePath, YamlParser.Serializer.Serialize(new CommandTranslation()));
                Log.Info($"Created {(File.Exists(globalDll) ? "global" : "local")} directory and file for command translations.", "GhostSpectator");
            }
            CommandTranslation translation = YamlParser.Deserializer.Deserialize<CommandTranslation>(File.ReadAllText(filePath));
            Log.Info("Loaded command translations.", "GhostSpectator");
            File.WriteAllText(filePath, YamlParser.Serializer.Serialize(translation));
            return translation;   
        }

        public static CommandTranslation commandTranslation;

        private static readonly string globalDll = Path.Combine(Paths.GlobalPlugins.Plugins, "GhostSpectator.dll");

        private static readonly string globalTranslation = Path.Combine(Paths.GlobalPlugins.Plugins, "GhostSpectator");

        private static readonly string localTranslation = Path.Combine(Paths.LocalPlugins.Plugins, "GhostSpectator");

        private const string fileName = "commandtranslation.yml";
    }
}
