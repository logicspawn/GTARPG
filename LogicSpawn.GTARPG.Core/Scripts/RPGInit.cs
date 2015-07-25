using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace LogicSpawn.GTARPG.Core
{
    public class RPGInit : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return true; } }
        public static bool Enabled = true;
        public RPGInit()
        {
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (Enabled && keyEventArgs.KeyCode == Keys.Y)
            {
                RPGLog.Clear();
                World.DestroyAllCameras();
                Game.Player.CanControlCharacter = true;
                RPG.Initialise();
                Enabled = false;
            }
        }

        protected override void Start()
        {
            RPGLog.Log("Waiting to start rpg mode...");

            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            World.RenderingCamera = null;
            Function.Call(Hash.SET_TIME_SCALE, 1.0f);
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
            Game.Player.CanControlCharacter = true;


            Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, Game.Player, 1.0f);
            Function.Call(Hash.SET_AI_WEAPON_DAMAGE_MODIFIER, 1.0f);
        }

        public override void Update()
        {
            if (!Enabled) return;

            new UIText("press y to play gta:rpg", new Point(UI.WIDTH - 100 , 10), 0.22f, Color.White, 0, false).Draw();
            new UIRectangle(new Point(UI.WIDTH - 100, 10), new Size(95, 15), Color.FromArgb(120, 8, 8, 8)).Draw();
        }
    }
}