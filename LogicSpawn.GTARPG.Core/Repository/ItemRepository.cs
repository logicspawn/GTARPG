using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;

namespace LogicSpawn.GTARPG.Core.Repository
{
    public static class ItemRepository
    {
        public static List<Item> Items;

        static ItemRepository()
        {
            Items = new List<Item>();
            Items.Add(new Item("Bandages", "Good for healing the most basic of injuries. Restores 5 health.", 50, 1, ItemType.Usable, Rarity.Common, () => { Game.Player.Character.Health += 5; }));
            Items.Add(new Item("Health Kit", "Basic health kit, restores 10 health.", 400, 1, ItemType.Usable, Rarity.Uncommon, () => { Game.Player.Character.Health += 10; })
            {
                CoolDownMs = 15000
            });
            Items.Add(new Item("Adv Health Kit", "Advanced health kit, restores 50 health.", 1000, 1, ItemType.Usable, Rarity.Uncommon, () => { Game.Player.Character.Health += 50; })
            {
                CoolDownMs = 10000,
                CanBuy = false,
                CanCraft = true,
                CraftItems = new Dictionary<string, int> { { "Health Kit", 1 }, {"Bandages", 5} }
            });

            Items.Add(new Item("Simple Protective Gear", "Better than nothing I guess. Restores 5 armor", 100, 1, ItemType.Usable, Rarity.Common, () => { Game.Player.Character.Armor += 5; }));
            Items.Add(new Item("Refurbished Kevlar", "Bet you won't find this on ebay... Restores 10 armor", 800, 1, ItemType.Usable, Rarity.Uncommon, () => { Game.Player.Character.Armor += 10; }));
            Items.Add(new Item("Adv Armor Kit", "A strong piece of armor. Restores 50 armor", 2000, 1, ItemType.Usable, Rarity.Uncommon, () => { Game.Player.Character.Armor += 50; })
            {
                CoolDownMs = 10000,
                CanBuy = false,
                CanCraft = true,
                CraftItems = new Dictionary<string, int> { { "Refurbished Kevlar", 1 }, { "Simple Protective Gear", 5 } }
            });

            Items.Add(new Item("Ammo Pack I", "Tier 1 ammo pack, restores a small amount of ammo.", 500, 1, ItemType.Usable, Rarity.Common, GiveAmmo(1)));

            Items.Add(new Item("Ammo Pack II", "Tier 2 ammo pack, restores a medium amount of ammo.", 1000, 1, ItemType.Usable, Rarity.Uncommon, GiveAmmo(3))
            {
                CanCraft = true,
                CraftItems = new Dictionary<string, int> { { "Ammo Pack I", 2 }, {"Basic Scraps", 10} }
            });

            Items.Add(new Item("Ammo Pack III", "Tier 3 ammo pack, restores a large amount of ammo.", 2500, 1, ItemType.Usable, Rarity.Uncommon, GiveAmmo(8))
            {
                CanBuy = false,
                CanCraft = true,
                CraftItems = new Dictionary<string, int> { { "Ammo Pack II", 3 }, { "Basic Scraps", 20 } }
            });

            Items.Add(new Item("Vehicle Repair Kit", "Restores vehicle to maximum condition.", 2000, 1, ItemType.Usable, Rarity.Uncommon, () => { var v = Game.Player.Character.CurrentVehicle; if (v != null) v.Repair(); })
            {
                CanBuy = false,
                CanCraft =  true, 
                CraftItems = new Dictionary<string,int>{ {"Vehicle Parts",3}}
            });

            Items.Add(new Item("Basic Scraps", "Random scraps. Useful for crafting.", 100, 5, ItemType.Component, Rarity.Common, () => { }) { CanBuy = true });
            Items.Add(new Item("Vehicle Parts", "Combine 10 of these to make a repair kit.", 0, 1, ItemType.Component, Rarity.Common, () => { }) { CanBuy = false });

            //Quest
            Items.Add(new Item("Sweet Green", "Woah, this bag literally reeks of cash.", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("'Promising Goods'", "I don't know what's in this... or if I should ask.", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Car Radio", "Just about managed to salvage this.", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Police Sirens", "WOOOOoop WOOOOop dat's the sound of da police!.", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Cowhide", "Looks like a cowhide", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Feather", "Plucked straight from a chicken.", 0, 5, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Shredding Machine", "Now this is what I call a guitar!", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Incriminating Phone", "This phone has data Apexers don't want to be public.", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Woman's Purse", "It has money in it, and a mirror. Better not drop this.", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Men's Wallet", "Hmm, quite a few credit cards in here...", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });
            Items.Add(new Item("Boxed Package", "I wonder what's in here?", 0, 1, ItemType.QuestItem, Rarity.Quest, () => { }) { CanBuy = false });

        }

        private static Action GiveAmmo(int multiplier)
        {
            return () =>
            {
                for (int i = 0; i < Data.WeaponHashes.Length; i++)
                {
                    var wepName = Data.WeaponHashes[i];
                    var wep = Game.Player.Character.Weapons[wepName];
                    if (wep != null)
                    {
                        wep.Ammo += wep.MaxAmmoInClip  * multiplier;
                    }
                }
            };
        }

        public static Item Cash(int amount)
        {
            return new Item("Money", "It's money!",0, 1, ItemType.Money, Rarity.Common, () => { }){MoneyValue = amount};
        }

        public static Item Get(string name)
        {
            return GM.Copy(Items.FirstOrDefault(i => i.Name == name));
        }

        public static Action GetAction(string name)
        {
            var item = Items.FirstOrDefault(i => i.Name == name);
            if(item != null)
            {
                return item.OnUse;
            }

            return null;
        }
    }
}