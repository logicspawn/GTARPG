using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Scripts.Popups;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class WeaponDefinition
    {
        public WeaponHash WeaponHash;
        public string WeaponName = "Weapon";
        public string Description = "Weapon Desc.";
        public int PointsToUnlock;
        public int LevelToUnlock;
        public int AmmoCount;
        public bool Unlocked;

        public WeaponDefinition(string name, WeaponHash hash, string description, int pointstounlock, int leveltounlock)
        {
            WeaponName = name;
            WeaponHash = hash;
            Description = description;
            PointsToUnlock = pointstounlock;
            LevelToUnlock = leveltounlock;
        }

        public void Unlock()
        {
            if (RPG.PlayerData.SkillExp >= PointsToUnlock && RPG.PlayerData.Level >= LevelToUnlock)
            {
                RPG.PlayerData.SkillExp -= PointsToUnlock;
                Unlocked = true;
                RPG.GetPopup<WeaponUnlocked>().Show(this);
                EventHandler.Do(o =>
                {
                    Game.Player.Character.Weapons.Give(WeaponHash, 0, false, false);
                    Script.Wait(150);
                    Game.Player.Character.Weapons[WeaponHash].Ammo = 100;
                });
            }
            else
            {
                if (RPG.PlayerData.SkillExp < PointsToUnlock)
                {
                    RPG.Notify("Not enough SP");
                }
                else if(RPG.PlayerData.Level < LevelToUnlock)
                {
                    RPG.Notify("Requires Level " + LevelToUnlock);
                }

            }
        }
    }
}