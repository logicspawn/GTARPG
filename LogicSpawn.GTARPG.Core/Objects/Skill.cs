using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class Skill
    {
        public string Name;
        public string Description;
        public int PointsToUnlock;
        public bool Unlocked;

        public CoolDown CoolDownTimer;
        public List<string> UsedMods;
        public Dictionary<string, object> Parameters;
        public Dictionary<string, int> Mods;
        [JsonIgnore]
        public Dictionary<string, Func<Skill,string>> VisibleParameters;
        [JsonIgnore] public NTree ModTree;
    
        [JsonIgnore]
        public int ElapsedCoolDownMs
        {
            get { return CoolDownTimer.Current; }
        }
        [JsonIgnore]
        public int CoolDownMs
        {
            get { return CoolDownTimer.CoolDownMsTime; }
            set { CoolDownTimer.CoolDownMsTime = value; }
        }

        [JsonIgnore]
        public Action<Skill> OnUse;

        

        [JsonIgnore]
        public bool Usable
        {
            get { return CoolDownTimer.Usable; }
        }

        public Skill()
        {
        }

        public Skill(string name, string desc, int pointsToUnlock)
        {
            Name = name;
            Description = desc;
            PointsToUnlock = pointsToUnlock;
            Unlocked = false;
            CoolDownTimer = new CoolDown(1000);
            Parameters = new Dictionary<string, object>();
            VisibleParameters = new Dictionary<string, Func<Skill, string>>();
            UsedMods = new List<string>();
            Mods = new Dictionary<string, int>();
        }


        public void Use(Skill obj)
        {
            if (CoolDownTimer.Usable)
            {
                SkillRepository.GetAction(Name).Invoke(obj);
                CoolDownTimer.Reset();
            }
            else
            {
                var secs = CoolDownTimer.SecondsRemaining.ToString("0.0");
                RPG.Notify("cannot use " + Name + " for " + secs + "s");
            }
        }

        public void Unlock()
        {
            if(RPG.PlayerData.SkillExp >= PointsToUnlock)
            {
                RPG.PlayerData.SkillExp -= PointsToUnlock;
                Unlocked = true;
                RPG.GetPopup<SkillUnlocked>().Show(this);
                CoolDownTimer.Current = CoolDownTimer.CoolDownMsTime;
                var slots = RPG.SkillHandler.Slots;

                if(slots[2].IsEmpty)
                {
                    slots[2].Set(Name, true);
                }
                else
                {
                    for (int i = 0; i < slots.Length; i++)
                    {
                        var slot = slots[i];
                        if (slot.IsEmpty)
                        {
                            slot.Set(Name, true);
                            break;
                        }
                    } 
                }
            }
        }

        public bool UnlockMod(string modName)
        {
            var pointsToUnlockMod = Mods[modName];
            if (RPG.PlayerData.SkillExp >= pointsToUnlockMod)
            {
                RPG.PlayerData.SkillExp -= pointsToUnlockMod;
                UsedMods.Add(modName);
                return true;
            }

            return false;
        }

        public int GetIntParam(string paramName)
        {
            object value;
            var found = Parameters.TryGetValue(paramName, out value);

            if (found)
            {
                return Convert.ToInt32(value);
            }
            else
            {
                RPGLog.LogError("Skill param does not exist:" + paramName);
            }

            return -1;
        }
        
        public float GetFloatParam(string paramName)
        {
            object value;
            var found = Parameters.TryGetValue(paramName, out value);

            if (found)
            {
                return Convert.ToSingle(value);
            }
            else
            {
                RPGLog.LogError("Skill param does not exist:" + paramName);
            }

            return -1;
        }
        
        public string GetStringParam(string paramName)
        {
            object value;
            var found = Parameters.TryGetValue(paramName, out value);

            if (found)
            {
                return Convert.ToString(value);
            }
            else
            {
                RPGLog.LogError("Skill param does not exist:" + paramName);
            }

            return null;
        }

        public bool GetBoolParam(string paramName)
        {
            object value;
            var found = Parameters.TryGetValue(paramName, out value);

            if (found)
            {
                return Convert.ToBoolean(value);
            }
            else
            {
                RPGLog.LogError("Skill param does not exist:" + paramName);
            }

            return false;
        }


        public void SetParam(string paramName, object value)
        {
            if(Parameters.ContainsKey(paramName))
            {
                Parameters[paramName] = value;
            }
            else
            {
                RPGLog.LogError("Skill param does not exist:" + paramName);
            }
        }

        public Skill WithStartingUnlocked()
        {
            Unlocked = true;
            return this;
        }
        public Skill WithCooldown(int coolDownMs)
        {
            CoolDownMs = coolDownMs;
            return this;
        }
        public Skill WithAction(Action<Skill> action)
        {
            OnUse = action;
            return this;
        }
        public Skill WithModTree(NTree modTree)
        {
            ModTree = modTree;
            return this;
        }

        public Skill WithParam(string paramName, object startingValue)
        {
            Parameters.Add(paramName,startingValue);
            return this;
        }
        public Skill WithParam(string paramName, object startingValue, Func<Skill, string> visibleParam, string visibleName = null)
        {
            Parameters.Add(paramName, startingValue);
            VisibleParameters.Add(visibleName ?? paramName, visibleParam);
            return this;
        }
        public Skill WithVisibleParam(string visibleName, Func<Skill,string> visibleParam)
        {
            VisibleParameters.Add(visibleName, visibleParam);
            return this;
        }

        public Skill WithMods(params KeyValuePair<string,int>[] mods )
        {
            foreach(var mod in mods)
            {
                Mods.Add(mod.Key,mod.Value);
            }
            return this;
        }

    }
}
