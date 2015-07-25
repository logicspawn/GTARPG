using System.Collections.Generic;
using System.Linq;
using GTA.Math;
using LogicSpawn.GTARPG.Core.Objects;

namespace LogicSpawn.GTARPG.Core.Repository
{
    public static class CutsceneRepository
    {
        public static List<Cutscene> Cutscenes;

        static CutsceneRepository()
        {
            Cutscenes = new List<Cutscene>();

            var cut1 = new Cutscene("");
            //cut1.AddCamera("Main", Vector3.Zero, Vector3.Zero)
            //    .AddObject()
        }

        public static Cutscene Get(string cutsceneId)
        {
            return Cutscenes.FirstOrDefault(i => i.CutsceneID == cutsceneId);
        }
    }
}