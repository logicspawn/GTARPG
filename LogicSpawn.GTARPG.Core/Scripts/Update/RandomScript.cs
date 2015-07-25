//using System.Drawing;
//using GTA;
//
//namespace LogicSpawn.GTARPG.Core
//{
//    public class RandomScript : UpdateScript
//    {
//        private int curColor;
//        public override void Update()
//        {
//            while(RPG.GameLoaded)
//            {
//                var cur = Game.Player.Character.CurrentVehicle;
//                if( cur != null)
//                {
//                    cur.ToggleMod(VehicleToggleMod.TireSmoke, true);
//                    cur.TireSmokeColor = NextColor();
//                }
//                Wait(100);
//            }
//        }
//
//        private Color NextColor()
//        {
//            var colors = new Color[]
//                             {
//                                 Color.Red,
//                                 Color.Orange,
//                                 Color.Yellow,
//                                 Color.Green,
//                                 Color.DeepSkyBlue,
//                                 Color.Blue,
//                                 Color.MediumPurple,
//                                 Color.DeepPink
//                             };
//            var c =  colors[curColor];
//            curColor++;
//            if (curColor > colors.Length - 1) curColor = 0;
//            return c;
//        }
//    }
//}