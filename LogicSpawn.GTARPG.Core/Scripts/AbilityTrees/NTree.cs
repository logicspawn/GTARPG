using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;

namespace LogicSpawn.GTARPG.Core.AbilityTrees
{
    public delegate void TreeDraw(Node nodeData, Node parent, TreeDirection direction);

    public class Node
    {
        public Node(string reference, GTASprite sprite, NodeType type, TreeDirection direction = TreeDirection.Auto)
        {
            Ref = reference;
            Direction = direction;
            Type = type;
            Sprite = sprite;
        }

        public Node(string reference, GTASprite sprite, NodeType type)
        {
            Ref = reference;
            Direction = TreeDirection.Auto;
            Type = type;
            Sprite = sprite;
        }

        public Node(string reference, WeaponHash wepHash, GTASprite sprite, NodeType type, TreeDirection direction = TreeDirection.Auto)
        {
            Ref = reference;
            Direction = direction;
            Type = type;
            Sprite = sprite;
            WepHash = wepHash;
        }

        public Node(string reference, WeaponHash wepHash, GTASprite sprite, NodeType type)
        {
            Ref = reference;
            Direction = TreeDirection.Auto;
            Type = type;
            Sprite = sprite;
            WepHash = wepHash;
        }

        public NodeType Type;
        public WeaponHash WepHash;
        public GTASprite Sprite;
        public string Ref;
        public bool NotUsable;
        public string Description;
        public TreeDirection Direction;
        public Point Position;
        public NTree Parent;
        public Action<object> UnlockAction;

        public string GetName()
        {
            return Ref;
        }
        public string GetFooter(NTree dataTree)
        {

            switch(Type)
            {
                case NodeType.Skill:
                    var skill = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == Ref);
                    return skill.Unlocked ? "unlocked" : skill.PointsToUnlock + " SP";
                case NodeType.Weapon:
                    var wep = RPG.PlayerData.Weapons.FirstOrDefault(s => s.WeaponHash == WepHash);
                    return wep.Unlocked ? "unlocked" : wep.PointsToUnlock + " SP at Level " + wep.LevelToUnlock;
                case NodeType.SkillMod:
                    var skillForMod = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == dataTree.TreeRef);
                    var mod = skillForMod.UsedMods.FirstOrDefault(s => s == Ref);
                    return mod != null ? "unlocked" : skillForMod.Mods[Ref] + " SP";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void DrawDetails(NTree dataTree)
        {
            if(Type == NodeType.Skill)
            {
                var skill = RPG.PlayerData.Skills.First(s => s.Name == Ref);
                new UIText(Ref, new Point(UI.WIDTH / 2, UI.HEIGHT - 95), 0.3f, Color.White, 0, true).Draw();
                new UIText(skill.Description, new Point(UI.WIDTH / 2, UI.HEIGHT - 75), 0.22f, Color.White, 0, true).Draw();

                var topPoint = new Point(UI.WIDTH/2, UI.HEIGHT - 62);

                var i = 0;
                var sparams = SkillRepository.GetVisibleParams(Ref);
                foreach (var kvp in sparams)
                {
                    new UIText(kvp.Key +": " + kvp.Value.Invoke(skill) ,topPoint + new Size(0,10 * i) , 0.20f, Color.DodgerBlue, 0, true).Draw();
                    i++;
                }
                
                new UIText(skill.Unlocked ? "unlocked" : "unlock for " + skill.PointsToUnlock + " SP", new Point(UI.WIDTH / 2, UI.HEIGHT - 22), 0.20f, Color.Gray, 0, true).Draw();
                Sprite.Draw(new Point(UI.WIDTH / 2 - 140, UI.HEIGHT - 50), 40, 40, Color.FromArgb(120, 255, 255, 255));
            }
            else if(Type == NodeType.Weapon)
            {
                var wep = RPG.PlayerData.Weapons.FirstOrDefault(s => s.WeaponHash == WepHash);
                new UIText(Ref, new Point(UI.WIDTH / 2, UI.HEIGHT - 95), 0.3f, Color.White, 0, true).Draw();
                new UIText(wep.Description, new Point(UI.WIDTH / 2, UI.HEIGHT - 75), 0.22f, Color.White, 0, true).Draw();

                var topPoint = new Point(UI.WIDTH/2, UI.HEIGHT - 62);

                //var i = 0;
                //var sparams = SkillRepository.GetVisibleParams(Ref);
                //foreach (var kvp in sparams)
                //{
                //    new UIText(kvp.Key +": " + kvp.Value.Invoke(skill) ,topPoint + new Size(0,10 * i) , 0.20f, Color.DodgerBlue, 0, true).Draw();
                //    i++;
                //}
                
                new UIText(wep.Unlocked ? "unlocked" : "unlock for " + wep.PointsToUnlock + " SP [Requires Lv." + wep.LevelToUnlock + "]", new Point(UI.WIDTH / 2, UI.HEIGHT - 22), 0.20f, Color.Gray, 0, true).Draw();
                Sprite.Draw(new Point(UI.WIDTH / 2 - 140, UI.HEIGHT - 50), 40, 40, Color.FromArgb(120, 255, 255, 255));
            }
            else if (Type == NodeType.SkillMod)
            {
                new UIText(Ref, new Point(UI.WIDTH / 2, UI.HEIGHT - 95), 0.3f, Color.White, 0, true).Draw();
                new UIText(Description, new Point(UI.WIDTH / 2, UI.HEIGHT - 75), 0.22f, Color.White, 0, true).Draw();

                var skillForMod = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == dataTree.TreeRef);
                var mod = skillForMod.UsedMods.FirstOrDefault(s => s == Ref);
                var unlockedText =  mod != null ? "unlocked" : "unlock for " + skillForMod.Mods[Ref] + " SP";

                new UIText(unlockedText, new Point(UI.WIDTH / 2, UI.HEIGHT - 22), 0.20f, Color.Gray, 0, true).Draw();
                Sprite.Draw(new Point(UI.WIDTH / 2 - 140, UI.HEIGHT - 50), 40, 40, Color.FromArgb(120, 255, 255, 255));
            }
            
        }

