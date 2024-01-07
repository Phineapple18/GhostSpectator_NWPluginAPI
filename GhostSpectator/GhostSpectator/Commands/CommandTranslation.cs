using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

using GhostSpectator.Commands.ClientConsole;
using GhostSpectator.Commands.RemoteAdminConsole;
using PluginAPI.Helpers;
using Serialization;

namespace GhostSpectator.Commands
{
    public class CommandTranslation
    {
        [Description("Translations for commands. Make sure not to duplicate commands or aliases.")]
        public string ParentCommand { get; set; } = _parentCommand;

        public string ParentDescription { get; set; } = _parentDescription;

        public string[] ParentAliases { get; set; } = _parentAliases;

        public string CreatetargetCommand { get; set; } = CreateTarget._command;

        public string CreatetargetDescription { get; set; } = CreateTarget._description;

        public string[] CreatetargetAliases { get; set; } = CreateTarget._aliases;

        public string DespawnCommand { get; set; } = Despawn._command;

        public string DespawnDescription { get; set; } = Despawn._description;

        public string[] DespawnAliases { get; set; } = Despawn._aliases;

        public string DestroytargetCommand { get; set; } = DestroyTarget._command;

        public string DestroytargetDescription { get; set; } = DestroyTarget._description;

        public string[] DestroytargetAliases { get; set; } = DestroyTarget._aliases;

        public string DisableVoicechannelCommand { get; set; } = DisableVoicechannel._command;

        public string DisableVoicechannelDescription { get; set; } = DisableVoicechannel._description;

        public string[] DisableVoicechannelAliases { get; set; } = DisableVoicechannel._aliases;

        public string EnableVoicechannelCommand { get; set; } = EnableVoicechannel._command;

        public string EnableVoicechannelDescription { get; set; } = EnableVoicechannel._description;

        public string[] EnableVoicechannelAliases { get; set; } = EnableVoicechannel._aliases;

        public string GhostmeCommand { get; set; } = GhostMe._command;

        public string GhostmeDescription { get; set; } = GhostMe._description;

        public string[] GhostmeAliases { get; set; } = GhostMe._aliases;

        public string GivefirearmCommand { get; set; } = GiveFirearm._command;

        public string GivefirearmDescription { get; set; } = GiveFirearm._description;

        public string[] GivefirearmAliases { get; set; } = GiveFirearm._aliases;

        public string ListCommand { get; set; } = List._command;

        public string ListDescription { get; set; } = List._description;

        public string[] ListAliases { get; set; } = List._aliases;

        public string SpawnCommand { get; set; } = Spawn._command;

        public string SpawnDescription { get; set; } = Spawn._description;

        public string[] SpawnAliases { get; set; } = Spawn._aliases;

        [Description("Translations for commands responses. Don't translate words put between two '%'.")]
        public string Aliases { get; set; } = "Aliases";

        public string Description { get; set; } = "Description";

        public string Subcommands { get; set; } = "Available subcommands";     

        public string Usage { get; set; } = "Usage";

        public string AvailableFirearms { get; set; } = "Available firearms";

        public string GhostList { get; set; } = "List of Ghosts <color=red>(%num%)</color>";

        public string TargetList { get; set; } = "List of spawned targets <color=red>(%num%)</color>";

        public string SenderNull { get; set; } = "Commandsender is null.";

        public string NoPerms { get; set; } = "You don't have permission to use that command.";

        public string NoPlayers { get; set; } = "Provided player(s) doesn't exist.";

        public string NoTargets { get; set; } = "You don't have any spawned shooting targets.";

        public string NoTargetId { get; set; } = "You don't have any spawned shooting target with this id.";

        public string NotEnabled { get; set; } = "GhostSpectator is not enabled.";

        public string NotGhost { get; set; } = "You can only use this command, if you are a Ghost.";

        public string NotGrounded { get; set; } = "You must be standing on the ground to spawn shooting target.";

        public string NotItemtype { get; set; } = "Provided argument is not an ItemType.";

        public string AfterWarhead { get; set; } = "You can't use that command after warhead detonation.";

        public string BeforeRound { get; set; } = "You can't use that command before round start.";

        public string DedicatedServer { get; set; } = "You can't use that command on Dedicated Server.";

        public string OnlyFirearm { get; set; } = "You can only give yourself a firearm.";

        public string TargetNotAllowed { get; set; } = "You have reached the maximum allowed number of spawned shooting targets.";

        public string WrongArea { get; set; } = "You can spawn shooting targets only in a designated area.";      

        public string WrongArgument { get; set; } = "Provided argument(s) don't exist.";

        public string WrongFormat { get; set; } = "Argument must be an id of your spawned shooting target.";

        public string CreatetargetSuccess { get; set; } = "You have spawned a shooting target with id %id%.";

        public string DespawnSuccess { get; set; } = "Succesfully despawned <color=green>%num%</color> existing player(s) from Ghosts";

        public string DespawnFail { get; set; } = "Command failed for <color=red>%num%</color> existing player(s) (not a Ghost or a Dedicated Server)";

        public string DestroytargetSuccess { get; set; } = "You have despawned your shooting target.";

        public string DisableVoicechatFail { get; set; } = "You have either already disabled the provided voicechannel(s) or don't have permission to have them disabled.";

        public string DisableVoicechatSuccess { get; set; } = "Successfully disabled listening to following groups";

        public string EnableVoicechatFail { get; set; } = "You have either already enabled the provided voicechannel(s) or don't have permission to have them enabled.";

        public string EnableVoicechatSuccess { get; set; } = "Successfully enabled listening to following groups";

        public string GivefirearmSuccess { get; set; } = "You have given yourself %itemType%. If you want to get rid of it, simply drop the firearm.";

        public string SelfGhost { get; set; } = "You have changed yourself to a Ghost.";

        public string SelfSpec { get; set; } = "You have changed yourself to Spectator.";

        public string SelfFail { get; set; } = "You can't use this command, if you are neither dead nor a Ghost.";

        public string SpawnSuccess { get; set; } = "Succesfully turned <color=green>%num%</color> existing player(s) into Ghosts";

        public string SpawnFail { get; set; } = "Command failed for <color=red>%num%</color> existing player(s) (already a Ghost or a Dedicated Server)";

        internal static CommandTranslation PrepareTranslations()
        {
            string directoryPath = File.Exists(globalDll) ? globalTranslation : localTranslation;
            string filePath = Path.Combine(directoryPath, fileName);
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(filePath, YamlParser.Serializer.Serialize(new CommandTranslation()));
            }
            CommandTranslation translation = YamlParser.Deserializer.Deserialize<CommandTranslation>(File.ReadAllText(filePath));
            loadedTranslation = translation;
            File.WriteAllText(filePath, YamlParser.Serializer.Serialize(translation));
            return translation;   
        }

        public static CommandTranslation loadedTranslation;

        internal static readonly string _parentCommand = "ghostspectator";

        internal static readonly string _parentDescription = "Parent command for GhostSpectator.";

        internal static readonly string[] _parentAliases = new string[] { "ghost", "gsp", "gs" };

        private static readonly string globalDll = Path.Combine(Paths.GlobalPlugins.Plugins, "GhostSpectator.dll");

        private static readonly string globalTranslation = Path.Combine(Paths.GlobalPlugins.Plugins, "GhostSpectator");

        private static readonly string localTranslation = Path.Combine(Paths.LocalPlugins.Plugins, "GhostSpectator");

        private static readonly string fileName = "commandtranslation.yml";
    }
}
