using System;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.Repository;
using LogicSpawn.GTARPG.Core.Scripts.Popups;

namespace LogicSpawn.GTARPG.Core.General
{
    public class TreeMenu : MenuBase
    {
        private bool _showUIOld;
        private NTree DataTree;
        private NTree Branch;
        private Node SelectedObj
        {
            get { return Branch.Node; }
        }
        public TreeType TreeType
        {
            get { return DataTree.Type; }
        }

        public TreeMenu(NTree tree)
        {
            DataTree = tree;
            Branch = tree;
        }

        public override void Draw()
        {
            string title;
            string subText;
            string activateText;

            GetTitle(DataTree, out title, out subText, out activateText);

            new UIRectangle(new Point(3, 3), new Size(UI.WIDTH - 6, UI.HEIGHT - 6), Color.FromArgb(240, 25, 25, 25)).Draw();
            new UIRectangle(new Point(UI.WIDTH-342,UI.HEIGHT-23), new Size(332, 14), Color.FromArgb(120, 8, 8, 8)).Draw();
            new UIText("[backspace] back [5] " + activateText + " [num4] left [num6] right  [num8] up [num2] down",
                       new Point(UI.WIDTH-340,UI.HEIGHT-23), 0.18f, Color.White,0,false).Draw();

            new UIText(title,new Point(50,20), 1.2f, Color.White,0,false).Draw();
            new UIText(subText,new Point(50,70), 0.5f, Color.White,0,false).Draw();
            new UIText(RPG.PlayerData.Name.ToLower() + " level " + RPG.PlayerData.Level  + " criminal",new Point(UI.WIDTH - 250,40), 0.4f, Color.White,0,false).Draw();

            new UIRectangle(new Point(UI.WIDTH/2 - 150, UI.HEIGHT - 110), new Size(300, 100), Color.FromArgb(120, 8, 8, 8)).Draw();
            SelectedObj.DrawDetails(DataTree);  


            DataTree.Draw(DataTree, null, DrawNode);
        }

        private void GetTitle(NTree tree, out string title, out string subText, out string activateText)
        {
            var type = tree.Type;
            switch(type)
            {
                case TreeType.Skill:
                    title = "SKILLS";
                    subText = "SP: " + RPG.PlayerData.SkillExp.ToString("N0");
                    activateText = "unlock skill / customise skill";
                    break;
                case TreeType.SkillMod:
                    var s = RPG.PlayerData.GetSkill(tree.TreeRef);
                    title = s.Name;
                    subText = "SP: " + RPG.PlayerData.SkillExp.ToString("N0");
                    activateText = "unlock skill mod";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(type.ToString());
            }
        }

        private void DrawNode(Node nodedata, Node parent, TreeDirection direction)
        {
            var selected = SelectedObj == nodedata;
            var color = selected ? Color.DodgerBlue : Color.FromArgb(120, 128, 128, 128);
            if(parent != null)
            {
                var parentUnlocked = parent.Unlocked(DataTree);
                if (!parentUnlocked && !selected) color = Color.FromArgb(150, 120, 120, 120);
                if (parentUnlocked && parent == SelectedObj && !RPG.PlayerData.GetSkill(SelectedObj.Ref).Unlocked) color = Color.FromArgb(150, 255, 150, 0);
            }
            
            new UIRectangle(nodedata.Position, new Size(NTree.NodeSize, NTree.NodeSize), Color.Black).Draw();
            new UIRectangle(nodedata.Position + new Size(2,2), new Size(NTree.NodeSize -4, NTree.NodeSize-4), color).Draw();
            new UIRectangle(nodedata.Position + new Size(4, 4), new Size(NTree.NodeSize - 8, NTree.NodeSize - 8), Color.FromArgb(255, 24, 24, 24)).Draw();
            nodedata.Sprite.Draw(nodedata.Position + new Size(4, 4),56,56,Color.White);

            new UIText(nodedata.GetName(), nodedata.Position + new Size(NTree.NodeSize / 2, NTree.NodeSize), 0.35f, Color.White, 0, true).Draw();
            new UIText(nodedata.GetFooter(DataTree), nodedata.Position + new Size(NTree.NodeSize / 2, NTree.NodeSize + 17), 0.21f, Color.White, 0, true).Draw();

            if (parent == null) return;
            var pPos = parent.Position;
            var dir = nodedata.Direction;

            var linkColor = parent == SelectedObj && !nodedata.Unlocked(DataTree) ? Color.FromArgb(150, 255, 150, 0) : Color.White;
            switch (dir)
            {
                case TreeDirection.Right:
                    new UIRectangle(pPos + NTree.NodeCenter + new Size(NTree.NodeSize / 2 + 29, -NTree.LinkThickness / 2), new Size(NTree.LinkLength - 19, NTree.LinkThickness), linkColor).Draw();
                    break;
                case TreeDirection.Down:
                    new UIRectangle(pPos + NTree.NodeCenter + new Size(-NTree.LinkThickness / 2, NTree.NodeSize / 2 + 29), new Size(NTree.LinkThickness, NTree.LinkLength - 19), linkColor).Draw();
                    break;
                case TreeDirection.Left:
                    new UIRectangle(pPos + NTree.NodeCenter + new Size(-(NTree.NodeSize / 2 + 29) - 50, -NTree.LinkThickness / 2), new Size(NTree.LinkLength - 19, NTree.LinkThickness), linkColor).Draw();
                    break;
                case TreeDirection.Up:
                    new UIRectangle(pPos + NTree.NodeCenter + new Size(-NTree.LinkThickness / 2, -(NTree.NodeSize / 2 + 29) - 50), new Size(NTree.LinkThickness, NTree.LinkLength - 19), linkColor).Draw();
                    break;
            }
        }

