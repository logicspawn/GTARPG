using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using Notification = LogicSpawn.GTARPG.Core.Objects.Notification;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class Notifier : Popup
    {
        private readonly List<Notification> _notifications = new List<Notification>();

        public void Notify(Notification notification)
        {
            Show(notification);
        }

        protected override bool FadeInPopup
        {
            get { return false; }
        }

        protected override bool FadeOutPopup
        {
            get { return false; }
        }

        protected override void OnPopup(UIContainer popup, object[] args)
        {
            if (args.Length > 0)
            {
                AddNotification((Notification) args[0]);
            }


            popup.Size = new Size(UI.WIDTH / 7, UI.HEIGHT - 150);
            popup.Position = new Point((UI.WIDTH/2) - (UI.WIDTH/7/2), 25);

            for (int i = 0; i < _notifications.Count; i++)
            {
                popup.Items.Add(new UIText("- " + _notifications[i].Text + " -", new Point(popup.Size.Width / 2, popup.Position.Y + popup.Size.Height - 50 - 25 - (i * 25)), 0.23f, Color.White, 0, true));
                popup.Items.Add(new UIRectangle(new Point(0, popup.Position.Y + popup.Size.Height - 50 - 25 - (i * 25)), new Size(popup.Size.Width, 14), _notifications[i].GetColor()));
            }

            ResetTimeWaited();
        }

        private void AddNotification(Notification notification)
        {
            var updated = false;
            if(_notifications.Any())
            {
                if(notification.Type == NotifyType.Kill)
                {
                    if(notification.Text.Contains("target"))
                    {
                        var toUpdate = _notifications.FirstOrDefault(n => n.Type == NotifyType.Kill && n.Text.Contains("target"));
                        if (toUpdate != null)
                        {
                            toUpdate.Tracking++;

                            _notifications.Remove(toUpdate);
                            _notifications.Add(toUpdate);
                            if(toUpdate.Tracking > 1)
                            {
                                RPG.GetPopup<KillStreaks>().Pop(toUpdate.Tracking);
                            }
                            toUpdate.Text = "targets eliminated: +" + (toUpdate.Tracking * 5) + " Exp";
                            updated = true;
                        }
                    }
                } 
                if(notification.Type == NotifyType.Loot)
                {
                    if(notification.Text.Contains("gta$"))
                    {
                        var toUpdate = _notifications.FirstOrDefault(n => n.Type == NotifyType.Loot && n.Text.Contains("gta$"));
                        if (toUpdate != null)
                        {
                            toUpdate.Tracking += notification.Tracking;

                            _notifications.Remove(toUpdate);
                            _notifications.Add(toUpdate);
                            toUpdate.Text = "looted gta$" + toUpdate.Tracking.ToString("N0");
                            updated = true;
                        }
                    }
                } 

                if(notification.Type == NotifyType.Alert)
                {
                    if(notification.Text.Contains("cannot use"))
                    {
                        var toRemove = _notifications.FirstOrDefault(n => n.Type == NotifyType.Alert && n.Text.Contains("cannot use"));
                        if (toRemove != null)
                        {
                            _notifications.Remove(toRemove);
                        }
                    }
                }            
            }
            
            if(!updated)
            {
                if(_notifications.Count >= 5)
                {
                    _notifications.RemoveAt(0);
                }
                _notifications.Add(notification);
            }
                
        }

        protected override void OnFinish()
        {
            _notifications.Clear();
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
