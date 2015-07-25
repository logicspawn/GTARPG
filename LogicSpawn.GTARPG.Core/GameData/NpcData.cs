using GTA.Math;

namespace LogicSpawn.GTARPG.Core
{
    //todo: update to NpcObject in WorldData
    public class NpcData
    {
        public string Name;
        public string ModelName;
        public Vector3 Position; //could be positions, so we have the same npc spawn in differnt places(e.g. every gun shop)
        public float Heading;
        public bool Spawned;
    }
}