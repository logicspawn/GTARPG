using System;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class CoolDown
    {
        [JsonIgnore]
        public int Current;
        public int CoolDownMsTime;

        [JsonIgnore]
        public bool Usable
        {
            get { return Current >= CoolDownMsTime; }
        }

        [JsonIgnore]
        public float SecondsRemaining
        {
            get
            {
                var secs = ((float)CoolDownMsTime - Current)/1000 + 0.5f;
                return secs;
            }
        }


        public CoolDown()
        {
            
        }

        public CoolDown(int msTime)
        {
            CoolDownMsTime = msTime;
        }

        public void Reset()
        {
            Current = 0;
        }
    }
}