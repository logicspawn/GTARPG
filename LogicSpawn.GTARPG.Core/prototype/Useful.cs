using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Repository;

namespace LogicSpawn.GTARPG.Core
{
    class Useful
    {
	    void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!RPG.GameLoaded) return;
            if (!RPG.GameLoaded) return;


            //Disable player firing when near NPC
            //void DISABLE_PLAYER_FIRING(Player player, BOOL toggle) // 5E6CC07646BBEAB8 30CB28CB


            //Damage + armor

            //Spawn ped, shoot with pistol, log damage dealt, 
                //change the damge modifier, log damage dealt, restore ped to full hp
            //repeat


            //void SET_PLAYER_WEAPON_DEFENSE_MODIFIER(Player player, float modifier)
            //void SET_AI_WEAPON_DAMAGE_MODIFIER(float value)
            //void RESET_AI_WEAPON_DAMAGE_MODIFIER() // 0xEA16670E7BA4743C 0x6E965420
            //void REMOVE_PED_ELEGANTLY(Ped *pedHandle) // 0xAC6D445B994DF95E 0x4FFB8C6C

            //void _SET_PLAYER_HEALTH_REGENERATION_RATE(Player player, float regenRate)
            //void SET_PLAYER_WEAPON_DAMAGE_MODIFIER(Player player, float damageAmount)
            //void SET_PLAYER_MELEE_WEAPON_DAMAGE_MODIFIER(Player player, float modifier)
            //void SET_PLAYER_MELEE_WEAPON_DEFENSE_MODIFIER(Player player, float modifier)
            //Hash GET_ENTITY_MODEL(Entity entity) // 0x9F47B058362C84B5 0xDAFCB3EC
            //void SET_ENTITY_CAN_BE_DAMAGED(Entity entity, BOOL Toggle)
            //void SET_ENTITY_CAN_BE_DAMAGED_BY_RELATIONSHIP_GROUP(Any p0, BOOL p1, Any p2) 
            //void SET_ENTITY_ALPHA(Entity entity, int AlphaLVL, BOOL p2)
            //void SET_PED_ARMOUR(Ped ped, int amount) // 0xCEA04D83135264CC 0x4E3A0CC4
            //void EXPLODE_PED_HEAD(Ped ped, Hash weaponHash)
            //void SET_PED_ACCURACY(Ped ped, int accuracy) // accuracy = 0-100, 100 being perfectly accurate
            //void SET_PED_CAN_BE_TARGETTED_BY_PLAYER(Ped ped, Player player, BOOL toggle)
            //void SET_PED_SEEING_RANGE(Ped ped, float value)
            //void SET_PED_HEARING_RANGE(Ped ped, float value)
            //Any GET_PED_CAUSE_OF_DEATH(Ped ped) // 0x16FFE42AB2D2DC59 0x63458C27
            //void APPLY_DAMAGE_TO_PED(Ped ped, Any damageAmount, BOOL p2)
            //void SET_ENABLE_HANDCUFFS(Ped ped, BOOL toggle)
            //void SET_ENABLE_BOUND_ANKLES(Ped ped, BOOL toggle)
            //void RESURRECT_PED(Ped ped) // 0x71BC8E838B9C6035 0xA4B82097

            
            //Char Creation
            //BOOL IS_PED_MALE(Ped ped) // 0x6D9F5FAA7488BA46 0x90950455
            //BOOL IS_PED_HUMAN(Ped ped) // 0xB980061DA992779D 0x194BB7B0
            //BOOL GET_PED_LAST_DAMAGE_BONE(Ped ped, Any *outBone)


            //Car mods
            var vec = Game.Player.Character.CurrentVehicle;
            
            Function.Call(Hash.SET_VEHICLE_MOD_KIT, vec.Handle, 0);
            vec.ToggleMod(VehicleToggleMod.Turbo, true);
            vec.ToggleMod(VehicleToggleMod.TireSmoke, true);
            vec.SetMod(VehicleMod.Spoilers, 2, true);
            vec.SetMod(VehicleMod.Engine, 3, true);
            vec.SetMod(VehicleMod.BackWheels, 3, true);
            vec.SetMod(VehicleMod.FrontWheels, 3, true);
            vec.SetNeonLightsOn(VehicleNeonLight.Front, true);
            vec.SetNeonLightsOn(VehicleNeonLight.Back, true);
            vec.SetNeonLightsOn(VehicleNeonLight.Right, true);
            vec.SetNeonLightsOn(VehicleNeonLight.Left, true);
            vec.NeonLightsColor = Color.Red;
            vec.TireSmokeColor = Color.Red;

