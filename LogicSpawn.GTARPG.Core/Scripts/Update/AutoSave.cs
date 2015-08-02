using System;

namespace LogicSpawn.GTARPG.Core
{
    public class AutoSave : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return false; } }
        
        public override void Update()
        {
            if (!RPGSettings.EnableAutoSave) return;
            if (!RPG.GameLoaded || !RPG.LoadedSuccessfully || CharCreationNew.Enabled) return;

            Wait(RPGSettings.AutosaveInterval * 1000);
            if (!RPGSettings.EnableAutoSave) return;
            if (!RPG.GameLoaded || !RPG.LoadedSuccessfully || CharCreationNew.Enabled) return;

            while(RPG.GameLoaded)
            {
                RPG.SaveAllData();
                Wait(RPGSettings.AutosaveInterval * 1000);
            }
        }



    }
}