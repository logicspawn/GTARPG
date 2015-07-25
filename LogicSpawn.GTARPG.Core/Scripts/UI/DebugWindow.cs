using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using LogicSpawn.GTARPG.Core.Scripts.Questing;
using NAudio.Wave;
using Menu = GTA.Menu;

namespace LogicSpawn.GTARPG.Core
{
    public class DebugWindow : UpdateScript 
    {
        private NTree skillTree;
        private TreeMenu Menu;
        private IWavePlayer flashMusic;
        private bool Debug;
        protected override bool RunWhenGameIsNotLoaded
        {
            get { return true; }
        }

        public DebugWindow()
        {
            //AppDomain.CurrentDomain.Load("System.Drawing.dll");

            

            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if(keyEventArgs.KeyCode == Keys.F11)
            {
                Debug = !Debug;
            }

            if(keyEventArgs.KeyCode == Keys.F5)
            {

                RPG.PlayerData.SkillExp += 100000;
//                RPG.PlayerData.Setup.SafeArea -= 1;
//                if (RPG.PlayerData.Setup.SafeArea < 0) RPG.PlayerData.Setup.SafeArea = 0;
//                RPG.Notify("ayy lmao?");
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack I"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack I"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack I"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack I"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack I"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack II"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack III"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack III"));
                //RPG.PlayerData.AddItem(ItemRepository.Get("Ammo Pack III"));
                //EventHandler.Do(o => RPG.Audio.Say("Listen, there's a lot to do, I'll require some of your, let say, specific set of skills."));

                //RPG.PlayerData.SkillExp += 10000;
                //IWavePlayer waveOutDevice = new WaveOutEvent();
                //AudioFileReader audioFileReader = new AudioFileReader(@"C:\Test\pretend.mp3");
                //
                //audioFileReader.Volume = 0.5f;
                //waveOutDevice.Init(audioFileReader);
                //waveOutDevice.Play();

                //Game.Player.Character.Weapons.Remove(Game.Player.Character.Weapons.Current);


                //Game.Player.Character.Health -= 25;
                //RPG.PlayerData.SkillExp += 1000;
                //var pedx = Game.Player.Character;
                //var m = new Model(GM.GetHashKey("prop_box_guncase_01a"));
                //var p = World.CreateProp(m, pedx.Position + new Vector3(0, 0, 2.0f), pedx.Rotation, true, false);
                //p.ApplyForce(new Vector3(0, 0, -0.05f));
                //RPG.WorldData.AddLoot(new LootItem(ItemRepository.Get("Vehicle Parts"), p));


                //Model m2 = PedHash.Genstreet01AMY;
                //var ped2 = World.CreatePed(m2, new Vector3(Game.Player.Character.Position.X + 0.5f, Game.Player.Character.Position.Y + 0.5f, Game.Player.Character.Position.Z + 0.5f));
                //RPG.PlayerData.AddExp(100);

            }

