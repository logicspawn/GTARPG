using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class PatchNotes : Popup
    {
        protected override bool HideOnlyWhenClosed { get { return true; } }
        protected override void OnPopup(UIContainer popup, object[] args)
        {
            popup.Size = new Size(UI.WIDTH, 105);

            string[] lines = new string[]
                                 {
                                     "Sorry for such a delay in the update!",
                                     "",
                                     "Patch Notes:",
                                     "- Added in weapon unlock system. You can now unlock weapons",
                                     "  using SP once you reach the required level.",
                                     "  You will also be notified which weapons are available upon level up",
                                     "  Go to Menu > Character Menu > Weapons to unlock weapons.",
                                     "  Weapon mods are planned for the future.",
                                     "- Weapon icons will be fixed in a future patch",
                                     " ",
                                     "Coming Soon: ",
                                     "- New Quests",
                                     "- Survival Mode",
                                     "- Unique Class Skills",
                                     "",
                                     "Please remember this a free mod and I work on it in my spare time, if you",
                                     "would like to support the project feel free to donate, but it is never required.",
                                     "Thanks for playing, hope you have a great time.",
                                     "LogicSpawn",
                                     "",
                                     "Press E to Close"
                                 };
            var extra = lines.Length;
            popup.Size = new Size(popup.Size.Width, popup.Size.Height + extra*20);

            popup.Position = new Point(0, UI.HEIGHT / 2 - popup.Size.Height/2);

            var headerText = "GTA:RPG";
            var color = Color.FromArgb(70, 145, 145, 145);
            popup.Color = color;

            popup.Items.Add(new UIText(headerText, new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIText("- PATCH NOTES and UPDATES -", new Point(UI.WIDTH / 2, 15), 1.5f, Color.White, 0, true));

            var point = 100;
            for (int index = 0; index < extra; index++)
            {
                var text = lines[index];
                popup.Items.Add(new UIText(text, new Point(UI.WIDTH / 2, point + (index * 15)), 0.24f, Color.White, 0, true));

            }
        }

        protected override void OnFinish()
        {

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
