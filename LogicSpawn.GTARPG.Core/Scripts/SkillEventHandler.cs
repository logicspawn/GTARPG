using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogicSpawn.GTARPG.Core
{
    public class SkillEventHandler : UpdateScript
    {
        public static Dictionary<Action<object>, object> a = new Dictionary<Action<object>, object>();
        public override void Update()
        {
            if(a.Any())
            {
                var kvp = a.First();                
                System.EventHandler o = null;
                o = (sender, args) => { 
                    kvp.Key(kvp.Value);
                    Tick -= o;
                };

                Tick += o;
                a.Remove(kvp.Key);
            }
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {

            Tick -= OnTick;
        }

        public static void Do(Action<object> action, object parameter = null)
        {
            a.Add(action, parameter);
        }
    }
}