using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core.Repository
{
    public static class SkillRepository
    {
        public static List<Skill> Skills;

        static SkillRepository()
        {
            Skills = new List<Skill>();

            //Get High
            Skills.Add(new Skill("Get High", "Time seems to move slower...What's the worst that could happen?", 1000)
                        .WithAction(GetHigh)
                        .WithModTree(GetHighTree())
                        .WithParam("Slow Time", 5000, o => (o.GetFloatParam("Slow Time") / 1000).ToString("0.0") + "s")
                        .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s")
                        .WithMods(Mod("Slow I",50),Mod("Slow II",100))
                        .WithCooldown(30000));
            //.WithStartingUnlocked()

            //Toughen Up
            Skills.Add(new Skill("Toughen Up", "Hard times call for tough measures. Suit up with some armor.", 100) { CoolDownMs = 30000 }
                .WithAction(o =>
                {
                    Game.Player.Character.Armor += o.GetIntParam("Armor Amount");
                })
                .WithModTree(ToughenUpTree())
                .WithParam("Armor Amount", 10, o => ("+" + o.GetIntParam("Armor Amount") + " Armor"))
                .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s")
                .WithMods(Mod("Increased Armor", 100), Mod("Swift Change", 200))
            );

            //Blazed Off Glory
            Skills.Add(new Skill("Blazed Off Glory", "This narcotic makes people go wild. Luckily in a good way.", 100) { CoolDownMs = 30000 }
                        .WithAction(BlazedOffGlory)
                        .WithModTree(BlazedOffGloryTree())
                        .WithParam("Blaze Time", 15000, o => (o.GetFloatParam("Blaze Time") / 1000).ToString("0.0") + "s")
                        .WithParam("Damage Buff", 0.5f, o => ( "+" + (o.GetFloatParam("Damage Buff") * 100).ToString("0.0") + "% damage"))
                        .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s")
                        .WithMods(Mod("Two Puffs", 100), Mod("Lethal Dose", 200))
                        .WithCooldown(120000));

            //Reject Nonsense
            Skills.Add(new Skill("Reject Nonsense", "Nearby enemies fly into the air.", 100) { CoolDownMs = 30000 }
                .WithAction(o =>
                                {
                                    var radius = o.GetFloatParam("Radius");
                                    var knockupHeight = o.GetFloatParam("Knockup Height");
                                    var nearbyPeds = World.GetNearbyPeds(Game.Player.Character, radius);
                                    foreach(var ped in nearbyPeds)
                                    {
                                        if (ped.RelationshipGroup != Game.Player.Character.RelationshipGroup)
                                            ped.ApplyForce(new Vector3(0, 0, knockupHeight));
                                    }
                                })
                .WithModTree(RejectNonsenseTree())
                .WithParam("Knockup Height", 15, o => (o.GetFloatParam("Knockup Height")) + "m")
                .WithParam("Radius", 5, o => (o.GetFloatParam("Radius")) + "m")
                .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s")
                .WithMods(Mod("Reach For The Stars", 100), Mod("Knocked Up", 200))
            );

            //Rampage
            Skills.Add(new Skill("Rampage", "Explosive ammo, melee and super jump. Go wild.", 100) { CoolDownMs = 30000 }
                .WithAction(o =>
                {
                    SkillEventHandler.Do(x =>
                        {
                            var ramgpageTime = o.GetIntParam("Rampage Time");
                            RPG.ExplosiveHits = true;
                            RPG.SuperJump = true;
                            SkillEventHandler.Wait(ramgpageTime);
                            RPG.ExplosiveHits = false;
                            RPG.SuperJump = false;
                        });
                })
                .WithModTree(RampageTree())
                .WithParam("Rampage Time", 5000, o => (o.GetFloatParam("Rampage Time") / 1000).ToString("0.0") + "s")
                .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s")
                .WithMods(Mod("Rampage I", 100), Mod("Rampage II", 200))
            );

            //Reinforcements
            Skills.Add(new Skill("Reinforcements", "Summon backup to assist you.", 100) { CoolDownMs = 30000 }
                .WithAction(Reinforcement)
                .WithModTree(ReinforcementTree())
                .WithParam("Crew Size", 1, o => (o.GetIntParam("Crew Size")).ToString() + " members")
                .WithParam("Member HP", 20, o => (o.GetIntParam("Member HP")).ToString())
                .WithParam("Weapon", WeaponHash.Pistol.ToString(), o => (o.GetStringParam("Weapon").ToString()))
                .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s")
                .WithMods(Mod("Additional Flyers", 200), Mod("Fund Kevlar", 200), Mod("Fund Weapons", 200))
                .WithCooldown(120000)
            );


            //Get Hammered
            Skills.Add(new Skill("Get Hammered", "Black out and stuff happens...", 2500) { CoolDownMs = 120000 }
            .WithAction(o =>
            {
                SkillEventHandler.Do(x =>
                {
                    Game.Player.Character.IsInvincible = true;
                    Game.FadeScreenOut(1000);
                    SkillEventHandler.Wait(1000);
                    var nearbyPeds = World.GetNearbyPeds(Game.Player.Character, 100);
                    foreach (var ped in nearbyPeds)
                    {
                        if (ped.RelationshipGroup != Game.Player.Character.RelationshipGroup)
                        {
                            ped.Health -= 100;
                            ped.Armor -= 100;
                            
                        }
                    } 
                    Game.Player.Character.IsInvincible = false;
                    SkillEventHandler.Wait(1000);
                    Game.FadeScreenIn(1000);
                });
            })
            .WithVisibleParam("Cooldown", o => ((float)o.CoolDownMs / 1000).ToString("0.0") + "s"));
        }

        private static void GetHigh(Skill skill)
        {
            var rng = Random.Range(0, 100);
            SkillEventHandler.Do(x =>
            {
                Function.Call(Hash.SET_TIME_SCALE, rng < 80 ? 0.4f : 0.2f);
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER, rng < 80 ? "phone_cam6" : "phone_cam7");
                var slowTime = skill.GetIntParam("Slow Time");
                if (rng > 80)
                {

                    RPG.Notify(Notification.Danger("Woah, what a hit! HP Lost"));
                    if (Game.Player.Character.Health > 10)
                        Game.Player.Character.Health -= 10;
                    else
                        Game.Player.Character.Health = 5;
                }
               SkillEventHandler.Wait(slowTime);
                Function.Call(Hash.SET_TIME_SCALE, 1f);
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
                
            });
        }
        private static NTree GetHighTree()
        {
            var pos = new Point(UI.WIDTH / 2 - NTree.NodeCenter.Width, UI.HEIGHT / 2 - NTree.NodeCenter.Height * 3);

            var startingNode = new Node("Get High", new GTASprite("mpinventory", "mp_specitem_weed"), NodeType.Skill).WithUnusable();

            var tree = new NTree("Get High", startingNode, TreeType.SkillMod, pos);
            tree.AddChild(new Node("Slow I", new GTASprite("mpinventory","team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Slow Time", 7000); })
                .WithDescription("Increases slow time to 7 seconds"));
            tree.AddChild(new Node("Slow II", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Slow Time", 8000);})
                .WithDescription("Increases slow time to 8 seconds"));
            return tree;
        }

        private static void Reinforcement(Skill skill)
        {
            var rng = Random.Range(0, 100);
            SkillEventHandler.Do(x =>
            {
                Function.Call(Hash.SET_TIME_SCALE, rng < 80 ? 0.4f : 0.2f);
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER, rng < 80 ? "phone_cam6" : "phone_cam7");
                var crewSize = skill.GetIntParam("Crew Size");
                var crewHp = skill.GetIntParam("Member HP");
                var crewWeapon = skill.GetStringParam("Weapon");
                var crewWeaponEnum = (WeaponHash)Enum.Parse(typeof(WeaponHash), crewWeapon);

                for (int i = 0; i < crewSize; i++)
                {
                    Model model = PedHash.Security01SMM;
                    model.Request(1000);
                    var ped = World.CreatePed(model, new Vector3(Game.Player.Character.Position.X + 0.5f, Game.Player.Character.Position.Y + 0.5f, Game.Player.Character.Position.Z + 0.5f)+Vector3.RandomXY());
                    RPG.WorldData.AddPed(new NpcObject("ReinforcementsPed_" + i,ped));
                    ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
                    ped.Health = crewHp;
                    if (crewHp > 20) ped.Armor = 20;
                    ped.Weapons.Give(crewWeaponEnum,1000,true,true);
                    ped.CanSwitchWeapons = true;
                }
            });
        }
        private static NTree ReinforcementTree()
        {
            var pos = new Point(UI.WIDTH / 2 - NTree.NodeCenter.Width, UI.HEIGHT / 2 - NTree.NodeCenter.Height * 3);

            var startingNode = new Node("Reinforcements", new GTASprite("mpinventory", "mp_specitem_weed"), NodeType.Skill).WithUnusable();

            var tree = new NTree("Reinforcements", startingNode, TreeType.SkillMod, pos);
            tree.AddChild(new Node("Additional Flyers", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Crew Size", 3); })
                .WithDescription("Distribute more flyers to increase crew size to 3"));
            tree.AddChild(new Node("Fund Kevlar", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Member HP", 60);})
                .WithDescription("Crew members survive longer"));
            tree.AddChild(new Node("Fund Weapons", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Weapon", WeaponHash.AssaultRifle);})
                .WithDescription("Fund your reinforcements to carry rifles instead of pistols"));
            return tree;
        }

        private static void BlazedOffGlory(Skill skill)
        {
            SkillEventHandler.Do(x =>
            {
                Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "phone_cam1");
                var blazeTime = skill.GetIntParam("Blaze Time");
                var damageBuff = skill.GetFloatParam("Damage Buff");
                
                //set damage to +damageBuff
                Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, Game.Player, 0.5f * (1 + damageBuff));

               SkillEventHandler.Wait(blazeTime);

                //set damage to default
                Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, Game.Player, 0.5f);

                Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
                
            });
        }
        private static NTree BlazedOffGloryTree()
        {
            var pos = new Point(UI.WIDTH / 2 - NTree.NodeCenter.Width, UI.HEIGHT / 2 - NTree.NodeCenter.Height * 3);

            var startingNode = new Node("Blazed Off Glory", new GTASprite("mpinventory", "mp_specitem_weed"), NodeType.Skill).WithUnusable();

            var tree = new NTree("Blazed Off Glory", startingNode, TreeType.SkillMod, pos);
            tree.AddChild(new Node("Two Puffs", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                    .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Blaze Time", 20000); })
                    .WithDescription("Increases blaze time to 20 seconds"));
            tree.AddChild(new Node("Lethal Dose", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Damage Buff", 2.0f);})
                .WithDescription("Increases damage buff to 200%"));
            return tree;
        }

        private static NTree ToughenUpTree()
        {
            var pos = new Point(UI.WIDTH / 2 - NTree.NodeCenter.Width, UI.HEIGHT / 2 - NTree.NodeCenter.Height * 3);

            var startingNode = new Node("Toughen Up", new GTASprite("mpinventory", "drug_trafficking"), NodeType.Skill).WithUnusable();

            var tree = new NTree("Toughen Up", startingNode, TreeType.SkillMod, pos);
            tree.AddChild(new Node("Increased Armor", new GTASprite("mpinventory","team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Armor Amount", 20); })
                .WithDescription("Increases armor gain to 20"));
            tree.AddChild(new Node("Swift Change", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.CoolDownMs = 15000;})
                .WithDescription("Decreases cooldown time to 15 seconds"));
            return tree;
        }

        private static NTree RejectNonsenseTree()
        {
            var pos = new Point(UI.WIDTH / 2 - NTree.NodeCenter.Width, UI.HEIGHT / 2 - NTree.NodeCenter.Height * 3);

            var startingNode = new Node("Reject Nonsense", new GTASprite("mpinventory", "drug_trafficking"), NodeType.Skill).WithUnusable();

            var tree = new NTree("Reject Nonsense", startingNode, TreeType.SkillMod, pos);
            tree.AddChild(new Node("Reach For The Stars", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Knockup Height", 50); })
                .WithDescription("Increases knockup height"));
            tree.AddChild(new Node("Knocked Up", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Radius", 10); })
                .WithDescription("Target more nearby characters"));
            return tree;
        }

        private static NTree RampageTree()
        {
            var pos = new Point(UI.WIDTH / 2 - NTree.NodeCenter.Width, UI.HEIGHT / 2 - NTree.NodeCenter.Height * 3);

            var startingNode = new Node("Rampage", new GTASprite("mpinventory", "drug_trafficking"), NodeType.Skill).WithUnusable();

            var tree = new NTree("Rampage", startingNode, TreeType.SkillMod, pos);
            tree.AddChild(new Node("Rampage I", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Rampage Time", 8000); })
                .WithDescription("Increases rampage time to 8 seconds"));
            tree.AddChild(new Node("Rampage II", new GTASprite("mpinventory", "team_deathmatch"), NodeType.SkillMod)
                .WithUnlockAction(o => { var skill = (Skill)o; skill.SetParam("Rampage Time", 10000); })
                .WithDescription("Increases rampage time to 10 seconds"));
            return tree;
        }


        public static Skill Get(string name)
        {
            return GM.Copy(Skills.FirstOrDefault(i => i.Name == name));
        }

        public static Action<Skill> GetAction(string name)
        {
            var skill = Skills.FirstOrDefault(i => i.Name == name);
            if(skill != null)
            {
                return skill.OnUse;
            }

            return null;
        }

        public static Dictionary<string, Func<Skill, string>> GetVisibleParams(string name)
        {
            var skill = Skills.FirstOrDefault(i => i.Name == name);
            if(skill != null)
            {
                return skill.VisibleParameters;
            }

            return null;
        }

        public static NTree GetModTree(string skillName)
        {
            var skill = Skills.FirstOrDefault(i => i.Name == skillName);
            if (skill != null)
            {
                return skill.ModTree;
            }

            return null;
        }

        public static KeyValuePair<string,int> Mod(string k, int v)
        {
            return new KeyValuePair<string, int>(k,v);
        }
    }
}