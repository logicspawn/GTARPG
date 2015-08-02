using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using Microsoft.Win32;

namespace LogicSpawn.GTARPG.Core
{
    public class RPGInit : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return true; } }
        public static bool Enabled = true;
        private bool hasScriptHookV;
        private bool hasScriptHookDotNet;
        private bool hasNet45;

        public RPGInit()
        {
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (Enabled && keyEventArgs.KeyCode == Keys.Y)
            {
                StartRPGMode();
            }
        }

        private void StartRPGMode()
        {
            World.DestroyAllCameras();
            Game.Player.CanControlCharacter = true;

            string missing;
            var statusGood = CheckStatus(out missing);
            if (statusGood || !RPGSettings.ShowPrerequisiteWarning)
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

        private void PlayAnyway()
        {
            View.CloseAllMenus();
            RPG.Initialise();
        }

        private bool CheckStatus(out string missing)
        {
            var scripthookvpath = Path.Combine(Application.StartupPath, "ScriptHookV.dll");
            var scripthookvnetpath = Path.Combine(Application.StartupPath, "ScriptHookVDotNet.dll");
            hasScriptHookV = GetMD5Hash(scripthookvpath) == "4be83badebac3da379555c83f18b6e94";
            hasScriptHookDotNet = GetMD5Hash(scripthookvnetpath) == "68bf4bf432f95c1c18a6d290c8241c71";
            hasNet45 = IsNet45OrNewer();

            missing = "";

            if (!hasScriptHookV) missing += "[ScriptHookV v1.0.393.4] ";
            if (!hasScriptHookDotNet) missing += "[ScriptHookVNET v1.1] ";
            if (!hasNet45) missing += "[.NET v4.5]";

            return hasScriptHookV && hasScriptHookDotNet && hasNet45;

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

        public string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public string FriendlyName()
        {
            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }

        protected override void Start()
        {
            RPGLog.Clear();
            string str;
            CheckStatus(out str);
            RPGLog.Log("Starting GTARPG Log:");
            RPGLog.Log("Date: " + DateTime.Now.ToLongDateString());
            RPGLog.Log("OS: " + FriendlyName());
            RPGLog.Log("GTAV Path: " + Application.StartupPath);
            RPGLog.Log("ScriptHook V v1.0.393.4: " + (hasScriptHookV ? "Found" : "Not Found"));
            RPGLog.Log("ScriptHookVNET v1.1: " + (hasScriptHookDotNet ? "Found" : "Not Found"));
            RPGLog.Log(".NET v4.5: " + (hasNet45 ? "Found" : "Not Found"));
            RPGLog.LogRaw("");
            RPGLog.Log("Waiting to start rpg mode...");

            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            World.RenderingCamera = null;
            Function.Call(Hash.SET_TIME_SCALE, 1.0f);
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, "");
            Game.Player.CanControlCharacter = true;


            Function.Call(Hash.SET_PLAYER_WEAPON_DAMAGE_MODIFIER, Game.Player, 1.0f);
            Function.Call(Hash.SET_AI_WEAPON_DAMAGE_MODIFIER, 1.0f);

            if (RPGSettings.AutostartRPGMode)
            {
                Wait(500);
                StartRPGMode();
            }
        }




        public override void Update()
        {
            if (!Enabled) return;
            if (!RPGSettings.ShowPressYToStart) return;

            new UIText("press y to play gta:rpg", new Point(UI.WIDTH - 100 , 10), 0.22f, Color.White, 0, false).Draw();
            new UIRectangle(new Point(UI.WIDTH - 100, 10), new Size(95, 15), Color.FromArgb(120, 8, 8, 8)).Draw();
        }
    }
}