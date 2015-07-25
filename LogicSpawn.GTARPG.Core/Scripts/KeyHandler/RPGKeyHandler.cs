using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Scripts.Popups;

namespace LogicSpawn.GTARPG.Core
{
    public class RPGKeyHandler : KeyHandlerScript
    {
        private PlayerData PlayerData
        {
            get { return RPG.PlayerData; }
        }

        protected override void Init()
        {
            RegisterKeyUp(Keys.T, UseSkillT);
            RegisterKeyUp(Keys.Y, UseSkillY);
            RegisterKeyUp(Keys.CapsLock, UseSkillCaps);
            RegisterKeyUp(Keys.U, UseSkillU);
            RegisterKeyUp(Keys.B, UseSkillB);


            RegisterKeyUp(Keys.L, ShowQuestLog);
            RegisterKeyUp(Keys.O, ShowCharacterMenu);
            RegisterKeyUp(Keys.I, ShowInventory);
            RegisterKeyUp(Keys.J, ShowMenu);
            RegisterKeyUp(Keys.K, SpawnCar);
            RegisterKeyUp(Keys.E, Interact);

            RegisterKeyUp(Keys.F7, ShowHelp);
            RegisterKeyUp(Keys.F8, SaveGame);
            RegisterKeyUp(Keys.F9, ToggleUI);
            RegisterKeyUp(Keys.F10, NewGame);


            RegisterKeyUp(Keys.Back,CloseViewMenu);
            RegisterKeyUp(Keys.F4, CloseViewMenu);

            RegisterKeyUp(Keys.NumPad0, EndDialogFix);
        }

        private void EndDialogFix()
        {
            if(RPG.UIHandler.CurrentDialog != null)
            {
                RPG.UIHandler.DialogEnd();
            }
        }

        private void ToggleUI()
        {
            RPG.UIHandler.ShowUI = false;
        }

        private void ShowQuestLog()
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            RPG.UIHandler.OpenQuestLog();
        }

        private void CloseViewMenu()
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            if (RPGInfo.KeyboardActive) return; 

            if (RPG.UIHandler.CurrentMenu is TreeMenu)
            {
                var t = RPG.UIHandler.CurrentMenu as TreeMenu;
                RPG.UIHandler.CurrentMenu = null;
                RPG.UIHandler.View.PopMenu();

                if(t.TreeType == TreeType.SkillMod)
                {
                    RPG.UIHandler.View.AddMenu(RPG.SkillHandler.GetSkillMenu());
                    return;
                }
            }
                
