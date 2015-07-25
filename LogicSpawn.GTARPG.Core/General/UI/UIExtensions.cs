using System;
using GTA;

namespace LogicSpawn.GTARPG.Core.General
{
    public static class UIExtensions
    {
         public static MenuButton WithActivate(this MenuButton button, Action action)
         {
             button.Activated += (sender, args) => action.Invoke();
             return button;
         }

         public static MenuEnumScroller WithEnumActions(this MenuEnumScroller enumScroller, Action<int> changeAction, Action<int> activateAction)
         {
             enumScroller.Changed += (sender, args) => changeAction.Invoke(enumScroller.Index);
             enumScroller.Activated += (sender, args) => activateAction.Invoke(enumScroller.Index);
             return enumScroller;
         }

         public static MenuNumericScroller WithNumericActions(this MenuNumericScroller numScroller, Action<double> changeAction, Action<double> activateAction)
         {
             numScroller.Changed += (sender, args) => changeAction.Invoke(numScroller.Value);
             numScroller.Activated += (sender, args) => activateAction.Invoke(numScroller.Value);
             return numScroller;
         }

         public static MenuToggle WithToggles(this MenuToggle toggle, Action on, Action off)
         {
             toggle.Changed += (sender, args) =>
             {
                 if (toggle.Value)
                 {
                     on.Invoke();
                 }
                 else
                 {
                     off.Invoke();
                 }
             };
             return toggle;
         }
    }
}