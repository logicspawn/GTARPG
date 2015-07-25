using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.General;

namespace LogicSpawn.GTARPG.Core
{
    public abstract class Popup : UpdateScript
    {

        public static List<Popup> CloseablePopups = new List<Popup>();
        public static void CloseLastPopup()
        {
            if (!CloseablePopups.Any()) return;

            var last = CloseablePopups[CloseablePopups.Count - 1];
            last._show = false;
        }

        protected Popup()
        {
            RPG.Popups.Register(this);
            DoCustom = Custom;
        }

        private int _timeWaited = 0;
        protected bool _show;
        protected UIContainer _popup;
        protected Action<UIContainer, int, int, int> DoCustom = null;

        public bool Showing
        {
            get { return _show; }
        }

        public virtual void Show(params object[] args)
        {
            _popup = new UIContainer();
            _timeWaited = 0;
            OnPopup(_popup, args);
            _show = true;
        }

        public override void Update()
        {
            if (!CanRun) return;
            if (!_show) return;

            int showTime = TimeToShowMs/14;
            int fadeTime = showTime/8;

            if(HideOnlyWhenClosed)
                CloseablePopups.Add(this);

            while (_timeWaited < showTime && _show)
            {
                if(_timeWaited < fadeTime)
                {
                    if (FadeInPopup)
                    {
                        RPGUI.SetAlpha(_popup, (int)(((float)_timeWaited / fadeTime) * 255));
                    }
                }

                if (_timeWaited > showTime - fadeTime)
                {
                    if (HideOnlyWhenClosed)
                    {
                        while (_show)
                        {
                            while (ShowParameter)
                            {
                                _popup.Draw();
                                Wait(0);
                            }
                        }
                    }

                    if(FadeOutPopup)
                    {
                        var timeOfFrame = _timeWaited - (showTime - fadeTime);
                        RPGUI.SetAlpha(_popup, (int)(255 - (255 * ((float)timeOfFrame / fadeTime))));
                    }
                }

                if(DoCustom != null)
                    DoCustom.Invoke(_popup, _timeWaited, showTime, fadeTime);

                _popup.Draw();

                _timeWaited++;
                Wait(0);
            }

            

            Finish();
        }


        public void ResetTimeWaited()
        {
            _timeWaited = 0;
        }

        public void Hide()
        {
            _show = false;
        }

        private void Finish()
        {
            if(HideOnlyWhenClosed)
                CloseablePopups.Remove(this);
            _timeWaited = 0;
            _show = false;
            OnFinish();
        }

        protected abstract void OnPopup(UIContainer popup, object[] args);

        protected abstract void OnFinish();

        protected abstract bool CanRun { get; }

        protected abstract int TimeToShowMs { get; }

        protected virtual void Custom(UIContainer popup, int timeWaited, int showTime, int fadeTime)
        {

        }

        protected virtual bool FadeInPopup { get { return true; } }
        protected virtual bool ShowParameter { get { return true; } }
        protected virtual bool HideOnlyWhenClosed { get { return false; } }
        protected virtual bool FadeOutPopup { get { return true; } }
    }
}