            //RPG Style UI - WIP
            new UIRectangle(new Point(RPGInfo.IsWideScreen ? 51 : 170, UI.HEIGHT - 41), new Size(181,9), Color.FromArgb(240,66,66,66)).Draw();
            new UIRectangle(new Point(5,5), new Size(200,8), Color.Green).Draw();
            new UIRectangle(new Point(5,16), new Size(200,8), Color.DodgerBlue).Draw();
            new UIText("prologicx" + " level " + 5 + " criminal", new Point(51, UI.HEIGHT - 44), 0.25f, Color.White, 0, false).Draw();

            //Weapon skins
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Game.Player.Character.Handle, Convert.ToInt32(WeaponHash.MicroSMG), 3);

            //Animations
            Game.Player.Character.Task.PlayAnimation("missfbi5ig_30monkeys", "monkey_a_freakout_loop", 1, -1, false, 1.0f);


            //Set model
            if (e.KeyCode == Keys.Y)
            {
                Model m = "s_m_y_cop_01";
                m.Request(1000);
                Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, m.Hash);
                Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION,Game.Player.Character.Handle);
                        
            }

            if (e.KeyCode == Keys.Y)
            {
                Game.FadeScreenOut(2000);
            
            }

            //Load Game
            if (e.KeyCode == Keys.T)
            {
            
                var m = new Model(GM.GetHashKey("prop_box_guncase_02a"));
                var m2 = new Model(GM.GetHashKey("prop_box_guncase_01a"));
                var m3 = new Model(GM.GetHashKey("prop_box_guncase_03a"));
            
                var p = World.CreateProp(m, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f + new Vector3(0, 1, 0.1f), Game.Player.Character.Rotation, true, false);
                p.ApplyForce(Vector3.Zero);
                RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get("Ammo Pack I"), p));
            
                var p2 = World.CreateProp(m2, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f + new Vector3(0, 2, 0.2f), Game.Player.Character.Rotation, true, false);
                p2.ApplyForce(Vector3.Zero);
                RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get("Ammo Pack II"), p2));
            
                var p3 = World.CreateProp(m3, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f + new Vector3(0, 0, 0.3f), Game.Player.Character.Rotation, true, false);
                p3.ApplyForce(Vector3.Zero);
                RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get("Ammo Pack III"), p3));
            }


            //Add Exp
            if (e.KeyCode == Keys.O)
            {
                RPG.Subtitle("Gained 50 Exp.", 800);
                RPG.PlayerData.AddExp(50);
            }

            //Use Health Potion
            if (e.KeyCode == Keys.H)
            {
                var ped = Game.Player.Character;
                ped.Health += 20;
                ped.Armor += 20;
                RPG.Subtitle("You smoked a quick joint, +20 HP and ARMOUR", 800);
            }

            //Add Cash
            if (e.KeyCode == Keys.Y)
            {
                Game.Player.Character.Money += 100000;
                RPG.Subtitle("Added GTA$100000",500);
            }


            //Load Game
            if (e.KeyCode == Keys.I)
            {
                Model m = PedHash.Stripper01Cutscene;
                var ped = World.CreatePed(m, new Vector3(Game.Player.Character.Position.X + 0.2f, Game.Player.Character.Position.Y + 0.2f, Game.Player.Character.Position.Z + 0.2f));
                Model m2 = PedHash.Stripper02Cutscene;
                var ped2 = World.CreatePed(m2, new Vector3(Game.Player.Character.Position.X + 0.5f, Game.Player.Character.Position.Y + 0.5f, Game.Player.Character.Position.Z + 0.5f));
            }
            if (e.KeyCode == Keys.K)
            {
                //showQuestPanel = !showQuestPanel;
            }

            if(true)
            {

	            //LoadGame();
                //World.AddExplosion(Game.Player.Character.Position + Game.Player.Character.ForwardVector * 10f,ExplosionType.BigFire,1,2f);
                var ped = Game.Player.Character;
                if (ped.CurrentVehicle == null) return;

                //var v = World.CreateVehicle(new Model(VehicleHash.EntityXF), ped.Position + ped.ForwardVector*3f, ped.Heading);
                var v = ped.CurrentVehicle;
                //v.WheelType = (VehicleWheelType)Random.Range(0,5);
                //v.WindowTint = (VehicleWindowTint)Random.Range(0,5);


                Function.Call(Hash.ROLL_DOWN_WINDOWS, v.Handle);

                //Function.Call(Hash.SET_VEHICLE_MOD_KIT, vec.Handle, 0);

                //v.SetMod(VehicleMod.FrontWheels, Random.Range(0, 5), true);
                var allColors = (VehicleColor[])Enum.GetValues(typeof (VehicleColor));
                v.PrimaryColor = allColors[Random.Range(0, allColors.Length)];
                v.SecondaryColor = allColors[Random.Range(0, allColors.Length)];

                //var blipHandle = Function.Call<int>(Hash.ADD_BLIP_FOR_ENTITY, ped);
                //Function.Call(Hash.SET_BLIP_SPRITE,blipHandle,2);

            }

            //Save Game
