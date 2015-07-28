using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using LogicSpawn.GTARPG.Core.Scripts.Questing;
using Newtonsoft.Json.Linq;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core
{
    public class GameHandler : UpdateScript
    {
        private WorldData WorldData;
        public List<NpcObject> NpcDatas
        {
            get { return RPG.WorldData.Npcs; }
        }

        private PlayerData PlayerData
        {
            get { return RPG.PlayerData; }
        }


        public List<int> KilledPeds = new List<int>();
        public List<int> KilledVecs = new List<int>();
        public bool InitiateNpcs;

        public GameHandler()
        {
            RPG.GameHandler = this;
            Interval = 0;
        }

        protected override void Start()
        {
            //Request texture dictionaries here
            //Then draw sprite , using OPENIV to see texture names
            //Hash.DRAW_SPRITE

            NpcDatas.AddRange(NpcRepository.Npcs);
            Init();
        }

        public void Init()
        {
            //Set damage
            Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, Game.Player, 0.5f);
            Function.Call(Hash.SET_AI_WEAPON_DAMAGE_MODIFIER, 0.1f);

            if(RPG.PlayerData.Exp == 0 && !RPG.PlayerData.Quests.Any(q => q.InProgress))
            {
                RPG.PlayerData.Quests.First(q => q.Name == "Welcome to GTA:RPG").Start();
            }
        }

        public override void Update()
        {
            WorldData = RPG.WorldData;

            Ped player = Game.Player.Character;


            if(RPG.ExplosiveHits)
            {
                Function.Call(Hash.SET_EXPLOSIVE_AMMO_THIS_FRAME,Game.Player.Handle);
                Function.Call(Hash.SET_EXPLOSIVE_MELEE_THIS_FRAME,Game.Player.Handle);
            }

            if(RPG.SuperJump)
            {
                Function.Call(Hash.SET_SUPER_JUMP_THIS_FRAME, Game.Player.Handle);
            }

            if(!InitiateNpcs)
            {
                NpcDatas.AddRange(NpcRepository.Npcs);

                foreach (var npc in NpcDatas)
                {
                    npc.Spawned = false;

                    var blip = World.CreateBlip(npc.Position);
                    blip.Sprite = npc.BlipSprite;
                    WorldData.Blips.Add(new BlipObject("Blip_" + npc.Name, blip));
                }
                InitiateNpcs = true;
            }

            if(!PlayerData.Tutorial.BoughtAmmoFromShop && PlayerData.Tutorial.PressJToOpenMenu)
            {
                var tut = RPG.GetPopup<TutorialBox>();
                var ammo = PlayerData.Inventory.FirstOrDefault(i => i.Name.Contains("Ammo"));
                if(ammo != null)
                {
                    PlayerData.Tutorial.BoughtAmmoFromShop = true;
                    EventHandler.Do(o =>
                    {
                        tut.Hide();
                        Wait(300);
                        if(!RPG.PlayerData.Tutorial.GetAKill)
                        {
                            tut.Pop("Getting kills, completing missions are just two ways to earn XP and Skill Points", "Get a kill.");
                        }
                    });
                }
            }

	        CheckNpcs();
            
            foreach(var l in RPG.WorldData.Loot)
            {
                if(l.Prop != null && l.Prop.Exists() && l.Item != null)
                {
                    if(RPG.UIHandler.ShowUI)
                    {
                        var dist = l.Prop.Position.DistanceTo(Game.Player.Character.Position);
                        if (dist < 60)
                        {
                            OutputArgument xArg = new OutputArgument();
                            OutputArgument yArg = new OutputArgument();
                            var pos = l.Prop.Position;
                            var dimensions = l.Prop.Model.GetDimensions();
                            pos.Y -= dimensions.Y / 2;
                            pos.Z += 0.4f;
                            Function.Call(Hash._WORLD3D_TO_SCREEN2D, pos.X, pos.Y, pos.Z, xArg, yArg);
                            var x = xArg.GetResult<float>();
                            var y = yArg.GetResult<float>();

                            var itemName = l.Item.Type == ItemType.Money
                                ? "gta$" + l.Item.MoneyValue
                                : l.Item.Quantity + "x " + l.Item.Name;

                            new UIRectangle(new Point((int)(UI.WIDTH * x) - 50, (int)(UI.HEIGHT * y) + 12), new Size(100, 2), l.Item.GetRarityColor()).Draw();
                            new UIText(itemName, new Point((int)(UI.WIDTH * x), (int)(UI.HEIGHT * y)), 0.21f, Color.White, 0, true).Draw();
                        }
                    }

                    l.Prop.ApplyForce(new Vector3(0,0,-0.1f));
                }
            }

            var nearbyForVecs = World.GetAllVehicles();
            foreach (var vec in nearbyForVecs.Where(v => v.EngineHealth <= 0))
            {

                if (RPG.PlayerData.CurrentVehicle != null && vec.Handle == RPG.PlayerData.CurrentVehicle.Handle) continue;

                if (!KilledVecs.Contains(vec.Handle))
                {
                    KilledVecs.Add(vec.Handle);


                    var rng = Random.Range(0, 100 + 1);
                    if(rng < 70)
                    {
                        var m = new Model(GM.GetHashKey("prop_box_guncase_03a"));
                        m.Request(1000);
                        var p = World.CreateProp(m, vec.Position + new Vector3(0, 0, 1.0f) + Vector3.RandomXY(), vec.Rotation, true, false);
                        p.ApplyForce(new Vector3(0, 0, -0.05f));
                        RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get("Vehicle Parts"), p));
                    }
                    else if (rng < 80)
                    {
                        var m = new Model(GM.GetHashKey("prop_box_guncase_03a"));
                        m.Request(1000);
                        var p = World.CreateProp(m, vec.Position + new Vector3(0, 0, 1.0f) + Vector3.RandomXY(), vec.Rotation, true, false);
                        p.ApplyForce(new Vector3(0, 0, -0.05f));
                        RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get("Vehicle Repair Kit"), p));
                    }
                    

                    if (!vec.HasBeenDamagedBy(Game.Player.Character)) continue;

                    if (vec.Model.IsHelicopter)
                    {
                        RPG.Notify(Notification.Kill("destroyed helicopter: +100 Exp"));
                        PlayerData.AddExp(100);
                    }
                    else if (vec.Model.IsPlane)
                    {
                        RPG.Notify(Notification.Kill("destroyed aircraft: +150 Exp"));
                        PlayerData.AddExp(150);
                    }
                    else if (vec.Model.IsBoat)
                    {
                        RPG.Notify(Notification.Kill("Destroyed Boat: +200 Exp"));
                        PlayerData.AddExp(200);
                    }
                    else if (vec.Model.IsBike)
                    {
                        RPG.Notify(Notification.Kill("Destroyed Bike: +20 Exp"));
                        PlayerData.AddExp(20);
                    }
                    else if (vec.Model.IsTrain)
                    {
                        RPG.Notify(Notification.Kill("Destroyed Train: +500 Exp"));
                        PlayerData.AddExp(500);
                    }
                    else
                    {
                        RPG.Notify(Notification.Kill("Destroyed Vehicle: +15 Exp"));
                        PlayerData.AddExp(15);
                    }

                    CheckIfForQuest(vec, vec.Model.Hash, false);
                }
            }

            var nearbyForKills = World.GetAllPeds();
            //var nearbyForKills = World.GetAllPeds();
            foreach (var ped in nearbyForKills.Where(p => !p.IsAlive))
            {
                if (ped.CurrentBlip != null)
                    ped.CurrentBlip.Remove();

                var n = RPG.WorldData.Npcs.FirstOrDefault(x => x.Ped != null && x.Ped.Handle == ped.Handle);
                if (n != null)
                {
                    RPG.WorldData.Npcs.Remove(n);
                }

                if (!KilledPeds.Contains(ped.Handle))
                {
                    KilledPeds.Add(ped.Handle);
                    //Check if kill needed for quest
                    CheckIfForQuest(ped,ped.Model.Hash, true);
                    //If player didn't damage, continue
                    if (!ped.HasBeenDamagedBy(Game.Player.Character)) continue;


                    if (!PlayerData.Tutorial.GetAKill && PlayerData.Tutorial.PressJToOpenMenu && PlayerData.Tutorial.BoughtAmmoFromShop)
                    {
                        var tut = RPG.GetPopup<TutorialBox>();
                        PlayerData.Tutorial.GetAKill = true;
                        PlayerData.SkillExp += 100;
                        EventHandler.Do(o =>
                        {
                            tut.Hide();
                            Wait(300);
                            if (!RPG.PlayerData.Tutorial.UnlockSkillWithSp)
                            {
                                tut.Pop("Hope you haven't attracted the cops. If so lose them. Time to unlock some skills.", "Access the menu > Character Menu > Skills. Unlock your first skill.");
                            }
                        });
                    }

                    RPG.Notify(Notification.Kill("target eliminated: +5 Exp"));

                    //Basic loot rng
                    var rng = Random.Range(0, 100 + 1);

                    if(rng < 70)
                    {
                        var m = new Model(GM.GetHashKey("prop_cash_pile_01"));
                        m.Request(1000);
                        var p = World.CreateProp(m, ped.Position + new Vector3(0, 0, 2.2F) + Vector3.RandomXYZ(), ped.Rotation, true, false);
                        p.ApplyForce(new Vector3(0, 0, 0.05f));
                        RPG.WorldData.AddLoot(new LootItem(ItemRepository.Cash(Random.Range(20, 80)), p));
                    }

                    if(rng < 30)
                    {
                        var items2 = new[] { "Bandages", "Basic Scraps", "Simple Protective Gear","Ammo Pack I" };

                        var m2 = new Model(GM.GetHashKey("prop_money_bag_01"));
                        m2.Request(1000);
                        var p2 = World.CreateProp(m2, ped.Position + new Vector3(0, 0.5f, 2.2F) + Vector3.RandomXY(), ped.Rotation, true, false);
                        p2.ApplyForce(new Vector3(0, 0, 0.05f));
                        RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get(items2[Random.Range(0, items2.Length)]), p2));

                    }
                    else if(rng < 40)
                    {
                        var items2 = new[] { "Health Kit" , "Refurbished Kevlar","Ammo Pack II" };

                        var m2 = new Model(GM.GetHashKey("prop_money_bag_01"));
                        m2.Request(1000);
                        var p2 = World.CreateProp(m2, ped.Position + new Vector3(0.5f, 0, 2.2F) + Vector3.RandomXY(), ped.Rotation, true, false);
                        p2.ApplyForce(new Vector3(0, 0, 0.05f));
                        RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get(items2[Random.Range(0, items2.Length)]), p2));

                    }

                    PlayerData.AddExp(5);
                }
            }
            
            var activeQuests = PlayerData.Quests.Where(q => q.InProgress).ToList();
            for (int i = 0; i < activeQuests.Count; i++)
            {
                activeQuests[i].CheckState();
            }

            
            QuestUpdates();
        }

        private void QuestUpdates()
        {
            var questsInProgress = PlayerData.Quests.Where(q => q.InProgress).ToList();
            var qNum = 1;
            foreach (var q in questsInProgress)
            {
                foreach(var b in q.BlipObjects)
                {
                    if(qNum < 10)
                        b.Blip.ShowNumber(qNum);
                }

                var conditionsToFix = q.Conditions.Where(c =>
                                                        !c.Done &&
                                                         (c.Type == ConditionType.Kill || c.Type == ConditionType.Loot) &&
                                                         c.Parameters.ContainsKey("ModelHash"));

                foreach(var c in conditionsToFix)
                {
                    var quest = q;

                    var npcs = WorldData.Npcs.Where(n => n.Name == "Quest_" + quest.Name).ToList();

                    for (int i = 0; i < npcs.Count; i++)
                    {
                        var n = npcs[i];
                        if (n.Ped.Exists() && !n.Ped.IsAlive)
                        {
                            n.Destroy();
                        }
                    }

                    //If no npcs found, or npcs are too high
                    if (WorldData.Npcs.All(n => n.Name != "Quest_" + quest.Name) ||
                        WorldData.Npcs.Where(n => n.Name == "Quest_" + quest.Name).All(w => w.Ped.Position.Z > Game.Player.Character.Position.Z + 45) ||
                        WorldData.Npcs.Where(n => n.Name == "Quest_" + quest.Name).All(w => w.Ped.Position.Z < Game.Player.Character.Position.Z - 45))
                    {
                        q.ClearObjectsAndBlips();
                        q.SetupCondition(c, false);
                    }

                }

                qNum++;
            }
        }

        private void CheckIfForQuest(Entity entity, int Hash, bool isPed)
        {
            var hashKey = isPed ? "ModelHash" : "VehHash";
            var activeQuests = PlayerData.Quests.Where(q => q.InProgress).ToList();
            for (int i = 0; i < activeQuests.Count; i++)
            {
                var quest = activeQuests[i];
                foreach (var c in quest.Conditions)
                {
                    if(c.Type == ConditionType.Kill || c.Type == ConditionType.DestroyVehicle ||
                        c.Type == ConditionType.Loot)
                    {
                        if (!c.Parameters.ContainsKey(hashKey))
                        {
                            if((c.Type == ConditionType.Kill && isPed) || (c.Type == ConditionType.DestroyVehicle && !isPed))
                            {

                                var cur = Convert.ToInt32(c.Parameters["Current"]);
                                c.Parameters["Current"] = cur + 1;
                            }
                            else if(c.Type == ConditionType.Loot)
                            {
                                if (c.Parameters.ContainsKey("Vehicles"))
                                {
                                    if(!isPed)
                                    {
                                        DropQuestItem(entity, (string)c.Parameters["PropModel"], (string)c.Parameters["ItemName"], Convert.ToInt32(c.Parameters["DropRate"]));
                                    }
                                }
                                else
                                {
                                    if(isPed)
                                    {
                                        DropQuestItem(entity, (string)c.Parameters["PropModel"], (string)c.Parameters["ItemName"], Convert.ToInt32(c.Parameters["DropRate"]));
                                    }
                                }
                            }
                            continue;
                        }

                        int[] hashes = RPGMethods.GetModelHashes(c.Parameters[hashKey]);
                        


                        var p = Hash;

                        foreach (var m in hashes)
                        {
                            var x = m;
                            if (p == x)
                            {
                                if(c.Type != ConditionType.Loot)
                                {
                                    var cur = Convert.ToInt32(c.Parameters["Current"]);
                                    c.Parameters["Current"] = cur + 1;
                                }
                                else
                                {
                                    DropQuestItem(entity,(string) c.Parameters["PropModel"], (string) c.Parameters["ItemName"], Convert.ToInt32(c.Parameters["DropRate"]));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DropQuestItem(Entity entity, string propModel, string itemName, int dropRateOutOf100)
        {
            var rng = Random.Range(0, 100 + 1);
            if (rng > dropRateOutOf100) return;

            var item = ItemRepository.Get(itemName);

            var m = new Model(GM.GetHashKey(propModel));
            m.Request(1000);

            var p = World.CreateProp(m, entity.Position + new Vector3(0, 0, 2.2F) + Vector3.RandomXY(), entity.Rotation, true, false);
            p.ApplyForce(new Vector3(0, 0, 0.05f));
            RPG.WorldData.AddLoot(new LootItem(item, p));
        }

        private void CheckNpcs()
        {
            foreach(var npc in NpcDatas)
            {
                var dist = npc.Position.DistanceTo(Game.Player.Character.Position);

                if (dist < 20)
                {
                    var pos = npc.Ped != null ? npc.Ped.Position : npc.Position;
                    pos.Z += 1.0f;
                    OutputArgument xArg = new OutputArgument();
                    OutputArgument yArg = new OutputArgument();
                    Function.Call(Hash._WORLD3D_TO_SCREEN2D, pos.X, pos.Y, pos.Z, xArg, yArg);
                    var x = xArg.GetResult<float>();
                    var y = yArg.GetResult<float>();

                    new UIRectangle(new Point((int)(UI.WIDTH * x) - 50, (int)(UI.HEIGHT * y) + 12), new Size(100, 2), Color.DodgerBlue).Draw();
                    new UIText(npc.Name, new Point((int)(UI.WIDTH * x), (int)(UI.HEIGHT * y)), 0.21f, Color.White, 0, true).Draw();
                }

                if(npc.IsQuestNpc && !npc.Spawned)
                {
                    //RPGLog.Log("Found unspawned NPC");
                    if (dist < 100)
                    {
                        RPGLog.Log("Spawning NPC");
                        var model = new Model(npc.ModelName);
                        model.Request(1000);
                        var ped = World.CreatePed(model, npc.Position, npc.Heading);

                        try
                        {
                            ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
                            ped.IsInvincible = true;
                            EventHandler.Do(o =>
                                                {
                                                    EventHandler.Wait(1000);
                                                    ped.FreezePosition = true;
                                                });
                            Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, ped.Handle);
                            Function.Call(Hash.SET_PED_CAN_BE_TARGETTED, ped.Handle, false);
                            Function.Call(Hash.SET_PED_CAN_BE_TARGETTED_BY_PLAYER, ped.Handle, false);
                        }
                        catch
                        {
                            RPGLog.Log("Error setting npc and player friendliness.");
                        }
                        npc.SetPed(ped);
                        npc.Spawned = true;
                    }
                }
            }
        }

        protected override void Dispose(bool A_0)
        {
            if (!A_0) return;

            if(!RPG.GameLoaded)
            {
                RPGLog.Log("Game ended.");
                return;
            }

            RPG.Audio.DisposeAll();
            Function.Call(Hash.DESTROY_MOBILE_PHONE);    
            Function.Call(Hash.CREATE_MOBILE_PHONE,0);    

            int objDestroyed = 0;
            var count = RPG.WorldData.AllObjects.Count;
            while(RPG.WorldData.AllObjects.Any())
            {
                RPG.WorldData.AllObjects.First().Destroy();
                objDestroyed++;
            }
            
            RPGLog.Log("Cleaned up " + objDestroyed + "/" + count + " objects");
            RPGLog.Log("Thread ended.");
            RPGLog.Log("");

            base.Dispose(true);
       
        }
    }
}
