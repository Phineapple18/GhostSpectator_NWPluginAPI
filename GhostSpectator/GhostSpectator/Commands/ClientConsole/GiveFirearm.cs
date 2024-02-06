using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class GiveFirearm : ICommand, IUsageProvider
    {
        public GiveFirearm ()
        {
            translation = CommandTranslation.commandTranslation;
            Command = !string.IsNullOrWhiteSpace(translation.GivefirearmCommand) ? translation.GivefirearmCommand : _command;
            Description = !string.IsNullOrWhiteSpace(translation.GivefirearmDescription) ? translation.GivefirearmDescription : _description;
            Aliases = translation.GivefirearmAliases;
            Log.Debug("Loaded GiveFirearm command.", translation.Debug, "GhostSpectator");
        }

        public string[] Usage { get; } = new string[]
        {
            "%item%/list"
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Plugin.Singleton == null)
            {
                response = translation.NotEnabled;
                return false;
            }
            if (sender == null)
            {
                response = translation.SenderNull;
                return false;
            }
            if (!sender.CheckPermission("gs.firearm"))
            {
                response = translation.NoPerms;
                return false;
            }
            Player commandsender = Player.Get(sender);
            if (!commandsender.IsGhost())
            {
                response = translation.NotGhostSelf;
                return false;
            }
            if (arguments.IsEmpty())
            {
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
            }
            if (arguments.At(0).ToLower() == "list")
            {
                response = $"{translation.FirearmList}:" + string.Join("\n- ", from g in InventoryItemLoader.AvailableItems where g.Value.Category == ItemCategory.Firearm select g.Key);
                return true;
            }
            if (!Enum.TryParse(arguments.At(0), out ItemType itemType))
            {
                response = translation.NotItemtype;
                return false;
            }
            bool isFirearm = InventoryItemLoader.AvailableItems.Any(i => i.Key == itemType && i.Value.Category == ItemCategory.Firearm);    
            if (!isFirearm)
            {
                response = translation.OnlyFirearm;
                return false;
            }        
            Firearm firearm = commandsender.AddItem(itemType) as Firearm;
            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(commandsender.ReferenceHub, out Dictionary<ItemType, uint> dictionary) && dictionary.TryGetValue(itemType, out uint code))
            {
                firearm.ApplyAttachmentsCode(code, true);
            }
            firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());
            response = translation.GivefirearmSuccess.Replace("%itemType%", itemType.ToString());
            Log.Debug($"Player {commandsender.Nickname} gave himself a firearm {itemType}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.GiveFirearm");
            return true;
        }

        internal const string _command = "givefirearm";

        internal const string _description = "Give yourself a firearm or print a list of available firearms.";

        internal static readonly string[] _aliases = new string[] { "firearm", "givegun", "gun" };

        private readonly CommandTranslation translation;

        public string Command { get; }
        public string[] Aliases { get; }
        public string Description { get; }
    }
}
