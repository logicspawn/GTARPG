using System;
using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class IntroTitle : Popup
    {
        protected override bool RunWhenGameIsNotLoaded
        {
            get
            {
                return true;
            }
        }

        protected override void Custom(UIContainer popup, int timeWaited, int showTime, int fadeTime)
        {
            if (timeWaited > showTime - fadeTime)
            {
                var timeOfFrame = timeWaited - (showTime - fadeTime);
                popup.Position = new Point(popup.Position.X, (int)((UI.HEIGHT / 2 + 45) + (UI.HEIGHT * ((float)timeOfFrame / fadeTime))));
            }
        } 

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            popup.Size = new Size(UI.WIDTH, 105);
            popup.Position = new Point(0, UI.HEIGHT / 2 + 45);
            popup.Items.Add(new UIRectangle(new Point(0, 0), popup.Size, Color.FromArgb(150, 2, 70, 200)));
            popup.Items.Add(new UIText("grand theft auto", new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIText("- RPG - ", new Point(UI.WIDTH / 2, 15), 2.02f, Color.White, 0, true));
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
            get { return 4000; }
        }
    }
}
