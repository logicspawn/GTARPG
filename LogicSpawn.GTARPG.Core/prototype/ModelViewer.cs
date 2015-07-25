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
//using Menu = GTA.Menu;
//
//namespace LogicSpawn.GTARPG.Core
//{
//    class ModelViewer : UpdateScript
//    {
//        Menu MenuA;
//        private MenuButton CurGroup;
//        private int CurrentModel;
//        public static bool Enabled;
//        private ModelGroup CurrentGroup;
//
//        private Entity Object;
//        private Vector3 PositionToSpawnAt;
//        private Camera Camera;
//
//        private enum ModelGroup
//        {
//            Ped,
//            Prop,
//            Vehicle
//        };
//
//        public ModelViewer()
//        {
//            CurrentGroup = ModelGroup.Ped;
//            CurrentModel = 0;
//            View.MenuTransitions = true;
//            View.MenuPosition = new Point(UI.WIDTH/2 - 150, UI.HEIGHT - 200);
//            //Instantiate our menu
//
//            CurGroup = new MenuButton("Switch Model Group", "", SwitchGroup);
//            MenuA = new Menu("Model Viewer", new GTA.MenuItem[] {
//                        new MenuButton("Next", NextModel),
//                        new MenuButton("Previous", "", PreviousModel),
//                        new MenuButton("Go to Last", "", LastModel),
//                        new MenuButton("Go to First", "", FirstModel),
//                        new MenuButton("Go to N", "", GotoN),
//                        CurGroup
//            });
//            MenuA.Width = 200;
//
//            RPGUI.FormatMenu(MenuA);
//
//            KeyDown += OnKeyDown;    
//        }
//
//        private void GotoN()
//        {
//            String test = Game.GetUserInput(15); //new code
//            int result;
//            if(int.TryParse(test, out result))
//            {
//                int max;
//                switch (CurrentGroup)
//                {
//                    case ModelGroup.Ped:
//                        max = Data.PedHash.Length - 1;
//                        break;
//                    case ModelGroup.Prop:
//                        max = Data.propNames.Length - 1;
//                        break;
//                    case ModelGroup.Vehicle:
//                        max = Data.VehicleHash.Length - 1;
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//
//                if(result > 0 && result <= max)
//                {
//                    ClearPrevModel();
//                    CurrentModel = result;
//                    return;
//                }
//            }
//
//            RPGLog.Log("Error parsing int.");
//        }
//
//        private void SwitchGroup()
//        {
//            ClearPrevModel();
//            switch(CurrentGroup)
//            {
//                case ModelGroup.Ped:
//                    CurrentGroup = ModelGroup.Prop;
//                    break;
//                case ModelGroup.Prop:
//                    CurrentGroup = ModelGroup.Vehicle;
//                    break;
//                case ModelGroup.Vehicle:
//                    CurrentGroup = ModelGroup.Ped;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//
//            Reset();
//        }
//
//        private void Reset()
//        {
//            CurrentModel = 0;
//        }
//
//        private void FirstModel()
//        {
//            ClearPrevModel();
//            CurrentModel = 0;
//        }
//
//        private void LastModel()
//        {
//            ClearPrevModel();
//
//            switch(CurrentGroup)
//            {
//                case ModelGroup.Ped:
//                    CurrentModel = Data.PedHash.Length - 1;
//                    break;
//                case ModelGroup.Prop:
//                    CurrentModel = Data.propNames.Length - 1;
//                    break;
//                case ModelGroup.Vehicle:
//                    CurrentModel = Data.VehicleHash.Length - 1;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }
//
//        private void NextModel()
//        {
//            ClearPrevModel();
//
//            CurrentModel++;
//
//            switch (CurrentGroup)
//            {
//                case ModelGroup.Ped:
//                    CurrentModel = (Data.PedHash.Length - 1) >= CurrentModel ? CurrentModel : 0;
//                    break;
//                case ModelGroup.Prop:
//                    CurrentModel = (Data.propNames.Length - 1) >= CurrentModel ? CurrentModel : 0;
//                    break;
//                case ModelGroup.Vehicle:
//                    CurrentModel = (Data.VehicleHash.Length - 1) >= CurrentModel ? CurrentModel : 0;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }
//
//        private void PreviousModel()
//        {
//            ClearPrevModel();
//
//            CurrentModel--;
//            if(CurrentModel < 0 )
//            {
//                switch (CurrentGroup)
//                {
//                    case ModelGroup.Ped:
//                        CurrentModel = Data.PedHash.Length - 1;
//                        break;
//                    case ModelGroup.Prop:
//                        CurrentModel = Data.propNames.Length - 1;
//                        break;
//                    case ModelGroup.Vehicle:
//                        CurrentModel = Data.VehicleHash.Length - 1;
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//            }
//        }
//
//        private void ClearPrevModel()
//        {
//            if(Object != null)
//            {
//                try
//                {
//                    Object.Delete(); 
//                }
//                catch
//                {
//                    
//                }
//            }
//
//
//            Object = null;
//            
//        }
//
//        private void OnKeyDown(object sender, KeyEventArgs e)
//        {
//            if (!RPG.GameLoaded) return;
//
//            if(e.KeyCode == Keys.F5)
//            {
//                Enabled = !Enabled;
//
//                if(Enabled == true)
//                {
//                    InitCamAndPos();
//                    View.AddMenu(MenuA);
//
//                }
//                else
//                {
//                    ClearPrevModel();
//                    View.CloseAllMenus();
//                    World.RenderingCamera = null;
//                }
//            }
//        }
//
//        private void InitCamAndPos()
//        {
//            View.MenuTransitions = true;
//
//
//            if(Camera != null) Camera.Destroy();
//            var p = Game.Player.Character.Position;
//
//            Camera = World.CreateCamera(p + Game.Player.Character.ForwardVector * 1, Vector3.Zero, GameplayCamera.FOV);
//            PositionToSpawnAt = p + Game.Player.Character.ForwardVector * 8f;
//            World.RenderingCamera = Camera;
//        }
//
//        public override void Update()
//        {
//            if (!Enabled) return;
//
//            var p = new Point(UI.WIDTH/2 - 150 + 210, UI.HEIGHT - 180);
//            new UIRectangle(p, new Size(200, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//            new UIText("Current Group: " + CurrentGroup, new Point(p.X + 100, p.Y), 0.25f, Color.White, 0, true).Draw();
//
//            new UIRectangle(new Point(p.X, p.Y + 25), new Size(200, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//            new UIText("Current: " + CurrentModel, new Point(p.X + 100, p.Y + 25), 0.25f, Color.White,0,true).Draw();
//
//            new UIRectangle(new Point(p.X, p.Y + 50), new Size(200, 20), Color.FromArgb(180, 8, 8, 8)).Draw();
//
//            var name = "";
//            switch (CurrentGroup)
//            {
//                case ModelGroup.Ped:
//                    name = " \"" + Data.PedHash[CurrentModel] + "\"";
//                    break;
//                case ModelGroup.Prop:
//                    name = "\"" + Data.propNames[CurrentModel] + "\"";
//                    break;
//                case ModelGroup.Vehicle:
//                    name = Data.VehicleHash[CurrentModel].ToString();
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//
//            new UIText(name, new Point(p.X + 100, p.Y + 50), 0.25f, Color.White, 0, true).Draw();
//
//
//            if (Object == null)
//            {
//                switch(CurrentGroup)
//                {
//                    case ModelGroup.Ped:
//                        Object = World.CreatePed(new Model((PedHash)CurrentModel), PositionToSpawnAt);
//                        break;
//                    case ModelGroup.Prop:
//                        Object = World.CreateProp(new Model(Data.propNames[CurrentModel]), PositionToSpawnAt, false, false);
//                        if(!Object.Exists())
//                        {
//                            NextModel();
//                            return;
//                        }
//                        break;
//                    case ModelGroup.Vehicle:
//                        Object = World.CreateVehicle(new Model(Data.VehicleHash[CurrentModel]), PositionToSpawnAt);
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//
//                Camera.PointAt(Object);
//            }
//            else
//            {
//                Object.FreezePosition = true;
//                Object.Rotation = new Vector3(Object.Rotation.X, Object.Rotation.Y, Object.Rotation.Z + 1F);
//            }
//        }
//    }
//}
