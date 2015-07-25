using System.Drawing;
using GTA;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class TutorialBox : Popup
    {
        protected override bool HideOnlyWhenClosed { get { return true; } }

        public void Pop(string text1, string text2)
        {
            Show(new object[]{text1,text2});
        }

        protected new void Show(params object[] args)
        {
            base.Show(args);
        }

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            var text1 = (string)args[0];
            var text2 = (string)args[1];
            popup.Size = new Size(500,60);
            popup.Position = new Point(UI.WIDTH/2 - 250, 25);
            popup.Color = Color.FromArgb(150, 2, 70, 200);
            popup.Items.Add(new UIText("Tutorial", new Point(15, 0), 0.5f, Color.White, 0,false));
            popup.Items.Add(new UIText(text1, new Point(35, 25), 0.25f, Color.White, 0, false));
            popup.Items.Add(new UIText(text2, new Point(35, 40), 0.25f, Color.Orange,0, false));

        }

        protected override void OnFinish()
        {

        }


        protected override bool ShowParameter
        {
            get { return true; }
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