        public bool Unlocked(NTree dataTree)
        {
            if (Type == NodeType.Skill)
            {
                var skill = RPG.PlayerData.Skills.First(s => s.Name == Ref);
                return skill.Unlocked;
            }

            if (Type == NodeType.Weapon)
            {
                var wep = RPG.PlayerData.Weapons.First(s => s.WeaponHash == WepHash);
                return wep.Unlocked;
            }
            
            if (Type == NodeType.SkillMod)
            {
                var skillForMod = RPG.PlayerData.Skills.FirstOrDefault(s => s.Name == dataTree.TreeRef);
                var mod = skillForMod.UsedMods.FirstOrDefault(s => s == Ref);
                return  mod != null;
            }

            return false;
        }

        public Node WithUnlockAction(Action<object> action)
        {
            UnlockAction = action;
            return this;
        }

        public Node WithDescription(string desc)
        {
            Description = desc;
            return this;
        }

        public Node WithUnusable()
        {
            NotUsable = true;
            return this;
        }
    }

    public enum NodeType
    {
        Skill,
        SkillMod,
        Weapon
    }

    public enum TreeDirection
    {
        Auto = -1,
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    public class NTree
    {
        public const int NodeSize = 40;
        public const int LinkThickness = 2;
        public const int LinkLength = 75;
        public const int CellLength = NodeSize + 20 + LinkLength;
        public static Size NodeCenter = new Size(NodeSize / 2, NodeSize / 2);

        public Node Node;
        public TreeType Type;
        public string TreeRef;
        public List<NTree> children;

        public NTree(string treeRef, Node node, TreeType type, Point startingPosition)
        {
            TreeRef = treeRef;
            Node = node;
            Type = type;
            Node.Position = startingPosition;
            children = new List<NTree>();
        }

        public NTree(Node node, TreeType type, Point startingPosition)
        {
            Node = node;
            Type = type;
            Node.Position = startingPosition;
            children = new List<NTree>();
        }

        public NTree(Node node)
        {

            Node = node;
            children = new List<NTree>();
        }

        public NTree AddChild(Node dataa)
        {
            var check = dataa.Direction != TreeDirection.Auto ? (int)dataa.Direction : children.Count;
            dataa.Parent = this;
            var tree = new NTree(dataa);
            children.Add(tree);

            if (check == 0)
            {
                dataa.Position = Node.Position + new Size(NodeSize + 20 + LinkLength, 0);
                dataa.Direction = TreeDirection.Right;
                dataa.Parent = this;
            }
            else if (check == 1)
            {
                dataa.Position = Node.Position + new Size(0, NodeSize + 20 + LinkLength);
                dataa.Direction = TreeDirection.Down;
                dataa.Parent = this;
            }
            else if (check == 2)
            {
                dataa.Position = Node.Position + new Size(-(NodeSize + 20 + LinkLength), 0);
                dataa.Direction = TreeDirection.Left;
                dataa.Parent = this;
            }
            else if (check == 3)
            {
                dataa.Position = Node.Position + new Size(0, -(NodeSize + 20 + LinkLength));
                dataa.Direction = TreeDirection.Up;
                dataa.Parent = this;
            }
            return tree;
        }

        public NTree GetChild(int i)
        {
            return children[i];
        }

        public void Draw(NTree node, Node parent, TreeDraw visitor)
        {
            visitor(node.Node, node.Node.Parent != null ? node.Node.Parent.Node : null, node.Node.Direction);
            foreach (NTree kid in node.children)
                Draw(kid, this.Node, visitor);
        }
    }

    public enum TreeType
    {
        Skill,
        SkillMod,
        Weapon
    }
}