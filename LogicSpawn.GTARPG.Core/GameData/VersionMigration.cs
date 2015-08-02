using System.Linq;
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
                case "0.1.1":
                case "0.1.2":
                case "0.1.3":
                case "0.1.4":
                case "0.1.5":
                case "0.1.6":
                case "0.1.7":
                case "0.1.8":
                case "0.1.9":
                case "0.1.10":
                case "0.1.11":
                    FixTutorial();
                    break;
            }
        }

        private static void FixTutorial()
        {
            if(RPG.PlayerData.Quests.First(q => q.Name == "The Grind Begins").Done)
            {
                RPG.PlayerData.Tutorial.LearntAboutIcons = true;
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
