using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LogicSpawn.GTARPG.Core
{
    public class SkillSlot
    {
        public Keys Key;
        public string ItemName;
        public string SkillName;

        public SkillSlot()
        {
            Key = Keys.None;
            ItemName = null;
            SkillName = null;
        }

        [JsonIgnore]
        public bool Usable
        {
            get
            {
                if (IsEmpty) return false;
                if(IsItem)
                {
                    var item = RPG.PlayerData.Inventory.FirstOrDefault(t => t.Name == Name);
                    if (item != null && item.CoolDownTimer.Usable) return true;
                }
                else
                {
                    var skill = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == Name);
                    if (skill != null && skill.Usable) return true;
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                if(IsItem)
                {
                    var item = RPG.PlayerData.Inventory.FirstOrDefault(t => t.Name == Name);
                    if (item == null) return true;
                }
                else
                {
                    var skill = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == Name);
                    if (skill == null) return true;
                }

                return false;
            }
        }

        [JsonIgnore]
        public bool IsItem
        {
            get { return ItemName != null; }
        }

        [JsonIgnore]
        public string Name
        {
            get { return IsItem ? ItemName : SkillName; }
        }

        [JsonIgnore]
        public float CooldownRatio
        {
            get
            {
                if (IsItem)
                {
                    var item = RPG.PlayerData.Inventory.FirstOrDefault(t => t.Name == Name);
                    return ((float)item.CoolDownTimer.Current / item.CoolDownTimer.CoolDownMsTime);
                }
                else
                {
                    var skill = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == Name);
                    return ((float)skill.CoolDownTimer.Current / skill.CoolDownTimer.CoolDownMsTime);
                }
            }
        }

        public string GetText()
        {
            var text =  IsItem ? ItemName : SkillName;
            if (text == null) text = "";
            return text;
        }

        public void Set(string name, bool isSkill)
        {
            if(isSkill)
            {
                SkillName = name;
                ItemName = null;
            }
            else
            {
                ItemName = name;
                SkillName = null;
            }
        }

        public void Clear()
        {
            SkillName = null;
            ItemName = null;
        }   

        public string GetMenuKeyName()
        {
            return Key == Keys.CapsLock ? "CapsLock\t" : Key.ToString() + "\t\t\t";
        }
    }
}