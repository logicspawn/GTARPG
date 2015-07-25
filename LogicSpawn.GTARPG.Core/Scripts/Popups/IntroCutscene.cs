using System;
using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using Font = GTA.Font;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class IntroCutscene : Popup
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
            if (timeWaited < fadeTime)
            {
                popup.Position = new Point(UI.WIDTH - (int)(((float)timeWaited /fadeTime) *  400),popup.Position.Y);
            }

            if (timeWaited > showTime - fadeTime)
            {
                var timeOfFrame = timeWaited - (showTime - fadeTime);
                popup.Position = new Point((int)(300 - (UI.WIDTH * ((float)timeOfFrame / fadeTime))), popup.Position.Y);
            }
        } 

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            if (!args.Any()) return;

            var text = (string)args[0];
            var size = (float)args[1];

            if (string.IsNullOrEmpty(text)) return;

            popup.Size = new Size(UI.WIDTH, 200);
            popup.Position = new Point(UI.WIDTH, UI.HEIGHT - 80);
            popup.Items.Add(new UIText(text, new Point(2, 2), size, Color.FromArgb(60, 2, 2, 2), Font.Monospace, true));
            popup.Items.Add(new UIText(text, new Point(0, 0), size, Color.White, Font.Monospace, true));
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
            get { return 2500; }
        }
    }
}
