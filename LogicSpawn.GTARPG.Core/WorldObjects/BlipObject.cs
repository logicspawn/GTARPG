using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;

namespace LogicSpawn.GTARPG.Core
{
    public class BlipObject : WorldObject
    {
        public new string Name;
        public Blip Blip;

        public override EntityType Type
        {
            get { return EntityType.Blip; }
        }

        public BlipObject()
        {

        }

        public BlipObject(string name, Blip blip)
            : base(blip.Handle)
        {
            Name = name;
            Blip = blip;
        }
        
        protected override void RemoveFromWorld()
        {
            if(Blip != null)
                if(Blip.Exists())
                {
                    //RPGLog.Log("Attempting to remove exisitng blip:" + Name);

                    Blip.Remove();
                    //RPGLog.Log("removed blip: " + Name);
                }
        }
    }
}