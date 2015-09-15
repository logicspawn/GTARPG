using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class MessageToPlayer : Popup
    {
        protected override bool HideOnlyWhenClosed { get { return true; } }
        protected override void OnPopup(UIContainer popup, object[] args)
        {
            popup.Size = new Size(UI.WIDTH, 105);

            string[] lines = new string[]
                                 {
                                     "Thank you for playing the GTA:RPG experience. It's a work in progress and ",
                                     "hopefully this preview has been fun to play. More missions soon!",
                                     "",
                                     "Your feedback and support is important in making this a great mod.",
                                     "Feel free to continue playing, completing contracts and earning exp.",
                                     "",
                                     "Planned Features in the future:",
                                     "Vehicle: Mod tree, cruise control and weapon systems",
                                     "Quests: Animated cutscenes and more quest condition types ",
                                     "Player Customisation: More customisation for player stats and progression",
                                     "",
                                     "Once again thanks for playing, hope you had a great time.",
                                     "LogicSpawn",
                                     "Press E to Close",
                                 };
            var extra = lines.Length;
            popup.Size = new Size(popup.Size.Width, popup.Size.Height + extra*20);

            popup.Position = new Point(0, UI.HEIGHT / 2 - 50);

            var headerText = "GTA:RPG";
            var color = Color.FromArgb(150, 2, 70, 200);
            popup.Color = color;

            popup.Items.Add(new UIText(headerText, new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIText("- Thanks for Playing -", new Point(UI.WIDTH / 2, 15), 1.5f, Color.White, 0, true));

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
