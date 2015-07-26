using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using Font = GTA.Font;

namespace LogicSpawn.GTARPG.Core
{
    public class CharCreationNewUI : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return true; } }

        public CharCreationNewUI()
        {
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (CharCreationNew.Enabled && keyEventArgs.KeyCode == Keys.Back)
            {
                CharCreationNew.Instance.HandleBack();
            }
        }

        public override void Update()
        {
            if (!CharCreationNew.Enabled) return;

            if(CharCreationNew.LoadingCharCreate)
            {
                new UIRectangle(new Point(0,0),new Size(UI.WIDTH,UI.HEIGHT), Color.Black ).Draw();
                new GTASprite("loadingscreen5","background").Draw(new Point(),UI.WIDTH,UI.HEIGHT);
                new UIText("Loading Character Creation...", new Point(UI.WIDTH - 300, UI.HEIGHT - 80), 1.0f, Color.White, Font.HouseScript, true).Draw();
            }

            switch (CharCreationNew.State)
            {
                case CharCreationNew.CharCreationState.Character:
                case CharCreationNew.CharCreationState.Car:
                    new UIRectangle(new Point(UI.WIDTH - 334, UI.HEIGHT - 23), new Size(329, 14), Color.FromArgb(120, 8, 8, 8)).Draw();
                    new UIText("[backspace] back [5] select [num7] rotate left [num9] rotate right [num4] left [num6] right [num8] up [num2] down ", new Point(UI.WIDTH - 332, UI.HEIGHT - 23), 0.18f, Color.White, Font.ChaletLondon, false).Draw();
                    break;
                case CharCreationNew.CharCreationState.Finalise:
                    new UIRectangle(new Point(UI.WIDTH - 264, UI.HEIGHT - 23), new Size(259, 14), Color.FromArgb(120, 8, 8, 8)).Draw();
                    new UIText("[backspace] back [5] select [num4] left [num6] right [num8] up [num2] down ", new Point(UI.WIDTH - 262, UI.HEIGHT - 23), 0.18f, Color.White, 0, false).Draw();
                    break;
            }
            

            if (CharCreationNew.State != CharCreationNew.CharCreationState.Intro) return;


            switch(CharCreationNew.IntroState)
            {
                case CharCreationNew.CharIntroState.A:
                    A();
                    while (CharCreationNew.IntroState == CharCreationNew.CharIntroState.A) Wait(0);
                    break;
                case CharCreationNew.CharIntroState.B:
                    B();
                    while (CharCreationNew.IntroState == CharCreationNew.CharIntroState.B) Wait(0);
                    break;
                case CharCreationNew.CharIntroState.C:
                    C();
                    while (CharCreationNew.IntroState == CharCreationNew.CharIntroState.C) Wait(0);
                    break;
                case CharCreationNew.CharIntroState.D:
                    D();
                    while (CharCreationNew.IntroState == CharCreationNew.CharIntroState.D) Wait(0);
                    break;
                case CharCreationNew.CharIntroState.E:
                    E();
                    while (CharCreationNew.IntroState == CharCreationNew.CharIntroState.E) Wait(0);
                    break;
                case CharCreationNew.CharIntroState.Init:
                case CharCreationNew.CharIntroState.Finished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void A()
        {
            //new UIText("LogicSpawn Presents...", new Point(UI.WIDTH - 300, UI.HEIGHT - 80), 1.4f, Color.Black, 1, true).Draw();
            RPG.GetPopup<IntroCutscene>().Show("LogicSpawn Presents", 1.3f);
        }
        private void B()
        {
            RPG.GetPopup<IntroCutscene>().Show("A Mod for GTA V", 1.2f);
        }
        private void C()
        {
            var gender = Game.Player.Character.Gender == Gender.Male ? " as Himself" : " as Herself";
            var name = Game.Player.Name == RPG.PlayerData.Name ? Game.Player.Name + gender : Game.Player.Name + " as " + RPG.PlayerData.Name; //later herself with gender
            //new UIText("Starring " + name, new Point(UI.WIDTH - 350, UI.HEIGHT - 80), 1.2f, Color.Black, 1, true).Draw();
            var fixedText = new string(("Starring " + name).Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
            RPG.GetPopup<IntroCutscene>().Show(fixedText, 1.0f);
        }
        private void D()
        {
            //new UIText("Special Thanks to Crosire and Alexander Blade", new Point(UI.WIDTH - 400, UI.HEIGHT - 80), 1.0f, Color.Black, 1, true).Draw();
            RPG.GetPopup<IntroCutscene>().Show("Special Thanks to Crosire  AlexanderBlade and SmashADBurn", 0.8f);
        }
        private void E()
        {
            RPG.GetPopup<IntroTitle>().Show();

            //new UIText("grand theft auto", new Point(UI.WIDTH / 2, UI.HEIGHT / 2 + 45), 0.5f, Color.White, 0, true).Draw();
            //new UIRectangle(new Point(0, UI.HEIGHT/2 + 45), new Size(UI.WIDTH, 25), Color.FromArgb(150, 2, 70, 200)).Draw();
            //
            //new UIText("- RPG - ", new Point(UI.WIDTH / 2, UI.HEIGHT / 2 + 60), 2.02f, Color.White,0, true).Draw();
            //new UIRectangle(new Point(0, UI.HEIGHT / 2 + 45 + 25), new Size(UI.WIDTH, 80), Color.FromArgb(150, 2, 70, 200)).Draw();
        }
    }
}