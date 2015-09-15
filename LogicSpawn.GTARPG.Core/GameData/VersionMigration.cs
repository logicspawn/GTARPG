using System.Collections.Generic;
using System.Linq;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;

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
                case "0.1.12":
                    FixTutorial();
                    FixWeapons();
                    break;
            }

            if (dataVersion != version)
            {
                RPG.GetPopup<PatchNotes>().Show();
            }

            RPG.PlayerData.Version = RPG.Version;
        }

        private static void FixWeapons()
        {
            RPG.PlayerData.Weapons = new List<WeaponDefinition>();
            RPG.PlayerData.Weapons.AddRange(WeaponRepository.Weapons);
            RPG.PlayerData.GetWeapon(WeaponHash.Pistol).Unlocked = true;
            RPG.PlayerData.GetWeapon(WeaponHash.Pistol).AmmoCount = 100;
            RPG.PlayerData.GetWeapon(WeaponHash.AssaultRifle).Unlocked = true;
            RPG.PlayerData.GetWeapon(WeaponHash.AssaultRifle).AmmoCount = 1000;
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
            Migrate(RPG.PlayerData.Version, RPG.Version);
        }
    }
}
