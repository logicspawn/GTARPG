using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;

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

                string missing;
                var statusGood = CheckStatus(out missing);
                if(statusGood)
                {
                    RPG.Initialise();
                    Enabled = false;
                }
                else
                {
                    var mb = (RPGMessageBox)RPGMessageBox.Create("Missing: " + missing + " View readme for more info.",
                                               "Play Anyway ( WARNING: Bugs/errors expected))", "Return to normal mode", PlayAnyway, () => { RPGMethods.ReturnToNormal(); RPG.UIHandler.View.CloseAllMenus(); });
                    RPGUI.FormatMenu(mb);
                    mb.TopColor = Color.Red;
                    mb.HeaderScale = 0.5f;
                    RPG.UIHandler.View.AddMenu(mb);
                    Enabled = false;
                }
            }
        }

        private void PlayAnyway()
        {
            View.CloseAllMenus();
            RPG.Initialise();
        }

        private bool CheckStatus(out string missing)
        {
            var scripthookvpath = Path.Combine(Application.StartupPath, "ScriptHookV.dll");
            var scripthookvnetpath = Path.Combine(Application.StartupPath, "ScriptHookVDotNet.dll");
            var scripthookv = GetMD5Hash(scripthookvpath) == "4be83badebac3da379555c83f18b6e94";
            var scripthooknet = GetMD5Hash(scripthookvnetpath) == "68bf4bf432f95c1c18a6d290c8241c71";
            var net45 = IsNet45OrNewer();

            missing = "";

            if (!scripthookv) missing += "[ScriptHookV v1.0.393.4] ";
            if (!scripthooknet) missing += "[ScriptHookVNET v1.1] ";
            if (!net45) missing += "[.NET v4.5]";

            return scripthookv && scripthooknet && net45;

        }

        private string GetMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();

                }
            }
        }

        public static bool IsNet45OrNewer()
        {
            // Class "ReflectionContext" exists from .NET 4.5 onwards.
            return Type.GetType("System.Reflection.ReflectionContext", false) != null;
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