using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Objects;
using Newtonsoft.Json.Linq;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core.General
{
    public static class RPGMethods
    {
        private static PlayerData PlayerData
        {
            get { return RPG.PlayerData; }
        }
        private static Ped Player
        {
            get { return Game.Player.Character; }
        }

        public static Vehicle SpawnCar()
        {
            if(Player.IsInVehicle())
            {
                var c = Player.CurrentVehicle;
                Player.Task.WarpOutOfVehicle(c);
                c.Delete();
            }
            
            var m = new Model(PlayerData.CarHash);
            m.Request(5000);

            while (!m.IsLoaded)
                Script.Wait(0);

            var vec = World.CreateVehicle(m, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f, Game.Player.Character.Heading + 90);
            RPG.WorldData.AddVehicle(new VehicleObject("rpg_PlayerVehicle",vec));
            Script.Wait(100);
            vec.PrimaryColor = PlayerData.CarColor;
            vec.SecondaryColor = PlayerData.CarSecondaryColor;
            vec.NumberPlate = "";
            vec.NumberPlate = PlayerData.NumberPlate;

            return vec;
        }

        public static Vector3 GetSpawnPoint(float distanceFromPlayer)
        {
            try
            {
                Script.Wait(100);
                Model m = new Model(VehicleHash.Panto);
                m.Request(2000);

                var v = World.CreateVehicle(m, Game.Player.Character.Position.Around(distanceFromPlayer));
                Script.Wait(100);
                try
                {
                    if (v != null)
                        v.PlaceOnNextStreet();
                }
                catch (Exception ex)
                {
                    RPGLog.Log(ex);
                }

                var pos = v != null ? v.Position : Game.Player.Character.Position.Around(distanceFromPlayer);
                if (v != null) v.Delete();
                return pos;
            }
            catch(Exception ex)
            {
                RPGLog.Log(ex);
                throw;
            }
        }

        public static void ReturnToNormal()
        {
            RPG.SaveAllData();

            Game.FadeScreenOut(500);
            var c = Player.CurrentVehicle;
            if (c != null && c.Exists())
            {
                Player.Task.WarpOutOfVehicle(c);
                if (c.Exists())
                {
                    c.Delete();
                }
            }

            while (RPG.WorldData.AllObjects.Any())
            {
                RPG.WorldData.AllObjects.First().Destroy();
            }
            
            Model m = PedHash.Michael;
            m.Request(1000);
            Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, m.Hash);
            RPG.GameHandler.InitiateNpcs = false;
            RPG.GameLoaded = false;
            RPGInit.Enabled = true;

            Game.FadeScreenIn(500);
        }

        public static void UnlockWeapons()
        {
            RPG.PlayerData.Weapons = new List<WeaponDefinition>();
            Game.Player.Character.Weapons.RemoveAll();
            for (int i = 0; i < Data.WeaponHashes.Length; i++)
            {
                var wepName = Data.WeaponHashes[i];
                RPG.PlayerData.Weapons.Add(new WeaponDefinition()
                {
                    WeaponHash = wepName,
                    AmmoCount = 1000
                });
                Game.Player.Character.Weapons.Give(wepName, 1000, false, false);
            }
            Game.Player.Character.Weapons.Give(WeaponHash.Unarmed, 0, true, false);
            RPG.SaveAllData();
        }

        public static void LoadPlayerWeapons()
        {
            try
            {
                Game.Player.Character.Weapons.RemoveAll(); //
                for (int i = 0; i < PlayerData.Weapons.Count; i++)
                {
                    var wepDefinition = PlayerData.Weapons[i];
                    Game.Player.Character.Weapons.Give(wepDefinition.WeaponHash, 0, false, false);
                    Script.Wait(50);
                    Game.Player.Character.Weapons[wepDefinition.WeaponHash].Ammo = wepDefinition.AmmoCount;
                }
                Game.Player.Character.Weapons.Give(WeaponHash.Unarmed, 0, true, false);
            }
            catch(Exception ex)
            {
                RPGLog.Log(ex);
            }
        }

        public static void Loot(LootItem loot)
        {
            if (loot != null)
            {
                if (loot.Item.Type == ItemType.Money)
                {
                    var notification = Notification.Loot("Looted GTA$" + loot.Item.MoneyValue.ToString("N0"));
                    notification.Tracking = loot.Item.MoneyValue;
                    RPG.Notify(notification);
                    PlayerData.AddMoney(loot.Item.MoneyValue);
                    loot.Destroy();
                    Game.PlaySound("FocusOut", "HintCamSounds");
                }
                else
                {
                    RPG.Notify(Notification.Loot("Looted: " + loot.Name + " x" + loot.Item.Quantity));
                    PlayerData.AddItem(loot.Item);
                    loot.Destroy();
                    Game.PlaySound("FocusOut", "HintCamSounds");
                }
            }

            RPGInfo.NearbyLoot = null;
        }

        public static bool UseItem(Item itemToUse)
        {
            var used = itemToUse.Use();
            if (used)
            {
                itemToUse.Quantity -= 1;
                if (itemToUse.Quantity <= 0)
                {
                    PlayerData.Inventory.Remove(itemToUse);
                }
            }

            return used;
        }

        public static bool UseItem(string itemName)
        {
            var item = PlayerData.Inventory.FirstOrDefault(i => i.Name == itemName);
            if(item != null)
            {
                return UseItem(item);
            }

            return false;
        }

        public static void UseSkill(Skill skill)
        {

            if (!skill.Unlocked)
            {
                RPG.Notify(Notification.Alert("You have not unlocked this skill."));
                return;
            }

            skill.Use(skill);

            //Cooldowns, mana use etc
        }
        public static void UseSkill(string skillName)
        {
            var skill = PlayerData.Skills.FirstOrDefault(i => i.Name == skillName);
            if (skill != null)
            {
                UseSkill(skill);
            }
        }

        public static int[] GetModelHashes(object o)
        {
            int[] hashes;
            var models = o as uint[];
            var modelsA = o as JArray;

            if (models != null)
            {
                hashes = models.Select(m => (int)m).ToArray();
            }
            else
            {
                hashes = modelsA.Select(jv => (uint)jv).Select(t => (int)t).ToArray();
            }

            return hashes;
        }

        public static void OnRespawn()
        {
            SpawnCar();
            LoadPlayerWeapons();
            LoadVariations();
            Player.Health = 100;
            Player.MaxHealth = 100;
        }

        public static void CleanupObjects()
        {
            while (RPG.WorldData.AllObjects.Any())
            {
                RPG.WorldData.AllObjects.First().Destroy();
            }
        }

        public static void LoadVariations()
        {
            //Load variations
            foreach (var kvp in PlayerData.ModelVariations)
            {
                Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character, kvp.Key, kvp.Value, 0, 0);
            }
        }
    }
}