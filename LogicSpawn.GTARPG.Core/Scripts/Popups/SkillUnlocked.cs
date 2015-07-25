using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class SkillUnlocked : Popup
    {
        protected override void OnPopup(UIContainer popup, object[] args)
        {
            var skill = (Skill)args[0];

            popup.Size = new Size(UI.WIDTH, 115);
            popup.Position = new Point(0, 0);

            var startedText = "Skill Unlocked";
            var color = Color.FromArgb(150, 100, 180, 0);

            popup.Items.Add(new UIText(startedText, new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIRectangle(new Point(0, 0), new Size(UI.WIDTH, 25), color));

            popup.Items.Add(new UIText("- " + skill.Name + " -", new Point(UI.WIDTH / 2, 15), 1.3f, Color.White, 0, true));
            popup.Items.Add(new UIText(skill.Description, new Point(UI.WIDTH / 2, 80), 0.4f, Color.White, 0, true));
            popup.Items.Add(new UIRectangle(new Point(0, 25), new Size(UI.WIDTH, 80), color));
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
            get { return 2000; }
        }
    }
}
