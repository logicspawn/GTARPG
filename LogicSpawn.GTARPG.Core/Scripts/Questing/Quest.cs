using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogicSpawn.GTARPG.Core.Scripts.Questing
{
    public class Quest
    {
        public string Name;
        public string Description;
        public bool Done;
        public bool IsContract;
        public bool IsRepeatable;
        public bool AutoComplete;
        public bool Cancellable;

        public QuestMode QuestMode;
        public List<QuestCondition> Conditions;
        public List<QuestReward> AdditionalRewards;

        [JsonIgnore]
        public Action<Quest> OnComplete;

        [JsonIgnore]
        public Action<Quest> OnStart;

        //Progress
        public bool InProgress;
        public int CurrentCondition = 0;


        //Modifiers
        public bool SpawnTargets;
        public int AmountToSpawn;
        [JsonIgnore] public bool HasSpawnedTargets;

        public int ExpReward;
        public int MoneyReward;

        public bool ConditionsComplete
        {
            get { return Conditions.All(c => c.Done); }
        }

        
        public bool CreateHandInBlip = false;
        public Vector3 HandInBlipPosition = Vector3.Zero;

        [JsonIgnore] 
        public List<BlipObject> BlipObjects;


        public Quest()
        {
        }

        public Quest(string name, string desc,bool isRepeatable, bool isContract, int expReward, int moneyReward, QuestMode questMode = QuestMode.AllAtOnce)
        {
            Name = name;
            Description = desc;
            ExpReward = expReward;
            MoneyReward = moneyReward;
            IsContract = isContract;
            IsRepeatable = isRepeatable;
            Cancellable = true;
            Conditions = new List<QuestCondition>();
            AdditionalRewards = new List<QuestReward>();
            BlipObjects = new List<BlipObject>();
            QuestMode = questMode;
            OnComplete = q => { };
            OnStart = q => { };
            AmountToSpawn = -1;
            CreateHandInBlip = false;
            HandInBlipPosition = Vector3.Zero;
            HasSpawnedTargets = false;
        }

        public void CheckState()
        {
            foreach (var c in Conditions)
            {
                c.SetDone();
            }

            if ((AutoComplete || IsContract) && ConditionsComplete)
            {
                Complete();
            }
        }

        public void Start(bool withPopup = true)
        {
            SetupConditions();

            QuestRepository.GetStartAction(Name).Invoke(this);

            if(withPopup)
                RPG.GetPopup<QuestStarted>().Show(this);

            InProgress = true;
        }

        private void SetupConditions()
        {
            BlipObjects = new List<BlipObject>();

            if(CreateHandInBlip)
            {
                BlipObjects.Add(RPGBlips.QuestHandIn(Name, HandInBlipPosition));
            }

            foreach (var c in Conditions)
            {
                SetupCondition(c, true);
            }
        }

        public void SetupCondition(QuestCondition c, bool firstSetup)
        {
            if (c.Type == ConditionType.Kill && c.Parameters.ContainsKey("ModelHash")
                    || c.Type == ConditionType.Loot && c.Parameters.ContainsKey("ModelHash"))
            {
                int[] hashes;
                var models = c.Parameters["ModelHash"] as uint[];
                var modelsA = c.Parameters["ModelHash"] as JArray;

                if (models != null)
                {
                    hashes = models.Select(m => (int)m).ToArray();
                }
                else
                {
                    hashes = modelsA.Select(jv => (uint)jv).Select(t => (int)t).ToArray();
                }


                if (SpawnTargets)
                {
                    c.Position = c.Position == Vector3.Zero ? RPGMethods.GetSpawnPoint(350) : c.Position;
                    BlipObjects.Add(RPGBlips.QuestArea(Name, c.Position));

                    EventHandler.Do(q =>
                    {
                      var pos = c.Position;
                      var amountToSpawn = AmountToSpawn;
                      var playerGroup = Game.Player.Character.RelationshipGroup;
                      var enemies = World.AddRelationshipGroup("RPG_Enemies");
                      World.SetRelationshipBetweenGroups(Relationship.Neutral, playerGroup, enemies);
                      for (int i = 0; i < amountToSpawn; i++)
                      {
                          var hash = hashes[Random.Range(0, hashes.Length)];
                          var m = new Model(hash);
                          m.Request(1000);
                          var ped = World.CreatePed(m, pos + Vector3.RandomXY() * 2, Game.Player.Character.Heading + 180);
                          ped.RelationshipGroup = enemies;
                          var b = ped.AddBlip();
                          EventHandler.Wait(100);
                          b.IsFriendly = false;
                          b.Scale = 0.6f;
                          ped.Task.WanderAround();
                          //todo: apply some weapon damage scaling here lol
                          ped.Weapons.Give(WeaponHash.Pistol, 1000, false, true);
                          ped.CanSwitchWeapons = true;
                          RPG.WorldData.AddPed(new NpcObject("Quest_" + Name, ped));
                          RPG.WorldData.AddBlip(new BlipObject("Quest_" + Name, b));
                          HasSpawnedTargets = true;
                      }
                    });

                }
            }

            if (!c.Parameters.ContainsKey("Current"))
            {
                c.Parameters.Add("Current", 0);
            }

            if(firstSetup)
            {
                switch (c.Type)
                {
                    case ConditionType.Kill:
                    case ConditionType.Loot:
                    case ConditionType.DestroyVehicle:
                    case ConditionType.Acquire:
                        c.Parameters["Current"] = 0;
                        break;
                    case ConditionType.Interact:
                    case ConditionType.Escort:
                    case ConditionType.Race:
                    case ConditionType.EvadeCops:
                        c.Parameters["Current"] = false;
                        break;
                    case ConditionType.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
        }

        public void Complete()
        {
          if(IsContract)
          {
              RPG.PlayerData.CompletedContracts++;
          }
            RPG.PlayerData.AddExp(ExpReward);
            RPG.PlayerData.AddMoney(MoneyReward);

            foreach(var r in AdditionalRewards)
            {
                switch (r.RewardType)
                {
                    case RewardType.Exp:
                        RPG.PlayerData.AddExp(r.IntVal);
                        break;
                    case RewardType.Money:
                        RPG.PlayerData.AddMoney(r.IntVal);
                        break;
                    case RewardType.UnlockSkill:
                        RPG.PlayerData.Skills.First(s => s.Name == r.StringVal).Unlocked = true;
                        break;
                    case RewardType.Item:
                        RPG.PlayerData.AddItem(r.StringVal, r.IntVal);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            QuestRepository.GetCompleteAction(Name).Invoke(this);

            //Cleanup
            CleanupQuestOnDone();

            RPG.GetPopup<QuestComplete>().Show(this);
            RPG.UIHandler.View.CloseAllMenus();

            Done = true;
            InProgress = false;

            if(IsRepeatable) Reset();
        }

        private void CleanupQuestOnDone()
        {
            ClearObjectsAndBlips();

            foreach (var c in Conditions)
            {
                if (c.Type == ConditionType.Loot)
                {
                    var itemNeeded = (string)c.Parameters["ItemName"];
                    var amountNeeded = Convert.ToInt32(c.Parameters["Amount"]);
                    var item = RPG.PlayerData.Inventory.First(i => i.Name == itemNeeded);
                    if (item.Type == ItemType.QuestItem)
                    {
                        RPG.PlayerData.Inventory.Remove(item);
                    }
                    else
                    {
                        item.Quantity -= amountNeeded;
                        if (item.Quantity <= 0)
                        {
                            RPG.PlayerData.Inventory.Remove(item);
                        }
                    }
                    
                }
            }
        }

        private void CleanupQuestOnReset()
        {
            ClearObjectsAndBlips();

            foreach (var c in Conditions)
            {
                if (c.Type == ConditionType.Loot)
                {
                    var itemNeeded = (string)c.Parameters["ItemName"];
                    var item = RPG.PlayerData.Inventory.FirstOrDefault(i => i.Name == itemNeeded);
                    if (item != null)
                    {
                        if (item.Type == ItemType.QuestItem)
                        {
                            RPG.PlayerData.Inventory.Remove(item);
                        }
                    }
                }
            }
        }

        public void ClearObjectsAndBlips()
        {
            foreach (var b in BlipObjects)
            {
                b.Destroy();
            }
            BlipObjects = new List<BlipObject>();
            foreach (var c in Conditions)
            {
                if (c.Type == ConditionType.Loot)
                {
                    var excessNpcs = RPG.WorldData.Npcs.Where(n => n.Name == "Quest_" + Name).ToList();
                    for (int i = 0; i < excessNpcs.Count; i++)
                    {
                        var n = excessNpcs[i];
                        n.Destroy();
                    }

                    var itemNeeded = (string)c.Parameters["ItemName"];
                    var itemsInWorldData = RPG.WorldData.Loot.Where(l => l.Name == itemNeeded).ToList();
                    for (int i = 0; i < itemsInWorldData.Count; i++)
                    {
                        var n = itemsInWorldData[i];
                        n.Destroy();
                    }
                }

                if (c.Type == ConditionType.Kill)
                {
                    var excessNpcs = RPG.WorldData.Npcs.Where(n => n.Name == "Quest_" + Name).ToList();
                    var excessNpcBlips = RPG.WorldData.Blips.Where(n => n.Name == "Quest_" + Name).ToList();
                    for (int i = 0; i < excessNpcs.Count; i++)
                    {
                        var n = excessNpcs[i];
                        n.Destroy();
                    }
                    for (int i = 0; i < excessNpcBlips.Count; i++)
                    {
                        var n = excessNpcBlips[i];
                        n.Destroy();
                    }
                }
            }
        }

        public void OnReload()
        {
            SetupConditions();
        }

        public void Reset()
        {
            CleanupQuestOnReset();
            CurrentCondition = 0;
            Done = false;
            InProgress = false;
        }

        public Quest AddCondiiton (QuestCondition condition)
        {
            condition.QuestName = Name;
            Conditions.Add(condition);
            return this;
        }
        
        public Quest AddReward(QuestReward reward)
        {
            AdditionalRewards.Add(reward);
            return this;
        }

        public Quest AddReward(params QuestReward[] rewards)
        {
            AdditionalRewards.AddRange(rewards);
            return this;
        }

        public Quest WithOnComplete(Action<Quest> onComplete)
        {
            OnComplete = onComplete;
            return this;
        }

        public Quest WithOnStart(Action<Quest> onStart)
        {
            OnStart = onStart;
            return this;
        }

        public string GetProgressString()
        {
            var str = new List<string>();
            foreach(var c in Conditions)
            {
                switch (c.Type)
                {
                    case ConditionType.Kill:
                    case ConditionType.DestroyVehicle:
                    case ConditionType.Loot:
                    case ConditionType.Acquire:
                        var amt = Convert.ToInt32(c.Parameters["Amount"]);
                        str.Add(c.ProgressPrefix + ": " + Math.Min(amt, Convert.ToInt32(c.Parameters["Current"])) + " / " + amt + " ");
                        break;
                    case ConditionType.Interact:
                        break;
                    case ConditionType.Escort:
                        break;
                    case ConditionType.Race:
                        break;
                    case ConditionType.EvadeCops:
                        break;
                    case ConditionType.Custom:
                        str.Add(c.ProgressPrefix + ": " + (c.Done ? "Done" : "Not Done") + " ");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var progressString = str.Any() ? String.Join("\n", str) : (Done ? "Completed " : "Incomplete " );

            return progressString;
        }

        public string GetBasicRewardString()
        {
            return ExpReward + " Exp" + " -  GTA$" + MoneyReward.ToString("N0");
        }

        public Quest WithSpawnedTargets()
        {
            SpawnTargets = true;
            return this;
        }
        public Quest WithSpawnedTargets(int amount)
        {
            SpawnTargets = true;
            AmountToSpawn = amount;
            return this;
        }

        public Quest WithAutoComplete()
        {
            AutoComplete = true;
            return this;
        }

        public Quest WithCannotBeCancelled()
        {
            Cancellable = false;
            return this;
        }

        public Quest AddFinishBlip(Vector3 position)
        {
            CreateHandInBlip = true;
            HandInBlipPosition = position;
            return this;
        }
    }

    public class QuestReward
    {
        public RewardType RewardType;
        public string StringVal;
        public int IntVal;

        public QuestReward() { }

        public static QuestReward Item(string itemName, int amount)
        {
            var r = new QuestReward {RewardType = RewardType.Item, StringVal = itemName, IntVal = amount};
            return r;
        }
        public static QuestReward Skill(string skillName)
        {
            var r = new QuestReward {RewardType = RewardType.UnlockSkill, StringVal = skillName};
            return r;
        }

        public string GetRewardString()
        {
            switch (RewardType)
            {
                case RewardType.Exp:
                    return IntVal + " bonus exp";
                case RewardType.Money:
                    return IntVal + " bonus gold";
                case RewardType.UnlockSkill:
                    return "unlocked [" + StringVal + "]";
                case RewardType.Item:
                    return IntVal + "x " + StringVal;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum RewardType
    {
        Exp,
        Money,
        UnlockSkill,
        Item
    }

    public enum QuestMode
    {
        AllAtOnce,
        Linear
    }

    public enum ConditionType
    {
        Kill,
        Acquire,
        Loot,
        Interact,
        Escort,
        Race,
        EvadeCops,
        Custom,
        DestroyVehicle
    }

    public class QuestCondition
    {
        public string Name;
        public ConditionType Type;
        public bool Done;
        public Dictionary<string, object> Parameters;
        public string ProgressPrefix;

        public string QuestName;
        public string ID;

        public Vector3 Position;
        [JsonIgnore]
        public Func<bool> CustomCondition;

        public QuestCondition()
        {
            ID = Guid.NewGuid().ToString();
            Parameters = new Dictionary<string, object>();
            ProgressPrefix = "Progress:";
        }


        public void SetDone()
        {
            if (Type == ConditionType.Loot || Type == ConditionType.Acquire)
            {
                var itemNeeded = (string)Parameters["ItemName"];
                var amountNeeded = (int)(long)Parameters["Amount"];
                var item = RPG.PlayerData.Inventory.FirstOrDefault(i => i.Name == itemNeeded);
                if(!Parameters.ContainsKey("Current")) Parameters.Add("Current",0);
                Parameters["Current"] = item != null ? Math.Min(amountNeeded, item.Quantity) : 0;
                Done = item != null && item.Quantity >= amountNeeded;
            }
            else
            {
                switch (Type)
                {
                    case ConditionType.Kill:
                    case ConditionType.DestroyVehicle:
                        var currentAmount = Convert.ToInt32(Parameters["Current"]);
                        var amountNeeded = Convert.ToInt32(Parameters["Amount"]);
                        Done = currentAmount >= amountNeeded;
                        break;
                    case ConditionType.Interact:
                        break;
                    case ConditionType.Escort:
                        break;
                    case ConditionType.Race:
                        break;
                    case ConditionType.EvadeCops:
                        break;
                    case ConditionType.Custom:
                        Done = QuestRepository.GetCustomCondition(QuestName, Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static QuestCondition Custom(string progressPrefix, string name, Func<bool> condition)
        {
            var c = new QuestCondition {Type = ConditionType.Custom};
            c.Name = name;
            c.ProgressPrefix = progressPrefix;
            c.CustomCondition = condition;
            return c;
        }
        public static QuestCondition Kill(string progressPrefix, int amount, params PedHash[] modelHashes)
        {
            var c = new QuestCondition {Type = ConditionType.Kill};

            var models = modelHashes;
            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("ModelHash", models);
            c.Parameters.Add("Amount",amount);
            return c;
        }
        public static QuestCondition KillAny(string progressPrefix, int amount)
        {
            var c = new QuestCondition {Type = ConditionType.Kill};

            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("Amount",amount);
            return c;
        }

        public static QuestCondition Kill(string progressPrefix, int amount, Vector3 position, params PedHash[] modelHashes)
        {
            var c = new QuestCondition {Type = ConditionType.Kill};

            var models = modelHashes;
            c.Position = position;
            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("ModelHash", models);
            c.Parameters.Add("Amount",amount);
            return c;
        }
        public static QuestCondition KillAny(string progressPrefix, int amount, Vector3 position)
        {
            var c = new QuestCondition {Type = ConditionType.Kill};
            c.Position = position;
            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("Amount",amount);
            return c;
        }

        public static QuestCondition DestroyVehicle(string progressPrefix, int amount,params VehicleHash[] vehicleHashes)
        {
            var c = new QuestCondition { Type = ConditionType.DestroyVehicle };

            var models = vehicleHashes;
            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("VehHash", models);
            c.Parameters.Add("Amount", amount);
            return c;
        }
        public static QuestCondition DestroyAnyVehicle(string progressPrefix, int amount)
        {
            var c = new QuestCondition { Type = ConditionType.DestroyVehicle };

            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("Amount", amount);
            return c;
        }


        public static QuestCondition Loot(string progressPrefix, string itemName, string propModel, int dropRateOutOf100, int amountNeeded, params PedHash[] modelHashes)
        {
            var c = new QuestCondition {Type = ConditionType.Loot};
            c.ProgressPrefix = progressPrefix;
            var models = modelHashes;
            c.Parameters.Add("ModelHash", models);
            c.Parameters.Add("ItemName",itemName);
            c.Parameters.Add("Amount",amountNeeded);
            c.Parameters.Add("DropRate", dropRateOutOf100);
            c.Parameters.Add("PropModel", propModel);
            return c;
        }
        public static QuestCondition LootVehicles(string progressPrefix, string itemName, string propModel, int dropRateOutOf100, int amountNeeded, params VehicleHash[] vehicleHashes)
        {
            var c = new QuestCondition {Type = ConditionType.Loot};
            var models = vehicleHashes;

            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("VehHash", models);
            c.Parameters.Add("ItemName",itemName);
            c.Parameters.Add("Amount",amountNeeded);
            c.Parameters.Add("DropRate", dropRateOutOf100);
            c.Parameters.Add("PropModel", propModel);
            c.Parameters.Add("Vehicles", true);
            return c;
        }
        public static QuestCondition LootAnyPed(string progressPrefix, string itemName, string propModel, int dropRateOutOf100, int amountNeeded)
        {
            var c = new QuestCondition {Type = ConditionType.Loot};
            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("ItemName",itemName);
            c.Parameters.Add("Amount",amountNeeded);
            c.Parameters.Add("DropRate", dropRateOutOf100);
            c.Parameters.Add("PropModel", propModel);
            return c;
        }
        public static QuestCondition LootAnyVehicle(string progressPrefix, string itemName, string propModel, int dropRateOutOf100, int amountNeeded)
        {
            var c = new QuestCondition {Type = ConditionType.Loot};

            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("ItemName",itemName);
            c.Parameters.Add("Amount",amountNeeded);
            c.Parameters.Add("DropRate", dropRateOutOf100);
            c.Parameters.Add("PropModel", propModel);
            c.Parameters.Add("Vehicles", true);
            return c;
        }

        public static QuestCondition Acquire(string progressPrefix, string itemName, int amountNeeded)
        {
            var c = new QuestCondition { Type = ConditionType.Acquire };
            c.ProgressPrefix = progressPrefix;
            c.Parameters.Add("ItemName", itemName);
            c.Parameters.Add("Amount", amountNeeded);
            return c;
        }
    }
}