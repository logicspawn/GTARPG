using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicSpawn.GTARPG.Core.Repository;

namespace LogicSpawn.GTARPG.Core.GameData
{
    public static class VersionMigration
    {
        public static void Migrate(string dataVersion, string version)
        {
            switch(dataVersion)
            {
                case "0.1":
                    Migrate_v010_To_v014();
                    break;
            }
        }

        private static void Migrate_v010_To_v014()
        {
            foreach(var q in RPG.PlayerData.Quests)
            {
                var exisitingQuest = QuestRepository.Quests.FirstOrDefault(qu => qu.Name == q.Name);
                q.SpawnTargets = exisitingQuest.SpawnTargets;
                q.AmountToSpawn = exisitingQuest.AmountToSpawn;
                q.Cancellable = exisitingQuest.Cancellable;
                q.CreateHandInBlip = exisitingQuest.CreateHandInBlip;
                q.HandInBlipPosition = exisitingQuest.HandInBlipPosition;
            }

            foreach(var skill in RPG.PlayerData.Skills)
            {
                var exisitingSkill = SkillRepository.Skills.FirstOrDefault(s => s.Name == skill.Name);
                skill.PointsToUnlock = exisitingSkill.PointsToUnlock;
            }

            RPG.PlayerData.Version = "0.1.4";
        }
    }
}
