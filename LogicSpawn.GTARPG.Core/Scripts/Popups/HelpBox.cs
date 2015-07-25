using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class HelpBox : Popup
    {
        protected override bool HideOnlyWhenClosed { get { return true; } }
        protected override void OnPopup(UIContainer popup, object[] args)
        {
            popup.Size = new Size(UI.WIDTH, 105);

            string[] lines = new string[]
                                 {
                                     "press j to open the main menu to browse inventory, buy goods, receive contracts",
                                     "Earn XP by getting kills, completing contracts, destroying vehicles",
                                     "A quest has been started, use the menu to purchase an ammo pack, they will come in handy",
                                     "In the Character Menu section, you can unlock new skills for SP ( Gained alongside normal XP) ",
                                     "Remember, you can press K at any time to respawn your personal vehicle.",
                                     "Press F7 to reopen this help menu",
                                     "this mod is a work in progress, expect things to change, like this tutorial :)",
                                     "Hotkeys:",
                                     "[J] Main Menu -- [I] Inventory -- [O] Character Menu -- [K] Spawn Your Vehicle -- [L] Quest Log ",
                                     "[F8] Save Game -- [F9] Load Game -- [F10] New Game -- [F11] Debug -- ",
                                     "[T] Skill 1 -- [Y] Skill 2 -- [CapsLock] Skill 3 -- [U] Skill 4 -- [B] Skill 5",
                                     "Close",
                                     "Press E to close this popup",
                                 };
            var extra = lines.Length;
            popup.Size = new Size(popup.Size.Width, popup.Size.Height + extra*20);

            popup.Position = new Point(0, UI.HEIGHT / 2 - 50);

            var headerText = "welcome to gta:rpg";
            var color = Color.FromArgb(150, 2, 70, 200);
            popup.Color = color;

            popup.Items.Add(new UIText(headerText, new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIText("- Game Guide -", new Point(UI.WIDTH / 2, 15), 1.5f, Color.White, 0, true));
            popup.Items.Add(new UIText("- basic info to get you started -", new Point(UI.WIDTH / 2, 80), 0.25f, Color.White, 0, true));

            var point = 100;
            for (int index = 0; index < extra; index++)
            {
                var text = lines[index];
                popup.Items.Add(new UIText("- " + text + " -", new Point(UI.WIDTH / 2, point + (index * 15)), 0.24f, Color.White, 0, true));

            }
        }

        protected override void OnFinish()
        {
            RPG.Notify("Good luck!");
        }

        protected override bool CanRun
        {
            get { return true; }
        }

        protected override int TimeToShowMs
        {
            get { return 1000; }
        }
    }
}
