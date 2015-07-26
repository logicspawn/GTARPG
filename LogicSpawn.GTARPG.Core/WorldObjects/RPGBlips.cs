using GTA;
using GTA.Math;

namespace LogicSpawn.GTARPG.Core
{
    public class RPGBlips
    {
        public static BlipObject QuestArea(string name, Vector3 position)
        {
            var blip = World.CreateBlip(position);

            var blipObj = new BlipObject(name, blip);
            RPG.WorldData.AddBlip(blipObj);

            blip.Scale = 3f;
            blip.Sprite = BlipSprite.GTAOMission;
            blip.Color = BlipColor.Green;
            blip.Alpha = 180;
            return blipObj;
        }
        public static BlipObject QuestHandIn(string name, Vector3 position)
        {
            var blip = World.CreateBlip(position);

            var blipObj = new BlipObject(name, blip);
            RPG.WorldData.AddBlip(blipObj);

            blip.Scale = 2f;
            blip.Sprite = BlipSprite.DollarSign;
            blip.Color = BlipColor.Green;
            blip.Alpha = 180;
            return blipObj;
        }

            //b.SetAsHostile();
            //b.SetAsFriendly();
    }
}