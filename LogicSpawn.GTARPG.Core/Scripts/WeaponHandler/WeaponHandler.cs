using System.Drawing;
using GTA.Native;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;

namespace LogicSpawn.GTARPG.Core
{
    public class WeaponHandler
    {
        public TreeMenu GetWeaponMenu()
        {
            var weaponTree = new NTree(new Node("Pistol", WeaponHash.Pistol, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon), TreeType.Weapon, new Point(110, 350));
            weaponTree.AddChild(new Node("AP Pistol", WeaponHash.APPistol, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));
            weaponTree.AddChild(new Node("Stun Gun", WeaponHash.StunGun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));

            var rifles = weaponTree.AddChild(new Node("Assault Rifle", WeaponHash.AssaultRifle, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var throwables = rifles.AddChild(new Node("Sticky Bomb", WeaponHash.StickyBomb, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var smg = throwables.AddChild(new Node("Micro SMG", WeaponHash.MicroSMG, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var shotgun = smg.AddChild(new Node("Shotgun", WeaponHash.PumpShotgun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var snipers = shotgun.AddChild(new Node("Sniper", WeaponHash.SniperRifle, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var heavy = snipers.AddChild(new Node("RPG", WeaponHash.RPG, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var melee = heavy.AddChild(new Node("Knife", WeaponHash.Knife, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));
            var railgun = melee.AddChild(new Node("Railgun MK 3000", WeaponHash.Railgun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Right));


            rifles.AddChild(new Node("Bullpup Rifle", WeaponHash.BullpupRifle, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down))
                .AddChild(new Node("Advanced Rifle", WeaponHash.AdvancedRifle, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            rifles.AddChild(new Node("Carbine Rifle", WeaponHash.CarbineRifle, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up))
                .AddChild(new Node("Special Carbine", WeaponHash.SpecialCarbine, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));

            throwables.AddChild(new Node("Grenade", WeaponHash.Grenade, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down))
                .AddChild(new Node("Smoke Grenade", WeaponHash.SmokeGrenade, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            throwables.AddChild(new Node("Molotov", WeaponHash.Molotov, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));


            smg.AddChild(new Node("SMG", WeaponHash.SMG, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            smg.AddChild(new Node("MG", WeaponHash.MG, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up))
                .AddChild(new Node("Combat MG", WeaponHash.CombatMG, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));


            shotgun.AddChild(new Node("Assault Shotgun", WeaponHash.AssaultShotgun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            shotgun.AddChild(new Node("Bullpup Shotgun", WeaponHash.BullpupShotgun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up))
                .AddChild(new Node("Sawn-Off Shotgun", WeaponHash.SawnOffShotgun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));


            snipers.AddChild(new Node("Heavy Sniper", WeaponHash.HeavySniper, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            snipers.AddChild(new Node("Marksman Rifle", WeaponHash.MarksmanRifle, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));


            heavy.AddChild(new Node("Firework Launcher", WeaponHash.Firework, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down))
                .AddChild(new Node("Homing Launcher", WeaponHash.HomingLauncher, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            heavy.AddChild(new Node("Grenade Launcher", WeaponHash.GrenadeLauncher, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up))
                .AddChild(new Node("Minigun", WeaponHash.Minigun, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));

            melee.AddChild(new Node("Nightstick", WeaponHash.Nightstick, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down))
                .AddChild(new Node("Hammer", WeaponHash.Hammer, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Down));
            melee.AddChild(new Node("Golf Club", WeaponHash.GolfClub, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up))
                .AddChild(new Node("Hatchet", WeaponHash.Hatchet, new GTASprite("mpweaponscommon_small", "w_sb_assaultsmg"), NodeType.Weapon, TreeDirection.Up));
            //var weaponTree = new NTree(new Node("Pistol", WeaponHash.Pistol, new GTASprite("srange_weap", "weap_hg_1"), NodeType.Weapon), TreeType.Weapon, new Point(110, 350));
            //weaponTree.AddChild(new Node("AP Pistol", WeaponHash.APPistol, new GTASprite("srange_weap", "weap_hg_2"), NodeType.Weapon, TreeDirection.Up));
            //weaponTree.AddChild(new Node("Stun Gun", WeaponHash.StunGun, new GTASprite("mpweaponsgang0_small", "w_pi_stungun"), NodeType.Weapon, TreeDirection.Down));
            //
            //var rifles = weaponTree.AddChild(new Node("Assault Rifle", WeaponHash.AssaultRifle, new GTASprite("srange_weap", "weap_ar_1"), NodeType.Weapon, TreeDirection.Right));
            //var throwables = rifles.AddChild(new Node("Sticky Bomb", WeaponHash.StickyBomb, new GTASprite("mpweaponscommon_small", "w_ex_pe"), NodeType.Weapon, TreeDirection.Right));
            //var smg = throwables.AddChild(new Node("Micro SMG", WeaponHash.MicroSMG, new GTASprite("srange_weap2", "weap_smg_1"), NodeType.Weapon, TreeDirection.Right));
            //var shotgun = smg.AddChild(new Node("Shotgun", WeaponHash.PumpShotgun, new GTASprite("srange_weap2", "weap_sg_1"), NodeType.Weapon, TreeDirection.Right));
            //var snipers = shotgun.AddChild(new Node("Sniper", WeaponHash.SniperRifle, new GTASprite("mpweaponsgang0_small", "w_sr_sniperrifle"), NodeType.Weapon, TreeDirection.Right));
            //var heavy = snipers.AddChild(new Node("RPG", WeaponHash.RPG, new GTASprite("mpweaponscommon_small", "w_lr_rpg"), NodeType.Weapon, TreeDirection.Right));
            //var melee = heavy.AddChild(new Node("Knife", WeaponHash.Knife, new GTASprite("mpweaponsunusedfornow", "w_me_knife_01"), NodeType.Weapon, TreeDirection.Right));
            //var railgun = melee.AddChild(new Node("Railgun MK 3000", WeaponHash.Railgun, new GTASprite("mpweaponscommon_small", "weap_rg_1"), NodeType.Weapon, TreeDirection.Right));
            //
            //
            //rifles.AddChild(new Node("Bullpup Rifle", WeaponHash.BullpupRifle, new GTASprite("srange_weap", "weap_ar_5"), NodeType.Weapon, TreeDirection.Down))
            //    .AddChild(new Node("Advanced Rifle", WeaponHash.AdvancedRifle, new GTASprite("srange_weap", "weap_ar_3"), NodeType.Weapon, TreeDirection.Down));
            //rifles.AddChild(new Node("Carbine Rifle", WeaponHash.CarbineRifle, new GTASprite("srange_weap", "weap_ar_2"), NodeType.Weapon, TreeDirection.Up))
            //    .AddChild(new Node("Special Carbine", WeaponHash.SpecialCarbine, new GTASprite("srange_weap", "weap_ar_6"), NodeType.Weapon, TreeDirection.Up));
            //
            //throwables.AddChild(new Node("Grenade", WeaponHash.Grenade, new GTASprite("mpweaponsgang0_small", "w_ex_grenadefrag"), NodeType.Weapon, TreeDirection.Down))
            //    .AddChild(new Node("Smoke Grenade", WeaponHash.SmokeGrenade, new GTASprite("mpweaponscommon_small", "w_ex_grenadesmoke"), NodeType.Weapon, TreeDirection.Down));
            //throwables.AddChild(new Node("Molotov", WeaponHash.Molotov, new GTASprite("mpweaponsunusedfornow", "w_ex_molotov"), NodeType.Weapon, TreeDirection.Up));
            //
            //
            //smg.AddChild(new Node("SMG", WeaponHash.SMG, new GTASprite("srange_weap2", "weap_smg_2"), NodeType.Weapon, TreeDirection.Down));
            //smg.AddChild(new Node("MG", WeaponHash.MG, new GTASprite("srange_weap2", "weap_lmg_1"), NodeType.Weapon, TreeDirection.Up))
            //    .AddChild(new Node("Combat MG", WeaponHash.CombatMG, new GTASprite("srange_weap2", "weap_lmg_2"), NodeType.Weapon, TreeDirection.Up));
            //
            //
            //shotgun.AddChild(new Node("Assault Shotgun", WeaponHash.AssaultShotgun, new GTASprite("srange_weap2", "weap_sg_3"), NodeType.Weapon, TreeDirection.Down));
            //shotgun.AddChild(new Node("Bullpup Shotgun", WeaponHash.BullpupShotgun, new GTASprite("srange_weap2", "weap_sg_4"), NodeType.Weapon, TreeDirection.Up))
            //    .AddChild(new Node("Sawn-Off Shotgun", WeaponHash.SawnOffShotgun, new GTASprite("srange_weap2", "weap_sg_2"), NodeType.Weapon, TreeDirection.Up));
            //
            //
            //snipers.AddChild(new Node("Heavy Sniper", WeaponHash.HeavySniper, new GTASprite("mpweaponsgang0_small", "w_sr_heavysniper"), NodeType.Weapon, TreeDirection.Down));
            //snipers.AddChild(new Node("Marksman Rifle", WeaponHash.MarksmanRifle, new GTASprite("mpweaponsgang0_small", "ocelot"), NodeType.Weapon, TreeDirection.Up));
            //
            //
            //heavy.AddChild(new Node("Firework Launcher", WeaponHash.Firework, new GTASprite("mpawards1", "parachut1min"), NodeType.Weapon, TreeDirection.Down))
            //    .AddChild(new Node("Homing Launcher", WeaponHash.HomingLauncher, new GTASprite("mpweaponscommon_small", "rocket"), NodeType.Weapon, TreeDirection.Down));
            //heavy.AddChild(new Node("Grenade Launcher", WeaponHash.GrenadeLauncher, new GTASprite("mpweaponscommon_small", "w_r_grenadelauncher"), NodeType.Weapon, TreeDirection.Up))
            //    .AddChild(new Node("Minigun", WeaponHash.Minigun, new GTASprite("mpweaponscommon_small", "weap_hvy_1"), NodeType.Weapon, TreeDirection.Up));
            //
            //melee.AddChild(new Node("Nightstick", WeaponHash.Nightstick, new GTASprite("mpweaponsunusedfornow", "w_me_nightstick"), NodeType.Weapon, TreeDirection.Down))
            //    .AddChild(new Node("Hammer", WeaponHash.Hammer, new GTASprite("mpweaponsunusedfornow", "w_me_hammer"), NodeType.Weapon, TreeDirection.Down));
            //melee.AddChild(new Node("Golf Club", WeaponHash.GolfClub, new GTASprite("mpweaponsunusedfornow", "w_me_gclub"), NodeType.Weapon, TreeDirection.Up))
            //    .AddChild(new Node("Hatchet", WeaponHash.Hatchet, new GTASprite("mpweaponsunusedfornow", "w_me_fireaxe"), NodeType.Weapon, TreeDirection.Up));

            return new TreeMenu(weaponTree);
        }
    }
}