            RPG.UIHandler.View.PopMenu();
        }

        private void ShowHelp()
        {
            RPG.GetPopup<HelpBox>().Show();
        }

        private void ShowCharacterMenu()
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            RPG.UIHandler.OpenCharacterMenu();
        }

        private void UseSkillT() { RPG.UIHandler.CloseAll(); RPG.SkillHandler.Use(Keys.T); }
        private void UseSkillY() { RPG.UIHandler.CloseAll(); RPG.SkillHandler.Use(Keys.Y); }
        private void UseSkillCaps()
        {
            if (!RPG.PlayerData.Tutorial.UsingSkills && RPG.PlayerData.Tutorial.PressJToOpenMenu && RPG.PlayerData.Tutorial.BoughtAmmoFromShop && RPG.PlayerData.Tutorial.GetAKill && RPG.PlayerData.Tutorial.UnlockSkillWithSp)
            {
                var tut = RPG.GetPopup<TutorialBox>();
                RPG.PlayerData.Tutorial.UsingSkills = true;
                EventHandler.Do(o =>
                {
                    tut.Hide();
                    EventHandler.Wait(300);
                    if (!RPG.PlayerData.Tutorial.UsingSkills)
                    {
                        tut.Pop("You're ready to begin, but you need to learn to speak in a new way.", "Press K to spawn your vehicle.");
                    }
                });
            } 
            
            RPG.UIHandler.CloseAll(); RPG.SkillHandler.Use(Keys.CapsLock);
        }
        private void UseSkillU() { RPG.UIHandler.CloseAll(); RPG.SkillHandler.Use(Keys.U); }
        private void UseSkillB() { RPG.UIHandler.CloseAll(); RPG.SkillHandler.Use(Keys.B); }

        private void ShowInventory()
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            RPG.UIHandler.OpenInventory();  
        }

        private void SpawnCar()
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            if (!RPG.PlayerData.Tutorial.SpawnVehicle && RPG.PlayerData.Tutorial.PressJToOpenMenu && RPG.PlayerData.Tutorial.BoughtAmmoFromShop && RPG.PlayerData.Tutorial.GetAKill && RPG.PlayerData.Tutorial.UnlockSkillWithSp && RPG.PlayerData.Tutorial.UsingSkills)
            {
                var tut = RPG.GetPopup<TutorialBox>();
                RPG.PlayerData.Tutorial.SpawnVehicle = true;
                EventHandler.Do(o =>
                {
                    tut.Hide();
                    EventHandler.Wait(300);
                    if (!RPG.PlayerData.Tutorial.UsingSkills)
                    {
                        Function.Call(Hash.SET_NEW_WAYPOINT, -8.9106f, -1090.779f);
                        tut.Pop("Vehicles are important, right? Time to make your way to Matthew and finish the tutorial quest. He's on the map with the marijuana symbol.", "Drive to Matthew and press E to speak with him.");
                    }
                });
            } 
            RPGMethods.SpawnCar(); 
        }

        private void ShowMenu()
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            
            if (!RPG.PlayerData.Tutorial.PressJToOpenMenu)
            {
                var tut = RPG.GetPopup<TutorialBox>();
                RPG.PlayerData.Tutorial.PressJToOpenMenu = true;
                EventHandler.Do(o =>
                {
                    tut.Hide();
                    Wait(300);
                    if(!RPG.PlayerData.Tutorial.BoughtAmmoFromShop)
                    {
                            tut.Pop("Through here you can find everything you need.", "Select Actions > Purchase Goods > Ammo Pack I to purchase an ammo pack.");
                    }
                });
            }

            RPG.UIHandler.ShowMenu();
        }

        private void Interact()
        {
            RPG.UIHandler.CloseAll();
            Popup.CloseLastPopup();
            var nearbyLoot = PlayerMethods.GetNearbyLoot(2.5f).FirstOrDefault();
            if(nearbyLoot != null)
            {
                RPGMethods.Loot(nearbyLoot);
            }

            var nearestPed = RPGInfo.NearestPed;
            if (nearestPed != null && !Game.Player.Character.IsInCombat)
            {
                var npcObject = RPG.WorldData.Npcs.FirstOrDefault(n => n.IsQuestNpc && n.EntityHandle == nearestPed.Handle);
                if(npcObject != null)
                {
                    if (npcObject.Name == "Matthew" && RPG.PlayerData.Tutorial.PressJToOpenMenu && RPG.PlayerData.Tutorial.BoughtAmmoFromShop && RPG.PlayerData.Tutorial.GetAKill && RPG.PlayerData.Tutorial.UnlockSkillWithSp && RPG.PlayerData.Tutorial.UsingSkills && RPG.PlayerData.Tutorial.SpawnVehicle)
                    {
                        var tut = RPG.GetPopup<TutorialBox>();
                        EventHandler.Do(o =>
                        {
                            tut.Hide();
                        });
                    } 

                    RPG.UIHandler.StartDialog(npcObject);
                }
            }
        }

        private void NewGame()
        {
            var confirm = RPGMessageBox.Create("Are you sure you want to start over?","Start new game","Continue playing", () =>
            {
                RPG.GameLoaded = false;
                CharCreationNew.RestartCharCreation();
            }, () => { });
            
            RPGUI.FormatMenu(confirm);

            RPG.UIHandler.View.AddMenu(confirm);
           
        }

        private void SaveGame()
        {
            RPG.SaveAllData();
            RPG.Subtitle("Saved.");
        }
        private void LoadGame()
        {
            RPG.LoadAllData();
            RPG.Subtitle("Loaded.");
        }
    }
}