using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using LogicSpawn.GTARPG.Core.Scripts.Questing;

namespace LogicSpawn.GTARPG.Core.Repository
{
    public static class QuestRepository
    {
        public static List<Quest> Quests;

        static QuestRepository()
        {
            try
            {


                Quests = new List<Quest>();

                //Starting Quest
                Quests.Add(new Quest("Welcome to GTA:RPG", "Show me you've got what it takes.", false, false, 10, 500)
                               .AddCondiiton(QuestCondition.Custom("Complete the tutorial", "Tutorial", () => RPG.PlayerData.Tutorial.TutorialDoneExceptSpeak))
                               .WithOnStart(q => EventHandler.Do(o =>
                               {
                                   RPG.Subtitle("???: Welcome. Welcome. So you wanna make bank yeah?", 5000);
                                   EventHandler.Wait(5000);
                                   RPG.Subtitle("???: Alright, let's see what you've got. Complete some basic assignments and you're in.", 5000);
                                   EventHandler.Wait(5000);
                                   RPG.Subtitle("???: Good luck.", 5000);
                               }))
                               .WithOnComplete(q => EventHandler.Do(o =>
                                                                        {
                                                                            EventHandler.Wait(5000);
                                                                            RPG.PlayerData.Quests.First(qu => qu.Name == "The Grind Begins").Start();
                                                                        }))
                );
                Quests.Add(new Quest("The Grind Begins", "Complete a contract and return to Matthew.", false, false, 15, 600)
                               .AddCondiiton(QuestCondition.Custom("Contract completed","q_Start_contract", ()=> RPG.PlayerData.CompletedContracts > 0))
                               .WithOnStart(q => EventHandler.Do(o =>
                                                                      {
                                                                          RPG.Subtitle("???: I am Apex, the top of the food chain, leader of the Apexers. The one you want to impress.", 5000);EventHandler.Wait(5000);
                                                                          RPG.Subtitle("Apex: Matthew has told me what you've done, not bad... for a rookie.", 5000);EventHandler.Wait(5000);
                                                                          RPG.Subtitle("Apex: Access our network and purchase a contract through the actions interface.", 5000);EventHandler.Wait(5000);
                                                                          RPG.Subtitle("Apex: Hope those words weren't to big for you, get a contract done, and then return to Matthew.", 5000);EventHandler.Wait(5000);
                                                                      }))
                );
                Quests.Add(new Quest("Potential", "Apex and co. are impressed by your abilities. Pass their test and prove you're truly good.", false, false, 25, 800)
                               .AddCondiiton(QuestCondition.Acquire("Acquire the package", "Boxed Package",1))
                               .AddCondiiton(QuestCondition.Kill("Eliminate threats",5,new Vector3(-302,-1136,23),PedHash.Genstreet01AMO, PedHash.Genstreet01AMY)).WithSpawnedTargets()
                               .AddReward(QuestReward.Item("Bandages", 2), QuestReward.Item("Simple Protective Gear", 3))
                );
                Quests.Add(new Quest("Trouble in the Cap", "Help John Doe get some money so he can get some snacks.", false, false, 10, 800)
                               .AddCondiiton(QuestCondition.LootAnyPed("Wallets Stolen","Men's Wallet","prop_ld_wallet_02",80,5))
                               .AddCondiiton(QuestCondition.LootAnyPed("Purses Stolen","Woman's Purse","prop_ld_purse_01",80,5))
                               .AddReward(QuestReward.Item("Bandages", 2), QuestReward.Item("Simple Protective Gear", 3))
                               .WithOnComplete(q => EventHandler.Do(o =>
                               {
                                   EventHandler.Wait(5000);
                                   RPG.PlayerData.Quests.First(qu => qu.Name == "An Assassin's Greed").Start();
                               }))
                );
                Quests.Add(new Quest("An Assassin's Greed", "Assisinate the targets till you find the stolen phone", false, false, 15, 800)
                               .AddCondiiton(QuestCondition.Loot("Phone Recovered", "Incriminating Phone", "prop_npc_phone_02", 25, 1, PedHash.Genstreet01AMO, PedHash.Genstreet01AMY)).WithSpawnedTargets(1)
                               .AddReward(QuestReward.Item("Refurbished Kevlar", 5))
                               .WithOnComplete(q => EventHandler.Do(o =>
                               {
                                   RPG.Subtitle("Apex: Do you know what you have done? You've just killed one of our undercover agents inside CraftsSquad.", 5000); EventHandler.Wait(5000);
                                   RPG.Subtitle("Apex: Be happy you were just following orders, John will get the real smack for this.", 5000); EventHandler.Wait(5000);
                                   RPG.Subtitle("Apex: In the mean time you better sort out the armada that's converging on your position.", 5000); EventHandler.Wait(5000);
                                   RPG.Subtitle("Apex: You better talk to Alicia.", 5000); EventHandler.Wait(8000);
                                   RPG.Subtitle("Apex: Now.", 5000);
                               }))
                               .WithAutoComplete()
                );
                
                //-- John 'side-quest'
                Quests.Add(new Quest("Doe!", "Accept John's apology.", false, false, 25, 800)
                               .AddCondiiton(QuestCondition.Custom("Get to Jackson", "q_Reach_Jackson", () => Game.Player.Character.Position.DistanceTo(new Vector3(-567, -1071, 22)) < 5))
                               .AddReward(QuestReward.Item("Adv Health Kit", 5), QuestReward.Item("Adv Armor Kit", 5))
                );

                Quests.Add(new Quest("Smash the CraftSquad", "Take out the enemies of CraftSquad", false, false, 35, 800)
                               .AddCondiiton(QuestCondition.Kill("Targets Killed",20,PedHash.Genstreet01AMO, PedHash.Genstreet01AMY)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Adv Health Kit", 1), QuestReward.Item("Adv Armor Kit", 1))
                               .WithAutoComplete()
                               .WithOnComplete(q => EventHandler.Do(o =>
                               {
                                   RPG.Subtitle("Apex: Woah, you actually did it.", 3000); EventHandler.Wait(5000);
                                   RPG.Subtitle("Apex: Listen, good job, I'll have to think a while about this, you're suited for much bigger things.", 5000); EventHandler.Wait(5000);
                                   RPG.Subtitle("Apex: We have a heist coming up, but it's not ready yet, give me some time and I'll let you know.", 5000); EventHandler.Wait(5000);
                                   EventHandler.Wait(3000);
                                   RPG.GetPopup<MessageToPlayer>().Show();
                               }))
                );
                


                //Contracts:
                Quests.Add(new Quest("Are you Chicken?", "Cluck cluck cluck, these are worst than the cows... silence them", true, true, 15, 500)
                               .AddCondiiton(QuestCondition.Kill("Chickens silenced", 5, PedHash.Hen)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Bandages", 2), QuestReward.Item("Simple Protective Gear", 3))
                               .WithSpawnedTargets()
                );
                Quests.Add(new Quest("Moooooove them", "Mooo. Moo.. MOO? Mooo? Are you kidding me? Remove these pests.", true, true, 15, 500)
                               .AddCondiiton(QuestCondition.Kill("Moos silenced", 5, PedHash.Cow)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Bandages", 3))
                               .WithSpawnedTargets()
                );

                Quests.Add(new Quest("Boar'd", "We've got some boars roaming around, sort em out will ya?", true, true, 15, 500)
                               .AddCondiiton(QuestCondition.Kill("Boars eliminated", 5, PedHash.Boar)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Simple Protective Gear", 3))
                               .WithSpawnedTargets()
                );

                Quests.Add(new Quest("Makin' Bank", "Time to fill those pockets. Pick your targets yourself... Just get their cash!", true, true, 25, 1000)
                               .AddCondiiton(QuestCondition.LootAnyPed("Money bags acquired", "Sweet Green", "prop_money_bag_01", 70, 5))
                );

                Quests.Add(new Quest("Nacho' Cheese", "I just had a crate of promising goods stolen. Get them back for me will ya?", true, true, 50, 750)
                               .AddCondiiton(QuestCondition.Loot("Crate found", "'Promising Goods'", "prop_mil_crate_01", 20, 1, PedHash.StrPunk01GMY)).WithSpawnedTargets(1)
                               .AddReward(QuestReward.Item("Ammo Pack II", 2))
                               .WithSpawnedTargets()
                );
                Quests.Add(new Quest("Guitar Hero", "No need to fret. One of my ventures is in need of some shredding machines. Get them for me.", true, true, 50, 750)
                               .AddCondiiton(QuestCondition.Loot("Guitars Acquired", "Shredding Machine", "prop_el_guitar_01", 80, 5, PedHash.Paparazzi, PedHash.MovieStar, PedHash.PoloGoon01GMY)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Refurbished Kevlar", 5))
                               .WithSpawnedTargets()
                );

                Quests.Add(new Quest("Grandrune", "Buddy of mine starting playing a new game, he's a roleplayer you know? Get some cowhides for him.", true, true, 15, 650)
                               .AddCondiiton(QuestCondition.Loot("Cowhide @ 130gp ea", "Cowhide", "prop_paper_box_01", 100, 5, PedHash.Cow)).WithSpawnedTargets(5)
                               .WithSpawnedTargets()
                );
                Quests.Add(new Quest("Theftscape", "Buddy of mine starting playing a new game, he's a roleplayer you know? Get some feathers for him.", true, true, 15, 650)
                               .AddCondiiton(QuestCondition.Loot("Feathers collected", "Feather", "prop_cs_cardbox_01", 100, 50, PedHash.Hen)).WithSpawnedTargets(5)
                               .WithSpawnedTargets(10)
                );

                Quests.Add(new Quest("Fed up", "Let's make some noise! Grab some radios, because why not? Then get me some police sirens too", true, true, 75, 1500)
                               .AddCondiiton(QuestCondition.LootAnyVehicle("Radios Salvaged", "Car Radio", "prop_mil_crate_01", 40, 3))
                               .AddCondiiton(QuestCondition.LootVehicles("Sirens stolen", "Police Sirens", "prop_mil_crate_02", 100, 1,
                                                                         VehicleHash.Police, VehicleHash.Police2, VehicleHash.Police3, VehicleHash.Police4, VehicleHash.PoliceOld1,
                                                                         VehicleHash.PoliceOld2, VehicleHash.PoliceT, VehicleHash.Policeb))
                               .AddReward(QuestReward.Item("Adv Health Kit", 2), QuestReward.Item("Adv Armor Kit", 2))
                );

                Quests.Add(new Quest("Elimination", "You know what to do", true, true, 30, 500)
                               .AddCondiiton(QuestCondition.Kill("Kill Street Thugs", 5, PedHash.Genstreet01AMO, PedHash.Genstreet01AMY)).WithSpawnedTargets(5)
                               .AddCondiiton(QuestCondition.Kill("Kill Street Punks", 5, PedHash.StrPunk01GMY, PedHash.StrPunk01GMY)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Ammo Pack I", 1), QuestReward.Item("Health Kit", 5), QuestReward.Item("Adv Health Kit", 2), QuestReward.Item("Vehicle Repair Kit", 1))
                               .WithSpawnedTargets()
                );

                Quests.Add(new Quest("Gutter Trash", "Cleanse them. They are not as good as you.", true, true, 15, 500)
                               .AddCondiiton(QuestCondition.Kill("Street Thugs Cleansed", 5, PedHash.Genstreet01AMO, PedHash.Genstreet01AMY)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Ammo Pack I", 1), QuestReward.Item("Health Kit", 5))
                               .WithSpawnedTargets()
                );

                Quests.Add(new Quest("Punk'd", "These pests need to be killed. Why? You know better than to ask...", true, true, 15, 500)
                               .AddCondiiton(QuestCondition.Kill("Punk Eliminated", 5, PedHash.StrPunk01GMY, PedHash.StrPunk02GMY)).WithSpawnedTargets(5)
                               .AddReward(QuestReward.Item("Adv Health Kit", 2), QuestReward.Item("Vehicle Repair Kit", 1))
                               .WithSpawnedTargets()
                );

                Quests.Add(new Quest("Rampage", "Trevor would like this one. Kill and destroy. That is all.", true, true, 25, 1000)
                               .AddCondiiton(QuestCondition.KillAny("Mindless kills", 10))
                               .AddCondiiton(QuestCondition.DestroyAnyVehicle("Ruthless destruction", 5))
                               .AddReward(QuestReward.Item("Adv Health Kit", 2), QuestReward.Item("Vehicle Repair Kit", 1))
                );
            }
            catch(Exception ex)
            {
                RPGLog.LogError(ex.GetType() + ": " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        public static Action<Quest> GetStartAction(string name)
        {
            var quest = Quests.FirstOrDefault(i => i.Name == name);
            if (quest != null)
            {
                return quest.OnStart;
            }

            return null;
        }
        public static Action<Quest> GetCompleteAction(string name)
        {
            var quest = Quests.FirstOrDefault(i => i.Name == name);
            if (quest != null)
            {
                return quest.OnComplete;
            }

            return null;
        }

        public static bool GetCustomCondition(string questName, string name)
        {
            var quest = Quests.FirstOrDefault(i => i.Name == questName);
            if (quest != null)
            {
                var c =  quest.Conditions.FirstOrDefault(co => co.Name == name);
                if(c != null)
                {
                    return c.CustomCondition.Invoke();
                }
            }

            RPGLog.Log("Quest condition [" + name + "] not found for quest [" + questName + "]");
            return false;
        }
    }
}