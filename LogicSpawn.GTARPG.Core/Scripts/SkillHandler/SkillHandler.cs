using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;

namespace LogicSpawn.GTARPG.Core
{
    public class SkillHandler
    {
        public SkillSlot[] Slots
        {
            get { return RPG.PlayerData.SkillSlots; }
        }
        
        public void Use(Keys skillKey)
        {
            if (RPG.UIHandler.CurrentDialog != null) return;

            var slot = Slots.First(s => s.Key == skillKey);
            if(slot.ItemName != null)
            {
                RPGMethods.UseItem(slot.ItemName);
            }
            else if(slot.SkillName != null)
            {
                RPGMethods.UseSkill(slot.SkillName);
            }
        }

        public TreeMenu GetSkillMenu()
        {
            var skillTree = new NTree(new Node("Get High", new GTASprite("mpinventory", "mp_specitem_weed"), NodeType.Skill),TreeType.Skill, new Point(50, 120));
            skillTree.AddChild(new Node("Blazed Off Glory", new GTASprite("mpcarhud", "ocelot"), NodeType.Skill))
                .AddChild(new Node("Reject Nonsense", new GTASprite("mpinventory", "mp_specitem_heroin"), NodeType.Skill))
                .AddChild(new Node("Get Hammered", new GTASprite("mpinventory", "survival"), NodeType.Skill));
            var toughen = skillTree.AddChild(new Node("Toughen Up", new GTASprite("mpinventory", "drug_trafficking"), NodeType.Skill))
                .AddChild(new Node("Reinforcements", new GTASprite("mpinventory", "team_deathmatch"), NodeType.Skill));
            toughen.AddChild(new Node("Rampage", new GTASprite("heisthud", "hc_trevor"), NodeType.Skill,TreeDirection.Down));

            return new TreeMenu(skillTree);
        }

        public UIContainer GetSkillBar(int skillOffset)
        {
            var skillBarUI = new UIContainer(new Point(UI.WIDTH / 2 - 178, UI.HEIGHT - 45 + skillOffset), new Size(356, 35));

            skillBarUI.Items.Add(new UIRectangle(new Point(5, 5), new Size(64, 18), Slots[0].Usable ? Color.FromArgb(120, 85, 85, 85) : Color.FromArgb(50, 8, 8, 8)));
            if (!Slots[0].IsEmpty && !Slots[0].Usable && Slots[0].CooldownRatio < 1)
            {
                skillBarUI.Items.Add(new UIRectangle(new Point(5, 5), new Size((int)(65 * Slots[0].CooldownRatio), 18), Color.FromArgb(120, 85, 85, 85)));
            }
            skillBarUI.Items.Add(new UIText(Slots[0].GetText(), new Point(35, 8), 0.18f, Slots[0].Usable ? Color.White : Color.DarkGray, 0, true));
            skillBarUI.Items.Add(new UIText("[" + Slots[0].Key + "]", new Point(32, 23), 0.16f, Color.White, 0, true));

            skillBarUI.Items.Add(new UIRectangle(new Point(75, 5), new Size(64, 18), Slots[1].Usable ? Color.FromArgb(120, 85, 85, 85) : Color.FromArgb(50, 8, 8, 8)));
            if (!Slots[1].IsEmpty && !Slots[1].Usable && Slots[1].CooldownRatio < 1)
            {
                skillBarUI.Items.Add(new UIRectangle(new Point(75, 5), new Size((int)(65 * Slots[1].CooldownRatio), 18), Color.FromArgb(120, 85, 85, 85)));
            }
            skillBarUI.Items.Add(new UIText(Slots[1].GetText(), new Point(107, 8), 0.18f, Slots[1].Usable ? Color.White : Color.DarkGray, 0, true));
            skillBarUI.Items.Add(new UIText("[" + Slots[1].Key + "]", new Point(105, 23), 0.16f, Color.White, 0, true));

            skillBarUI.Items.Add(new UIRectangle(new Point(145, 5), new Size(64, 18), Slots[2].Usable ? Color.FromArgb(120, 85, 85, 85) : Color.FromArgb(50, 8, 8, 8)));
            if (!Slots[2].IsEmpty && !Slots[2].Usable && Slots[2].CooldownRatio < 1)
            {
                skillBarUI.Items.Add(new UIRectangle(new Point(145, 5), new Size((int)(65 * Slots[2].CooldownRatio), 18), Color.FromArgb(120, 85, 85, 85)));
            }
            skillBarUI.Items.Add(new UIText(Slots[2].GetText(), new Point(177, 8), 0.18f, Slots[2].Usable ? Color.White : Color.DarkGray, 0, true));
            skillBarUI.Items.Add(new UIText("[CapsLock]", new Point(175, 23), 0.16f, Color.White, 0, true));

            skillBarUI.Items.Add(new UIRectangle(new Point(215, 5), new Size(64, 18), Slots[3].Usable ? Color.FromArgb(120, 85, 85, 85) : Color.FromArgb(50, 8, 8, 8)));
            if (!Slots[3].IsEmpty && !Slots[3].Usable && Slots[3].CooldownRatio < 1)
            {
                skillBarUI.Items.Add(new UIRectangle(new Point(215, 5), new Size((int)(65 * Slots[3].CooldownRatio), 18), Color.FromArgb(120, 85, 85, 85)));
            }
            skillBarUI.Items.Add(new UIText(Slots[3].GetText(), new Point(247, 8), 0.18f, Slots[3].Usable ? Color.White : Color.DarkGray, 0, true));
            skillBarUI.Items.Add(new UIText("[" + Slots[3].Key + "]", new Point(245, 23), 0.16f, Color.White, 0, true));

            skillBarUI.Items.Add(new UIRectangle(new Point(285, 5), new Size(64, 18), Slots[4].Usable ? Color.FromArgb(120, 85, 85, 85) : Color.FromArgb(50, 8, 8, 8)));
            if (!Slots[4].IsEmpty && !Slots[4].Usable && Slots[4].CooldownRatio < 1)
            {
                skillBarUI.Items.Add(new UIRectangle(new Point(285, 5), new Size((int)(65 * Slots[4].CooldownRatio), 18), Color.FromArgb(120, 85, 85, 85)));
            }
            skillBarUI.Items.Add(new UIText(Slots[4].GetText(), new Point(317, 8), 0.18f, Slots[4].Usable ? Color.White : Color.DarkGray, 0, true));
            skillBarUI.Items.Add(new UIText("[" + Slots[4].Key + "]", new Point(315, 23), 0.16f, Color.White, 0, true));

            return skillBarUI;
        }

        public string[] GetEntries()
        {
            var items = new List<string>();
            items.Add("Empty");

            items.AddRange(RPG.PlayerData.Skills.Where(s=> s.Unlocked).Select(s => "Skill_" + s.Name));
            items.AddRange(RPG.PlayerData.Inventory.Where(it => it.Usable).Select(i => "Item_" + i.Name));
            
            return items.ToArray();
        }

        public string[] GetEntriesFormatted()
        {
            var entries = GetEntries();
            
            var items = entries.Where(e => e.StartsWith("Item_")).Select(e => e.Replace("Item_", ""));
            var skills = entries.Where(e => e.StartsWith("Skill_")).Select(e => e.Replace("Skill_", ""));

            var allFormatted = new string[]{"Empty"}.Concat(skills).Concat(items).ToArray();
            return allFormatted;
        }
    }
}