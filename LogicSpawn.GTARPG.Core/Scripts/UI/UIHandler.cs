using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using Font = GTA.Font;
using Menu = GTA.Menu;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core
{
    public class UIHandler : UpdateScript
    {
        public MenuBase CurrentMenu = null;

        private Menu MainMenu;
        private Menu OptionsMenu;
        private Menu CharacterMenu;
        private Menu ActionsMenu;
        private Menu SkillbarMenu;

        private RPGListMenu InventoryMenu;
        private RPGListMenu ShopMenu;
        private RPGListMenu CraftingMenu;
        private RPGListMenu QuestLogMenu;
        public TreeMenu SkillTreeMenu;


        private RPGDialogMenu DialogMenu;

        private bool CharacterMenuVisible
        {
            get { return IsOpen(CharacterMenu); }
        }

        private const string ModInfo = "GTA:RPG v0.0.Dev by PROLogicX ";
        private PlayerData PlayerData
        {
            get { return RPG.PlayerData; }
        }

        //ShowHide
        public bool ShowUI = true;
        public bool ShowSkillBar = true;
        public bool ShowQuestTracker = true;
        public bool ShowingSubtitle;

        //Dialog
        private NpcObject CurrentNpc;
        public DialogContainer CurrentDialog;

        private Camera NpcCamera;

        public UIHandler()
        {
            RPG.UIHandler = this;
            //Use some fancy transitions
            View.MenuTransitions = true;
            View.PopMenu();
            View.MenuOffset = new Point(-302, 0);
            View.MenuPosition = new Point(UI.WIDTH -300, 0);

            OptionsMenu = new RPGMenu("Options", new GTASprite("CommonMenu", "interaction_bgd", Color.ForestGreen), new IMenuItem[] {
                        new MenuButton("Save Game", "").WithActivate(() => { RPG.SaveAllData(); RPG.Subtitle("Saved");}),
                        new MenuButton("New Game", "").WithActivate(NewGame),
                        new MenuNumericScroller("SafeArea Setting","",0,10,1,RPG.PlayerData.Setup.SafeArea).WithNumericActions(ChangeSafeArea,d => { }), 
                        new MenuToggle("Toggle Skill Bar", "",ShowSkillBar).WithToggles(() => ShowSkillBar = true, () => ShowSkillBar = false), 
                        new MenuToggle("Toggle Quest Tracker", "").WithToggles(() => ShowQuestTracker = true, () => ShowQuestTracker = false), 
                        new MenuButton("Back", "").WithActivate(() => View.PopMenu())
                    });
            CharacterMenu = new RPGMenu("Character Menu", new GTASprite("CommonMenu", "interaction_bgd", Color.ForestGreen), new IMenuItem[] { 
                        new MenuButton("Quests", "").WithActivate(OpenQuestLog),
                        new MenuButton("Set Skillbar", "").WithActivate(OpenSkillBarMenu),
                        new MenuButton("Skills", "").WithActivate(OpenSkillsMenu),
                        //new MenuButton("Talents", "", () => { View.PopMenu(); }),
                        //new MenuButton("Mod Weapons", "", () => { View.PopMenu(); }),
                        //new MenuButton("Mod Cars", "", () => { View.PopMenu();}),
                        new MenuButton("Back", "").WithActivate(() => View.PopMenu())
                    });

            ActionsMenu = new RPGMenu("ACTIONS", new GTASprite("CommonMenu", "interaction_bgd", Color.Red), new IMenuItem[] {
                        new MenuButton("Spawn Personal Vehicle", "").WithActivate(() => RPGMethods.SpawnCar()),        
                        new MenuButton("Get Random Contract", "").WithActivate(GetRandomContract),
                        new MenuButton("Purchase Goods", "").WithActivate(OpenShop),
                        new MenuButton("Craft Items", "").WithActivate(OpenCrafting),
                        new MenuButton("Back", "").WithActivate(View.PopMenu)
                    });

            SkillTreeMenu = RPG.SkillHandler.GetSkillMenu();

            //var o = new MenuNumericScroller("Number", "", d => { }, d => { }, 0, 100, 1);
            //var p = new MenuToggle("Toggle", "", ()=> { }, () => { });

            //"GTA RPG ALPHA BY PROLOGICX", 
            MainMenu = new RPGMenu("RPG Menu", new GTASprite("CommonMenu", "interaction_bgd", Color.DodgerBlue), new IMenuItem[] {                
                new MenuButton("Inventory", "").WithActivate(OpenInventory), 
                new MenuButton("Character Menu", "").WithActivate(OpenCharacterMenu),
                new MenuButton("Actions ", "").WithActivate(() => View.AddMenu(ActionsMenu)),
                new MenuButton("Options", "").WithActivate(() => OpenOptionsMenu()),                
                new MenuButton("Return to Normal Mode ", "").WithActivate(ConfirmReturnToNormal),
                new MenuButton("Close", "").WithActivate(View.PopMenu) 
            });

            

            RPGUI.FormatMenu(OptionsMenu);
            RPGUI.FormatMenu(ActionsMenu);
            RPGUI.FormatMenu(MainMenu);
            RPGUI.FormatMenu(CharacterMenu);
        }

        private void ChangeSafeArea(double obj)
        {
            RPG.PlayerData.Setup.SafeArea = (int) obj;
        }


        protected override void Start()
        {
            NpcCamera = World.CreateCamera(Game.Player.Character.Position, Game.Player.Character.Rotation, GameplayCamera.FieldOfView);
        }
        private void NewGame()
        {
            var confirm = RPGMessageBox.Create("Are you sure you want to start over?", "Start new game", "Continue playing", () =>
            {
                RPG.GameLoaded = false;
                CharCreationNew.RestartCharCreation();
            }, () => { });

            RPGUI.FormatMenu(confirm);

            RPG.UIHandler.View.AddMenu(confirm);
        }

        private void ConfirmReturnToNormal()
        {
            var box = RPGMessageBox.Create("Are you sure you want to return to normal mode?","Return to normal mode","Continue Playing GTA:RPG", () => { View.CloseAllMenus(); RPGMethods.ReturnToNormal(); }, () => { View.MenuPosition = new Point(UI.WIDTH - 300, 0); });
            RPGUI.FormatMenu(box);
            View.AddMenu(box);
        }

        private void GetRandomContract()
        {
            var currentContract = RPG.PlayerData.Quests.FirstOrDefault(q => q.IsContract && q.InProgress);
            if(currentContract != null)
            {
                RPG.Notify("Already assigned to a contract.");
            }
            else
            {
                var possibleContracts = PlayerData.Quests.Where(c => c.IsContract).Where(c => RPG.PlayerData.LastContracts.All(l => l != c.Name)).ToList();
                var nextContract = possibleContracts[Random.Range(0, possibleContracts.Count)];

                var last = RPG.PlayerData.LastContracts.ToList();
                last.RemoveAt(0);
                last.Add(nextContract.Name);
                RPG.PlayerData.LastContracts = last.ToArray();

                nextContract.Start();
                View.CloseAllMenus();
            }
        }

        public override void Update()
        {
            try
            {
                UpdateX();
            }
            catch(Exception ex)
            {
                RPGLog.Log("UIHandler Err.");
                RPGLog.Log(ex);
            }

        }

        public void UpdateX()
        {
            Ped player = Game.Player.Character;

            Vehicle vehicle = player.CurrentVehicle;

            //Loot Interact
            var nearbyLoot = RPGInfo.NearbyLoot;
            var showingLoot = false;
            if (nearbyLoot != null)
            {
                if (nearbyLoot.Item.Type == ItemType.Money || nearbyLoot.Item.Type == ItemType.QuestItem)
                {
                    RPGMethods.Loot(nearbyLoot);
                }
                else if (ShowUI)
                {
                    var interactUI = new UIContainer(new Point(UI.WIDTH / 2 - 120, UI.HEIGHT - 100), new Size(240, 17), Color.FromArgb(70, 70, 200, 70));
                    interactUI.Items.Add(new UIText("Press E To Loot " + nearbyLoot.Name, new Point(240 / 2, 1), 0.25f, Color.White, 0, true));
                    interactUI.Draw();
                    showingLoot = true;
                }
            }


            if (CurrentDialog != null && !IsOpen(DialogMenu))
            {
                World.RenderingCamera = NpcCamera;
                OpenDialog();
            }

            if (!ShowUI || CurrentDialog != null) return;

            //NPC Interact
            if(!showingLoot && CurrentDialog == null && !Game.Player.Character.IsInCombat)
            {
                var nearestPed = RPGInfo.NearestPed;
                if (nearestPed != null)
                {
                    var npcObject = RPG.WorldData.Npcs.FirstOrDefault(n => n.IsQuestNpc && n.EntityHandle == nearestPed.Handle);
                    if (npcObject != null)
                    {
                        var interactUI = new UIContainer(new Point(UI.WIDTH / 2 - 120, UI.HEIGHT - 122), new Size(240, 17), Color.FromArgb(70, 190, 190, 190));
                        interactUI.Items.Add(new UIText("Press E to Interact with " + npcObject.Name, new Point(240 / 2, 1), 0.25f, Color.White, 0, true));
                        interactUI.Draw();
                    }
                }
            }
            

            if (vehicle != null)
            {
                var speed = ((int)(vehicle.Speed * 2.45)).ToString("000");
                new UIText(" MPH", new Point(RPGInfo.IsWideScreen ? 207 : 248, UI.HEIGHT - 41), 0.22f, Color.White, 0, false).Draw(); //55
                new UIText(speed, new Point(RPGInfo.IsWideScreen ? 192 : 233, UI.HEIGHT - 41), 0.22f, Color.White, 0, false).Draw(); //55
            }

            //Player text
            //new UIText(PlayerData.Name.ToLower() + " level " + PlayerData.Level + " criminal", new Point(51, UI.HEIGHT - 55), 0.25f, Color.White, 0, false).Draw();
            
            //Expbar
            var expBarUI = new UIContainer(new Point(0, UI.HEIGHT - 2), new Size(UI.WIDTH, 2), Color.FromArgb(180, 20, 20, 20));
            var percentExp = (float)PlayerData.Exp / PlayerData.ExpToLevel;
            expBarUI.Items.Add(new UIRectangle(new Point(0, 0), new Size((int)(percentExp * UI.WIDTH), 2), Color.FromArgb(220, 255, 255, 0)));

            expBarUI.Draw();

            #region "RPG Style UI"
            var borderColor = Color.FromArgb(255, 75, 75, 75);
            Point rectanglePoint;
            Point textPoint;

            switch(PlayerData.Setup.SafeArea)
            {
                case 0:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 63 : 63), UI.HEIGHT - 47);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 63 : 63), UI.HEIGHT - 48);
                    break;
                case 1:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 57 : 57), UI.HEIGHT - 43);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 57 : 57), UI.HEIGHT - 44);
                    break;
                case 2:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 51 : 50), UI.HEIGHT - 40); //
                    textPoint = new Point((RPGInfo.IsWideScreen ? 51 : 51), UI.HEIGHT - 41); //
                    break;
                case 3:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 45 : 45), UI.HEIGHT - 36);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 45 : 45), UI.HEIGHT - 37);
                    break;
                case 4:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 39 : 39), UI.HEIGHT - 33);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 39 : 39), UI.HEIGHT - 34);
                    break;
                case 5:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 32 : 32), UI.HEIGHT - 29);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 32 : 32), UI.HEIGHT - 30);
                    break;
                case 6:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 26 : 26), UI.HEIGHT - 26);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 26 : 26), UI.HEIGHT - 27);
                    break;
                case 7:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 19 : 19), UI.HEIGHT - 22);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 19 : 19), UI.HEIGHT - 23);
                    break;
                case 8:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 13 : 13), UI.HEIGHT - 18);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 13 : 13), UI.HEIGHT - 19);
                    break;
                case 9:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 6 : 6), UI.HEIGHT - 15);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 6 : 6), UI.HEIGHT - 16);
                    break;
                case 10:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 0 : 0), UI.HEIGHT - 10);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 0 : 0), (RPGInfo.IsWideScreen ? UI.HEIGHT - 11 : UI.HEIGHT - 12));
                    break;
                default:
                    rectanglePoint = new Point((RPGInfo.IsWideScreen ? 0 : 0), UI.HEIGHT - 10);
                    textPoint = new Point((RPGInfo.IsWideScreen ? 0 : 0), (RPGInfo.IsWideScreen ? UI.HEIGHT - 11 : UI.HEIGHT - 12));
                    break;
            }

            new UIRectangle(rectanglePoint, new Size(181, 10), borderColor).Draw(); //playerinfo
            new UIText(PlayerData.Name + " Level " + PlayerData.Level + " " + PlayerData.Class.ToString().Replace("_", " "), textPoint, 0.22f, Color.White, 0, false).Draw();


            var offset = ShowingSubtitle ? - 85 : 0;


            new UIRectangle(new Point(UI.WIDTH / 2 - 173, UI.HEIGHT - 45 - 28 + offset), new Size(345, 10), Color.FromArgb(60, 0, 0, 0)).Draw();
            new UIRectangle(new Point(UI.WIDTH / 2 - 173, UI.HEIGHT - 45 - 13 + offset), new Size(345, 10), Color.FromArgb(60, 0, 0, 0)).Draw();

            var hpText = string.Format("HP: {0}/{1}",Game.Player.Character.Health,Game.Player.Character.MaxHealth);
            var armorText = string.Format("Armor: {0}/{1}", Game.Player.Character.Armor, 100);
            var hp = ((float) Game.Player.Character.Health/Game.Player.Character.MaxHealth);
            var ap = ((float) Game.Player.Character.Armor/100);

            var hpColor = Color.FromArgb(120,33,149,34);
            if(hp < 0.2f)
            {
                hpColor = Color.FromArgb(120, 139, 0, 0);
            }
            else if(hp < 0.4f)
            {
                hpColor = Color.FromArgb(120, 255, 69, 0);
            }

            new UIRectangle(new Point(UI.WIDTH / 2 - 173, UI.HEIGHT - 45 - 28 + offset), new Size((int)(hp * 345), 10), hpColor).Draw();
            new UIRectangle(new Point(UI.WIDTH / 2 - 173, UI.HEIGHT - 45 - 13 + offset), new Size((int)(ap * 345), 10), Color.FromArgb(180, 30, 144, 255)).Draw();

            new UIText(hpText, new Point(UI.WIDTH / 2, UI.HEIGHT - 45 - 28 - 2 + offset), 0.22f, Color.White, 0, true).Draw();
            new UIText(armorText, new Point(UI.WIDTH / 2, UI.HEIGHT - 45 - 13 - 2 + offset), 0.22f, Color.White, 0, true).Draw();
            #endregion

            //Character window #2
            if (View.ActiveMenus > 0)
            {
                var charPanel = new UIContainer(new Point(UI.WIDTH - 300, View.MenuPosition.Y - 215), new Size(300, 200), Color.Gray);

                //todo: what should we draw here?

                //charPanel.Draw();
            }



            //Quest Tracker
            if(ShowQuestTracker)
            {
                GetQuestTracker().Draw();
            }

            //Skill bar
            if(ShowSkillBar)
            {
                var skillOffset = ShowingSubtitle ? -80 : 0;
                var skillBarUI = RPG.SkillHandler.GetSkillBar(skillOffset);
                skillBarUI.Draw();
            }

        }

        //todo: close shop/craft/contract menu if opened by an NPC and player is far away from NPC
        public void OpenDialog()
        {
            UpdateDialog();

            if (!IsOpen(DialogMenu))
                View.AddMenu(DialogMenu);
        }
        public void UpdateDialog(int selected = 0)
        {
            var dialogList = new List<IMenuItem>();

            var dialogs = CurrentDialog.Current.Responses.Where( r => r.ConditionsMet).ToList();

            for (int i = 0; i < dialogs.Count; i++)
            {
                var item = dialogs[i];
                dialogList.Add(new MenuButton((item.Text)));
            }

            foreach (var i in dialogList)
            {
                i.Parent = DialogMenu;
            }

            if (DialogMenu == null)
            {
                DialogMenu = new RPGDialogMenu(CurrentNpc.Name + ": " + CurrentDialog.Current.NpcText, SelectDialog, dialogList.ToArray());
                DialogMenu.ExtendWindowHeight = false;
            }
            else
            {
                DialogMenu.Caption = CurrentNpc.Name + ": " + CurrentDialog.Current.NpcText;
                DialogMenu.Items.Clear();
                DialogMenu.Items.AddRange(dialogList);
                DialogMenu.Initialize();
            }

            DialogMenu.Caption = CurrentNpc.Name + ": " + CurrentDialog.Current.NpcText;
            RPGUI.FormatMenu(DialogMenu);
            DialogMenu.HeaderHeight = 25;
            if (DialogMenu != null && IsOpen(DialogMenu))
            {
                DialogMenu.SelectedIndex = selected;
            }
        }
        private void SelectDialog(RPGDialogMenu obj)
        {
            var dialogs = CurrentDialog.Current.Responses.Where(r => r.ConditionsMet).ToList();
            var selected = obj.SelectedIndex;

            var selectedItem = dialogs[selected];

            if (selectedItem.Action != ResponseAction.None)
            {
                switch (selectedItem.Action)
                {
                    case ResponseAction.Vendor:
                        DialogEnd();
                        OpenShop();
                        break;
                    case ResponseAction.Return_To_Start:
                        CurrentDialog.Current = CurrentDialog.StartingDialog;
                        break;
                    case ResponseAction.Craft:
                        DialogEnd();
                        OpenCrafting();
                        break;
                    case ResponseAction.Contract:
                        DialogEnd();
                        GetRandomContract();
                        break;
                    case ResponseAction.Start_Quest:
                        var quest = RPG.PlayerData.Quests.First(q => q.Name == selectedItem.Paramater);
                        if(!quest.InProgress && !quest.Done)
                        {
                            quest.Start();
                        }
                        DialogEnd();
                        break;
                    case ResponseAction.Finish_Quest:
                        var qu = RPG.PlayerData.Quests.First(q => q.Name == selectedItem.Paramater);
                        if (qu.ConditionsComplete)
                        {
                            qu.Complete();
                        }
                        CurrentDialog.Current = CurrentDialog.StartingDialog;
                        break;
                    case ResponseAction.Custom_End:
                        selectedItem.CustomAction.Invoke();
                        DialogEnd();
                        break;
                    case ResponseAction.End:
                        DialogEnd();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                CurrentDialog.Current = CurrentDialog.Dialogs.First(d => d.Id == selectedItem.DialogId);
            }

            UpdateDialog();
        }

        private UIContainer GetQuestTracker()
        {
            var quests = PlayerData.Quests.Where(q => q.InProgress).ToList();
            if(!quests.Any()) return new UIContainer();

            //350 IS MAP height
            Point questTrackerPoint;
            switch (PlayerData.Setup.SafeArea)
            {
                case 0:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 63 : 63), 10);
                    break;
                case 1:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 57 : 57), 15);
                    break;
                case 2:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 51 : 50), 20);
                    break;
                case 3:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 45 : 45), 25);
                    break;
                case 4:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 39 : 39), 30);
                    break;
                case 5:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 32 : 32), 35);
                    break;
                case 6:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 26 : 26), 40);
                    break;
                case 7:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 19 : 19), 45);
                    break;
                case 8:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 13 : 13), 50);
                    break;
                case 9:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 6 : 6), 55);
                    break;
                case 10:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 0 : 0), 60);
                    break;
                default:
                    questTrackerPoint = new Point((RPGInfo.IsWideScreen ? 0 : 0), 65);
                    break;
            }

            var questTracker = new UIContainer(questTrackerPoint, new Size(180, UI.HEIGHT - 190)); //note: -180 for non-RPG-UI

            var bottomPoint = (questTracker.Size.Height);
            for (int i = quests.Count -1; i >= 0; i--)
            {
                var quest = quests[i];
                var text = quest.GetProgressString();
                var formattedText = RPGUI.FormatText(text, 80);
                var lines = formattedText.Length;
                var height = 15 + (lines * 10) + 2;
                bottomPoint = bottomPoint - height;

                var point = new Point(5,bottomPoint);
                questTracker.Items.Add(new UIRectangle(point, new Size(170, 15), Color.FromArgb(120, 255, 255, 255)));
                questTracker.Items.Add(new UIText(quest.Name, new Point(point.X + 1, point.Y),0.25f,Color.FromArgb(230,8,8,8),0,false));
                questTracker.Items.Add(new UIText((i+1).ToString("00"), new Point(point.X + 170 - 14, point.Y + 1),0.25f,Color.FromArgb(230,78,78,78),Font.Monospace, false));
                for (int j = 0; j < lines; j++) 
                {
                    questTracker.Items.Add(new UIText(formattedText[j], new Point(5 + point.X, point.Y + 15 + (j * 10)), 0.2f, Color.White, 0, false));
                }

                //questTracker.Items.Add(new UIText("- " + quest.GetProgressString(), new Point(point.X, point.Y + 15),0.2f,Color.White,0,false));
                //questTracker.Items.Add(new UIText("- Malvoro is upset with your last mission.\n- Kill Antonio\n- Speak to bob\n- Eat cakes\n- Dist: 5m", new Point(5 + point.X, point.Y + 15),0.2f,Color.White,0,false));

            }

            questTracker.Items.Add(new UIText("Missions", new Point(1, bottomPoint - 17), 0.28f, Color.White, 0, false));
            //questTracker.Items.Add(new UIRectangle(new Point(1, bottomPoint - 17), new Size(180 - 6, 15), Color.FromArgb(120, 150, 150, 150)));

            return questTracker;
        }

        public void OpenCharacterMenuAlt()
        {
            
        }
        public void OpenCharacterMenu()
        {
            if(!IsOpen(CharacterMenu))
             View.AddMenu(CharacterMenu);
        }

        public void OpenSkillBarMenu()
        {
            UpdateSkillBarMenu();

            if(!IsOpen(SkillbarMenu))
                View.AddMenu(SkillbarMenu);


        }
        private void UpdateSkillBarMenu()
        {
            var items = new IMenuItem[6];
            var skillSlots = RPG.PlayerData.SkillSlots;
            var entries = RPG.SkillHandler.GetEntriesFormatted();
            for (int i = 0; i < 5; i++)
            {
                var slot = skillSlots[i];
                if (!slot.IsEmpty)
                {
                    var indexOfCur = Array.IndexOf(entries, slot.Name);
                    items[i] = new MenuEnumScroller(slot.GetMenuKeyName() + ":", "Skill for " + slot.Key, RPG.SkillHandler.GetEntriesFormatted(), indexOfCur).WithEnumActions(ChangeAction, ClearAction);
                }
                else
                {
                    items[i] = new MenuEnumScroller(slot.GetMenuKeyName() + ":", "Skill for " + slot.Key, RPG.SkillHandler.GetEntriesFormatted()).WithEnumActions(ChangeAction, ClearAction);
                }

                
            }
            items[5] = new MenuButton("Back").WithActivate(View.PopMenu);

            SkillbarMenu = new RPGMenu("Set skills", new GTASprite("Commonmenu", "interaction_bgd", Color.LightBlue), items);
            RPGUI.FormatMenuWithFooter(SkillbarMenu);
        }
        private void ClearAction(int i)
        {
            var slot = SkillbarMenu.SelectedIndex;
            RPG.SkillHandler.Slots[slot].Clear();

            var menuEs = (MenuEnumScroller)SkillbarMenu.Items[slot];
            menuEs.Index = 0;

        }
        private void ChangeAction(int i)
        {
            var slot = SkillbarMenu.SelectedIndex;
            var entries = RPG.SkillHandler.GetEntries();
            var selectedEntry = entries[i];
            if(selectedEntry == "Empty")
            {
                RPG.SkillHandler.Slots[slot].Clear();
            }
            else if (selectedEntry.StartsWith("Item_"))
            {
                var itemName = selectedEntry.Replace("Item_", "");
                RPG.SkillHandler.Slots[slot].Set(itemName, false);
            }
            else
            {
                var skillName = selectedEntry.Replace("Skill_", "");
                RPG.SkillHandler.Slots[slot].Set(skillName, true);
            }
        }

        private void OpenOptionsMenu(int i = 0)
        {

            var toggleSkillbar = (MenuToggle)OptionsMenu.Items.First(m => m.Caption == "Toggle Skill Bar");
            var toggleQuestTracker = (MenuToggle)OptionsMenu.Items.First(m => m.Caption == "Toggle Quest Tracker");

            

            if(!IsOpen(OptionsMenu))
                View.AddMenu(OptionsMenu);

            toggleSkillbar.Value = ShowSkillBar;
            toggleQuestTracker.Value = ShowQuestTracker;

            OptionsMenu.SelectedIndex = i;
        }


        public void OpenSkillsMenu()
        {
            View.AddMenu(SkillTreeMenu);
        }

        public void OpenQuestLog()
        {
            UpdateQuestLog();

            if (!IsOpen(QuestLogMenu))
                View.AddMenu(QuestLogMenu);
        }
        public void UpdateQuestLog(int selected = 0)
        {
            var questList = new List<IMenuItem>();

            var storyQuests = RPG.PlayerData.Quests.Where(q => !q.IsContract && !q.IsRepeatable).ToList();
            var completion = (float) storyQuests.Count(s => s.Done)/storyQuests.Count;
            
            var quests = PlayerData.Quests.Where(s => s.InProgress).Concat(PlayerData.Quests.Where(c => !c.InProgress)).Where(c => !c.IsContract || c.InProgress).ToList();

            for (int i = 0; i < quests.Count; i++)
            {
                var item = quests[i];
                questList.Add(new MenuButton((item.InProgress ? "[In Progress] " : (item.Done ? "[Done] " : "[Not Started] ")) + item.Name, item.Description));
            }

            questList.Add(new MenuButton("Back"));

            foreach (var i in questList)
            {
                i.Parent = QuestLogMenu;
            }

            if (QuestLogMenu == null)
            {
                QuestLogMenu = new RPGListMenu("Quest Log", new GTASprite("CommonMenu", "interaction_bgd", Color.DarkOrange), UseQuestLogEntry, questList.ToArray());
            }
            else
            {
                QuestLogMenu.Caption = "Story Progress: " + completion.ToString("P0");
                QuestLogMenu.Items.Clear();
                QuestLogMenu.Items.AddRange(questList);
                QuestLogMenu.Initialize();
            }

            QuestLogMenu.Caption = "Story Progress: " + completion.ToString("P0");
            RPGUI.FormatMenuWithFooter(QuestLogMenu);
            QuestLogMenu.FooterHeight = 40;
            QuestLogMenu.HeaderHeight = 25;
            if (QuestLogMenu != null && IsOpen(QuestLogMenu))
            {
                QuestLogMenu.SelectedIndex = selected;
            }
        }
        private void UseQuestLogEntry(RPGListMenu obj)
        {
            var quests = PlayerData.Quests.Where(s => s.InProgress).Concat(PlayerData.Quests.Where(c => !c.InProgress)).Where(c => !c.IsContract || c.InProgress).ToList();
            var selected = obj.SelectedIndex;
            if (selected >= quests.Count)
            {
                View.PopMenu();
                return;
            }

            var selectedItem = quests[selected];

            if(selectedItem.InProgress)
            {
                var abandon = RPGMessageBox.Create("Would you like to abandon [" + selectedItem.Name + "] ?","Abandon Quest","Cancel", () => { selectedItem.Reset(); UpdateQuestLog(); }, () => { });
                RPGUI.FormatMenu(abandon);
                abandon.HeaderCentered = true;
                View.AddMenu(abandon);
            }
        }

        public void OpenInventory()
        {
            UpdateInventory();
            if (!IsOpen(CraftingMenu))
            {
                View.AddMenu(InventoryMenu);
            }
        }
        private void UpdateInventory(int selected = 0)
        {
            var inventoryList = new List<IMenuItem>();

            if (!PlayerData.Inventory.Any())
            {
                inventoryList.Add(new MenuButton("Empty"));
            }

            for (int i = 0; i < PlayerData.Inventory.Count; i++)
            {
                var item = PlayerData.Inventory[i];

                inventoryList.Add(new MenuButton(item.Quantity + "x\t" + item.Name, item.Description));
            }

            inventoryList.Add(new MenuButton("Back"));

            foreach(var i in inventoryList)
            {
                i.Parent = InventoryMenu;
            }

            if (InventoryMenu == null)
            {
                InventoryMenu = new RPGListMenu("Inventory", new GTASprite("CommonMenu", "interaction_bgd", Color.ForestGreen), UseInventoryItem, inventoryList.ToArray());
            }
            else
            {
                InventoryMenu.Caption = "GTA$" + PlayerData.Money.ToString("N0");
                InventoryMenu.Items.Clear();
                InventoryMenu.Items.AddRange(inventoryList);
                InventoryMenu.Initialize();
                RPGLog.Log("inv:" + InventoryMenu.Items.Count);
            }

            RPGUI.FormatMenuWithFooter(InventoryMenu);
            InventoryMenu.HeaderHeight = 25;
            InventoryMenu.Caption = "GTA$" + PlayerData.Money.ToString("N0");
            
            if (InventoryMenu != null && IsOpen(InventoryMenu))
            {
                InventoryMenu.SelectedIndex = selected;
            }
        }
        private void UseInventoryItem(RPGListMenu obj)
        {

            var selected = obj.SelectedIndex;
            if (selected >= PlayerData.Inventory.Count)
            {
                View.PopMenu();
                return;
            }

            if (!PlayerData.Inventory.Any()) return;


            var selectedItem = PlayerData.Inventory[selected];

            var used = RPGMethods.UseItem(selectedItem);

            if (selectedItem.Quantity <= 0)
            {
                selected--;
                if (selected < 0) selected = 0;
            }

            UpdateInventory(selected);
            InventoryMenu.SelectedIndex = selected;
            if(used) 
                RPG.Notify(Notification.Alert("Used : " + selectedItem.Name));
        }

        public void OpenShop()
        {
            UpdateShop();
            if (!IsOpen(ShopMenu))
                View.AddMenu(ShopMenu);
        }
        public void UpdateShop(int selected = 0)
        {
            var shopList = new List<IMenuItem>();
            
            var purchasableItems = ItemRepository.Items.Where(i => i.CanBuy).ToList();
            for (int i = 0; i < purchasableItems.Count; i++)
            {
                var item = purchasableItems[i];
                shopList.Add(new MenuButton(item.Quantity + "x\t" + item.Name,"[ GTA$" + item.Cost + " ]" + item.Description));
            }
            shopList.Add(new MenuButton("Back"));

            foreach(var i in shopList)
            {
                i.Parent = ShopMenu;
            }

            if (ShopMenu == null) 
            {
                ShopMenu = new RPGListMenu("Shop", new GTASprite("CommonMenu", "interaction_bgd", Color.ForestGreen), BuyShopItem, shopList.ToArray());
            }
            else
            {
                ShopMenu.Caption = "GTA$" + PlayerData.Money.ToString("N0");
                ShopMenu.Items.Clear();
                ShopMenu.Items.AddRange(shopList);
                ShopMenu.Initialize();
            }
            ShopMenu.Caption = "GTA$" + PlayerData.Money.ToString("N0");

            RPGUI.FormatMenuWithFooter(ShopMenu);
            ShopMenu.HeaderHeight = 25;

            if (ShopMenu != null && IsOpen(ShopMenu))
            {
                ShopMenu.SelectedIndex = selected;
            }
        }
        private void BuyShopItem(RPGListMenu obj)
        {
            var purchasableItems = ItemRepository.Items.Where(i => i.CanBuy).ToList();
            if (obj.SelectedIndex >= purchasableItems.Count)
            {
                View.PopMenu();
                return;
            }

            var itemToBuy = purchasableItems[obj.SelectedIndex];
            if(PlayerData.Money >= itemToBuy.Cost)
            {
                PlayerData.Money -= itemToBuy.Cost;
                var item = ItemRepository.Get(itemToBuy.Name);
                PlayerData.AddItem(item);
                RPG.Notify(Notification.Alert("Purchased: " + itemToBuy.Name));
                UpdateShop(obj.SelectedIndex);
            }
            else
            {
                RPG.Notify(Notification.Alert("Not enough money."));
            }
        }

        public void OpenCrafting()
        {
            UpdateCrafting();
            if (!IsOpen(CraftingMenu))
                View.AddMenu(CraftingMenu);
        }
        public void UpdateCrafting(int selected = 0)
        {
            var craftingList = new List<IMenuItem>();

            var craftableItems = ItemRepository.Items.Where(i => i.CanCraft).ToList();
            for (int i = 0; i < craftableItems.Count; i++)
            {
                var item = craftableItems[i];
                craftingList.Add(new MenuButton(item.Quantity + "x\t" + item.Name, item.GetCraftString()));
            }

            craftingList.Add(new MenuButton("Back"));

            foreach (var i in craftingList)
            {
                i.Parent = CraftingMenu;
            }

            if (CraftingMenu == null)
            {
                CraftingMenu = new RPGListMenu("Crafting", new GTASprite("CommonMenu", "interaction_bgd", Color.ForestGreen), CraftItem, craftingList.ToArray());   
            }
            else
            {
                CraftingMenu.Items.Clear();
                CraftingMenu.Items.AddRange(craftingList);
                CraftingMenu.Initialize();
            }

            RPGUI.FormatMenuWithFooter(CraftingMenu);
            CraftingMenu.FooterCentered = true;
            CraftingMenu.FooterHeight = 75;

            if (CraftingMenu != null && IsOpen(CraftingMenu))
            {
                CraftingMenu.SelectedIndex = selected;
            }
        }
        private void CraftItem(RPGListMenu obj)
        {
            var craftableItems = ItemRepository.Items.Where(i => i.CanCraft).ToList();

            if (obj.SelectedIndex >= craftableItems.Count)
            {
                View.PopMenu();
                return;
            }

            var itemToCraft = craftableItems[obj.SelectedIndex];

            //have items?
            var hasItems = true;
            foreach(var kvp in itemToCraft.CraftItems)
            {
                var inventoryItem = RPG.PlayerData.Inventory.FirstOrDefault(i => i.Name == kvp.Key);
                if(inventoryItem != null)
                {
                    if(inventoryItem.Quantity < kvp.Value)
                    {
                        hasItems = false;
                    }
                }
                else
                {
                    hasItems = false;
                }
            }
            
            if(hasItems)
            {
                foreach (var kvp in itemToCraft.CraftItems)
                {
                    var inventoryItem = RPG.PlayerData.Inventory.First(i => i.Name == kvp.Key);
                    inventoryItem.Quantity -= kvp.Value;
                    if(inventoryItem.Quantity <= 0)
                    {
                        PlayerData.Inventory.Remove(inventoryItem);
                    }
                }

                var item = ItemRepository.Get(itemToCraft.Name);
                PlayerData.AddItem(item);
                RPG.Notify(Notification.Alert("Crafted: " + itemToCraft.Name));
                UpdateCrafting(obj.SelectedIndex);            
            }
            else
            {
                RPG.Notify(Notification.Alert("You do not have the items."));
            }
        }

        public void ShowMenu()
        {
            if (!IsOpen(MainMenu))
            {
                View.CloseAllMenus();
                View.AddMenu(MainMenu);
            }
        }

        public bool IsOpen(MenuBase menu)
        {
            var items = (List<MenuBase>)typeof(Viewport).GetField("mMenuStack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(View);
            return items.Contains(menu);
        }
        
        public void CloseAll()
        {
            View.CloseAllMenus();
        }


        //Dialog
        public void StartDialog(NpcObject npcObject)
        {
            CurrentNpc = npcObject;
            CurrentDialog = npcObject.Dialog;
            CurrentDialog.Current = CurrentDialog.StartingDialog;
            Function.Call(Hash.DISPLAY_HUD, 0);
            Function.Call(Hash.DISPLAY_RADAR, 0);

            Game.Player.CanControlCharacter = false;

            NpcCamera.Position = CurrentNpc.Position + CurrentNpc.Ped.RightVector * 4f;
            var mid = (CurrentNpc.Position + Game.Player.Character.Position)/2;
            NpcCamera.PointAt(mid);
        }

        public void DialogEnd()
        {
            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            World.RenderingCamera = null;
            Game.Player.CanControlCharacter = true;
            View.RemoveMenu(DialogMenu);
            CurrentNpc = null;
            CurrentDialog = null;
        }
    }
}