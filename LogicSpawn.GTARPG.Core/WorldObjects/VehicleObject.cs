using GTA;
using GTA.Math;
using GTA.Native;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core
{
    public class VehicleObject : WorldObject
    {
        [JsonIgnore] public string VehicleName { get { return Veh.DisplayName; } }
        [JsonIgnore] public Vehicle Veh;

        public override EntityType Type
        {
            get { return EntityType.Vehicle; }
        }

        public VehicleObject()
        {

        }

        public VehicleObject(string name, Vehicle veh)
            : base(veh.Handle)
        {
            Name = name;
            Veh = veh;
        }
        
        protected override void RemoveFromWorld()
        {
            Veh.Delete();
        }
    }
}