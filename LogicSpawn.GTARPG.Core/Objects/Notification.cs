using System;
using System.Drawing;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class Notification
    {
        public NotifyType Type;
        public string Text;
        public int Tracking;

        protected Notification(NotifyType t, string message)
        {
            Type = t;
            Text = message;
            Tracking = 0;
        }

        public static Notification Kill(string message)
        {
            var n = new Notification(NotifyType.Kill, message) {Tracking = 1};
            return n;
        }
        public static Notification Loot(string message)
        {
            var n = new Notification(NotifyType.Loot, message);
            return n;
        }
        public static Notification Alert(string message)
        {
            var n = new Notification(NotifyType.Alert, message);
            return n;
        }
        public static Notification Danger(string message)
        {
            var n = new Notification(NotifyType.Danger, message);
            return n;
        }
        public Color GetColor()
        {
            switch(Type)
            {
                case NotifyType.Kill:
                    return Color.FromArgb(35, 30, 144, 255);
                case NotifyType.Loot:
                    return Color.FromArgb(35, 255,165,0);
                case NotifyType.Alert:
                    return Color.FromArgb(35, 190,190,190);
                case NotifyType.Danger:
                    return Color.FromArgb(35, 255,50,0);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum NotifyType
    {
        Kill,
        Loot,
        Alert,
        Danger
    }
}