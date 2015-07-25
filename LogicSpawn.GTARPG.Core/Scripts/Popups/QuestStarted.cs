using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class QuestStarted : Popup
    {
        public override void Show(params object[] args)
        {
            var qc = RPG.GetPopup<QuestComplete>();
            while (qc.Showing)
            {
                Wait(0);
            }

           base.Show(args);
        }

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            

            var quest = (Quest)args[0];

            popup.Size = new Size(UI.WIDTH, 110);
            popup.Position = new Point(0, UI.HEIGHT / 2 - 50);

            var startedText = quest.IsContract ? "Contract Accepted" : "Quest Started";
            var color = quest.IsContract ? Color.FromArgb(150, 180, 180, 180) : Color.FromArgb(150, 255, 180, 0);

            popup.Items.Add(new UIText(startedText, new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIRectangle(new Point(0, 0), new Size(UI.WIDTH, 120), color));

            popup.Items.Add(new UIText("- " + quest.Name + " -", new Point(UI.WIDTH / 2, 15), 1.4f, Color.White, 0, true));
            popup.Items.Add(new UIText("- " + quest.Description + " -", new Point(UI.WIDTH / 2, 90), 0.35f, Color.White, 0, true));
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
            get { return 3000; }
        }
    }
}
