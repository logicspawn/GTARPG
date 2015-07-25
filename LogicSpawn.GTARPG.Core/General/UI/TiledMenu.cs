using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;
using GTA.Native;
using LogicSpawn.GTARPG.Core.AbilityTrees;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Repository;
using Font = GTA.Font;

namespace LogicSpawn.GTARPG.Core.General
{
    public class TiledMenu : MenuBase
    {
        private readonly string _caption;
        private readonly List<TiledPanel> _panels;
        private TiledPanel SelectedPanel { get { return _panels[SelectedIndex]; } }
        private int SelectedIndex;

        private const int PanelWidth = 180;
        private const int PanelBorder = 10;
        private const int PanelHeight = 220;

        public TiledMenu(string caption, params TiledPanel[] panels)
        {
            _caption = caption;
            _panels = panels.ToList();
        }

        public override void Draw()
        {
            if (!RPG.UIHandler.ShowUI) return;

            new UIRectangle(new Point(0, 0), new Size(UI.WIDTH, UI.HEIGHT), Color.FromArgb(180, 80, 80, 80)).Draw();
            new UIRectangle(new Point(0, (int)(UI.HEIGHT * 0.30f)), new Size(UI.WIDTH, 220 + PanelBorder*2), Color.FromArgb(240, 28, 28, 28)).Draw();
            new UIText(_caption, new Point(UI.WIDTH / 2, (int)(UI.HEIGHT * 0.30f) - 80), 1.4f, Color.FromArgb(240, 200, 200, 200), Font.Monospace, true).Draw();
            new UIRectangle(new Point(UI.WIDTH - 212, UI.HEIGHT - 23), new Size(207, 14), Color.FromArgb(205, 8, 8, 8)).Draw();
            new UIText("[backspace] back [5] choose option [num4] left [num6] right",new Point(UI.WIDTH - 210, UI.HEIGHT - 23), 0.18f, Color.White, 0, false).Draw();


            var items = _panels.Count;
            var startingPoint = new Point(UI.WIDTH/2 - (items*(PanelWidth + 10))/2,(int)(UI.HEIGHT * 0.30f) + PanelBorder);

            for (int i = 0; i < _panels.Count; i++)
            {
                var panel = _panels[i];
                var panelColor = panel == SelectedPanel ? panel.Color : Color.FromArgb(panel.Color.A / 2, panel.Color.R, panel.Color.G, panel.Color.B);
                new UIRectangle(new Point(startingPoint.X + (i * (PanelWidth + PanelBorder / 2)), startingPoint.Y), new Size(PanelWidth, PanelHeight), panelColor).Draw();
                var spriteColor = panel == SelectedPanel ? panel.Sprite.Color : Color.FromArgb(panel.Sprite.Color.A / 3, (panel.Sprite.Color.R + 155) / 2, (panel.Sprite.Color.G + 155) / 2 , (panel.Sprite.Color.B + 155)/2); ;
                panel.Sprite.Draw(new Point(startingPoint.X + (i * (PanelWidth + PanelBorder / 2)), startingPoint.Y), PanelWidth, PanelHeight, spriteColor);
                var color = panel == SelectedPanel ? Color.White : Color.Gray;
                new UIText(panel.Caption, new Point(startingPoint.X + (i * (PanelWidth + PanelBorder / 2)) + PanelWidth/2 + 2, startingPoint.Y + PanelHeight - 40 + 2), 0.8f, Color.FromArgb(60,2,2,2), Font.Monospace, true).Draw();
                new UIText(panel.Caption, new Point(startingPoint.X + (i * (PanelWidth + PanelBorder / 2)) + PanelWidth/2, startingPoint.Y + PanelHeight - 40), 0.8f, color, Font.Monospace, true).Draw();

            }


            new UIRectangle(new Point(UI.WIDTH / 2 - 190, (int)(UI.HEIGHT * 0.30f) + PanelBorder * 2 + PanelHeight + 15), new Size(380, 70), Color.FromArgb(120, 55, 55, 55)).Draw();
            var descPoint = new Point(UI.WIDTH/2, (int) (UI.HEIGHT*0.30f) + PanelBorder*2 + PanelHeight + 15);
            var lines = RPGUI.FormatText(SelectedPanel.Description, 75);
            for (int i = 0; i < lines.Length; i++)
            {
                new UIText(lines[i], new Point(descPoint.X, descPoint.Y + (i * 10)), 0.22f, Color.White, 0, true).Draw();
            }

            var point = new Point(UI.WIDTH / 2 - 190, (int)(UI.HEIGHT * 0.30f) + PanelBorder * 2 + PanelHeight + 15) + new Size(0, 74);
            for (int i = 0; i < SelectedPanel.ExtraInfo.Length; i++)
            {
                new UIRectangle(new Point(point.X, point.Y + (i * 18)), new Size(380, 14), Color.FromArgb(120, 55, 55, 55)).Draw();
                new UIText(SelectedPanel.ExtraInfo[i], new Point(UI.WIDTH / 2, point.Y + (i * 18)), 0.22f, Color.WhiteSmoke, 0, true).Draw();
            }
        }

        
        public override void Draw(Size offset)
        {
            if (!RPG.UIHandler.ShowUI) return;

            Draw();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void OnOpen()
        {

        }

        public override void OnClose()
        {

        }

        public override void OnActivate()
        {
            SelectedPanel.ActivateAction.Invoke(SelectedIndex);
        }

        public override void OnChangeSelection(bool down)
        {
            
        }

        public override void OnChangeItem(bool right)
        {
            if(right)
            {
                SelectedIndex++;
                if(SelectedIndex > _panels.Count - 1)
                {
                    SelectedIndex = 0;
                }
            }
            else
            {
                SelectedIndex--;
                if (SelectedIndex < 0)
                {
                    SelectedIndex = _panels.Count - 1;
                }
            }
        }
    }

    public class TiledPanel
    {
        public readonly Color Color;
        public readonly string Description;
        public readonly string[] ExtraInfo;
        public readonly string Caption;
        public readonly GTASprite Sprite;
        public readonly Action<int> ActivateAction;

        public TiledPanel(string caption, GTASprite sprite, Color color, Action<int> activateAction, string description, params string[] extraInfo)
        {
            Color = color;
            Description = description;
            ExtraInfo = extraInfo;
            Caption = caption;
            Sprite = sprite;
            ActivateAction = activateAction;
        }
    }
}