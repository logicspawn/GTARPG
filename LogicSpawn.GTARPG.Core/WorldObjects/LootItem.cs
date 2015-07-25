using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core
{
    public class LootItem : WorldObject
    {
        [JsonIgnore]
        public new string Name { get { return Item.Name; } }
        public Item Item;
        [JsonIgnore] public Prop Prop;

        public override EntityType Type
        {
            get { return EntityType.Prop; }
        }

        public LootItem()
        {

        }

        public LootItem(Item item, Prop prop) : base(prop.Handle)
        {
            Item = item;
            Prop = prop;
        }
        
        protected override void RemoveFromWorld()
        {
            if(Prop.Exists())
                Prop.Delete();
        }
    }
}