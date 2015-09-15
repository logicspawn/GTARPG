using System;
using System.Collections.Generic;
using System.Drawing;
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
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using Control = GTA.Control;
using Font = GTA.Font;
using Menu = GTA.Menu;

namespace LogicSpawn.GTARPG.Core
{
    class CharCreationNew : UpdateScript
    {
        public enum CharIntroState
        {
            A,
            B,
            C,
            D,
            E,
            Init,
            Finished
        }

        public enum CharCreationState
        {
            PickMotives =0,
            PickClass = 1,
            GenderSelect = 2,
            Character = 3,
            Car = 4,
            Finalise = 5,
            Intro,
        }

        public static CharCreationNew Instance;

        Menu CharSelectMenu;
        Menu CarSelectMenu;
        Menu FinaliseMenu;
        Menu SetupMenu;
        private int CurrentCharModel;
        private int CurrentCarModel;
        private int CurrentCarColor;
        private int CurrentCarSecondaryColor;
        public static bool Enabled;
        public static bool Initialised;
        public static bool OpenedSetup;
        public static bool SetupFinished;
        public static bool LoadingCharCreate;
        private string PlayerName = "";
        private int SafeArea = 10;
        private string CarPlate;
        private PlayerMotive Motive;
        private PlayerClass Class;
        private Gender Gender;
        private Dictionary<int, int> Variations;
        private Dictionary<int, int> CurVariations;

        private Ped CharacterModel;
        private Vehicle CarModel;
        private Camera Camera;
        public static CharCreationState State;
        public static CharIntroState IntroState;

        private bool _shownMotives;
        private bool _shownClass;
        private bool _shownGender;

        //Per section
        private readonly Vector3 _carPosition = new Vector3(-751,-72,41.2f);
        private readonly float _carHeading = 160.5672f;

        private readonly Vector3 _playerPosition = new Vector3(-755,-74,41);
        private readonly float _playerHeading = 237.6025f;

        //Final
        private readonly Vector3 _carPositionFinal = new Vector3(-751,-72,41.2f);
        private readonly float _carHeadingFinal = 160.5672f;

        private readonly Vector3 _playerPositionFinal = new Vector3(-751, -72, 41);
        private readonly float _playerHeadingFinal = 237.6025f;

        private readonly Vector3 _camPosition = new Vector3(-753.5f, -77.5f, 42.2f);

        private PedHash[] _malePeds { get { return Data.MalePeds; } }
        private PedHash[] _femalePeds { get { return Data.FemalePeds; } }

        private VehicleHash[] _availableCars;

        private PedHash[] AvailablePeds
        {
            get { return Gender == Gender.Male ? _malePeds : _femalePeds; }
        }

        private VehicleHash[] AvailableCars
        {
            get { return _availableCars ?? (_availableCars = GetVehicles()); }
        }

        public int SelectedColorGroup = 0;
        public int SelectedSecondaryColorGroup = 0;
        public string[] ColorGroups = new string[] { "All", "Metallic", "Matte", "Util", "Worn", "Red", "Blue", "Orange", "Yellow", "Black", "Purple", "Brown", "Tan", "Green", "Gray", "Silver", "White", "Pink", "Olive" };
        private VehicleColor[] AvailableColors
        {
            get
            {
                var all = Data.VehicleColors;
                if (ColorGroups[SelectedColorGroup] == "All") return all;
                return all.Where(a => a.ToString().Contains(ColorGroups[SelectedColorGroup])).ToArray();
            }
        }
        private VehicleColor[] AvailableSecondaryColors
        {
            get
            {
                var all = Data.VehicleColors;
                return all.Where(a => a.ToString().Contains(ColorGroups[SelectedSecondaryColorGroup])).ToArray();
            }
        }

        private VehicleHash[] GetVehicles()
        {
            return Data.VehicleHashes;
        }


        protected override bool RunWhenGameIsNotLoaded { get { return true; } }

