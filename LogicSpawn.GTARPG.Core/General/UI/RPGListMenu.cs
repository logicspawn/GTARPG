using System;
using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using Font = GTA.Font;

namespace LogicSpawn.GTARPG.Core.General
{
    public class RPGListMenu : Menu
    {
        private readonly GTASprite _banner;
        private readonly Action<RPGListMenu> _activationAction;
        private readonly string _bannerCaption;

        public bool ExtendWindowHeight = true;

        public RPGListMenu(string bannerCaption, GTASprite banner, Action<RPGListMenu> activationAction, IMenuItem[] items) : base("",items)
        {
            _banner = banner;
            _bannerCaption = bannerCaption;
            _activationAction = activationAction;
        }

        public override void Draw(Size offset)
        {
            if (!RPG.UIHandler.ShowUI) return;

            if (ExtendWindowHeight)
            {
                new UIRectangle(new Point(offset.Width, offset.Height - UI.HEIGHT), new Size(300, UI.HEIGHT), UnselectedItemColor).Draw();
            }

            _banner.Draw(new Point(offset.Width, offset.Height), 300, 70);
            new UIText(_bannerCaption, new Point(offset.Width + 150, offset.Height + 18), 0.8f, Color.White, 0, true).Draw();
            base.Draw(offset + new Size(0, 70));

            var bottomY = offset.Height + 70 + Items.Count * ItemHeight;
            if (HasFooter) bottomY += FooterHeight;
            bottomY += HeaderHeight;
            
            if(ExtendWindowHeight)
            {
                new UIRectangle(new Point(offset.Width, bottomY), new Size(300, UI.HEIGHT), UnselectedItemColor).Draw();
                new UIRectangle(new Point(offset.Width + 300, offset.Height - UI.HEIGHT), new Size(2, UI.HEIGHT * 2), Color.FromArgb(200, 8, 8, 8)).Draw();
            }
        }

        public override void OnActivate()
        {
            _activationAction.Invoke(this);
        }

        public override void Draw()
        {
            if (!RPG.UIHandler.ShowUI) return;

            Draw(new Size());
        }

        public override void OnChangeSelection(bool down)
        {
            int newIndex = down ? SelectedIndex + 1 : SelectedIndex - 1;
            if (newIndex >= Items.Count) newIndex = 0;
            if (newIndex < 0) newIndex = Items.Count - 1;


            while (!Usable(Items[newIndex]))
            {
                newIndex += down ? 1 : -1;
                if (newIndex >= Items.Count) newIndex = 0;
                if (newIndex < 0) newIndex = Items.Count - 1;
            }

            OnChangeSelection(newIndex);
        }

        public override void OnChangeSelection(int newIndex)
        {
            if (!Usable(Items[newIndex]))
            {
                newIndex++;
                if (newIndex >= Items.Count) newIndex = 0;
                if (newIndex < 0) newIndex = Items.Count - 1;
            }

            base.OnChangeSelection(newIndex);
        }

        private bool Usable(IMenuItem item)
        {
            return new[] { typeof(MenuLabel) }.All(m => item.GetType() != m);
        }
    }

    public class RPGDialogMenu : Menu
    {
        private readonly Action<RPGDialogMenu> _activationAction;

        public bool ExtendWindowHeight = true;

        public RPGDialogMenu(string caption, Action<RPGDialogMenu> activationAction, IMenuItem[] items)
            : base("", items)
        {
            Caption = caption;
            _activationAction = activationAction;
        }

