using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;

namespace LogicSpawn.GTARPG.Core
{
    public abstract class KeyHandlerScript : Script
    {
        private bool _handlingInput;

        private Dictionary<Keys, Action> _registeredKeyUp;
        private Dictionary<Keys, Action> _registeredKeyDown;

        public KeyHandlerScript()
        {
            _registeredKeyUp = new Dictionary<Keys, Action>();
            _registeredKeyDown = new Dictionary<Keys, Action>();
            Init();

            _handlingInput = true;
            Interval = 0;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;


        }

        private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
        {
            if (CannotHandleInputs()) return;

            var button = keyEventArgs.KeyCode;
            Action action;

            if (_registeredKeyUp.TryGetValue(button, out action))
            {
                action.Invoke();
            }
        }

        private bool CannotHandleInputs()
        {
            return !(_handlingInput && RPG.GameLoaded && !RPG.PlayerDead); 

        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (CannotHandleInputs()) return;

            var button = keyEventArgs.KeyCode;
            Action action;

            if (_registeredKeyDown.TryGetValue(button, out action))
            {
                action.Invoke();
            }
        }


        public void StartHandlingInputs()
        {
            _handlingInput = true;
        }

        public void StopHandlingInputs()
        {
            _handlingInput = false;
        }


        protected void RegisterKeyUp(Keys keys, Action action)
        {
            _registeredKeyUp.Add(keys,action);
        }

        protected void RegisterKeyDown(Keys keys, Action action)
        {
            _registeredKeyDown.Add(keys, action);
        }

        protected abstract void Init();
    }
}