using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA;

namespace LogicSpawn.GTARPG.Core
{
    public abstract class UpdateScript: Script
    {
//        private static bool Disposed;
        private bool _running;
        private bool _emptyUpdate = true;
        private bool _firstUpdate = true;
        protected virtual bool RunWhenGameIsNotLoaded { get { return false; } }
        protected virtual bool RunWhenPlayerDead { get { return false; } }
        protected virtual bool RunWhenCutscene { get { return false; } }

        public UpdateScript()
        {
            Tick += OnTick;
            Interval = 0;
            _running = true;
        }

        private void OnTick(object sender, EventArgs e)
        {
            //var s = new Stopwatch();
            //s.Start();

            if (CannotUpdate()) return;
            try
            {

                if (_emptyUpdate)
                {
                    _emptyUpdate = false; 
                    return;
                }

                if (_firstUpdate)
                {
                    Start();
                    _firstUpdate = false;
                }

                Update();
            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    RPGLog.LogError(ex.GetType() + ": " + ex.Message + "\n" + ex.StackTrace);
                    RPGLog.Log("Terminated script: " + GetType());
                    _running = false;
                }
            }

            //if (s.ElapsedMilliseconds > 0)
            //    RPGLog.Log(GetType() + " took " + s.ElapsedMilliseconds + " to tick.");
            
        }

        protected virtual void Start()
        {

        }


        private bool CannotUpdate()
        {
            if (!RPG.GameLoaded && !RunWhenGameIsNotLoaded) return true;
            if (RPG.PlayerDead && !RunWhenPlayerDead) return true;
            if (RPG.CutsceneRunning && !RunWhenCutscene) return true;
            if (!_running) return true;

            return false;
        }

        public void Run()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        public abstract void Update();
//
//        protected override void Dispose(bool A_0)
//        {
//            if (Disposed) return;
//                
//            Disposed = true;
//
//            if (!RPG.GameLoaded)
//            {
//                RPGLog.Log("Thread ended.");
//                return;
//            }
//
//            int objDestroyed = 0;
//            var count = RPG.WorldData.AllObjects;
//            while (RPG.WorldData.AllObjects.Any())
//            {
//                RPGLog.Log("destroying something!!");
//                RPG.WorldData.AllObjects.First().Destroy();
//                objDestroyed++;
//            }
//            RPGLog.Log("Cleaned up " + objDestroyed + "/" + count + " objects");
//
//            base.Dispose(A_0);
//
//        }
    }
}