        public CharCreationNew()
        {
            KeyDown += OnKeyDown;
            Instance = this;
            CurrentCarModel = Random.Range(0, 50);
            CurrentCharModel = Random.Range(0, 50);
            View.MenuTransitions = false;
            View.MenuPosition = new Point(UI.WIDTH - 300, 0);
            View.MenuOffset = new Point(-300, 0);
            //Instantiate our menu
            State = CharCreationState.PickMotives;
            SetupMenu = new RPGMenu("RPG Setup", new GTASprite("commonmenu","interaction_bgd",Color.DodgerBlue), new IMenuItem[] {
                        new MenuNumericScroller("SafeArea Setting","Change the safearea setting until the RPG UI covers the bottom area of your minimap.",0,10,1,SafeArea).WithNumericActions(ChangeSafeArea,d => { }), 
                        new MenuButton("Finish Setup", "").WithActivate(() => { View.PopMenu(); SetupFinished = true;
                        })
            });
            CharSelectMenu = new RPGMenu("Character Creation", new GTASprite("commonmenu","interaction_bgd",Color.DodgerBlue), new IMenuItem[] {
                        //new MenuButton("Next Character", NextModel),
                        //new MenuButton("Previous Character", "", PreviousModel),
                        new MenuButton("Random Character", "").WithActivate(RandomModel),
                        new MenuEnumScroller("Character Model","",AvailablePeds.Select(x => x.ToString()).ToArray()).WithEnumActions(SetCharModel,d=> { }), 
                        //new MenuButton("Try Out New Clothes", "", RandomiseLook),
                        //new MenuButton("Switch To Car Select", "", SwitchBetweenModelAndCar),
                        //new MenuButton("Choose Name", "", ChooseName),
                        new MenuButton("Continue", "").WithActivate(() => { View.PopMenu(); State = CharCreationState.Car;})
            });
            var colors = ((VehicleColor[]) Enum.GetValues(typeof (VehicleColor))).Select(x => x.ToString()).ToArray();
            CarSelectMenu = new RPGMenu("Vehicle Selection", new GTASprite("commonmenu", "interaction_bgd", Color.Red), new IMenuItem[] {
                        //new MenuButton("Next Car", NextModel),
                        //new MenuButton("Previous Car", "", PreviousModel),
                        new MenuButton("Random Vehicle", "").WithActivate(RandomModel),
                        new MenuEnumScroller("Vehicle","",AvailableCars.Select(x => x.ToString()).ToArray()).WithEnumActions(SetCarModel,d=> { }), 
                        //new MenuButton("Switch To Model Select", "", SwitchBetweenModelAndCar),
                        new MenuEnumScroller("Primary Color Group","",ColorGroups).WithEnumActions(SetPrimaryColorGroup,d=> { }), 
                        new MenuEnumScroller("Primary Color","",AvailableColors.Select(x => x.ToString()).ToArray()).WithEnumActions(SetColor,d=> { }), 
                        new MenuEnumScroller("Secondary Color Group","",ColorGroups).WithEnumActions(SetSecondaryColorGroup,d=> { }), 
                        new MenuEnumScroller("Secondary Color","",AvailableSecondaryColors.Select(x => x.ToString()).ToArray()).WithEnumActions(SetSecondaryColor,d=> { }), 
                        new MenuButton("Number Plate").WithActivate(ChooseNumberPlate), 
                        new MenuButton("Continue", "").WithActivate(() => {View.PopMenu(); State = CharCreationState.Finalise;})
            });

            FinaliseMenu = new RPGMenu("Finish", new GTASprite("commonmenu", "interaction_bgd", Color.SkyBlue), new IMenuItem[] {
                        new MenuButton("Choose Name", "").WithActivate(ChooseName),
                        new MenuButton("", ""),
                        new MenuButton("Continue", "").WithActivate(() => { if(PlayerName != "") FinishCreation();})
            });
            CharSelectMenu.Width = 200;

            RPGUI.FormatMenuWithFooter(SetupMenu);
            RPGUI.FormatMenu(CharSelectMenu);
            RPGUI.FormatMenu(CarSelectMenu);
            RPGUI.FormatMenu(FinaliseMenu);
            SetupMenu.HeaderHeight = 25;
            CharSelectMenu.HeaderHeight = 25;
            CarSelectMenu.HeaderHeight = 25;
            FinaliseMenu.HeaderHeight = 25; 
        }

        private void SetCarModel(int obj)
        {
            ClearPrevModel();
            CurrentCarModel = obj;
            CarSelectMenu.SelectedIndex = 1;
        }
        private void SetCharModel(int obj)
        {
            ClearPrevModel();
            CurrentCharModel = obj;
            CharSelectMenu.SelectedIndex = 1;
        }

        private void ChangeSafeArea(double obj)
        {
            RPGSettings.SafeArea = (int)obj;
            SafeArea = (int) obj;
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (!Enabled || RPGInfo.KeyboardActive) return;

            if (keyEventArgs.KeyCode == Keys.Up)
            {
                View.HandleChangeSelection(false);
            }
            if (keyEventArgs.KeyCode == Keys.Down)
            {
                View.HandleChangeSelection(true);
            }
            if (keyEventArgs.KeyCode == Keys.Left)
            {
                View.HandleChangeItem(false);
            }
            if (keyEventArgs.KeyCode == Keys.Right)
            {
                View.HandleChangeItem(true);
            }
            if (keyEventArgs.KeyCode == Keys.Enter)
            {
                if (State == CharCreationState.Finalise && FinaliseMenu.SelectedIndex == 0)
                {
                    FinaliseMenu.SelectedIndex = 1;
                    ChooseName();
                }
                else if (State == CharCreationState.Car && CarSelectMenu.SelectedIndex == 6)
                {
                    CarSelectMenu.SelectedIndex = 1;
                    ChooseNumberPlate();
                }
                else
                {
                    View.HandleActivate();
                }
            }
        }

        private void SetSecondaryColorGroup(int obj)
        {
            SelectedSecondaryColorGroup = obj;
            CurrentCarSecondaryColor = 0;
            var index = CarSelectMenu.SelectedIndex;
            View.RemoveMenu(CarSelectMenu);
            CarSelectMenu = new RPGMenu("Vehicle Selection", new GTASprite("commonmenu", "interaction_bgd", Color.Red), new IMenuItem[] {
                        new MenuButton("Random Vehicle", "").WithActivate(RandomModel),
                        new MenuEnumScroller("Vehicle","",AvailableCars.Select(x => x.ToString()).ToArray()).WithEnumActions(SetCarModel,d=> { }), 
                        new MenuEnumScroller("Primary Color Group","",ColorGroups,SelectedColorGroup).WithEnumActions(SetPrimaryColorGroup,d=> { }), 
                        new MenuEnumScroller("Primary Color","",AvailableColors.Select(x => x.ToString()).ToArray(),CurrentCarColor).WithEnumActions(SetColor,d=> { }), 
                        new MenuEnumScroller("Secondary Color Group","",ColorGroups,SelectedSecondaryColorGroup).WithEnumActions(SetSecondaryColorGroup,d=> { }), 
                        new MenuEnumScroller("Secondary Color","",AvailableSecondaryColors.Select(x => x.ToString()).ToArray()).WithEnumActions(SetSecondaryColor,d=> { }),
                        new MenuButton("Number Plate").WithActivate(ChooseNumberPlate), 
                        new MenuButton("Continue", "").WithActivate(() => {View.PopMenu();State = CharCreationState.Finalise;})
            });
            RPGUI.FormatMenu(CarSelectMenu);
            CarSelectMenu.HeaderHeight = 25;
            View.AddMenu(CarSelectMenu);

            CarSelectMenu.SelectedIndex = index;
            if (CarModel != null)
            {
                CarModel.PrimaryColor = AvailableColors[CurrentCarColor];
                CarModel.SecondaryColor = AvailableSecondaryColors[CurrentCarSecondaryColor];
                CarModel.NumberPlate = "";
                CarModel.NumberPlate = CarPlate;
            }
        }

