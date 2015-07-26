using System;
using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Objects;
using LogicSpawn.GTARPG.Core.Scripts.Popups;
using Font = GTA.Font;

namespace LogicSpawn.GTARPG.Core
{
    public class LoadingUI : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return true; } }

        public override void Update()
        {
            if(RPG.Loading)
            {
                new UIRectangle(new Point(0,0),new Size(UI.WIDTH,UI.HEIGHT), Color.Black ).Draw();
                new GTASprite("loadingscreen5","background").Draw(new Point(),UI.WIDTH,UI.HEIGHT);
                new UIText("Loading ...", new Point(UI.WIDTH - 300, UI.HEIGHT - 80), 1.0f, Color.White, Font.HouseScript, true).Draw();
            }

        }
    }
}