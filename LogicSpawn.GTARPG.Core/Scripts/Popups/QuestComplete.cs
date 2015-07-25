using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class QuestComplete : Popup
    {

        public override void Show(params object[] args)
        {
            var qc = RPG.GetPopup<QuestStarted>();
            while (qc.Showing)
            {
                Wait(0);
            }
            base.Show(args);
        }

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            RPG.GetPopup<QuestStarted>().Hide();

            var quest = (Quest)args[0];

            popup.Size = new Size(UI.WIDTH, 115);

            var extra = quest.AdditionalRewards.Count;
            if(extra > 0)
            {
                popup.Size = new Size(popup.Size.Width, popup.Size.Height + extra*20);
            }

            popup.Position = new Point(0, UI.HEIGHT / 2 - 50);

            var completedText = quest.IsContract ? "Contract complete" : "Quest complete";
            var color = quest.IsContract ? Color.FromArgb(150, 180, 180, 180) : Color.FromArgb(150, 255, 180, 0);
            popup.Color = color;

            popup.Items.Add(new UIText(completedText, new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIText("- " + quest.Name + " -", new Point(UI.WIDTH / 2, 15), 1.4f, Color.White, 0, true));
            popup.Items.Add(new UIText("- " + quest.GetBasicRewardString() + " -", new Point(UI.WIDTH / 2, 80), 0.25f, Color.White, 0, true));

            var point = 110;
            for (int index = 0; index < quest.AdditionalRewards.Count; index++)
            {
                var reward = quest.AdditionalRewards[index];
                popup.Items.Add(new UIText("- " + reward.GetRewardString() + " -", new Point(UI.WIDTH / 2, point + (index * 15)), 0.24f, Color.White, 0, true));

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
            get { return 5000; }
        }
    }
}