        private void SetPrimaryColorGroup(int obj)
        {
            SelectedColorGroup = obj;
            CurrentCarColor = 0;
            var index = CarSelectMenu.SelectedIndex;
            View.RemoveMenu(CarSelectMenu);
            CarSelectMenu = new RPGMenu("Vehicle Selection", new GTASprite("commonmenu", "interaction_bgd", Color.Red), new IMenuItem[] {
                        new MenuButton("Random Vehicle", "").WithActivate(RandomModel),
                        new MenuEnumScroller("Vehicle","",AvailableCars.Select(x => x.ToString()).ToArray()).WithEnumActions(SetCarModel,d=> { }), 
                        new MenuEnumScroller("Primary Color Group","",ColorGroups,SelectedColorGroup).WithEnumActions(SetPrimaryColorGroup,d=> { }), 
                        new MenuEnumScroller("Primary Color","",AvailableColors.Select(x => x.ToString()).ToArray(),0).WithEnumActions(SetColor,d=> { }), 
                        new MenuEnumScroller("Secondary Color Group","",ColorGroups,SelectedSecondaryColorGroup).WithEnumActions(SetSecondaryColorGroup,d=> { }), 
                        new MenuEnumScroller("Secondary Color","",AvailableSecondaryColors.Select(x => x.ToString()).ToArray(),CurrentCarSecondaryColor).WithEnumActions(SetSecondaryColor,d=> { }), 
                        new MenuButton("Number Plate").WithActivate(ChooseNumberPlate), 
                        new MenuButton("Continue", "").WithActivate(() => {View.PopMenu(); State = CharCreationState.Finalise;})
            });
            RPGUI.FormatMenu(CarSelectMenu);
            CarSelectMenu.HeaderHeight = 25;
            View.AddMenu(CarSelectMenu);
            CarSelectMenu.SelectedIndex = index;

            if (CarModel != null)
            {
                CarModel.PrimaryColor = AvailableColors[CurrentCarColor];
                CarModel.SecondaryColor = AvailableSecondaryColors[CurrentCarSecondaryColor];
                CarModel.NumberPlate = "";
                CarModel.NumberPlate = CarPlate;
            }
        }

        private void SetSecondaryColor(int obj)
        {
            CurrentCarSecondaryColor = obj;
            var c = AvailableSecondaryColors[CurrentCarColor];
            if (CarModel != null)
                CarModel.SecondaryColor = c;
        }

        private void SetColor(int obj)
        {
            CurrentCarColor = obj;
            var c = AvailableColors[CurrentCarColor];
            if(CarModel != null){}
                CarModel.PrimaryColor = c;
        }

       public static void RestartCharCreation()
       {
           State = CharCreationState.PickMotives;
           IntroState = CharIntroState.Init;
           RPG.Audio.DisposeAll();
           Initialised = false;
           Enabled = true;
       }

        private void SwitchBetweenModelAndCar()
        {
            ClearPrevModel();
            State = State == CharCreationState.Character ? CharCreationState.Car : CharCreationState.Character;
            View.CloseAllMenus();
            View.MenuPosition = State == CharCreationState.Character 
                ? new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 110)
                : new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 150);

