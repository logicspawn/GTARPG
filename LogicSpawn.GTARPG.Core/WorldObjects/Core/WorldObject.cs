using System;
using System.Collections.Generic;
using LogicSpawn.GTARPG.Core.General;

namespace LogicSpawn.GTARPG.Core
{
    public abstract class WorldObject
    {
        public string Name;
        public int EntityHandle;

        public WorldObject()
        {

        }

        public WorldObject(int Handle)
        {
            EntityHandle = Handle;
        }

        public void Destroy()
        {
            RemoveFromWorldObjects();
            //RPGLog.Log("Removed " + Name + " from world objects");
            if(Type == EntityType.Blip || EntityMethods.Exists(EntityHandle))
            {
                RemoveFromWorld();    
            }
            //RPGLog.Log("Removed " + Name + " from world");
        }

        protected void RemoveFromWorldObjects()
        {
            switch(Type)
            {
                case EntityType.Ped:
                    RPG.WorldData.RemoveNpc(this as NpcObject);
                    break;
                case EntityType.Prop:
                    RPG.WorldData.RemoveLoot(this as LootItem);
                    break;
                case EntityType.Vehicle:
                    RPG.WorldData.RemoveVehicle(this as VehicleObject);
                    break;
                case EntityType.Blip:
                    RPG.WorldData.RemoveBlip(this as BlipObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract EntityType Type { get; }
        protected abstract void RemoveFromWorld();
    }
}