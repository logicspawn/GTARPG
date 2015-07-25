using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using LogicSpawn.GTARPG.Core.Scripts.Questing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LogicSpawn.GTARPG.Core
{
    public class PlayerData
    {
        public int Version = RPG.Version;
        public string Name;
        public int Exp;
        public int SkillExp;
        public int Level = 1;
        public string NumberPlate;
        [JsonConverter(typeof(StringEnumConverter))]
        public PedHash ModelHash;
        [JsonConverter(typeof(StringEnumConverter))]
        public VehicleHash CarHash;
        [JsonConverter(typeof(StringEnumConverter))]
        public VehicleColor CarColor;
        [JsonConverter(typeof(StringEnumConverter))]
        public VehicleColor CarSecondaryColor;
        [JsonConverter(typeof(StringEnumConverter))]
        public PlayerMotive Motive;
        public PlayerClass Class;
        public Gender Gender;
        public int Money;
        public int CompletedContracts;
        public string[] LastContracts;
        public Dictionary<int, int> ModelVariations;
        public List<Item> Inventory;
        public List<WeaponDefinition> Weapons;
        public SkillSlot[] SkillSlots;
        public List<Skill> Skills;
        public List<Quest> Quests;
        public Tutorial Tutorial;
        public Setup Setup;

        [JsonIgnore]
        public Vehicle CurrentVehicle;


        public PlayerData()
        {
            Name = "";
            NumberPlate = "";
            Inventory = new List<Item>();
            Weapons = new List<WeaponDefinition>();
            Skills = GM.Copy(SkillRepository.Skills);
            Quests = GM.Copy(QuestRepository.Quests);
            Money = 1000;
            CompletedContracts = 0;
            Motive = PlayerMotive.Rebel;
            Gender = Gender.Male;
            Class = PlayerClass.Time_Master;
            CarColor = VehicleColor.MetallicClassicGold;
            CarSecondaryColor = VehicleColor.MetallicClassicGold;
            SkillSlots = new[]
                        {
                            new SkillSlot {Key = Keys.T}, 
                            new SkillSlot {Key = Keys.Y}, 
                            new SkillSlot {Key = Keys.CapsLock}, 
                            new SkillSlot {Key = Keys.U}, 
                            new SkillSlot {Key = Keys.B}, 
                        };
            Setup = new Setup();
            Tutorial = new Tutorial();
            LastContracts = new[] { "","","", "", "" };
            ModelVariations = new Dictionary<int, int>();
        }

        [JsonIgnore]
        public int ExpToLevel
        {
            get
            {
                var x = ExpForLevelLogarithmic(Level);
                var y = ExpForLevelLogarithmic(Level - 1);
                var result = x - y;
                return result;
            }
        }

        public void AddItem(Item item)
        {
            var it = Inventory.FirstOrDefault(i => i.Name == item.Name);
            if(it != null)
            {
                it.Quantity += item.Quantity;
            }
            else
            {
                Inventory.Add(item);
            }
        }

        public void AddItem(string stringVal, int intVal)
        {
            var i = ItemRepository.Get(stringVal);
            i.Quantity = intVal;
            AddItem(i);
        }

        public void AddExp(int i)
        {
            Exp += i;
            SkillExp += Exp * 15;
            if (Exp >= ExpToLevel)
            {
                Exp -= ExpToLevel;
                Level++;
                RPG.GetPopup<LevelUp>().Show();
                RPG.UIHandler.View.CloseAllMenus();
            }

            RPG.SaveAllData();
        }

        private int ExpForLevelLogarithmic(int currentLevel)
        {
            var XpForFirstLevel = 100;
            var XpForLastLevel = 2000000000;
            var MaxLevel = 100;

            double B = Math.Log((double)XpForLastLevel / XpForFirstLevel) / (MaxLevel - 1);
            double A = (double)XpForFirstLevel / (Math.Exp(B) - 1.0);

            var x = (int)(A * Math.Exp((B) * currentLevel ));
            var y = Math.Pow(10, (int)(Math.Log(x) / Math.Log(10) - 2.2));
            return (int)((int)(x / y) * y);
        }


        public void AddMoney(int moneyValue)
        {
            Money += moneyValue;
        }

        public void StartQuest(string questName)
        {
            var quest = Quests.FirstOrDefault(q => q.Name == questName);
            if(quest != null)
            {
                quest.Start(false);
            }
            else
            {
                RPGLog.LogError("Failed to start quest. Quest not found: " + questName);
            }
        }

        public Skill GetSkill(string skillName)
        {
            return Skills.FirstOrDefault(s => s.Name == skillName);
        }
    }

    public class Setup
    {
        public int SafeArea = 10;
    }

    public class Tutorial
    {
        public bool PressJToOpenMenu;
        public bool BoughtAmmoFromShop;
        public bool GetAKill;
        public bool UnlockSkillWithSp;
        public bool UsingSkills;
        public bool SpawnVehicle;
        public bool TutorialDoneExceptSpeak;
        public bool SpokeToNpc;
    }

    public enum PlayerClass
    {
        Time_Master,
        Speedster,
        Berserker
    }

    public enum PlayerMotive
    {
        Rebel,
        Lawless
    }
}