using System;

namespace LogicSpawn.GTARPG.Core
{
    public class AutoSave : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return false; } }
        
        public override void Update()
        {
            if (!RPG.LoadedSuccessfully || CharCreationNew.Enabled) return;

            Wait(30000);
            if (!RPG.LoadedSuccessfully || CharCreationNew.Enabled) return;

            while(RPG.GameLoaded)
            {
                RPG.SaveAllData();
                Wait(30000);
            }
        }



    }
}