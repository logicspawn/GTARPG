using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LogicSpawn.GTARPG.Core.Repository;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class Item
    {
        public ItemType Type;
        public string Name;
        public string Description;
        public int Quantity;
        public int Cost;
        public Rarity Rarity;
        public CoolDown CoolDownTimer;

        [JsonIgnore]
        public int CoolDownMs
        {
            get { return CoolDownTimer.CoolDownMsTime; }
            set { CoolDownTimer.CoolDownMsTime = value; }
        }

        [JsonIgnore]
        public Action OnUse;

        public int MoneyValue;
        public bool CanBuy;
        public bool CanCraft;
        public Dictionary<string, int> CraftItems;

        public Item()
        {
            Rarity = Rarity.Common;
        }

        public Item(string name, string desc,int cost, int initialQuantity, ItemType type, Rarity rarity, Action onUse)
        {
            Name = name;
            Description = desc;
            Cost = cost;
            Rarity = rarity;
            Quantity = initialQuantity;
            Type = type;
            OnUse = onUse;
            CanBuy = true;
            CoolDownTimer = new CoolDown(0);
        }

        [JsonIgnore]
        public bool Usable
        {
            get { return Type == ItemType.Usable; }
        }

        public bool Use()
        {
            if (!Usable) return false;
            if (CoolDownTimer.Usable)
            {
                ItemRepository.GetAction(Name).Invoke();
                CoolDownTimer.Reset();
                return true;
            }

            var secs = CoolDownTimer.SecondsRemaining.ToString("0.0");
            RPG.Notify("cannot use " + Name + " for " + secs + "s");

            return false;
        }

        public string GetCraftString()
        {
            string result = "";
            foreach (var item in CraftItems)
            {
                var itemInPlayerInvo = RPG.PlayerData.Inventory.FirstOrDefault(i => i.Name == item.Key);
                if(itemInPlayerInvo != null)
                {
                    result += (item.Key + " - " + itemInPlayerInvo.Quantity + "/" + item.Value + "\n");
                }
                else
                {
                    result += (item.Key + " - 0/" + item.Value + "\n");
                }
            }
            return result;
        }

        public Color GetRarityColor()
        {
            switch(Rarity)
            {
                case Rarity.Common:
                    return Color.FromArgb(150, 180, 180, 180);
                case Rarity.Uncommon:
                    return Color.FromArgb(150, 0, 180, 0);
                case Rarity.Rare:
                    return Color.FromArgb(150, 15, 80, 255);
                case Rarity.Legendary:
                    return Color.FromArgb(150, 220, 0, 255);
                case Rarity.GrandTheftAuto:
                    return Color.FromArgb(150, 220, 0, 0);
                case Rarity.Unique:
                    return Color.Gold;
                case Rarity.Quest:
                    return Color.Cyan;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3,
        GrandTheftAuto =4,
        Quest = 5,
        Unique = 6,
    }

    public enum ItemType
    {
        Money,
        Usable,
        QuestItem,
        Component
    }
}