//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using GTA;
//using GTA.Math;
//using GTA.Native;
//using LogicSpawn.GTARPG.Core.General;
//using LogicSpawn.GTARPG.Core.Objects;
//using LogicSpawn.GTARPG.Core.Scripts.Popups;
//using Menu = GTA.Menu;
//
//namespace LogicSpawn.GTARPG.Core
//{
//    class ModelViewerNew : UpdateScript
//    {
//        Menu CharSelectMenu;
//        Menu CarSelectMenu;
//        private int CurrentCharModel;
//        private int CurrentCarModel;
//        public static bool Enabled;
//        public static bool Initialised;
//        private Ped CharacterModel;
//        private Vehicle CarModel;
//        private Camera Camera;
//
//
//        protected override bool RunWhenGameIsNotLoaded { get { return true; } }
//
//        public ModelViewerNew()
//        {
//            CurrentCharModel = 0;
//            CurrentCarModel = 0;
//            View.MenuTransitions = true;
//            View.MenuPosition = new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 110);
//            //Instantiate our menu
//            CharSelectMenu = new Menu("Character Creation", new GTA.MenuItem[] {
//                        new MenuButton("Next Character", NextModel),
//                        new MenuButton("Previous Character", "", PreviousModel),
//                        new MenuButton("Reroll Character", "", RandomModel),
//                        new MenuButton("Switch To Car Select", "", SwitchBetweenModelAndCar),
//                        new MenuButton("Choose Name", "", ChooseName),
//                        new MenuButton("Finish", "", FinishCreation)
//            });
//            var colors = ((VehicleColor[])Enum.GetValues(typeof(VehicleColor))).Select(x => x.ToString()).ToArray();
//            CarSelectMenu = new Menu("Character Creation", new GTA.MenuItem[] {
//                        new MenuButton("Next Car", NextModel),
//                        new MenuButton("Previous Car", "", PreviousModel),
//                        new MenuButton("Random Car", "", RandomModel),
//                        new MenuButton("Switch To Model Select", "", SwitchBetweenModelAndCar),
//                        new MenuEnumScroller("Car Color","",SetColor,d=> { },colors), 
//                        new MenuButton("Number Plate",ChooseNumberPlate), 
//                        new MenuButton("Choose Name", "", ChooseName),
//                        new MenuButton("Finish", "", FinishCreation)
//            });
//            CharSelectMenu.Width = 200;
//
//            RPGUI.FormatMenu(CharSelectMenu);
//            RPGUI.FormatMenu(CarSelectMenu);
//        }
//
//        private void SetColor(int obj)
//        {
//            CurrentCarColor = obj;
//            var c = ((VehicleColor[])Enum.GetValues(typeof(VehicleColor)))[CurrentCarColor];
//            CarModel.PrimaryColor = c;
//            CarModel.SecondaryColor = c;
//        }
//
//        public static void RestartCharCreation()
//        {
//            state = CharCreationState.Character;
//            IntroState = CharIntroState.Init;
//
//            Initialised = false;
//            Enabled = true;
//        }
//
//        private void SwitchBetweenModelAndCar()
//        {
//            ClearPrevModel();
//            state = state == CharCreationState.Character ? CharCreationState.Car : CharCreationState.Character;
//            View.CloseAllMenus();
//            View.MenuPosition = state == CharCreationState.Character
//                ? new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 110)
//                : new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 150);
//
//            View.AddMenu(state == CharCreationState.Character ? CharSelectMenu : CarSelectMenu);
//        }
//
//        private void RandomModel()
//        {
//            ClearPrevModel();
//            if (state == CharCreationState.Character)
//                CurrentCharModel = Random.Range(0, Data.PedHash.Length);
//            else
//                CurrentCarModel = Random.Range(0, Data.VehicleHash.Length);
//        }
//
//        private void FinishCreation()
//        {
//            if (CharacterModel != null) CharacterModel.Delete();
//            if (CarModel != null) CarModel.Delete();
//
//            RPG.PlayerData = new PlayerData()
//            {
//                Name = PlayerName,
//                NumberPlate = CarPlate,
//                ModelHash = Data.PedHash[CurrentCharModel],
//                CarHash = Data.VehicleHash[CurrentCarModel],
//                CarColor = ((VehicleColor[])Enum.GetValues(typeof(VehicleColor)))[CurrentCarColor]
//            };
//            View.CloseAllMenus();
//            state = CharCreationState.Intro;
//        }
//
//        private void ChooseName()
//        {
//            String test = Game.GetUserInput(15); //new code
//            PlayerName = test;
//        }
//
//        private void ChooseNumberPlate()
//        {
//            String test = Game.GetUserInput(9); //new code
//            CarPlate = test;
//        }
//
//        private void RandomiseLook()
//        {
//            Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, CharacterModel.Handle);
//        }
//
//        private void NextModel()
//        {
//            ClearPrevModel();
//
//            if (state == CharCreationState.Character)
//            {
//                CurrentCharModel++;
//                CurrentCharModel = (Data.PedHash.Length - 1) >= CurrentCharModel ? CurrentCharModel : 0;
//            }
//            else
//            {
//                CurrentCarModel++;
//                CurrentCarModel = (Data.VehicleHash.Length - 1) >= CurrentCarModel ? CurrentCarModel : 0;
//            }
//        }
//
//        private void PreviousModel()
//        {
//            ClearPrevModel();
//
//            if (state == CharCreationState.Character)
//            {
//                CurrentCharModel--;
//                CurrentCharModel = CurrentCharModel < 0 ? Data.PedHash.Length - 1 : CurrentCharModel;
//            }
//            else
//            {
//                CurrentCarModel--;
//                CurrentCarModel = CurrentCarModel < 0 ? Data.VehicleHash.Length - 1 : CurrentCarModel;
//            }
//        }
//
//        private void ClearPrevModel()
//        {
//            if (state == CharCreationState.Character)
//            {
//                if (CharacterModel != null)
//                {
//                    CharacterModel.Delete();
//                }
//
//                CharacterModel = null;
//            }
//            else
//            {
//                if (CarModel != null)
//                {
//                    CarModel.Delete();
//                }
//
//                CarModel = null;
//            }
//        }
//
//        private void Init()
//        {
//            Game.FadeScreenOut(1000);
//            Wait(1000);
//
//            while (RPG.WorldData.AllObjects.Any())
//            {
//                RPG.WorldData.AllObjects.First().Destroy();
//            }
//
//            RPG.UIHandler.View.CloseAllMenus();
//
//            Function.Call(Hash.DISPLAY_HUD, 0);
//            Function.Call(Hash.DISPLAY_RADAR, 0);
//
//
//            LoadingCharCreate = true;
//
//            Game.FadeScreenIn(100);
//            View.MenuTransitions = true;
//            View.AddMenu(CharSelectMenu);
//
//            PlayerName = "Player";
//            CarPlate = "PLAYER01";
//
//            if (Camera != null) Camera.Destroy();
//            Game.Player.Character.Position = new Vector3(_playerPosition.X - 20, _playerPosition.Y, _playerPosition.Z);
//            Wait(10000);
//            state = CharCreationState.Character;
//
//            LoadingCharCreate = false;
//
//            Game.FadeScreenIn(1000);
//            var p = _playerPosition;
//            Camera = World.CreateCamera(_camPosition, new Vector3(0, 0, 30), GameplayCamera.FOV);
//            World.RenderingCamera = Camera;
//            Initialised = true;
//        }
//
//        public override void Update()
//        {
//
//            if (!Enabled) return;
//            if (!Initialised) Init();
//
//            Game.Player.CanControlCharacter = false;
//
//            switch (state)
//            {
//                case CharCreationState.Character:
//                    CharacterSelection();
//                    break;
//                case CharCreationState.Car:
//                    CarSelection();
//                    break;
//                case CharCreationState.Intro:
//                    IntroCutscene();
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }
//
//        private void IntroCutscene()
//        {
//            IntroState = CharIntroState.Init;
//            //Fade out
//            Game.FadeScreenOut(2000);
//            Wait(2000);
//
//            World.CurrentDayTime = new TimeSpan(12, 30, 0);
//
//            Wait(1000);
//
//            //Init
//            if (Game.Player.Character.IsInVehicle())
//            {
//                Game.Player.Character.Task.WarpOutOfVehicle(Game.Player.Character.CurrentVehicle);
//            }
//
//            Game.Player.Character.Position = _playerPosition;
//
//            World.RenderingCamera = null;
//            Function.Call(Hash.DISPLAY_HUD, 0);
//            Function.Call(Hash.DISPLAY_RADAR, 0);
//            Function.Call(Hash.SET_PLAYER_INVINCIBLE, Game.Player.Handle, true);
//
//            World.Weather = Weather.Christmas;
//
//            Model m = RPG.PlayerData.ModelHash;
//            m.Request(1000);
//            Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, m.Hash);
//            Game.Player.Character.Position = new Vector3(_playerPosition.X, _playerPosition.Y, _playerPosition.Z + 200);
//            Wait(500);
//            Game.FadeScreenIn(2000);
//            Wait(3000);
//            Function.Call(Hash.SET_TIME_SCALE, 0.5f);
//            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "phone_cam4");
//            Game.PlayMusic("PROLOGUE_TEST_MISSION_START");
//
//            //A
//            IntroState = CharIntroState.A;
//
//            while (Game.Player.Character.HeightAboveGround > 80.0F)
//                Yield();
//
//            Game.FadeScreenOut(500);
//            Wait(500);
//
//            //B            
//            Game.PlayMusic("RAMPAGE_1_OS");
//            IntroState = CharIntroState.B;
//            World.Weather = Weather.ExtraSunny;
//            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
//            Game.Player.Character.Position = new Vector3(_playerPosition.X, _playerPosition.Y, _playerPosition.Z - 0.25f);
//            SetCam(Game.Player.Character.Position + new Vector3(0, 50, 25));
//            Game.FadeScreenIn(500);
//            Game.Player.CanControlCharacter = true;
//            Wait(5000);
//
//            //C
//            IntroState = CharIntroState.C;
//            SetCam(Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f + new Vector3(-0.8f, 0, 0.8f));
//            Wait(5000);
//
//            //D
//            IntroState = CharIntroState.D;
//            SetCam(Game.Player.Character.Position + new Vector3(25, 80, 25));
//
//            Wait(5000);
//
//            //E
//            IntroState = CharIntroState.E;
//            Game.PlayMusic("PROLOGUE_TEST_MISSION_END");
//            SetCam(Game.Player.Character.Position + new Vector3(75, 25, 100));
//            Wait(7500);
//
//            //Finished
//            IntroState = CharIntroState.Finished;
//            Game.FadeScreenOut(500);
//            Wait(500);
//            World.RenderingCamera = null;
//            Function.Call(Hash.DISPLAY_HUD, 1);
//            Function.Call(Hash.DISPLAY_RADAR, 1);
//            Function.Call(Hash.SET_PLAYER_INVINCIBLE, Game.Player.Handle, false);
//            World.Weather = Weather.ExtraSunny;
//            Function.Call(Hash.SET_TIME_SCALE, 1.0f);
//            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
//
//
//            //Move Player
//            Game.Player.Character.Position = _playerPosition;
//            Game.Player.Character.Heading = _playerHeading;
//
//            //Give and spawn stuff::
//            //Give Weps
//            for (int i = 0; i < Data.WeaponHash.Length; i++)
//            {
//                var wepName = Data.WeaponHash[i];
//                RPG.PlayerData.Weapons.Add(new WeaponDefinition()
//                {
//                    WeaponHash = wepName,
//                    AmmoCount = 10
//                });
//                Game.Player.Character.Weapons.Give(wepName, 10, false, false);
//            }
//
//            RPG.SaveAllData();
//
//            //Give personal vehicle
//            RPG.InitCharacter();
//
//            Game.FadeScreenIn(500);
//
//            RPG.GameLoaded = true;
//            Enabled = false;
//
//            RPG.GameHandler.Init();
//            RPG.GetPopup<HelpBox>().Show();
//        }
//
//        private void SetCam(Vector3 pos, bool lookAtPlayer = false)
//        {
//            if (Camera != null) Camera.Destroy();
//
//            Camera = World.CreateCamera(pos, Vector3.Zero, GameplayCamera.FOV);
//            if (lookAtPlayer)
//            {
//                Camera.PointAt(Game.Player.Character);
//            }
//            World.RenderingCamera = Camera;
//        }
//
//        private void CarSelection()
//        {
//            var p = new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 225);
//            new UIRectangle(new Point(p.X, p.Y + 25), new Size(300, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//            new UIText("Name: " + PlayerName, new Point(p.X + 150, p.Y + 25), 0.25f, Color.White, 0, true).Draw();
//            var name = "License Plate:" + CarPlate;
//
//            new UIRectangle(new Point(p.X, p.Y + 50), new Size(300, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//            new UIText(name, new Point(p.X + 150, p.Y + 50), 0.25f, Color.White, 0, true).Draw();
//
//
//            if (CarModel == null)
//            {
//                CarModel = World.CreateVehicle(new Model(Data.VehicleHash[CurrentCarModel]), _carPosition, _carHeading);
//                Camera.PointAt(CarModel);
//            }
//            else
//            {
//                CarModel.FreezePosition = true;
//                CarModel.Rotation = new Vector3(CarModel.Rotation.X, CarModel.Rotation.Y, CarModel.Rotation.Z + 0.05f);
//            }
//        }
//
//        private void CharacterSelection()
//        {
//            var p = new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 160);
//            new UIRectangle(new Point(p.X, p.Y + 25), new Size(300, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//            new UIText("Name: " + PlayerName, new Point(p.X + 150, p.Y + 25), 0.25f, Color.White, 0, true).Draw();
//            //var name = Data.PedHash[CurrentCharModel].ToString();
//            //
//            //new UIRectangle(new Point(p.X, p.Y + 50), new Size(200, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//            //new UIText(name, new Point(p.X + 100, p.Y + 50), 0.25f, Color.White, 0, true).Draw();
//
//
//            if (CharacterModel == null)
//            {
//                Model m = Data.PedHash[CurrentCharModel];
//                CharacterModel = World.CreatePed(m, new Vector3(_playerPosition.X, _playerPosition.Y, _playerPosition.Z - 0.25f), _playerHeading);
//                Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, CharacterModel.Handle);
//
//                Camera.PointAt(CharacterModel.Position + new Vector3(0, 0, 0.2f));
//            }
//            else
//            {
//                CharacterModel.FreezePosition = true;
//            }
//        }
//    }
//}