            if(keyEventArgs.KeyCode == Keys.F6)
            {
                RPG.PlayerData.Setup.SafeArea += 1;
                if (RPG.PlayerData.Setup.SafeArea == 11) RPG.PlayerData.Setup.SafeArea = 10;

                
                
                
                
                
                
                                                                                                                    //Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);

                //try
                //{
                //
                //    var componentHash = GM.GetHashKey("MICRO_SMG_MODIFIER");
                //    var componentHash1 = GM.GetHashKey("MICRO_SMG_AMMO_MODIFIER");
                //    var componentHash2 = GM.GetHashKey("WEAPONADDON_MICROSMG_SCOPE_EXPENDITURE_TUNABLE");
                //    RPG.Notify(componentHash.ToString());
                //    //(Ped PedHandle, Hash WeaponHash,Hash ComponentHash)
                //    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character.Handle, (int)Game.Player.Character.Weapons[WeaponHash.MicroSMG].Hash,componentHash);
                //
                //    //(Any p0, Any p1) //Assumed Hash WeaponHash hash ComponentHash
                //    var wep = Function.Call<int>(Hash.GET_WEAPON_OBJECT_FROM_PED, Game.Player.Character, false);
                //    var wepObj = new Prop(wep);
                //    Function.Call(Hash.SET_WEAPON_OBJECT_TINT_INDEX, wepObj, 2);
                //
                //    Game.Player.Character.Weapons.Remove(Game.Player.Character.Weapons.Current);
                //    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, wepObj.Handle, componentHash);
                //    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, wepObj.Handle, componentHash1);
                //    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, wepObj.Handle, componentHash2);
                //    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, wepObj, componentHash);
                //   Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, wepObj, componentHash1);
                //    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_WEAPON_OBJECT, wepObj, componentHash2);
                //    Function.Call(Hash.GIVE_WEAPON_OBJECT_TO_PED, wepObj, Game.Player.Character);
                //}
                //catch (Exception ex)
                //{
                //    
                //    RPGLog.Log("Err adding mods:");
                //    RPGLog.Log(ex.ToString());
                //    RPG.Notify("Err adding mods");
                //}



                //Model m = PedHash.StrPunk01GMY;
                //var ped = World.CreatePed(m, new Vector3(Game.Player.Character.Position.X + 0.2f, Game.Player.Character.Position.Y + 0.2f, Game.Player.Character.Position.Z + 0.2f));
                //ped.AddBlip();
                //ped.CurrentBlip.Scale = 0.7f;
                //RPG.WorldData.AddPed(new NpcObject("Random Enemy",ped){HasBlip = true});

                //Model m2 = PedHash.StrPunk02GMY;
                //var ped2 = World.CreatePed(m2, new Vector3(Game.Player.Character.Position.X + 0.5f, Game.Player.Character.Position.Y + 0.5f, Game.Player.Character.Position.Z + 0.5f));
                //foreach (var ped in World.GetNearbyPeds(Game.Player.Character,1000))
                //{
                //    ped.Delete();
                //}
                //foreach (var veh in World.GetNearbyVehicles(Game.Player.Character,1000))
                //{
                //    veh.Delete();
                //}
            }
        }


        private void ChangeVariation(int obj)
        {
            var index = Mennu.SelectedIndex;
            var kvp = variations.ElementAt(index);
            var component = kvp.Key;
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character, component, obj, 0,0);
        }

        private Menu Mennu;
        Dictionary<int, int> variations = new Dictionary<int, int>();


        private void SelectTile(int obj)
        {
            throw new NotImplementedException();
        }

      
        public override void Update()
        {
            
            if(Game.IsKeyPressed(Keys.F5))
            {
                //try
                //{

                    //RPG.PlayerData.AddMoney(20120324);
                    //var mmmmmenu = new TiledMenu("Select Your Gender",
                    //    new TiledPanel("Male", new GTASprite("mptattoos1", "tattoo_drugdeal", Color.Red), Color.DodgerBlue, SelectTile, "Play as an XY chromosome."),
                    //    new TiledPanel("Female", new GTASprite("mptattoos", "killplayerbountyhead", Color.Purple),Color.Pink, SelectTile, "Play as an XX chromosome."));
                    //View.AddMenu(mmmmmenu);

                    //variations = new Dictionary<int, int>();
                    //for (int i = 0; i < 10 ; i++)
                    //{
                    //    var a = Function.Call<int>(Hash._0x5FAF9754E789FB47, Game.Player.Character, i);
                    //    //if(a > 1)
                    //    //{
                    //    //    variations.Add(i,a);
                    //        RPGLog.Log("Tried " + i + " got variations: " + a);
                    //    //}
                    //}
                    //Function.Call(Hash.SET_PED_RANDOM_PROPS, Game.Player.Character);
                    //Function.Call(Hash.SET_PED_PROP_INDEX, Game.Player.Character, 0, adasdasdasdij);
                    //adasdasdasdij++;

//                    
//                    View.MenuPosition = new Point(UI.WIDTH/2 -  150, UI.HEIGHT - 300);
//                    Mennu.Width = 300;
//                    RPGUI.FormatMenu(Mennu);
//                    View.AddMenu(Mennu);

                //}
                //catch (Exception ex)
                //{
                //    RPGLog.Log(ex);
                //}

                //Wait(500);
                //try
                //{
                //    var outArg = new OutputArgument();
                //    var a =  Function.Call<Vector3>(Hash.GET_HUD_COMPONENT_POSITION, 15, outArg);
                //    RPG.Notify("a: " + a);
                //    RPG.Notify("a: " + outArg.GetResult<Vector3>());
                //
                //}
                //catch
                //{
                //    
                //}

                //string s = Game.GetUserInput(100);
                //try
                //{
                //    var propName = s;
                //    Model m = propName;
                //    var p = World.CreateProp(m, Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2, true, true);
                //    Wait(5000);
                //    if (p.Exists())
                //    {
                //        p.Delete();
                //    }
                //}
                //catch (Exception)
                //{
                //    RPG.Notify("err");
                //}
            }

            if (!Debug) return;
            var c = new UIContainer(new Point(UI.WIDTH - 305, UI.HEIGHT - 305), new Size(300, 300), Color.FromArgb(180, 50, 50, 50));

            var debug = "";
            debug += "player co-ordinates: " + VecStr(Game.Player.Character.Position) + "\n";
            debug += "- heading: " + Game.Player.Character.Heading + "\n";
            debug += "- rotation: " + VecStr(Game.Player.Character.Rotation) + "\n";
            debug += "- model hash: " + Game.Player.Character.Model.Hash + "\n";
             var v = new UIText(debug,new Point(0, 0), 0.25f, Color.White);

            var cardebug = "";
             if (Game.Player.Character.IsInVehicle())
             {
                 cardebug += "vehicle model: " + Game.Player.Character.CurrentVehicle.Model.Hash + "\n";
                 cardebug += "- heading: " + Game.Player.Character.CurrentVehicle.Heading + "\n";
                 cardebug += "- rotation: " + VecStr(Game.Player.Character.CurrentVehicle.Rotation) + "\n";
             }
             var x = new UIText(cardebug, new Point(0, 200), 0.25f, Color.White);

            c.Items.Add(v);
            c.Items.Add(x);
            c.Draw();
        }



        private string VecStr(Vector3 position)
        {
            return (int) position.X + "," + (int) position.Y + "," + (int) position.Z;
        }
    }
}
