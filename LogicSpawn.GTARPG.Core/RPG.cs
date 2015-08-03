using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.GameData;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core
{
    public static class RPG
    {
        public const string Version = "0.1.12";

        public static bool Loading;
        public static bool UsingController;
        public static bool GameLoaded;
        public static bool PlayerDead;
        public static bool CutsceneRunning;

        public static PlayerData PlayerData;
        public static WorldData WorldData;
        public static ScriptSettings Settings;

        public static GameHandler GameHandler;
        public static UIHandler UIHandler;
        public static CutsceneHandler CutsceneHandler;
        public static SubtitleHandler SubtitleHandler;
        public static SkillHandler SkillHandler;
        public static AudioHandler Audio;
        private static Notifier _notifier;
        public static bool LoadedSuccessfully = true;
        
        public static bool ExplosiveHits;
        public static bool SuperJump;
        public static GameMode GameMode;


        static RPG()
        {
            PlayerData = new PlayerData();
            WorldData = new WorldData();
            SkillHandler = new SkillHandler();
            Settings = ScriptSettings.Load("scripts\\GTARPG\\config.ini");
            InitSettings();
            GameMode = GameMode.NotPlaying;
        }

        private static void InitSettings()
        {
            RPGSettings.AudioVolume = Settings.GetValue("Options", "AudioVolume", 35);
            RPGSettings.PlayKillstreaks = Settings.GetValue("Options", "PlayKillAnnouncements", true);
            RPGSettings.ShowKillstreaks = Settings.GetValue("Options", "ShowKillAnnouncements", true);
            RPGSettings.ShowPrerequisiteWarning = Settings.GetValue("Options", "ShowPrerequisiteWarning", true);
            RPGSettings.ShowPressYToStart = Settings.GetValue("Options", "ShowPressYToStart", true);
            RPGSettings.EnableAutoSave = Settings.GetValue("Options", "EnableAutoSave", true);
            RPGSettings.AutosaveInterval = Settings.GetValue("Options", "AutosaveIntervalSeconds", 30);
            RPGSettings.AutostartRPGMode = Settings.GetValue("Options", "AutostartRPGMode", true);
            RPGSettings.ShowQuestTracker = Settings.GetValue("Options", "ShowQuestTracker", true);
            RPGSettings.ShowSkillBar = Settings.GetValue("Options", "ShowSkillBar", true);
            RPGSettings.SafeArea = Settings.GetValue("Options", "SafeArea", 10);
        }

        private static Notifier Notifier
        {
            get { return _notifier ?? (_notifier = GetPopup<Notifier>()); }
        }

        public static void Notify(Notification notification)
        {
            Notifier.Notify(notification);
        }
        public static void Notify(string notification)
        {
            Notifier.Notify(Notification.Alert(notification));
        }

        public static void Subtitle(string text)
        {
            SubtitleHandler.Do(text,1000);
        }
        public static void Subtitle(string text, int duration)
        {
            SubtitleHandler.Do(text, duration);
        }

        public static void Initialise()
        {
            RPGLog.Log("Initialising RPG Mod.");
            RPG.GameMode = GameMode.FullRPG;
            Loading = true;
            Script.Wait(1);


            //Game.FadeScreenOut(500);
            Script.Wait(500);

            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            World.RenderingCamera = null;
            Function.Call(Hash.SET_TIME_SCALE, 1.0f);
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
            Game.Player.CanControlCharacter = true;
            
            //load data
            bool NeedToCreateCharacter;
            LoadAllData(out NeedToCreateCharacter);

            if(NeedToCreateCharacter) RPGLog.Log("Character not found. Will be starting character creation.");


            if(!LoadedSuccessfully)
            {
                RPGLog.Log("Failed to load game successfully.");
                UI.Notify("Failed loading RPGMod. See Error 000");
                Game.FadeScreenIn(500);
                Loading = false;
                RPGInit.Enabled = true;
                return;
            }

            if(NeedToCreateCharacter)
            {
                RPGLog.Log("Loaded successfully with no character found. Starting character creation.");

                CharCreationNew.Enabled = true;
            }
            else
            {
                RPGLog.Log("Loaded successfully with character found.");
                GameLoaded = true;    
            }

            //Game.FadeScreenIn(500);
            Loading = false;
        }

        public static void InitCharacter()
        {
            LoadTutorial();
            RPG.GameMode = GameMode.FullRPG;

            //Settings
            RPGLog.Log("Setting model:");
            Model m = PlayerData.ModelHash;
            RPGLog.Log("Setting model to: " + m.Hash);
            m.Request(5000);
            if(m.IsValid && m.IsLoaded)
            {
                Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, m.Hash);
                Script.Wait(500);
            }
            
            //remember we can control max health/ /useful for skills later on
            RPGLog.Log("Setting player HP");
            Game.Player.Character.MaxHealth = 100;
            Game.Player.Character.Health = 100;

            RPGLog.Log("Init Cooldowns");
            var cooldowns = RPG.PlayerData.Inventory.Where(i => i.Usable).Select(i => i.CoolDownTimer)
                .Concat(RPG.PlayerData.Skills.Where(s => s.Unlocked).Select(s => s.CoolDownTimer));

            foreach (var cooldown in cooldowns)
            {
                cooldown.Current = cooldown.CoolDownMsTime;
            }
            RPGLog.Log("Loading Variations");

            RPGMethods.LoadVariations();
            RPGMethods.LoadVariations();
            RPGMethods.LoadVariations();
            RPGMethods.LoadVariations();
            RPGMethods.LoadVariations();


            //Reload            
            RPGLog.Log("Reload quests if needed");
            foreach(var q in PlayerData.Quests.Where(q => q.InProgress))
            {
                q.OnReload();
            }

            RPGLog.Log("Loading Weapons");
            if (m.IsValid && m.IsLoaded && Game.Player.Character != null && Game.Player.Character.Exists())
            {
                RPGMethods.LoadPlayerWeapons();
            }
        }

        private static void LoadTutorial()
        {
            if(!PlayerData.Tutorial.PressJToOpenMenu)
            {
                GetPopup<TutorialBox>().Show("A new quest has started and has been added to your tracker.", "Press J to open up your menu.");
            }
            else if(!PlayerData.Tutorial.BoughtAmmoFromShop)
            {
                GetPopup<TutorialBox>().Pop("Through here you can find everything you need.", "Select Actions > Purchase Goods > Ammo Pack I to purchase an ammo pack.");
            }
            else if(!PlayerData.Tutorial.GetAKill)
            {
                GetPopup<TutorialBox>().Pop("Getting kills and completing missions are just two ways to earn XP and Skill Points", "Get a kill.");

            }
            else if(!PlayerData.Tutorial.UnlockSkillWithSp)
            {
                GetPopup<TutorialBox>().Pop("Hope you haven't attracted the cops. If so, lose them. Time to unlock some skills.", "Access the menu > Character Menu > Skills. Unlock your first skill.");

            }
            else if(!PlayerData.Tutorial.UsingSkills)
            {
                GetPopup<TutorialBox>().Pop("Skills can get you the edge in your conflicts. You can further increase skills by 'activating' them in the menu once unlocked.", "Press [Caps-Lock] to use your unlocked skill.");

            }
            else if(!PlayerData.Tutorial.SpawnVehicle)
            {
                GetPopup<TutorialBox>().Pop("You're ready to begin, but you'll need to learn to speak in a new way.", "Press K to spawn your vehicle.");

            }
            else if(!PlayerData.Tutorial.TutorialDoneExceptSpeak)
            {
                GetPopup<TutorialBox>().Pop("Quest hand in locations are marked by the white star blip on the map.", "Drive to Matthew (Star) and press E to speak with him and finish your quest.");
            }
            else if(!PlayerData.Tutorial.SpokeToNpc)
            {
                GetPopup<TutorialBox>().Pop("Quest hand in locations are marked by the white star blip on the map.", "Drive to Matthew (Star) and press E to speak with him and finish your quest.");
            }
            else if(!PlayerData.Tutorial.PurchasedContract)
            {
                GetPopup<TutorialBox>().Pop("Contracts are an easy way to earn exp and loot outside of missions.", "Access the menu > Actions > Get Random Contract to get a new contract.");
            }
            else if (!PlayerData.Tutorial.LearntAboutIcons)
            {
                GetPopup<TutorialBox>().Pop("The information blip is for available quests and speech bubbles for interaction.", "The star blip is for handing in quests. Hand in the quest 'The Grind Begins'");
            }
            else if (!PlayerData.Tutorial.LearntAboutCrafting)
            {
                var vehKit = PlayerData.Inventory.FirstOrDefault(i => i.Name == "Vehicle Parts");
                if (vehKit == null || vehKit.Quantity < 3)
                {
                    PlayerData.AddItem("Vehicle Parts",9);
                }
                GetPopup<TutorialBox>().Pop("Crafting can help you get items you need.", "Access the menu > Actions > Craft Items and craft a 'Vehicle Repair Kit'");
            }
            else if (!PlayerData.Tutorial.LearntAboutSkillbar)
            {
                GetPopup<TutorialBox>().Pop("You can use items directly from the inventory or bind them to your skillbar.", "Access the menu > Character Menu > Set SkillBar and bind 'Health Kit' to T");
            }
        }

        public static void LoadAllData()
        {
            bool needCharacter;
            LoadAllData(out needCharacter);
        }


        public static void LoadAllData(out bool NeedToCreateCharacter)
        {

            RPGMethods.CleanupObjects();
            NeedToCreateCharacter = false;
            var newDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(newDir, @"Rockstar Games\GTA V\RPGMod\");
            var playerDataFile = "PlayerData.save";

            var playerDataPath = Path.Combine(dir, playerDataFile);

            if (File.Exists(playerDataPath))
            {
                try
                {
                    var loadedData = File.ReadAllText(playerDataPath);
                    PlayerData = JsonConvert.DeserializeObject<PlayerData>(loadedData, GM.GetSerialisationSettings());
                    InitCharacter();
                }
                catch (Exception e)
                {
                    LoadedSuccessfully = false;
                    RPGLog.Log(e.ToString());
                    RPGLog.Log("Error Loading or Initialising Player data.");
                }
            }
            else
            {
                NeedToCreateCharacter = true;
            }

            var dataVersion = PlayerData.Version;
            if(dataVersion != RPG.Version)
            {
                VersionMigration.Migrate(dataVersion, RPG.Version);
            }

            ApplySettings();
        }

        private static void ApplySettings()
        {
            //apply script settings
        }

        public static void SaveAllData()
        {
            if (RPG.PlayerData.CarHash == 0) return;

            var newDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var dir = Path.Combine(newDir, @"Rockstar Games\GTA V\RPGMod\");
            var playerDataFile = "PlayerData.save";

            var playerDataPath = Path.Combine(dir, playerDataFile);

            //Update values
            for (int i = 0; i < PlayerData.Weapons.Count; i++)
            {
                var wepDefinition = PlayerData.Weapons[i];
                var w = Game.Player.Character.Weapons;
                var x = w[wepDefinition.WeaponHash];
                if( x != null)
                {
                    wepDefinition.AmmoCount = x.Ammo;
                }
            }

            PlayerData.SkillSlots = SkillHandler.Slots;
            PlayerData.Version = Version;
            Directory.CreateDirectory(dir);

            using (var stringwriter = new StreamWriter(playerDataPath, false))
            {
                var saveFile = JsonConvert.SerializeObject(PlayerData, GM.GetSerialisationSettings());
                stringwriter.Write(saveFile);
            }

            Settings.Save();
        }

        public static T GetPopup<T>() where T : class
        {
            var x = Popups.All.FirstOrDefault(p => p is T);
            var r = x == null ? null : x as T;
            if(r == null) RPGLog.Log("Could not find popup");
            return r;
        }


        public static class Popups
        {
            public static List<Popup> All = new List<Popup>();

            public static void Register(Popup popup)
            {
                All.Add(popup);
            }
        }
    }

    public enum GameMode
    {
        FullRPG,
        PlayingAsTrio,
        NotPlaying
    }
}
