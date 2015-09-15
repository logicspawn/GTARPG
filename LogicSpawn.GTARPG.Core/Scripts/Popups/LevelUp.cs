using System.Drawing;
using System.Linq;
using GTA;
using LogicSpawn.GTARPG.Core.Repository;

namespace LogicSpawn.GTARPG.Core.Scripts.Popups
{
    public class LevelUp : Popup
    {
        protected override void OnPopup(UIContainer popup, object[] args)
        {
            popup.Size = new Size(UI.WIDTH, 105);
            popup.Position = new Point(0, UI.HEIGHT / 2 - 200);

            popup.Items.Add(new UIText("congratulations", new Point(UI.WIDTH / 2, 0), 0.5f, Color.White, 0, true));
            popup.Items.Add(new UIRectangle(new Point(0, 0), new Size(UI.WIDTH, 25), Color.FromArgb(150, 2, 70, 200)));

            popup.Items.Add(new UIText("- you are now level " + RPG.PlayerData.Level + " -", new Point(UI.WIDTH / 2, 15), 1.5f, Color.White, 0, true));


            var unlockedWeapons = RPG.PlayerData.Weapons.Where(w => w.LevelToUnlock == RPG.PlayerData.Level).Select(w => w.WeaponName).ToArray();
            var unlocks = string.Join(", ", unlockedWeapons);

            if(unlockedWeapons.Any())
                popup.Items.Add(new UIText("- weapons unlocked: "+ unlocks + " -", new Point(UI.WIDTH / 2, 80), 0.32f, Color.White, 0, true));

            popup.Items.Add(new UIRectangle(new Point(0, 25), new Size(UI.WIDTH, 80), Color.FromArgb(150, 2, 70, 200)));
        }

        protected override void OnFinish()
        {
            
        }

        protected override bool CanRun
        {
            get { return true; }
        }

        protected override int TimeToShowMs
        {
            get { return 3000; }
        }
    }
}