//            if (e.KeyCode == Keys.N)
//            {
//                try
//                {
//                    Function.Call(Hash.SET_TIME_SCALE, 1.0f);
//                    Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");     //RESET
//                }
//                catch
//                {
//                    RPG.Subtitle("Err.");
//                }
//            }
//	        if (e.KeyCode == Keys.U)
//            {
//                    try
//                    {
//                        Function.Call(Hash.SET_TIME_SCALE, 0.4f);
//                        Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "phone_cam9");     //RESET
//                        //phonecam4 :D , phonecam9
//                        
//                    }
//                    catch
//                    {
//                        RPG.Subtitle("Err.");
//                    }

//                Print("What is your name?");
//                String test = Game.GetUserInput(15); //new code
//                PlayerData.Name = test;
//                int copHash = Function.Call<int>(Hash.GET_HASH_KEY, "s_m_y_cop_01");
//                
//                //Spawn 3  bodyguards
//                var player = Game.Player.Character;
//                
//                var playerIndex = Function.Call<int>(Hash.GET_PLAYER_INDEX);
//                var groupId = Function.Call<int>(Hash.GET_PLAYER_GROUP,playerIndex );
//
//                for (int i = 0; i < 3; i++)
//                {
//                    Model m = Data.GetNameOfPedModel("GENHOT");
//                    var ped = World.CreatePed(m, new Vector3(player.Position.X + 0.2f, player.Position.Y + 0.2f, player.Position.Z + 0.2f));
//                    Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, ped.Handle);
//                    Function.Call(Hash.SET_PED_AS_GROUP_MEMBER, groupId);
//                    var wepHashA = Function.Call<int>(Hash.GET_HASH_KEY, Data.WeaponNames.WEAPON_ASSAULTRIFLE.ToString());
//                    var wepHashB = Function.Call<int>(Hash.GET_HASH_KEY, Data.WeaponNames.WEAPON_APPISTOL.ToString());
//                    Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, wepHashA, 1000, false, false);
//                    Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, wepHashB, 1000, false, false);
//                    Function.Call(Hash.SET_PED_ACCURACY, ped, true);
//                    Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped.Handle, true);
//                    Function.Call(Hash.SET_PED_COMBAT_ABILITY, ped, 2);
//                    Function.Call(Hash.SET_CURRENT_PED_WEAPON, ped, wepHashA, true);
//                    ped.Task.GoTo(Game.Player.Character, new Vector3(i == 0 ? 1 : 0, i == 1 ? 1 : 0, i == 2 ? 1 : 0));
//                    bodyguards.Add(ped);
//                    
//                }
                //Clone player
                //var ped = Function.Call<Ped>(Hash.CLONE_PED, player.Handle, player.Position.X + 0.2f, player.Position.Y + 0.2f, player.Position.Z + 0.2f);

                //Create random aggresive peds
//                for (int i = 0; i < 1; i++)
//                {
//                    var model = new Model(copHash);
//
//                    var randomVec = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), -1);
//                    var ped = World.CreatePed(model, Game.Player.Character.Position + randomVec, 0);
//                    Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, ped.Handle);
//
//                    //ped.Task.FightAgainst(Game.Player.Character);
//                    //ped.Task.HandsUp(50000);
//                    //ped.Task.UseMobilePhone(50000);
//                    try
//                    {
//                        var wepHash = Function.Call<int>(Hash.GET_HASH_KEY, Data.WeaponNameArray[Random.Range(0,Data.WeaponNameArray.Length)]);
//                        Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped.Handle, true);
//                        Function.Call(Hash.GIVE_WEAPON_TO_PED, ped, wepHash, 1000, false, false);
//                        Function.Call(Hash.SET_CURRENT_PED_WEAPON, ped, wepHash, true);    
//                        ped.Task.ShootAt(Game.Player.Character);
//                        ped.Task.FightAgainst(Game.Player.Character);
//                    }
//                    catch
//                    {
//                        Print("error");
//                    }
//                    
//                }
                //SaveGame();
//            }




	    }
    }
}
