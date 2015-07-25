using System.Collections.Generic;
using System.Linq;
using GTA;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core
{
    public class WorldData
    {
        public int Version = RPG.Version;
        public List<LootItem> Loot;
        public List<VehicleObject> Vehicles;
        public List<NpcObject> Npcs;
        public List<BlipObject> Blips;

        public WorldData()
        {
            Loot = new List<LootItem>();
            Vehicles = new List<VehicleObject>();
            Npcs = new List<NpcObject>();
            Blips = new List<BlipObject>();
        }

        public void AddPed(NpcObject n)
        {
            Npcs.Add(n);
        }

        public void AddLoot(LootItem l)
        {
            Loot.Add(l);
            TreatObjectAsDeletableIfTooMany(l);
        }


        public void AddVehicle(VehicleObject v)
        {
            if (v.Name == "rpg_PlayerVehicle")
            {
                var existingPlayerVehicle = Vehicles.FirstOrDefault(vh => vh.Name == "rpg_PlayerVehicle");
                var vx = Game.Player.Character.CurrentVehicle;
                if(existingPlayerVehicle != null &&  (vx == null ||  vx.Handle != existingPlayerVehicle.EntityHandle))
                {
                    existingPlayerVehicle.Destroy();
                }
                RPG.PlayerData.CurrentVehicle = v.Veh;
            }

            Vehicles.Add(v);

            if (v.Name != "rpg_PlayerVehicle")
                TreatObjectAsDeletableIfTooMany(v);
        }
        public void AddBlip(BlipObject b)
        {
            Blips.Add(b);
        }

        public void RemoveLoot(LootItem lootItem)
        {
            Loot.Remove(lootItem);
        }

        public void RemoveNpc(NpcObject npcObject)
        {
            Npcs.Remove(npcObject);
        }

        public void RemoveVehicle(VehicleObject veh)
        {
            Vehicles.Remove(veh);
        }

        public void RemoveBlip(BlipObject blipObject)
        {
            Blips.Remove(blipObject);
        }

        [JsonIgnore]
        public List<WorldObject> AllObjects
        {
            get { return Loot.Concat<WorldObject>(Vehicles).Concat<WorldObject>(Npcs).Concat<WorldObject>(Blips).ToList();}
        }


        protected List<WorldObject> _deletableObjects = new List<WorldObject>();

        public void TreatObjectAsDeletableIfTooMany(WorldObject w)
        {
            _deletableObjects.Add(w);
            if (_deletableObjects.Count > 50)
            {
                _deletableObjects[0].Destroy();
                _deletableObjects.RemoveAt(0);
            }
        }
    }
}