            View.AddMenu(State == CharCreationState.Character ? CharSelectMenu : CarSelectMenu);
        }

        private void RandomModel()
        {
            ClearPrevModel();
            if(State == CharCreationState.Character)
                CurrentCharModel = Random.Range(0, AvailablePeds.Length);
            else
            {
                CurrentCarModel = Random.Range(0, AvailableCars.Length);
                var c = (MenuEnumScroller)CarSelectMenu.Items.First(i => i.Caption == "Vehicle");
                c.Index = CurrentCarModel;
            }
                
        }

        private void FinishCreation()
        {
            if(CharacterModel != null) CharacterModel.Delete();
            if (CarModel != null) CarModel.Delete();

            RPG.PlayerData = new PlayerData
                                 {
                                     Name = PlayerName,
                                     NumberPlate = CarPlate,
                                     ModelHash = AvailablePeds[CurrentCharModel],
                                     CarHash = AvailableCars[CurrentCarModel],
                                     Class = Class,
                                     Motive = Motive,
                                     Gender = Gender,
                                     CarColor = AvailableColors[CurrentCarColor],
                                     CarSecondaryColor = AvailableSecondaryColors[CurrentCarSecondaryColor],
                                     ModelVariations = CurVariations, Setup = {SafeArea = SafeArea}
                                 };
            RPG.SaveAllData();
            View.CloseAllMenus();
            State = CharCreationState.Intro;
        }

        private void ChooseName()
        {
            RPGInfo.KeyboardActive = true;
            Script.Wait(1);
            String test = Game.GetUserInput(15); //new code
            RPGInfo.KeyboardActive = false;
            PlayerName = test;
        }

        private void ChooseNumberPlate()
        {
            RPGInfo.KeyboardActive = true;
            Script.Wait(1);
            String test = Game.GetUserInput(9); //new code
            RPGInfo.KeyboardActive = false;
            CarPlate = test;
            if(CarModel != null)
            {
                CarModel.NumberPlate = "";
                CarModel.NumberPlate = CarPlate;
            }
        }

        private void RandomiseLook()
        {
            Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, CharacterModel.Handle);
        }

        private void NextModel()
        {
            ClearPrevModel();

            if (State == CharCreationState.Character)
            {
                CurrentCharModel++;
                CurrentCharModel = (AvailablePeds.Length - 1) >= CurrentCharModel ? CurrentCharModel : 0;
            }
            else
            {
                CurrentCarModel++;
                CurrentCarModel = (AvailableCars.Length - 1) >= CurrentCarModel ? CurrentCarModel : 0;
            }
        }

        private void PreviousModel()
        {
            ClearPrevModel();

            if (State == CharCreationState.Character)
            {
                CurrentCharModel--;
                CurrentCharModel = CurrentCharModel < 0 ? AvailablePeds.Length - 1 : CurrentCharModel;
            }
            else
            {
                CurrentCarModel--;
                CurrentCarModel = CurrentCarModel < 0 ? AvailableCars.Length - 1 : CurrentCarModel;
            }
        }

        private void ClearPrevModel()
        {
            if (State == CharCreationState.Character)
            {
                if (CharacterModel != null)
                {
                    CharacterModel.Delete();
                }

                CharacterModel = null;
            }
            else
            {
                if (CarModel != null)
                {
                    CarModel.Delete();
                }

                CarModel = null;
            }
        }

        private void InitSmall()
        {
            _shownMotives = false;
            _shownGender = false;
            _shownClass = false;
            RPG.GetPopup<TutorialBox>().Hide();
            RPGSettings.SafeArea = SafeArea;
            RPGMethods.CleanupObjects();

            if (RPG.UIHandler != null && RPG.UIHandler.View != null)
                RPG.UIHandler.View.CloseAllMenus();
        }

        private void Init()
        {
            Game.FadeScreenOut(1000);
            Wait(1000);

            _shownMotives = false;
            _shownGender = false;
            _shownClass = false;

            RPGMethods.CleanupObjects();

            if (RPG.UIHandler != null && RPG.UIHandler.View != null)
                RPG.UIHandler.View.CloseAllMenus();

            Function.Call(Hash.DISPLAY_HUD, 0);
            Function.Call(Hash.DISPLAY_RADAR, 0);
            
            LoadingCharCreate = true;

            Game.FadeScreenIn(100);

            PlayerName = "Player";
            CarPlate = "PLAYER01";

            if(Camera != null) Camera.Destroy();
            Game.Player.Character.Position = new Vector3(_playerPosition.X - 20, _playerPosition.Y, _playerPosition.Z);
            Wait(10000);
            State = CharCreationState.PickMotives;

            LoadingCharCreate = false;

            Game.FadeScreenIn(1000);
            Camera = World.CreateCamera(_camPosition, new Vector3(0,0,30), GameplayCamera.FieldOfView);
            Camera.MotionBlurStrength = 0.4f;
            World.RenderingCamera = Camera;
            RPG.GameHandler.InitiateNpcs = false;
            Initialised = true;

        }

        public override void Update()
        {
            
            if (!Enabled) return;

            var up = Game.IsControlJustPressed(0, Control.ScriptPadUp);
            var down = Game.IsControlJustPressed(0, Control.ScriptPadDown);
            var left = Game.IsControlJustPressed(0, Control.ScriptPadLeft);
            var rightpress = Game.IsControlJustPressed(0, Control.ScriptPadRight);
            var back = Game.IsControlJustPressed(0, Control.Reload);
            var interact = Game.IsControlJustPressed(0, Control.Sprint);

            if (left)
            {
                View.HandleChangeItem(false);
            }
            if (rightpress)
            {
                View.HandleChangeItem(true);
            }
            if (up)
            {
                View.HandleChangeSelection(false);
            }
            if (down)
            {
                View.HandleChangeSelection(true);
            }
            if (interact)
            {
                if (State == CharCreationState.Finalise && FinaliseMenu.SelectedIndex == 0)
                {
                    FinaliseMenu.SelectedIndex = 1;
                    ChooseName();
                }
                else if (State == CharCreationState.Car && CarSelectMenu.SelectedIndex == 6)
                {
                    CarSelectMenu.SelectedIndex = 1;
                    ChooseNumberPlate();
                }
                else
                {
                    View.HandleActivate();
                }
            }


            if(!OpenedSetup)
            {
                InitSmall();
                View.AddMenu(SetupMenu);
                OpenedSetup = true;
            }

            if (!SetupFinished)
            {
                Point rectanglePoint;
                Point textPoint;

                switch (RPGSettings.SafeArea)
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

                new UIRectangle(new Point(rectanglePoint.X, rectanglePoint.Y - 121), new Size(181, 10), Color.FromArgb(255, 51, 153, 204)).Draw(); //topborder
                new UIRectangle(new Point(rectanglePoint.X, rectanglePoint.Y - 121), new Size(10, 121), Color.FromArgb(255, 51, 153, 204)).Draw(); //leftborder
                new UIRectangle(new Point(rectanglePoint.X + 171, rectanglePoint.Y - 121), new Size(10, 121), Color.FromArgb(255, 51, 153, 204)).Draw(); //leftborder
                new UIRectangle(rectanglePoint, new Size(181, 10), Color.FromArgb(255, 51, 153, 204)).Draw(); //playerinfo
                new UIText("Adjust till this borders your map", textPoint, 0.22f, Color.White, 0, false).Draw();

                return;
            }

            if(!Initialised) Init();

            Game.Player.CanControlCharacter = false;
            

            if (Game.IsKeyPressed(Keys.NumPad7) || Game.IsKeyPressed(Keys.NumPad9))
            {
                var right = Game.IsKeyPressed(Keys.NumPad9);
                if (State == CharCreationState.Character && CharacterModel != null && CharacterModel.Exists())
                {
                    CharacterModel.Rotation += right ? new Vector3(0, 0, 3f) : new Vector3(0, 0, -3f);
                }
                else if (State == CharCreationState.Car && CarModel != null && CarModel.Exists())
                {
                    CarModel.Rotation += right ? new Vector3(0, 0, 3f) : new Vector3(0, 0, -3f);
                }
            }
            
            switch (State)
            {
                case CharCreationState.PickMotives:
                case CharCreationState.PickClass:
                case CharCreationState.GenderSelect:
                case CharCreationState.Intro:
                    if(CarModel != null )
                    {
                        CarModel.Delete();
                        CarModel = null;
                    }
                    if(CharacterModel != null )
                    {
                        CharacterModel.Delete();
                        CharacterModel = null;
                    }
                    break;
                case CharCreationState.Character:
                    if (CarModel != null)
                    {
                        CarModel.Delete();
                        CarModel = null;
                    }
                    break;
                case CharCreationState.Car:
                    if (CharacterModel != null)
                    {
                        CharacterModel.Delete();
                        CharacterModel = null;
                    }
                    break;
            }

            switch(State)
            {
                case CharCreationState.PickMotives:
                    PickMotives();
                    break;
                case CharCreationState.PickClass:
                    PickClass();
                    break;
                case CharCreationState.GenderSelect:
                    PickGender();
                    break;
                case CharCreationState.Character:
                    CharacterSelection();
                    break;
                case CharCreationState.Car:
                    CarSelection();
                    break;
                case CharCreationState.Finalise:
                    FinaliseSelection();
                    break;
                case CharCreationState.Intro:
                    IntroCutscene();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void PickMotives()
        {
            if (_shownMotives) return;
            _shownMotives = true;
            var mmmmmenu = new TiledMenu("Select Your Motive",
                           new TiledPanel("Rebel", new GTASprite("loadingscreen3", "foreground"), Color.DodgerBlue, SelectMotive, "You started from the bottom, and you're still there. You know that nothing's free in this world, and even working for it can be too much of hassle. Maybe you're doing this for the right reasons, but you know you have to fight for what you deserve.", ""), //Motivation Passive: Provides +25% Normal Experience
                           new TiledPanel("Lawless", new GTASprite("loadingscreen4", "foreground", Color.SteelBlue), Color.Red, SelectMotive, "You didn't choose this life, it chose you. And since the day that you knew life would always bite you in the ass, you made a vow. You're all about mayhem and destruction. You're going to watch this world burn.", "")); //Motivation Passive: Provides +25% Skill Experience
            View.AddMenu(mmmmmenu);
        }
        private void SelectMotive(int obj)
        {
            if(obj == 0)
            {
                Motive = PlayerMotive.Rebel;
            }
            else
            {
                Motive = PlayerMotive.Lawless;
            }
            State = CharCreationState.PickClass;
            View.PopMenu();
        }

        private void PickClass()
        {
            if (_shownClass) return;
            _shownClass = true;
            var mmmmmenu = new TiledMenu("Select Your Class",
                           new TiledPanel("Time Master", new GTASprite("loadingscreen5", "foreground"), Color.DodgerBlue, SelectClass, "Actually, you were born on a distant planet far far away. You harness the power of time itself to destroy your enemies, before they even know it. Some say you're not a master, but a lord. You'll be the judge of that.", ""), //Class Skill: Harmony - Slow down time to a halt and mark multiple targets for death
                           new TiledPanel("Speedster", new GTASprite("loadingscreen1", "foreground_franklin"), Color.DarkRed, SelectClass, "You're the fastest man in los santos, possibly in the universe. You've harnessed the forces which give you your powers to become the best driver in the city. But hey, without a car you still kick ass in a flash too.", ""), //Class Skill: Turbo - Dramatically increase driving speed and break the laws of physics
                           new TiledPanel("Berserker", new GTASprite("loadingscreen9", "foreground"), Color.White, SelectClass, "Nobody's got time for that nonsense with shooting guns. Well...most of the time anyway. You know that up close and personal is the only way you'll get satisfaction. You're about to enter the danger zone, and you welcome it.", "")); //Class Skill: Rampage - Become invulnerable for a short time and summon the four horsemen
            View.AddMenu(mmmmmenu);
        }
        private void SelectClass(int obj)
        {
            if(obj == 0)
            {
                Class = PlayerClass.Time_Master;
            }
            else if (obj == 1)
            {
                Class = PlayerClass.Speedster;
            }
            else if (obj == 2)
            {
                Class = PlayerClass.Berserker;
            }

            State = CharCreationState.GenderSelect;
            View.PopMenu();
        }

        private void PickGender()
        {
            if (_shownGender) return;
            _shownGender = true;
            var mmmmmenu = new TiledMenu("Select Your Gender",
                               new TiledPanel("Male", new GTASprite("loadingscreen10", "foreground"), Color.DodgerBlue, SelectGender, "Male characters, typically played by men. "),
                               new TiledPanel("Female", new GTASprite("loadingscreen16", "foreground", Color.LightPink), Color.Purple, SelectGender, "Female characters, typically played by women...and also men. ")
                           );
            View.AddMenu(mmmmmenu);
        }
        private void SelectGender(int obj)
        {
            if(obj == 0)
            {
                Gender = Gender.Male;
                CharSelectMenu = new RPGMenu("Character Creation", new GTASprite("commonmenu", "interaction_bgd", Color.DodgerBlue), new IMenuItem[] {
                        new MenuEnumScroller("Character Model","",AvailablePeds.Select(x => x.ToString()).ToArray()).WithEnumActions(SetCharModel,d=> { }), 
                        new MenuButton("Continue", "").WithActivate(() => { View.PopMenu(); State = CharCreationState.Car;})
                });
            }
            else
            {
                Gender = Gender.Female;
                CharSelectMenu = new RPGMenu("Character Creation", new GTASprite("commonmenu", "interaction_bgd", Color.DodgerBlue), new IMenuItem[] {
                        new MenuEnumScroller("Character Model","",AvailablePeds.Select(x => x.ToString()).ToArray()).WithEnumActions(SetCharModel,d=> { }), 
                        new MenuButton("Continue", "").WithActivate(() => { View.PopMenu(); State = CharCreationState.Car;})
                });
            }

            State = CharCreationState.Character;
            View.PopMenu();
        }

        private void IntroCutscene()
        {
            IntroState = CharIntroState.Init;
            //Fade out
            Game.FadeScreenOut(2000);
            Wait(2000);

            World.CurrentDayTime = new TimeSpan(12,30,0);

            Wait(1000);

            //Init
            if (Game.Player.Character.IsInVehicle())
            {
                Game.Player.Character.Task.WarpOutOfVehicle(Game.Player.Character.CurrentVehicle);
            }

            Game.Player.Character.Position = _playerPosition;

            World.RenderingCamera = null;
            Function.Call(Hash.DISPLAY_HUD, 0);
            Function.Call(Hash.DISPLAY_RADAR, 0);
            Function.Call(Hash.SET_PLAYER_INVINCIBLE, Game.Player.Handle, true);

            World.Weather = Weather.Christmas;

            Model m = RPG.PlayerData.ModelHash;
            m.Request(1000);
            Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, m.Hash);
            RPGMethods.LoadVariations();
            Game.Player.Character.Position = new Vector3(_playerPosition.X, _playerPosition.Y, _playerPosition.Z + 200);
            Wait(500);
            Game.FadeScreenIn(2000);
            Wait(3000);
            Function.Call(Hash.SET_TIME_SCALE, 0.5f);
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "phone_cam4");
            //Game.PlayMusic("PROLOGUE_TEST_MISSION_START");
            var bgMusic = RPG.Audio.PlayMusic("intro");

            //A
            IntroState = CharIntroState.A;

            while(Game.Player.Character.HeightAboveGround > 80.0F)
                Yield();

            Game.FadeScreenOut(500);
            Wait(500);

            //B            
            //Game.PlayMusic("RAMPAGE_1_OS");
            IntroState = CharIntroState.B;
            World.Weather = Weather.ExtraSunny;
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
            Game.Player.Character.Position = new Vector3(_playerPosition.X, _playerPosition.Y, _playerPosition.Z - 0.25f);
            SetCam(Game.Player.Character.Position + new Vector3(0, 50, 25),true);
            Game.FadeScreenIn(500);
            Game.Player.CanControlCharacter = true;
            Wait(5000);

            //C
            IntroState = CharIntroState.C;
            SetCam(Game.Player.Character.Position + Game.Player.Character.ForwardVector * 2f + new Vector3(-0.8f,0,0.8f));
            Wait(5000);
            
            //D
            IntroState = CharIntroState.D;
            SetCam(Game.Player.Character.Position + new Vector3(25, 80, 25));

            Wait(5000);
            
            //E
            IntroState = CharIntroState.E;
            SetCam(Game.Player.Character.Position + new Vector3(75, 25, 100));
            Wait(7500);

            //Finished
            IntroState = CharIntroState.Finished;
            Game.FadeScreenOut(500);
            Wait(500);
            World.RenderingCamera = null;
            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            Function.Call(Hash.SET_PLAYER_INVINCIBLE, Game.Player.Handle, false);
            World.Weather = Weather.ExtraSunny;
            Function.Call(Hash.SET_TIME_SCALE, 1.0f);
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
            

            //Move Player
            Game.Player.Character.Position = _playerPosition;
            Game.Player.Character.Heading = _playerHeading;

            Wait(500);

            //Give and spawn stuff::
            //Give Weps
            RPG.PlayerData.Weapons.AddRange(WeaponRepository.Weapons);
            RPG.PlayerData.GetWeapon(WeaponHash.Pistol).Unlocked = true;
            RPG.PlayerData.GetWeapon(WeaponHash.Pistol).AmmoCount = 100;
            RPG.PlayerData.GetWeapon(WeaponHash.AssaultRifle).Unlocked = true;
            RPG.PlayerData.GetWeapon(WeaponHash.AssaultRifle).AmmoCount = 1000;

            RPG.SaveAllData();
            Wait(500);

            RPG.InitCharacter();

            Game.FadeScreenIn(500);
            RPG.GameHandler.Init();
            RPG.GameLoaded = true;
            bgMusic.Dispose();
            Wait(5000);

            RPG.GetPopup<TutorialBox>().Show("A new quest has started and has been added to your tracker.","Press J to open up your menu.");
            Enabled = false;

        }

        private void SetCam(Vector3 pos, bool lookAtPlayer = false)
        {
            if(Camera != null) Camera.Destroy();

            Camera = World.CreateCamera(pos, Vector3.Zero, GameplayCamera.FieldOfView);
            if(lookAtPlayer)
            {
                Camera.PointAt(Game.Player.Character);
            }
            World.RenderingCamera = Camera;
        }

       private void CarSelection()
        {
            //var p = new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 225);
            //new UIRectangle(new Point(p.X, p.Y + 25), new Size(300, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
            //new UIText("Name: " + PlayerName, new Point(p.X + 150, p.Y + 25), 0.25f, Color.White, 0, true).Draw();
            //new UIText((CarModel != null && (CarModel.Model.IsCar || CarModel.Model.IsBike || CarModel.Model.IsBicycle || CarModel.Model.IsHelicopter || CarModel.Model.IsPlane || CarModel.Model.IsQuadbike)
            //    ? "Drivable" : "Not Driveable"), new Point(p.X + 150, p.Y - 5), 0.25f, Color.White, 0, true).Draw();
            //var name = "License Plate:" + CarPlate;
            //
            //new UIRectangle(new Point(p.X, p.Y + 50), new Size(300, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
            //new UIText(name, new Point(p.X + 150, p.Y + 50), 0.25f, Color.White, 0, true).Draw();

            if (!IsOpen(CarSelectMenu))
            {
                if (CharacterModel != null) CharacterModel.Delete();
                View.AddMenu(CarSelectMenu);
            }

            if (CarModel == null)
            {
                var m = new Model(AvailableCars[CurrentCarModel]);
                m.Request(1000);
                CarModel = World.CreateVehicle(m, _carPosition, _carHeading);
                RPG.WorldData.AddVehicle(new VehicleObject("charCreation", CarModel));

                CarModel.PrimaryColor = AvailableColors[CurrentCarColor];
                CarModel.SecondaryColor = AvailableSecondaryColors[CurrentCarSecondaryColor];
                CarModel.NumberPlate = "";
                CarModel.NumberPlate = CarPlate;
                CarModel.RoofState = VehicleRoofState.Closed;
                Camera.Position = _camPosition + new Vector3(CarModel.Model.GetDimensions().Y, CarModel.Model.GetDimensions().Y/3, 0);
                if (CarModel != null && !(CarModel.Model.IsCar || CarModel.Model.IsBike || CarModel.Model.IsBicycle || CarModel.Model.IsHelicopter || CarModel.Model.IsPlane || CarModel.Model.IsQuadbike))
                {
                    CarModel.Delete();
                    CarModel = null;
                    NextModel();
                }

                if (CarModel != null)
                {
                    Camera.PointAt(CarModel);
                }
            }
            else
            {
                CarModel.FreezePosition = true;
                //CarModel.Rotation = new Vector3(CarModel.Rotation.X, CarModel.Rotation.Y, CarModel.Rotation.Z + 0.05f);
            }
        }

        private bool _setupFinalise;
        private void FinaliseSelection()
        {
            if (!IsOpen(FinaliseMenu))
            {
                View.AddMenu(FinaliseMenu);
            }

            if (!_setupFinalise)
            {
                _setupFinalise = true;

                if(CharacterModel != null) CharacterModel.Delete();
                if (CarModel != null) CarModel.Delete();

                Model mc = AvailableCars[CurrentCarModel];
                mc.Request(1000);
                CarModel = World.CreateVehicle(mc, _carPositionFinal, _carHeadingFinal);
                RPG.WorldData.AddVehicle(new VehicleObject("charCreation", CarModel));

                CarModel.PrimaryColor = AvailableColors[CurrentCarColor];
                CarModel.SecondaryColor = AvailableSecondaryColors[CurrentCarSecondaryColor];
                CarModel.NumberPlate = "";
                CarModel.NumberPlate = CarPlate;
                CarModel.RoofState = VehicleRoofState.Closed;

                Model m = AvailablePeds[CurrentCharModel];
                m.Request(1000);
                var leftVector = -Vector3.Cross(CarModel.ForwardVector, new Vector3(0, 0, 1));
                CharacterModel = World.CreatePed(m, new Vector3(_playerPositionFinal.X, _playerPositionFinal.Y, _playerPositionFinal.Z - 0.25f) + leftVector * 1.5f, _playerHeadingFinal);
                RPG.WorldData.AddPed(new NpcObject("charCreation",CharacterModel));
                foreach (var kvp in CurVariations)
                {
                    Function.Call(Hash.SET_PED_COMPONENT_VARIATION, CharacterModel, kvp.Key, kvp.Value, 0, 0);
                }
                 
                Camera.Position = _camPosition + new Vector3(CarModel.Model.GetDimensions().Y, CarModel.Model.GetDimensions().Y / 3, 0);
                CharacterModel.Position = (Camera.Position * 2.5f + CarModel.Position * 1.5f) / 4;

            }
            else
            {
                new UIText(PlayerName, new Point(57, UI.HEIGHT - 400), 6.5f, Color.FromArgb(60, 2, 2, 2), Font.Monospace, false).Draw();
                new UIText(PlayerName, new Point(55, UI.HEIGHT - 402), 6.5f, Color.White, Font.Monospace, false).Draw();
                new UIText(Gender + " " + Motive + " " + Class.ToString().Replace("_", " "), new Point(62, UI.HEIGHT - 175), 1.2f, Color.FromArgb(60, 2, 2, 2), Font.Monospace, false).Draw();
                new UIText(Gender + " " + Motive + " " + Class.ToString().Replace("_", " "), new Point(60, UI.HEIGHT - 177), 1.2f, Color.White, Font.Monospace, false).Draw();
            }
        }

        private void CharacterSelection()
        {
            //var p = new Point(UI.WIDTH / 2 - 150, UI.HEIGHT - 160);
            //new UIRectangle(new Point(p.X, p.Y + 25), new Size(300, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
            //new UIText("Name: " + PlayerName, new Point(p.X + 150, p.Y + 25), 0.25f, Color.White, 0, true).Draw();
            //var name = AvailablePeds[CurrentCharModel].ToString();
            //
            //new UIRectangle(new Point(p.X, p.Y + 50), new Size(200, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
            //new UIText(name, new Point(p.X + 100, p.Y + 50), 0.25f, Color.White, 0, true).Draw();

            if(!IsOpen(CharSelectMenu))
            {
                if (CarModel != null) CarModel.Delete();
                View.AddMenu(CharSelectMenu);
            }

            if (CharacterModel == null)
            {
                Model m = AvailablePeds[CurrentCharModel];
                m.Request(1000);
                CharacterModel = World.CreatePed(m, new Vector3(_playerPosition.X,_playerPosition.Y, _playerPosition.Z - 0.25f),_playerHeading);
                RPG.WorldData.AddPed(new NpcObject("charCreation", CharacterModel));

                if(CharacterModel != null && !CharacterModel.IsHuman)
                {
                    CharacterModel.Delete();
                    CharacterModel = null;
                    NextModel();
                }
                if(CharacterModel != null)
                {
                    View.RemoveMenu(CharSelectMenu);
                    //Get Variations
                    Variations = new Dictionary<int, int>();
                    CurVariations = new Dictionary<int, int>();
                    for (int i = 0; i < 25; i++)
                    {
                        var a = Function.Call<int>(Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS, CharacterModel, i);
                        if(a > 1)
                        {
                            Variations.Add(i,a);
                            CurVariations.Add(i,0);
                        }
                    }
                    //Add scrollers for each variation
                    var items = new List<IMenuItem>()
                                    {
                                        new MenuButton("Random Character", "").WithActivate(RandomModel),
                                        new MenuEnumScroller("Character Model","",AvailablePeds.Select(x => x.ToString()).ToArray(),CurrentCharModel).WithEnumActions(SetCharModel,d=> { }),
                                        new MenuLabel("", true)
                                    };

                    var continuebutton = new MenuButton("Continue", "").WithActivate(() => { View.PopMenu(); State = CharCreationState.Car; });

                    var count = 0;
                    foreach (var kvp in Variations)
                    {
                        var varCount = kvp.Value;
                        var enumStrings = new string[varCount];
                        for (int i = 0; i < varCount; i++)
                        {
                            enumStrings[i] = i.ToString();
                        }

                        items.Add(new MenuNumericScroller("Variation  " + (count + 1), "", 0, varCount - 1, 1).WithNumericActions(ChangeVariation, ChangeVariation));
                        count++;
                    }

                    items.Add(new MenuLabel("", true));
                    items.Add(continuebutton);
                    CharSelectMenu = new RPGMenu("Character Creation", new GTASprite("commonmenu", "interaction_bgd", Color.DodgerBlue), items.ToArray());
                    RPGUI.FormatMenu(CharSelectMenu);
                    View.AddMenu(CharSelectMenu);
                    CharSelectMenu.SelectedIndex = 1;
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, CharacterModel.Handle);
                    Camera.Position = _camPosition;
                    Camera.PointAt(CharacterModel.Position + CharacterModel.UpVector - new Vector3(0,0,0.2f));
                }
                    
            }
            else
            {
                CharacterModel.FreezePosition = true;
                CharacterModel.Task.ClearAll();
            }
        }


        private void ChangeVariation(double obj)
        {
            var val = (int) obj;
            var index = CharSelectMenu.SelectedIndex - 2;
            var kvp = Variations.ElementAt(index);

            var component = kvp.Key;
            CurVariations[component] = val;
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, CharacterModel, component, val, 0, 0);
        }

        public void HandleBack()
        {
            if (RPGInfo.KeyboardActive) return; 

            var curState = (int) State;
            var newState = curState - 1;
            if(newState >= 0)
            {
                
                View.CloseAllMenus();
                State = (CharCreationState) newState;
                //Resets
                _shownMotives = false;
                _shownGender = false;
                _shownClass = false;
                _setupFinalise = false;
            }
            else
            {
                Enabled = false;
                View.CloseAllMenus();
                if(CharacterModel != null) CharacterModel.Delete();
                if (CarModel != null) CarModel.Delete();
                RPG.Initialise();
            }
        }

        private bool IsOpen(MenuBase menu)
        {
            var items = (List<MenuBase>)typeof(Viewport).GetField("mMenuStack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(View);
            return items.Contains(menu);
        }
    }
}