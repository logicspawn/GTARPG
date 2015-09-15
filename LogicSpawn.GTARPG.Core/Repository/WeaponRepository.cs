using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;

namespace LogicSpawn.GTARPG.Core.Repository
{
    public static class WeaponRepository
    {
        public static List<WeaponDefinition> Weapons;

        static WeaponRepository()
        {
            Weapons = new List<WeaponDefinition>();
            //Melee
            Weapons.Add(new WeaponDefinition("Knife", WeaponHash.Knife, "Standard sharp poky-thingy. Good for making people bleed.", 100, 17));
            Weapons.Add(new WeaponDefinition("Nightstick", WeaponHash.Nightstick, "Taken off the body of one of Los Santos' law enforcers.", 200, 18));
            Weapons.Add(new WeaponDefinition("Hammer", WeaponHash.Hammer, "Really, you're going to kill people with this? Nice.", 300, 19));
            Weapons.Add(new WeaponDefinition("Bat", WeaponHash.Bat, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Golf Club", WeaponHash.GolfClub, "Don't think you plan to hit golf balls with this.", 150, 19));
            Weapons.Add(new WeaponDefinition("Crowbar", WeaponHash.Crowbar, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Dagger", WeaponHash.Dagger, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Hatchet", WeaponHash.Hatchet, "What's this axe made of? Bronze? Iron? Rune?", 250, 20));
            //Sniper
            Weapons.Add(new WeaponDefinition("Sniper", WeaponHash.SniperRifle, "Hope you hit a few 360s with this one.", 100, 10));
            Weapons.Add(new WeaponDefinition("Heavy Sniper", WeaponHash.HeavySniper, "Hope you hit a few 720s with this one.", 300, 15));
            Weapons.Add(new WeaponDefinition("Marksman Rifle", WeaponHash.MarksmanRifle, "Prepare for plenty of hitmarkers.", 250, 12));
            //Rifle
            Weapons.Add(new WeaponDefinition("Assault Rifle", WeaponHash.AssaultRifle, "You're average run of the mill assault rifle.", 100, 1));
            Weapons.Add(new WeaponDefinition("Carbine Rifle", WeaponHash.CarbineRifle, "It's like the assault rifle but ... different.", 250, 3));
            Weapons.Add(new WeaponDefinition("Advanced Rifle", WeaponHash.AdvancedRifle, "Prepare to unleash some advanced warfare with this one.", 500, 4));
            Weapons.Add(new WeaponDefinition("Special Carbine", WeaponHash.SpecialCarbine, "It's S.P.E.C.I.A.L.", 400, 4));
            Weapons.Add(new WeaponDefinition("Bullpup Rifle", WeaponHash.BullpupRifle, "Your enemies will run like a bull when they see this one.", 250, 3));
            //SMG
            Weapons.Add(new WeaponDefinition("Micro SMG", WeaponHash.MicroSMG, "Pocket SMG for surprising your enemies.", 100, 5));
            Weapons.Add(new WeaponDefinition("SMG", WeaponHash.SMG, "Resembles an MP5, must be good.", 200, 6));
            Weapons.Add(new WeaponDefinition("Assault SMG", WeaponHash.AssaultSMG, "For when the SMG just doesn't cut it.", 100, 1));
            Weapons.Add(new WeaponDefinition("Combat PDW", WeaponHash.CombatPDW, "Personal Defence Weapon. Nice.", 100, 1));
            Weapons.Add(new WeaponDefinition("MG", WeaponHash.MG, "Great for spray and praying.", 250, 7));
            Weapons.Add(new WeaponDefinition("Combat MG", WeaponHash.CombatMG, "Spray and praying Mk II with increased spray mechanics.", 300, 9));
            Weapons.Add(new WeaponDefinition("Gusenberg Sweeper", WeaponHash.Gusenberg, "A weapon that can inflict damage.", 100, 1));
            //Pistol
            Weapons.Add(new WeaponDefinition("Pistol", WeaponHash.Pistol, "What's a gunslinger without a pistol?", 100, 1));
            Weapons.Add(new WeaponDefinition("Combat Pistol", WeaponHash.CombatPistol, "Take this into combat and you're good to go.", 100, 1));
            Weapons.Add(new WeaponDefinition("AP Pistol", WeaponHash.APPistol, "Like the pistol but automatic.", 100, 2));
            Weapons.Add(new WeaponDefinition("Pistol .50", WeaponHash.Pistol50, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Stun Gun", WeaponHash.StunGun, "Tzzz. 'OH GOD'. Tzzzz.", 100, 2));
            Weapons.Add(new WeaponDefinition("SNS Pistol", WeaponHash.SNSPistol, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Heavy Pistol", WeaponHash.HeavyPistol, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Vintage Pistol", WeaponHash.VintagePistol, "A weapon that can inflict damage.", 100, 1));
            //Throwables
            Weapons.Add(new WeaponDefinition("Sticky Bomb", WeaponHash.StickyBomb, "A weapon that can inflict damage.", 100, 3));
            Weapons.Add(new WeaponDefinition("Grenade", WeaponHash.Grenade, "Classic shrapnel filled boom-boom thingy", 150, 4));
            Weapons.Add(new WeaponDefinition("Smoke Grenade", WeaponHash.SmokeGrenade, "Tactical esponaige operations are a go.", 200, 5));
            Weapons.Add(new WeaponDefinition("BZ Gas", WeaponHash.BZGas, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Molotov", WeaponHash.Molotov, "Through the fire and the flames...", 250, 5));
            Weapons.Add(new WeaponDefinition("Fire Extinguisher", WeaponHash.FireExtinguisher, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Petrol Can", WeaponHash.PetrolCan, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Proximity Mine", WeaponHash.ProximityMine, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Snowball", WeaponHash.Snowball, "A weapon that can inflict damage.", 100, 1));
            //Heavy
            Weapons.Add(new WeaponDefinition("Grenade Launcher", WeaponHash.GrenadeLauncher, "Remember the grenade? Yeah this launches those.", 300, 17));
            Weapons.Add(new WeaponDefinition("Grenade Launcher (Smoke)", WeaponHash.GrenadeLauncherSmoke, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("RPG", WeaponHash.RPG, "Holy macaroni. This one's going to hurt.", 100, 15));
            Weapons.Add(new WeaponDefinition("Minigun", WeaponHash.Minigun, "Walk into a room full of enemies. Walk out of an empty room.", 500, 20));
            Weapons.Add(new WeaponDefinition("Homing Launcher", WeaponHash.HomingLauncher, "Good thing they don't have flares.", 400, 18));
            Weapons.Add(new WeaponDefinition("Firework Launcher", WeaponHash.Firework, "Celebrate good times come on!", 150, 16));
            Weapons.Add(new WeaponDefinition("Railgun MK 3000", WeaponHash.Railgun, "Do what you must.", 500, 25));
            //Shotguns
            Weapons.Add(new WeaponDefinition("Shotgun", WeaponHash.PumpShotgun, "Time to get up close and personal.", 100, 7));
            Weapons.Add(new WeaponDefinition("Sawn-Off Shotgun", WeaponHash.SawnOffShotgun, "Surely they can't survive this?", 300, 10));
            Weapons.Add(new WeaponDefinition("Assault Shotgun", WeaponHash.AssaultShotgun, "If at first you don't succeed.. shoot and shoot again.", 200, 8));
            Weapons.Add(new WeaponDefinition("Bullpup Shotgun", WeaponHash.BullpupShotgun, "It really does feel like a bull hit you when this lands.", 250, 9));
            Weapons.Add(new WeaponDefinition("Musket", WeaponHash.Musket, "A weapon that can inflict damage.", 100, 1));
            Weapons.Add(new WeaponDefinition("Heavy Shotgun", WeaponHash.HeavyShotgun, "A weapon that can inflict damage.", 100, 1));
        }

        public static WeaponDefinition Get(string name)
        {
            return GM.Copy(Weapons.FirstOrDefault(i => i.WeaponName == name));
        }
    }
}