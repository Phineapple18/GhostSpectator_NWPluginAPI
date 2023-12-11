using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandSystem;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using NWAPIPermissionSystem;
using PluginAPI.Core;

namespace GhostSpectator.Commands.ClientConsole
{
    public class GiveFirearm : ICommand, IUsageProvider
    {
        public GiveFirearm (string command, string description, string[] aliases)
        {
            Command = !string.IsNullOrWhiteSpace(command) ? command : _command;
            Description = !string.IsNullOrWhiteSpace(description) ? description : _description;
            Aliases = !aliases.IsEmpty() ? aliases : _aliases;
        }

        public string Command { get; }

        public string[] Aliases { get; }

        public string Description { get; }

        public string[] Usage { get; } = new string[]
        {
            "ItemType/\"list\""
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandTranslation translation = CommandTranslation.loadedTranslation;
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
            if (arguments.IsEmpty())
            {
                response = $"{translation.Usage}: {this.DisplayCommandUsage()}.";
                return false;
            }
            if (arguments.At(0).ToLower() == "list")
            {
                response = $"{translation.AvailableFirearms}:" + string.Join("\n- ", from g in InventoryItemLoader.AvailableItems where g.Value.Category == ItemCategory.Firearm select g.Key);
                return true;
            }
            if (!Enum.TryParse(arguments.At(0), out ItemType itemType))
            {
                response = translation.NotItemtype;
                return false;
            }
            Player commandsender = Player.Get(sender);
            ItemBase item = commandsender.AddItem(itemType);
            if (item.Category != ItemCategory.Firearm)
            {
                response = translation.OnlyFirearm;
                commandsender.RemoveItem(item);
                return false;
            }
            Firearm firearm = item as Firearm;
            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(commandsender.ReferenceHub, out Dictionary<ItemType, uint> dictionary) && dictionary.TryGetValue(item.ItemTypeId, out uint code))
            {
                firearm.ApplyAttachmentsCode(code, true);
            }
            firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());
            response = translation.GivefirearmSuccess.Replace("%itemType%", itemType.ToString());
            Log.Debug($"Player {commandsender.Nickname} gave himself a firearm: {itemType}.", Plugin.Singleton.PluginConfig.Debug, $"{Plugin.Singleton.pluginHandler.PluginName}.GiveFirearm");
            return true;
        }

        internal static readonly string _command = "givefirearm";

        internal static readonly string _description = "Give yourself a firearm.";

        internal static readonly string[] _aliases = new string[] { "firearm", "givegun", "gun" };
    }
}
