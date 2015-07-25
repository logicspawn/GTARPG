using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;
using GTA.Native;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core
{
    public class NpcObject : WorldObject
    {
        [JsonIgnore] public Ped Ped;

        public bool IsQuestNpc;
        public PedHash ModelName;
        public BlipSprite BlipSprite;
        public Vector3 Position;
        public float Heading;
        public DialogContainer Dialog;
        public List<string> SimpleDialog;
        public bool Spawned;

        public override EntityType Type
        {
            get { return EntityType.Ped; }
        }

        public NpcObject()
        {
            BlipSprite = BlipSprite.SMG;
        }

        public NpcObject(string name, Ped ped)
            : base(ped.Handle)
        {
            BlipSprite = BlipSprite.SMG;
            Name = name;
            Ped = ped;
        }
        
        public NpcObject(string name, PedHash modelName, Vector3 pos, float heading)
            : base(0)
        {
            BlipSprite = BlipSprite.SMG;
            Name = name;
            ModelName = modelName;
            Position = pos;
            Heading = heading;
            IsQuestNpc = true;
            Dialog = new DialogContainer(this);
            SimpleDialog = new List<string>();
            Ped = null;
        }

        public DialogContainer SetDialog(string npcText, params Response[] playerOptions)
        {
            var responses = playerOptions;
            Dialog.StartingDialog = new DialogObject(-1, npcText, responses);
            return Dialog;
        }

        public NpcObject SetSimpleDialog(params string[] dialogs)
        {
            SimpleDialog.AddRange(dialogs);
            return this;
        }

        protected override void RemoveFromWorld()
        {
            if (Ped != null && Ped.Exists())
                Ped.Delete();
        }

        public void SetPed(Ped ped)
        {
            Ped = ped;
            EntityHandle = ped.Handle;
        }

        public void SetBlip(BlipSprite sprite)
        {
            BlipSprite = sprite;
        }
    }

    public class DialogContainer
    {
        public readonly NpcObject Parent;
        public DialogObject Current = null;
        public DialogObject StartingDialog;
        public List<DialogObject> Dialogs;
        

        public DialogContainer(NpcObject parent)
        {
            Parent = parent;
            Dialogs = new List<DialogObject>();
        }

        //Option_Shop = Shop
        //Option_Contract = Contracts
        //Option_Craft = Crafting
        //Option_End = End Convo
        public DialogContainer Add(int id, string npcText, params Response[] playerOptions)
        {
            Dialogs.Add(new DialogObject(id, npcText, playerOptions));
            return this;
        }
    }

    public class DialogObject
    {
        public int Id;
        public string NpcText;
        public List<Response> Responses;

        public DialogObject(int i, string n, IEnumerable<Response> r)
        {
            Id = i;
            NpcText = n;
            Responses = r.ToList();
        }
    }

    public class Response
    {
        public string Text;
        public int DialogId;
        public ResponseAction Action;
        public string Paramater;
        public List<ResponseCondition> Conditions;
        public Action CustomAction;

        public bool ConditionsMet
        {
            get
            {
                var met = true;
                foreach(var condition in Conditions)
                {
                    switch(condition.Type)
                    {
                        case ResponseReq.QuestConditionsDone:
                            var quest = RPG.PlayerData.Quests.First(q => q.Name == condition.StringParam);
                            if (!quest.ConditionsComplete || quest.Done) met = false;
                            break;
                        case ResponseReq.QuestComplete:
                            var quest1 = RPG.PlayerData.Quests.First(q => q.Name == condition.StringParam);
                            if (!quest1.Done) met = false;
                            break;
                        case ResponseReq.QuestInProgress:
                            var quest2 = RPG.PlayerData.Quests.First(q => q.Name == condition.StringParam);
                            if (!quest2.InProgress) met = false;
                            break;
                        case ResponseReq.QuestNotInProgress:
                            var quest3 = RPG.PlayerData.Quests.First(q => q.Name == condition.StringParam);
                            if (quest3.InProgress) met = false;
                            break;
                        case ResponseReq.QuestNotInProgressOrDone:
                            var quest4 = RPG.PlayerData.Quests.First(q => q.Name == condition.StringParam);
                            if (quest4.InProgress || quest4.Done) met = false;
                            break;
                        case ResponseReq.Level:
                            if (RPG.PlayerData.Level < condition.IntParam) met = false;
                            break;
                        case ResponseReq.ItemOwned:
                            if (RPG.PlayerData.Inventory.FirstOrDefault(i => i.Name == condition.StringParam) == null) met = false;
                            break;
                        case ResponseReq.SkillUnlocked:
                            if(!RPG.PlayerData.GetSkill(condition.StringParam).Unlocked) met = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return met;
            }
        }

        public Response(string t, int d)
        {
            Conditions = new List<ResponseCondition>();
            Text = t;
            DialogId = d;
        }

        public Response(string t, ResponseAction a, string param)
        {
            Conditions = new List<ResponseCondition>();
            Text = t;
            Action = a;
            Paramater = param;
        }
        public Response(string t, ResponseAction a)
        {
            Conditions = new List<ResponseCondition>();
            Text = t;
            Action = a;
            Paramater = null;
        }

        public Response(string t, ResponseAction a, Action action)
        {
            Conditions = new List<ResponseCondition>();
            Text = t;
            Action = a;
            CustomAction = action;
            Paramater = null;
        }

        public Response WithCondition(ResponseCondition condition)
        {
            Conditions.Add(condition);
            return this;
        }
        public Response WithConditions(params ResponseCondition[] conditions)
        {
            Conditions.AddRange(conditions);
            return this;
        }
    }

    public class ResponseCondition
    {
        public ResponseReq Type;
        public string StringParam;
        public int IntParam;

        public ResponseCondition(ResponseReq type, string param)
        {
            Type = type;
            StringParam = param;
        }

        public ResponseCondition(ResponseReq type, int param)
        {
            Type = type;
            IntParam = param;
        }

        public static ResponseCondition QuestConditionsDone(string questName)
        {
            return new ResponseCondition(ResponseReq.QuestConditionsDone, questName);
        }
        public static ResponseCondition QuestComplete(string questName)
        {
            return new ResponseCondition(ResponseReq.QuestComplete, questName);
        }
        public static ResponseCondition QuestInProgress(string questName)
        {
            return new ResponseCondition(ResponseReq.QuestInProgress, questName);
        }
        public static ResponseCondition QuestNotInProgress(string questName)
        {
            return new ResponseCondition(ResponseReq.QuestNotInProgress, questName);
        }
        public static ResponseCondition QuestNotInProgressOrDone(string questName)
        {
            return new ResponseCondition(ResponseReq.QuestNotInProgressOrDone, questName);
        }
        public static ResponseCondition Level(int level)
        {
            return new ResponseCondition(ResponseReq.Level, level);
        }
        public static ResponseCondition ItemOwned(string itemName)
        {
            return new ResponseCondition(ResponseReq.ItemOwned, itemName);
        }
        public static ResponseCondition SkillUnlocked(string skillName)
        {
            return new ResponseCondition(ResponseReq.SkillUnlocked, skillName);
        }
    }

    public enum ResponseReq
    {
        QuestConditionsDone,
        QuestComplete,
        QuestInProgress,
        Level,
        ItemOwned,
        SkillUnlocked,
        QuestNotInProgressOrDone,
        QuestNotInProgress
    }

    public enum ResponseAction
    {
        None,
        Vendor,
        Return_To_Start,
        Craft,
        Contract,
        Start_Quest,
        Finish_Quest,
        Custom_End,
        End
    }
}