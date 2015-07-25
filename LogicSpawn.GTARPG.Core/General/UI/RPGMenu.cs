using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;

namespace LogicSpawn.GTARPG.Core.General
{
    public class RPGMenu : Menu
    {
        private readonly GTASprite _banner;
        private readonly string _bannerCaption;

        public RPGMenu(string bannerCaption, GTASprite banner, IMenuItem[] items) : base("", items)
        {
            _banner = banner;
            _bannerCaption = bannerCaption;
        }

        public override void Draw(Size offset)
        {
            if (!RPG.UIHandler.ShowUI) return;

            new UIRectangle(new Point(offset.Width, offset.Height - UI.HEIGHT), new Size(300, UI.HEIGHT), UnselectedItemColor).Draw();
            _banner.Draw(new Point(offset.Width,offset.Height), 300, 70);
            new UIText(_bannerCaption, new Point(offset.Width + 150, offset.Height + 18), 0.8f, Color.White, 0, true).Draw();
            base.Draw(offset + new Size(0, 70));

            var bottomY = offset.Height + 70 + Items.Count*ItemHeight;
            if (HasFooter) bottomY += FooterHeight;
            bottomY += HeaderHeight;
            new UIRectangle(new Point(offset.Width, bottomY), new Size(300, UI.HEIGHT), UnselectedItemColor).Draw();
            new UIRectangle(new Point(offset.Width + 300, offset.Height - UI.HEIGHT), new Size(2, UI.HEIGHT*2), Color.FromArgb(200, 8, 8, 8)).Draw();

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
            return new[] {typeof (MenuLabel)}.All(m => item.GetType() != m);
        }
    }
}