using System;
using System.Collections.Generic;
using System.Linq;
using GTA;

namespace LogicSpawn.GTARPG.Core
{
    public class SubtitleHandler : UpdateScript
    {
        public SubtitleHandler()
        {
            RPG.SubtitleHandler = this;
        }

        private string text;
        private int duration;

        public override void Update()
        {
            if (string.IsNullOrEmpty(text)) return;
            var t = text;
            RPGSettings.ShowingSubtitle = true;
            UI.ShowSubtitle(text,duration);
            var intervals = duration/50 + 1;
            bool changed = false;
            for (int i = 0; i < intervals; i++)
            {
                if (text != t)
                {
                    changed = true;
                    break;
                }

                Wait(50);
            }

            if (!changed)
                RPGSettings.ShowingSubtitle = false;
            
            if(!changed)
                text = null;
        }

        public void Do(string subtitle, int time)
        {
            text = subtitle;
            duration = time + 50;
        }
    }
}