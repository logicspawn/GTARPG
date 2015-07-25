using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace LogicSpawn.GTARPG.Core
{
    public static class PlayerMethods
    {
        public static IEnumerable<LootItem> GetNearbyLoot(float radius)
        {
            if(Game.Player.Character == null) return new List<LootItem>();

            var n = new List<LootItem>();


            foreach (var l in RPG.WorldData.Loot)
            {
                if(l.Prop == null) continue;

                if (World.GetDistance(Game.Player.Character.Position, l.Prop.Position) < radius)
                {
                    n.Add(l);
                }
            }

            if(n.Any())
            {
                var neabyLoot = n.OrderBy(l => World.GetDistance(Game.Player.Character.Position, l.Prop.Position));
                return neabyLoot;
            }
            
            return new List<LootItem>();
        }
    }
}
