using System;
using System.Drawing;
using GTA;

namespace LogicSpawn.GTARPG.Core.General
{
    public class RPGMessageBox : Menu
    {
        private readonly string _headerCaption;
        private readonly string _yesText;
        private readonly string _noText;
        private bool _oldShowUI;
        public Color TopColor = Color.DodgerBlue;
        public float HeaderScale = 0.7f;

        public RPGMessageBox(string headerCaption, string yesText, string noText, Action yesAction, Action noAction) :
            base("", new IMenuItem[] { new MenuButton(yesText),new MenuButton(noText)})
        {
            _headerCaption = headerCaption;
            _yesText = yesText;
            _noText = noText;

            var buttonYes = (MenuButton)Items[0];
            buttonYes.Activated += (sender, args) => yesAction.Invoke();

            var buttonNo = (MenuButton)Items[1];
            buttonNo.Activated += (sender, args) => noAction.Invoke();

        }

        public override void Draw(Size offset)
        {

            HeaderColor = Color.White;

            var container = new UIContainer(new Point(), new Size(UI.WIDTH, UI.HEIGHT), Color.FromArgb(180, 45, 45, 45));

            container.Items.Add(new UIRectangle(new Point(0, UI.HEIGHT / 2 - 40), new Size(UI.WIDTH, 45), TopColor));
            container.Items.Add(new UIText(_headerCaption.ToUpper(), new Point(UI.WIDTH / 2, UI.HEIGHT / 2 - 40 + 5), HeaderScale, HeaderColor, 0, true));
            
            container.Items.Add(new UIRectangle(new Point(0, UI.HEIGHT / 2 - 40 + 45), new Size(UI.WIDTH, 20), SelectedIndex == 0 ? SelectedItemColor : UnselectedItemColor));
            container.Items.Add(new UIText("- " + _yesText + " -", new Point(UI.WIDTH / 2, UI.HEIGHT / 2 - 40 + 45), 0.3f, SelectedIndex == 0 ? SelectedTextColor : UnselectedTextColor, 0, true));

            container.Items.Add(new UIRectangle(new Point(0, UI.HEIGHT / 2 - 40 + 65), new Size(UI.WIDTH, 20), SelectedIndex == 1 ? SelectedItemColor : UnselectedItemColor));
            container.Items.Add(new UIText("- " + _noText + " -", new Point(UI.WIDTH / 2, UI.HEIGHT / 2 - 40 + 65), 0.3f, SelectedIndex == 1 ? SelectedTextColor : UnselectedTextColor, 0, true));

            container.Draw();
        }

        public override void OnOpen()
        {
            SelectedIndex = 1;
            _oldShowUI = RPG.UIHandler.ShowUI;
            RPG.UIHandler.ShowUI = false;
            Game.Player.CanControlCharacter = false;
        }

        public override void OnActivate()
        {
            RPG.UIHandler.View.RemoveMenu(this);
            base.OnActivate();
        }

        public override void OnClose()
        {
            Game.Player.CanControlCharacter = true;
            RPG.UIHandler.ShowUI = _oldShowUI;
        }

        public override void Draw()
        {
            Draw(new Size());
        }

        public static MenuBase Create(string headerCaption, string yesText, string noText, Action yesAction, Action noAction)
        {
            var b = new RPGMessageBox(headerCaption, yesText, noText, yesAction, noAction);
            return b;
        }
        public static MenuBase Create(string headerCaption, Action yesAction, Action noAction)
        {
            var b = new RPGMessageBox(headerCaption, "Yes", "No", yesAction, noAction);
            return b;
        }
        public static MenuBase CreateOkCancel(string headerCaption, Action yesAction, Action noAction)
        {
            var b = new RPGMessageBox(headerCaption, "OK", "Cancel", yesAction, noAction);
            return b;
        }
    }
}