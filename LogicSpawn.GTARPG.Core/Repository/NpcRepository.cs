using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

namespace LogicSpawn.GTARPG.Core.Repository
{
    public static class NpcRepository
    {
        public static List<NpcObject> Npcs;

        static NpcRepository()
        {
            Npcs = new List<NpcObject>();

            var npc1 = new NpcObject("Matthew", PedHash.Omega, new Vector3(-8.9106f, -1090.779f, 26.6720f), 90.2f);
            npc1.SetBlip(BlipSprite.Marijuana);
            npc1.SetQuestsToGive("Potential", "An Assassin's Greed");
            npc1.SetPersonalQuestLine("Welcome to GTA:RPG", "The Grind Begins", "Potential","Trouble in the Cap", "An Assassin's Greed");
            npc1.SetQuestHandIns("Welcome to GTA:RPG","The Grind Begins");
            npc1.SetSideQuests();
            npc1.SetQuestConditions();
            npc1.SetDialog("Can I Help you?",
                           Response("I'm still working on the assignments ...", 1).WithCondition(ResponseCondition.QuestInProgress("Welcome to GTA:RPG")),
                           Response("About these assignments... They're impossible.", 0).WithCondition(ResponseCondition.QuestInProgress("Welcome to GTA:RPG")),
                           Response("Listen... I'm done with the assignments.", 2).WithCondition(ResponseCondition.QuestConditionsDone("Welcome to GTA:RPG")),
                           Response("Apex told me to speak with you.", 99).WithCondition(ResponseCondition.QuestConditionsDone("The Grind Begins")),
                           Response("I want to prove myself.", 5).WithConditions(ResponseCondition.QuestComplete("The Grind Begins"), ResponseCondition.QuestNotInProgressOrDone("Potential")),
                           Response("I'm ready for those assassins", ResponseAction.Start_Quest, "An Assassin's Greed").WithConditions(ResponseCondition.QuestComplete("Trouble in the Cap"), ResponseCondition.QuestNotInProgressOrDone("An Assassin's Greed")),
                           Response("Later Homie", ResponseAction.End),
                           Response("You sellin'?", ResponseAction.Vendor))
                .Add(0, "Nothing is impossible. Don't let your dreams be dreams. JUST DO IT.",
                     Response("K, I'll just do it.", ResponseAction.End))
                .Add(1, "Just get it done, or there's nothing to say bro",
                     Response("Ite", ResponseAction.End))
                .Add(2, "Damn dude, you're legit. I'll let our boss know what's up",
                     Response("Good shit", ResponseAction.Finish_Quest, "Welcome to GTA:RPG"))
                .Add(99, "Alright alright homeslice, I can see from your face you've been busy",
                     Response("I'm good at this, what can I say?", 3),
                     Response("Yeah yeah, give me a reward already.", 3),
                     Response("I'm ready for the next task", ResponseAction.Finish_Quest, "The Grind Begins"))
                .Add(3, "Woah easy there. Our boy here's completed one contract and thinks he King B or something.",
                     Response("What's next?", 4))
                .Add(4, "Take this and speak to me again when you're ready for some big things.",
                     Response("[Take Items]", ResponseAction.Finish_Quest, "The Grind Begins"))
                .Add(5, "This sounds like a good mission for you to do. Might get bloody. Interested?",
                     Response("Let's do it", 6),
                     Response("Never mind, I'm not ready for that", ResponseAction.End))
                .Add(6, "Alright, so I need you to go and find John Doe, he'll have a package for you. Take the package and get it to Jackson.",
                     Response("Is that it?", 7))
                .Add(7, "Well the problem is we've spotted some trouble in between John and Jackson.",
                     Response("You mean they're beefin'?", 8),
                     Response("Whatever, ima go and do this", 9))
                .Add(8, "What? Nah man, John and Jackson are good friends man. We got gangbangers on the road that need sorting out. You good for that?",
                     Response("I'll handle it.", 10))
                .Add(9, "Fine. Just make sure you take out the other gang members on your way. That sound alright?",
                     Response("Oh I'm sorry, I thought you just meant they were beefin'", 8))
                .Add(10, "Right, he's the SMG symbol. You know how to use your map right?",
                     Response("Damn straight, I'll head there now", ResponseAction.Start_Quest, "Potential"));

            var npc2 = new NpcObject("King B", PedHash.JanitorSMM, new Vector3(-65f, -1202f, 27f), 135f);
            npc2.SetBlip(BlipSprite.Store);
            npc2.SetAsVendor();
            npc2.SetDialog("What's good my man?",
                                Response("Later Homie", ResponseAction.End),
                                Response("Who are you?", 0),
                                Response("You sellin'?", ResponseAction.Vendor))
                .Add(0, "Me? I'm King B brother! King of boom, king of bang, king of bow chica wow wow.", 
                    Response("What does that mean?", 1))
                .Add(1, "It means I make things happen, it means I get the girl at the end of the movie", 
                    Response("No really, what does it mean?", 2))
                .Add(2, "Shit bro. It means if you need something I'm your man. I've got tons of goods to haul here.", 
                    Response("Right, so you sell stuff?", 3))
                .Add(3, "Yeah.", 
                    Response("Alright homie, should have said.", ResponseAction.Vendor));

            var npc3 = new NpcObject("John Doe", PedHash.PoloGoon01GMY, new Vector3(-67f, -1208f, 28f), 312f);
            npc3.SetBlip(BlipSprite.SMG);
            npc3.SetQuestsToGive("Trouble in the Cap");
            npc3.SetPersonalQuestLine("Potential", "Trouble in the Cap", "An Assassin's Greed", "Doe!");
            npc3.SetQuestHandIns("Trouble in the Cap");
            npc3.SetSideQuests("Doe!");
            npc3.SetQuestConditions(Condition("Potential", "Acquire the package"));
            npc3.SetDialog("Yes? Keep it simple please.",
                           Response("Matthew sent me to get a package.", 1).WithCondition(ResponseCondition.QuestInProgress("Potential")),
                           Response("You need some help?", 8).WithConditions(ResponseCondition.QuestComplete("Potential"), ResponseCondition.QuestNotInProgressOrDone("Trouble in the Cap")),
                           Response("The fuck did you make me do man?", 19).WithConditions(ResponseCondition.QuestComplete("An Assassin's Greed"), ResponseCondition.QuestNotInProgressOrDone("Doe!")),
                           Response("Here's your 'Money holders'", 13).WithConditions(ResponseCondition.QuestConditionsDone("Trouble in the Cap")),
                           Response("Sorry, never mind", ResponseAction.End))
                .Add(1, "Oh really? You're that new recruit that Apex was talking about. How are you finding the criminal life?",
                     Response("Well I've been doing this a long time...", 2),
                     Response("It's alright, I was hoping to make more money though", 3),
                     Response("You gonna keep dicking about or hand over the package?", 4))
                .Add(2, "And here you are, standing before the Apexers.",
                     Response("There's only so much a single man can do.", 5),
                     Response("...", 6))
                .Add(3, "In due time, in due time. Take the package and good luck",
                     Response("[Take Package]", ResponseAction.Custom_End, () => RPG.PlayerData.AddItem(ItemRepository.Get("Boxed Package"))))
                .Add(4, "Damn son, you've got a bit of a bite to you. Don't try that shit with me or you'll be out of here before you know it",
                     Response("Who the fuck do you think you are? You wanna go right here?", 7),
                     Response("Woah chill, I didn't mean to piss you off man", 6))
                .Add(5, "I like the way you think. Most new recruits would've crumbled under that question. Take this and don't die alright?",
                     Response("[Take Package]", ResponseAction.Custom_End, () => RPG.PlayerData.AddItem(ItemRepository.Get("Boxed Package"))))
                .Add(6, "Don't worry, I'm just playing man. Anyways, here's the package.",
                     Response("[Take Package]", ResponseAction.Custom_End, () => RPG.PlayerData.AddItem(ItemRepository.Get("Boxed Package"))))
                .Add(7, "Ha, you're not even kidding around. Well you've earned my respect man. Chill down though, this works better if we get along. Here.",
                     Response("That's what's up!", ResponseAction.Custom_End, () => RPG.PlayerData.AddItem(ItemRepository.Get("Boxed Package"))))

                .Add(8, "Quite simply, yeah. I need some cash. You game?",
                     Response("Sure let's do it.", 9),
                     Response("Let's talk about something else.", ResponseAction.Return_To_Start))
                .Add(9, "Alright, it's simple, just cap some dudes and grab their money holders.",
                     Response("Just a couple of wallets? Alright no problem", 10))
                .Add(10, "You know what, make it some purses too, I like you man, and I wanna make sure you cover all the bases",
                     Response("What do you mean by that?", 11),
                     Response("Whatever, no difference to me", ResponseAction.Start_Quest, "Trouble in the Cap"))
                .Add(11, "One guy came through here, big as day. Killed a few guys, came up against a woman, hesitated and she killed him in seconds.",
                     Response("That won't happen to me, no one takes me down", 12))
                .Add(12, "Haha. Alicia would laugh if she heard you say that. Anyways, you doing this or what?",
                     Response("Sure, I'm down", ResponseAction.Start_Quest, "Trouble in the Cap"),
                     Response("Let's talk about something else.", ResponseAction.Return_To_Start))

                .Add(13, "What? They hold money, anyways thanks man.",
                     Response("What's next homie? Any more help?", 14))
                .Add(14, "Well funny story right, Apex told me to cap some dude who has one of our members' phone, real sensitive information...",
                     Response("Tell me who, I've got this", 15),
                     Response("Ok, so...", 15))

                .Add(15, "Well I wasn't paying attention, missed the name. It's one of those CraftSquad dudes.",
                     Response("Who's CraftSquad?", 16),
                     Response("So what do you want me to do?", 17))
                .Add(16, "Some wannabe gangbangers, they aint got shit on us. Well, other than the incriminating cell phone about our activities...",
                     Response("Relax man, what's the plan?", 17))

                .Add(17, "Just go and kill anyone you can find that's part of CraftSquad, I'm sure it'll turn up",
                     Response("Got it.", 18))
                .Add(18, "Oh yeah, and take this",
                     Response("[Take Reward]", ResponseAction.Finish_Quest, "Trouble in the Cap"))
                .Add(19, "I'm so sorry, please accept my sincerest apology man. I dun goofed, I know it, I admit it. My bad man.",
                     Response("You think a fucking sorry is gonna make this alright?", 20),
                     Response("Straight up, fuck you man", 20),
                     Response("Jeez man, next time do your job properly", 20))
                .Add(20, "Listen dude, I've sorted it with Apex, you won't get any of the heat man, it's all good... for you.",
                     Response("Best be. I'm tryna make a name for myself here", 21),
                     Response("I hope this is a lesson to you", 22))
                .Add(21, "I feel you, i feel you. Let me make it up to you",
                     Response("I'm listening", 23))
                .Add(22, "I've learned my lesson, shit, this is my only slip up in 6 months. Let me make it up to you.",
                     Response("Go on", 23))
                .Add(23, "Go to Jackson and tell him 'The Golden Doe has landed' he'll hook you up.",
                     Response("This better be worth it.", ResponseAction.Start_Quest, "Doe!"));

            var npc4 = new NpcObject("Alicia", PedHash.Mistress, new Vector3(-53f, -1216f, 28f), 56f);
            npc4.SetBlip(BlipSprite.Sniper);
            npc4.SetQuestsToGive("Smash the CraftSquad");
            npc4.SetPersonalQuestLine("An Assassin's Greed", "Smash the CraftSquad");
            npc4.SetQuestHandIns();
            npc4.SetSideQuests();
            npc4.SetQuestConditions();
            npc4.SetDialog("You can stand there and look at me or speak up. Just don't waste my time.",
                           Response("What do you do around here?", 1),
                           Response("Apex sounded pissed, what's going on?", 3).WithConditions(ResponseCondition.QuestComplete("An Assassin's Greed"), ResponseCondition.QuestNotInProgressOrDone("Smash the CraftSquad")),
                           Response("Later Homie", ResponseAction.End))
                .Add(0, "Nothing is impossible. Don't let your dreams be dreams bruh. JUST DO IT.",
                     Response("K, I'll just do it.", ResponseAction.End))
                .Add(1, "I'm Alicia. That's all you need to know right now.",
                     Response("Maybe I can get to know you a bit more...", 2),
                     Response("OK", ResponseAction.End))
                .Add(2, "Listen, I don't know where you think you are but if you're looking for a good time, the stripclub's over there. Back the fuck off me.",
                     Response("I'm sorry.", ResponseAction.End))
                .Add(3, "We spent over 2 months infiltrating the CraftSquad, then you go and kill our inside-guy and it's all fucked.",
                     Response("That's was John's problem not mine.", 4),
                     Response("Whatever, what do you need me to do?", 5))
                .Add(4, "I don't care who's problem it is, you better sort this mess out.",
                     Response("Tell me the plan",5))
                .Add(5, "We need to end CraftSquad. Take out as many of them as you can. Do this, and you'll have Apex's attention.",
                     Response("Consider it done.", ResponseAction.Start_Quest, "Smash the CraftSquad"));

            var npc5 = new NpcObject("Jackson", PedHash.Clay, new Vector3(-567, -1072, 22), 162f);
            npc5.SetBlip(BlipSprite.RPG);
            npc5.SetQuestsToGive();
            npc5.SetPersonalQuestLine("Potential", "Doe!");
            npc5.SetQuestHandIns("Potential","Doe!");
            npc5.SetSideQuests();
            npc5.SetQuestConditions();
            npc5.SetDialog("What's up man?",
                           Response("I've got the package right here, and the thugs are out of the picture", 0).WithCondition(ResponseCondition.QuestConditionsDone("Potential")),
                           Response("The Golden Doe has landed, or some shit", 8).WithCondition(ResponseCondition.QuestConditionsDone("Doe!")),
                           Response("Yo you're looking a little worried man.", 7).WithConditions(ResponseCondition.QuestComplete("Potential"), ResponseCondition.QuestNotInProgressOrDone("Trouble in the Cap")),
                           Response("Nothing, catch you later", ResponseAction.End))
                .Add(0, "You the real deal huh. You want in on a little secret?", 
                    Response("Go on", 1))
                .Add(1, "Do you know what's in that package? I can see it's still sealed, so you don't know, do you?", 
                    Response("No... Should I?", 2))
                .Add(2, "Well... new recruits get a test. If you would of looked at the package I'd of killed you right here.", 
                    Response("Well shit.", 3),
                    Response("Over my living body bitch, what'd you think this is?", 6))
                .Add(3, "Ha. Don't worry you're safe man. Open the package. Go on man, take a look.", 
                    Response("[Open Package]", 4))
                .Add(4, "Ha. Don't worry you're safe man. Open the package. Go on man, take a look.", 
                    Response("Wait a second, this box is empty.", 5))
                .Add(5, "Haha, the look on your face man. Congratulations, you're officially an Apexer bro. Speak to me later.", 
                    Response("[Finish Quest]", ResponseAction.Finish_Quest,"Potential"))
                .Add(6, "Chill the fuck out man. You're safe man, nothing to worry about, shit... open the damn package right now if you want", 
                    Response("Shit why not, we'll see if you've got the balls to shoot me.", 4))
                .Add(7, "There's a problem with- You know what never mind man, go and speak to John Doe for a bit, he needs some help", 
                    Response("Alright man.", ResponseAction.End))
                .Add(8, "Well there's only one thing that means.", 
                    Response("And that is?", 9))
                .Add(9, "It means I give you this. Enjoy.",
                    Response("[Accept John's Apology]", ResponseAction.Finish_Quest, "Doe!"));

            Npcs.Add(npc1);
            Npcs.Add(npc2);
            Npcs.Add(npc3);
            Npcs.Add(npc4);
            Npcs.Add(npc5);
        }

        static Response Response(string text, int dialogId)
        {
            return new Response(text, dialogId);
        }
        static Response Response(string text, ResponseAction action)
        {
            return new Response(text, action);
        }
        static Response Response(string text, ResponseAction action, string param)
        {
            return new Response(text, action, param);
        }
        static Response Response(string text, ResponseAction action, Action custaction)
        {
            return new Response(text, action, custaction);
        }
        static QuestConditionCheck Condition(string quest, string conditionName)
        {
            return new QuestConditionCheck(quest, conditionName);
        }
    }
}