        public override void Draw(Size offset)
        {
            if (!RPG.UIHandler.ShowUI) return;

            if (ExtendWindowHeight)
            {
                new UIRectangle(new Point(offset.Width, offset.Height - UI.HEIGHT), new Size(300, UI.HEIGHT), UnselectedItemColor).Draw();
            }

            var container = new UIContainer(new Point(UI.WIDTH/2 - 300, UI.HEIGHT - 150), new Size(600, 140), Color.FromArgb(70, 8, 8, 8));
            container.Items.Add(new UIText(Caption, new Point(301, 25 + 1), 0.42f, Color.Black, Font.ChaletComprimeCologne, true));
            container.Items.Add(new UIText(Caption, new Point(300, 25), 0.42f, Color.White, Font.ChaletComprimeCologne, true));


                
            for (int i = 0; i < 4; i++)
            {
                if (i > Items.Count - 1) break;

                var item = Items[i];
                var color = SelectedIndex == i ? Color.Orange : Color.White;
                var guessedWidth = (int)(6f * item.Caption.Length);
                guessedWidth -= item.Caption.Count(x => x == 'i' || x == 'l' || x == '!' || x == '\'' || x == ',' || x == '.' || x == 'I') * 4;
                guessedWidth -= item.Caption.Count(x => x == ' ') * 2;
                var pos = 268 - 10 - guessedWidth;
                switch(i)
                {
                    case 3:
                        container.Items.Add(new UIText(item.Caption, new Point(pos + 1, 56), 0.4f, Color.FromArgb(180, 0, 0, 0), Font.ChaletComprimeCologne, false));
                        container.Items.Add(new UIText(item.Caption, new Point(pos, 55), 0.4f, color, Font.ChaletComprimeCologne, false));
                        break;
                    case 0:
                        container.Items.Add(new UIText(item.Caption, new Point(341, 56), 0.4f, Color.FromArgb(180,0,0,0), Font.ChaletComprimeCologne, false));
                        container.Items.Add(new UIText(item.Caption, new Point(340, 55), 0.4f, color, Font.ChaletComprimeCologne, false));
                        break;
                    case 2:
                        container.Items.Add(new UIText(item.Caption, new Point(pos + 1, 86), 0.4f, Color.FromArgb(180, 0, 0, 0), Font.ChaletComprimeCologne, false));
                        container.Items.Add(new UIText(item.Caption, new Point(pos, 85), 0.4f, color, Font.ChaletComprimeCologne, false));
                        break;
                    case 1:
                        container.Items.Add(new UIText(item.Caption, new Point(341, 86), 0.4f, Color.FromArgb(180, 0, 0, 0), Font.ChaletComprimeCologne, false));
                        container.Items.Add(new UIText(item.Caption, new Point(340, 85), 0.4f, color, Font.ChaletComprimeCologne, false));
                        break;
                }
            }

            container.Draw();

            var colorA = SelectedIndex == 3 ? Color.Orange : Color.SteelBlue;
            var colorB = SelectedIndex == 0 ? Color.Orange : Color.SteelBlue;
            var colorC = SelectedIndex == 2 ? Color.Orange : Color.SteelBlue;
            var colorD = SelectedIndex == 1 ? Color.Orange : Color.SteelBlue;

            new GTASprite("helicopterhud", "hud_corner").Draw(new Point(UI.WIDTH / 2 - 33, UI.HEIGHT - 100), 32, 32, colorA); //1
            new GTASprite("helicopterhud", "hud_corner").Draw(new Point(UI.WIDTH / 2 + 1, UI.HEIGHT - 100), 32, 32, colorB,90); //2
            new GTASprite("helicopterhud", "hud_corner").Draw(new Point(UI.WIDTH / 2 - 33, UI.HEIGHT - 100 + 32), 32, 32, colorC,270); //3
            new GTASprite("helicopterhud", "hud_corner").Draw(new Point(UI.WIDTH / 2 + 1, UI.HEIGHT - 100 + 32), 32, 32, colorD, 180); //4

            if(Items.Count > 3)
            {
                new GTASprite("helicopterhud", "hud_line").Draw(new Point(UI.WIDTH / 2 - 33 - 10, UI.HEIGHT - 84), 20, 3, colorA); //1
            }

            if(Items.Count > 2)
            {
                new GTASprite("helicopterhud", "hud_line").Draw(new Point(UI.WIDTH / 2 - 33 - 10, UI.HEIGHT - 84 + 30), 20, 3, colorC); //3
            }

            if(Items.Count > 0)
            {
                new GTASprite("helicopterhud", "hud_line").Draw(new Point(UI.WIDTH / 2 + 1 + 20, UI.HEIGHT - 84), 20, 3, colorB); //2
            }

            if(Items.Count > 1)
            {
                new GTASprite("helicopterhud", "hud_line").Draw(new Point(UI.WIDTH / 2 + 1 + 20, UI.HEIGHT - 84 + 30), 20, 3, colorD); //4
            }

            var bottomY = offset.Height + 70 + Items.Count * ItemHeight;
            if (HasFooter) bottomY += FooterHeight;
            bottomY += HeaderHeight;
            
            if(ExtendWindowHeight)
            {
                new UIRectangle(new Point(offset.Width, bottomY), new Size(300, UI.HEIGHT), UnselectedItemColor).Draw();
                new UIRectangle(new Point(offset.Width + 300, offset.Height - UI.HEIGHT), new Size(2, UI.HEIGHT * 2), Color.FromArgb(200, 8, 8, 8)).Draw();
            }
        }

        public override void OnActivate()
        {
            _activationAction.Invoke(this);
        }

        public override void OnChangeItem(bool right)
        {
            switch(SelectedIndex)
            {
                case 0:
                    SelectedIndex = right ? SelectedIndex : (Items.Count >= 4 ? 3 : SelectedIndex);
                    break;
                case 1:
                    SelectedIndex = right ? SelectedIndex : (Items.Count >= 3 ? 2 : SelectedIndex);
                    break;
                case 2:
                    SelectedIndex = right ? (Items.Count >= 2 ? 1 : SelectedIndex) : SelectedIndex;
                    break;
                case 3:
                    SelectedIndex = right ? 0 : SelectedIndex;
                    break;
            }
        }

        public override void Draw()
        {
            if (!RPG.UIHandler.ShowUI) return;

            Draw(new Size());
        }

        public override void OnChangeSelection(bool down)
        {
            int newIndex = down ? SelectedIndex + 1 : SelectedIndex - 1;
            if (newIndex >= Items.Count) newIndex = 0;
            if (newIndex < 0) newIndex = Items.Count - 1;


            while (!Usable(Items[newIndex]))
            {
                newIndex += down ? 1 : -1;
                if (newIndex >= Items.Count) newIndex = 0;
                if (newIndex < 0) newIndex = Items.Count - 1;
            }

            OnChangeSelection(newIndex);
        }

        public override void OnChangeSelection(int newIndex)
        {
            if (!Usable(Items[newIndex]))
            {
                newIndex++;
                if (newIndex >= Items.Count) newIndex = 0;
                if (newIndex < 0) newIndex = Items.Count - 1;
            }

            base.OnChangeSelection(newIndex);
        }

        private bool Usable(IMenuItem item)
        {
            return new[] { typeof(MenuLabel) }.All(m => item.GetType() != m);
        }
    }
}