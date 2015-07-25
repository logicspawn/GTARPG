using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;

namespace LogicSpawn.GTARPG.Core
{
    public class CutsceneHandler : UpdateScript
    {
        protected override bool RunWhenCutscene
        {
            get { return true; }
        }

        public Cutscene CurrentCutscene;
        public List<Vehicle> NeededVehicles;
        public List<Ped> NeededPeds;

        public CutsceneHandler()
        {
            RPG.CutsceneHandler = this;
        }

        public bool RunCutscene(string cutsceneID)
        {
            if (RPG.CutsceneRunning) return false;

            var cutscene = CutsceneRepository.Get(cutsceneID);
            if (cutscene == null) return false;

            PrepareForCutscene();
            StartCutscene(cutscene);

            return true;
        }

        private void PrepareForCutscene()
        {
            RPG.CutsceneRunning = true;

            Game.FadeScreenOut(500);
            Wait(500);

            var neededEntities = CurrentCutscene.NeededEntities;
            NeededVehicles = neededEntities.Where(e => e.Model.IsVehicle).Select(e => (Vehicle)e).ToList();
            NeededPeds = neededEntities.Where(e => e.Model.IsPed).Select(e => (Ped)e).ToList();

            Function.Call(Hash.DISPLAY_HUD, 0);
            Function.Call(Hash.DISPLAY_RADAR, 0);
            RPG.UIHandler.ShowUI = false;
        }

        private void StartCutscene(Cutscene cutscene)
        {
            cutscene.Running = false;

            CurrentCutscene = cutscene;

            CurrentCutscene.OnStart();
        }

        private void EndCutscene()
        {
            Game.FadeScreenOut(500);
            Wait(500);
            
            World.DestroyAllCameras();

            CurrentCutscene.OnEnd();
            
            Game.FadeScreenIn(500);
            Wait(500);

            //Re-enable UI
            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            RPG.UIHandler.ShowUI = true;

            NeededPeds = null;
            NeededVehicles = null;
            CurrentCutscene = null;
            RPG.CutsceneRunning = false;
        }

        public override void Update()
        {
            if (CurrentCutscene == null) return;

            Game.FadeScreenIn(500);
            Wait(500);

            while(CurrentCutscene.Running)
            {
                PauseUnneededEntities();
                CurrentCutscene.Run();
                Wait(0);
            }
            
            EndCutscene();
        }

        private void PauseUnneededEntities()
        {
            //var allPeds = World.GetNearbyPeds(Game.Player.Character, 1000);
            //var allVehs = World.GetNearbyVehicles(Game.Player.Character, 1000);

            //var unneededPeds = allPeds.Where(e => NeededPeds.All(n => n != e));
            //var unneededVehs = allVehs.Where(e => NeededVehicles.All(n => n != e));

            //foreach(var ped in unneededPeds)
            //{
            //    ped.FreezePosition = true;
            //    ped.Task.WanderAround();
            //}

            //foreach(var veh in unneededVehs)
            //{
            //    veh.FreezePosition = true
            //}

            //get all peds and vehicles and pause them
            //unpause included peds
        }
    }
}