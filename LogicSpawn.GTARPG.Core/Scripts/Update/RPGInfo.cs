using System.Linq;
using GTA;
using GTA.Native;

namespace LogicSpawn.GTARPG.Core
{
    public class RPGInfo : UpdateScript
    {
        public static bool IsWideScreen = true;
        public static Ped NearestPed = null;
        public static LootItem NearbyLoot = null;
        public static bool KeyboardActive = false;

        public override void Update()
        {
            Ped player = Game.Player.Character;
            NearestPed = World.GetNearbyPeds(player, 3).FirstOrDefault();
            NearbyLoot = PlayerMethods.GetNearbyLoot(2.5f).FirstOrDefault();
            IsWideScreen = Function.Call<bool>(Hash.GET_IS_WIDESCREEN);
            Wait(400);
        }

    }
}