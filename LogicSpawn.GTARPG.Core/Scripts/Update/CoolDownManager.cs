using System;
using System.Linq;

namespace LogicSpawn.GTARPG.Core
{
    public class CoolDownManager : UpdateScript
    {
        public CoolDownManager()
        {
            LastTime = DateTime.Now;
        }
        public DateTime LastTime;
        public override void Update()
        {
            var cooldowns = RPG.PlayerData.Inventory.Where(i => i.Usable).Select(i => i.CoolDownTimer)
                .Concat(RPG.PlayerData.Skills.Where(s => s.Unlocked).Select(s => s.CoolDownTimer));

            foreach (var cooldown in cooldowns)
            {
                cooldown.Current += (DateTime.Now - LastTime).Milliseconds;
                if (cooldown.Current > cooldown.CoolDownMsTime) cooldown.Current = cooldown.CoolDownMsTime;
            }

            LastTime = DateTime.Now;
        }
    }
}