        public override void Draw(Size offset)
        {
            Draw();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnOpen()
        {
            Function.Call(Hash.DISPLAY_HUD, 0);
            Function.Call(Hash.DISPLAY_RADAR, 0);
            _showUIOld = RPG.UIHandler.ShowUI;
            RPG.UIHandler.ShowUI = false;
            RPG.UIHandler.CurrentMenu = this;
            Game.Player.CanControlCharacter = false;
        }

        public override void OnClose()
        {
            Function.Call(Hash.DISPLAY_HUD, 1);
            Function.Call(Hash.DISPLAY_RADAR, 1);
            Game.Player.CanControlCharacter = true;
            RPG.UIHandler.CurrentMenu = null;
            RPG.UIHandler.ShowUI = _showUIOld;
        }

        public override void OnActivate()
        {
            if (SelectedObj.NotUsable) return;

            if(DataTree.Type == TreeType.Skill && (SelectedObj.Parent == null || SelectedObj.Parent.Node.Unlocked(DataTree)))
            {
                var skill = RPG.PlayerData.GetSkill(SelectedObj.Ref);
                if(skill.Unlocked && skill.Mods.Count > 0)
                {
                    RPG.UIHandler.View.RemoveMenu(this);
                    RPG.UIHandler.View.AddMenu(new TreeMenu(SkillRepository.GetModTree(SelectedObj.Ref)));
                }
                else if(!skill.Unlocked)
                {
                    if (!RPG.PlayerData.Tutorial.UnlockSkillWithSp && RPG.PlayerData.Tutorial.PressJToOpenMenu && RPG.PlayerData.Tutorial.BoughtAmmoFromShop && RPG.PlayerData.Tutorial.GetAKill)
                    {
                        var tut = RPG.GetPopup<TutorialBox>();
                        RPG.PlayerData.Tutorial.UnlockSkillWithSp = true;
                        EventHandler.Do(o =>
                        {
                            tut.Hide();
                            EventHandler.Wait(300);
                            if (!RPG.PlayerData.Tutorial.UsingSkills)
                            {
                                tut.Pop("Skills can get you the edge in your conflicts. You can further increase skills by 'activating' them in the menu once unlocked.", "Press [Caps-Lock] to use your unlocked skill.");
                            }
                        });
                    }

                    skill.Unlock();
                }
            }
            else if (DataTree.Type == TreeType.SkillMod && SelectedObj.Parent.Node.Unlocked(DataTree))
            {
                var skill = RPG.PlayerData.GetSkill(DataTree.TreeRef);
                var modUnlocked = skill.UsedMods.FirstOrDefault(m => m == SelectedObj.Ref) != null;
                if (!modUnlocked)
                {
                    if(skill.UnlockMod(SelectedObj.Ref))
                    {
                        SelectedObj.UnlockAction.Invoke(skill);
                    }
                }
            }
        }

        public override void OnChangeSelection(bool down)
        {
            var dir = down ? TreeDirection.Down : TreeDirection.Up;
            var nextTree = Branch.children.FirstOrDefault(c => c.Node.Direction == dir);
            if(nextTree != null)
            {
                Branch = nextTree;
            }
            else
            {
                var parentDirToThis = dir == TreeDirection.Down ? TreeDirection.Up : TreeDirection.Down;
                var parentInDir = SelectedObj.Direction == parentDirToThis;
                if(parentInDir)
                {
                    Branch = SelectedObj.Parent;
                }

            }
        }

        public override void OnChangeItem(bool right)
        {
            var dir = right ? TreeDirection.Right : TreeDirection.Left;
            var nextTree = Branch.children.FirstOrDefault(c => c.Node.Direction == dir);
            if (nextTree != null)
            {
                Branch = nextTree;
            }
            else
            {
                var parentDirToThis = dir == TreeDirection.Right ? TreeDirection.Left : TreeDirection.Right;
                var parentInDir = SelectedObj.Direction == parentDirToThis;
                if (parentInDir)
                {
                    Branch = SelectedObj.Parent;
                }

            }
        }
    }
}