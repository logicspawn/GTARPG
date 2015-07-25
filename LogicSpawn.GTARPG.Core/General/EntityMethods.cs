using System;
using GTA;
using GTA.Native;

namespace LogicSpawn.GTARPG.Core.General
{
    public static class EntityMethods
    {
        public static bool Exists(int handle)
        {
            return Function.Call<bool>(Hash.DOES_ENTITY_EXIST, handle);
        }

        public static bool Exists(int handle, int modelHash, EntityType type)
        {
            return Function.Call<bool>(Hash.DOES_ENTITY_EXIST, handle);

            switch (type)
            {
                case EntityType.Ped:
                    return Entity.Exists(new Ped(handle)) && new Ped(handle).Model.Hash == modelHash;
                case EntityType.Prop:
                    return Entity.Exists(new Prop(handle)) && new Prop(handle).Model.Hash == modelHash;;
                case EntityType.Vehicle:
                    return Entity.Exists(new Vehicle(handle)) && new Vehicle(handle).Model.Hash == modelHash;;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }
    }
}
