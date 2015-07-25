using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using Font = GTA.Font;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class KillStreaks : Popup
    {
        public void Pop(int count)
        {
            Show(count);
        }

        public void Pop(string text)
        {
            Show(text);
        }

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            if (!args.Any()) return;

            var text = TextToShow(args[0]);

            if (string.IsNullOrEmpty(text)) return;

            popup.Size = new Size(UI.WIDTH, 105);
            popup.Position = new Point(0, 100);

            popup.Items.Add(new UIText(text, new Point(UI.WIDTH / 2, 0), 1.2f, Color.White, Font.Monospace, true));
        }

        private string TextToShow(object o)
        {
            if(o is int)
            {
                var c = (int) o;
                switch(c)
                {
                    case 0:
                    case 1:
                        return "";
                    case 2:
                        RPG.Notify(Notification.Kill("double kill + 2"));
                        RPG.PlayerData.AddExp(2);
                        RPG.Audio.PlayKillStreak("doublekill");
                        return "double kill";
                    case 3:
                        RPG.Notify(Notification.Kill("multi kill + 4"));
                        RPG.PlayerData.AddExp(4);
                        RPG.Audio.PlayKillStreak("multikill");
                        return "multi kill";
                    case 4:
                        RPG.Notify(Notification.Kill("mega kill + 8"));
                        RPG.PlayerData.AddExp(8);
                        RPG.Audio.PlayKillStreak("megakill");
                        return "mega kill";
                    case 5:
                        RPG.Notify(Notification.Kill("ultra kill + 12"));
                        RPG.PlayerData.AddExp(12);
                        RPG.Audio.PlayKillStreak("ultrakill");
                        return "ultra kill";
                    case 6:
                        RPG.Notify(Notification.Kill("monster kill + 24"));
                        RPG.PlayerData.AddExp(24);
                        RPG.Audio.PlayKillStreak("monsterkill");
                        return "m m m monster kill";
                    case 7:
                        RPG.Notify(Notification.Kill("ludicrous kill + 48"));
                        RPG.PlayerData.AddExp(48);
                        RPG.Audio.PlayKillStreak("ludicrouskill");
                        return "ludicrous kill";
                    case 8:
                        RPG.Notify(Notification.Kill("rampage kill + 60"));
                        RPG.PlayerData.AddExp(60);
                        RPG.Audio.PlayKillStreak("rampage");
                        return "rampage kill";
                    case 9:
                        RPG.Notify(Notification.Kill("wicked sick + 80"));
                        RPG.PlayerData.AddExp(80);
                        RPG.Audio.PlayKillStreak("wickedsick");
                        return "wicked sick";
                    case 10:
                        RPG.Notify(Notification.Kill("unstoppable + 100"));
                        RPG.PlayerData.AddExp(100);
                        RPG.Audio.PlayKillStreak("unstoppable");
                        return "unstoppable";
                }

                if(c > 10)
                {
                    RPG.Notify(Notification.Kill("holy shit + 100"));
                    RPG.PlayerData.AddExp(100);
                    RPG.Audio.PlayKillStreak("holyshit");
                    return "h o l y  s h i t";
                }
            }
            
            return (string) o;
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
            get { return 1500; }
        }
    }
}
