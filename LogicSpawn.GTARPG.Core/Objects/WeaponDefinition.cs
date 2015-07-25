using GTA.Native;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class WeaponDefinition
    {
        //[JsonConverter(typeof(StringEnumConverter))]
        public WeaponHash WeaponHash;
        public int AmmoCount;
        public bool Unlocked;
    }
}