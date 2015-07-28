using GTA.Native;

namespace LogicSpawn.GTARPG.Core
{
    public class GeneralUpdate : UpdateScript
    {
        public override void Update()
        {
            if (RPG.GameMode == GameMode.FullRPG || CharCreationNew.Enabled)
            {
                Function.Call(Hash.DESTROY_MOBILE_PHONE);
            }
        }
    }
}