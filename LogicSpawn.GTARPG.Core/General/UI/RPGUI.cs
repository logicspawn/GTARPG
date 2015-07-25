using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GTA;

namespace LogicSpawn.GTARPG.Core.General
{
    public static class RPGUI
    {
         public static void FormatMenu(MenuBase menu)
         {
             menu.HeaderFont = 0;
             menu.ItemFont = 0;
             menu.HeaderColor = Color.FromArgb(140,6,6,6);
             menu.HeaderTextColor = Color.White;
             menu.SelectedItemColor = Color.White;
             menu.SelectedTextColor = Color.Black;
             menu.UnselectedItemColor = Color.FromArgb(120, 8, 8, 8);
             menu.UnselectedTextColor = Color.White;
             menu.HeaderCentered = false;
             menu.ItemTextCentered = false;
             menu.ItemTextScale = 0.36f;
             menu.HeaderTextScale = 0.4f;
             
             var m = menu as Menu;
             if(m != null)
             {
                 m.ItemHeight = 25;
                 m.HeaderHeight = 28;
                 m.HasFooter = false;
                 m.Width = 300;
                 m.TextOffset = new Point(4,0);
                 m.HeaderHeight = 0;
             }

             var mb = menu as MessageBox;
             if(mb != null)
             {
                 mb.Width = 300;
                 mb.TextOffset = new Point(4, 0);
             }
         }

        public static void FormatMenuFooter(MenuBase menu)
        {
            var m = menu as Menu;
            if (m != null)
            {
                m.HasFooter = true;
                m.FooterHeight = 25;
            }

            menu.FooterCentered = true;
            menu.FooterColor = Color.FromArgb(120, 25, 25, 25);
            menu.FooterFont = 0;
            menu.FooterCentered = false;
            menu.FooterTextColor = Color.White;
            menu.FooterTextScale = 0.25f;
        }

        public static void FormatMenuWithFooter(MenuBase menuBase)
        {
            FormatMenu(menuBase);
            FormatMenuFooter(menuBase);
        }

        public static void SetAlpha(UIContainer popup, int alpha)
        {
            for (int index = 0; index < popup.Items.Count; index++)
            {
                var i = popup.Items[index]; 
                var r = i.Color.R;
                var g = i.Color.G;
                var b = i.Color.B;
                i.Color = Color.FromArgb(alpha, r, g, b);
            }
        }

        public static string[] FormatText(string text, int maxLineLength)
        {
            var formattedText = new List<string>();
            
            //var lines = text.Split('\n');
            //foreach(var line in lines)
            //{
                //if(line.Length > maxLineLength)
                formattedText.AddRange(EnumByNearestSpace(text, maxLineLength));
                //else
                //{
                //    formattedText.Add(line);
                //}
            //}
            return formattedText.ToArray();
        }

        public static IEnumerable<String> EnumByNearestSpace(this String value, int length)
        {
            if (String.IsNullOrEmpty(value))
                yield break;


            int bestDelta = int.MaxValue;
            int bestSplit = -1;

            int from = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                var Ch = value[i];

                if (Ch != ' ')
                    continue;

                int size = (i - from);
                int delta = (size - length > 0) ? size - length : length - size;

                if ((bestSplit < 0) || (delta < bestDelta))
                {
                    bestSplit = i;
                    bestDelta = delta;
                }
                else
                {
                    yield return value.Substring(from, bestSplit - from);

                    i = bestSplit;

                    from = i + 1;
                    bestSplit = -1;
                    bestDelta = int.MaxValue;
                }
            }

            // String's tail
            if (from < value.Length)
            {

                if (bestSplit >= 0)
                {
                    if (bestDelta < value.Length - from)
                    {
                        yield return value.Substring(from, bestSplit - from);
                        from = bestSplit + 1;
                    }

                }



                if (from < value.Length)
                    yield return value.Substring(from);


            }
        }

    }
}