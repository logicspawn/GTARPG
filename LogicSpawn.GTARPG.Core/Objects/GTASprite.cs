using System.Drawing;
using GTA;
using GTA.Native;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class GTASprite
    {
        public string TextureDict;
        public string TextureName;
        public Color Color;

        public GTASprite()
        {

        }

        public GTASprite(string dict, string name)
        {
            TextureDict = dict;
            TextureName = name;
            Color = Color.White;
        }

        public GTASprite(string dict, string name, Color color)
        {
            TextureDict = dict;
            TextureName = name;
            Color = color;
        }

        public void Draw(Point position, int width, int height, float rotation = 0.0f)
        {
            Draw(position,width,height,Color,rotation);
        }

        public void Draw(Point position, int width, int height, Color color, float rotation = 0.0f)
        {
            var x = (position.X + (float)width / 2) / UI.WIDTH;
            var y = (position.Y + (float)height / 2) / UI.HEIGHT;


            //scale x and y relative to UI.WIDTH/HEIGHT
            //width of 100 
            var scalex = (float) width/UI.WIDTH;
            var scaley = (float) height/UI.HEIGHT;
            if(!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED,TextureDict))
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TextureDict, true);
            }

            Function.Call(Hash.DRAW_SPRITE, TextureDict,TextureName,x,y,scalex, scaley, rotation,color.R, color.G, color.B, color.A);
        }

        public void DrawFlippedHori(Point position, int width, int height, float rotation = 0.0f)
        {
            DrawFlippedHori(position, width, height, Color, rotation);
        }
        public void DrawFlippedHori(Point position, int width, int height, Color color, float rotation = 0.0f)
        {
            var x = (position.X + (float)width / 2) / UI.WIDTH;
            var y = (position.Y + (float)height / 2) / UI.HEIGHT;


            //scale x and y relative to UI.WIDTH/HEIGHT
            //width of 100 
            var scalex = ((float)width / UI.WIDTH) * -1f;
            var scaley = (float)height / UI.HEIGHT;
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TextureDict))
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TextureDict, true);
            }

            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, x, y, scalex, scaley, rotation, color.R, color.G, color.B, color.A);
        }

        public void DrawFlippedVert(Point position, int width, int height, float rotation = 0.0f)
        {
            DrawFlippedVert(position, width, height, Color, rotation);
        }
        public void DrawFlippedVert(Point position, int width, int height, Color color, float rotation = 0.0f)
        {
            var x = (position.X + (float)width / 2) / UI.WIDTH;
            var y = (position.Y + (float)height / 2) / UI.HEIGHT;


            //scale x and y relative to UI.WIDTH/HEIGHT
            //width of 100 
            var scalex = ((float)width / UI.WIDTH);
            var scaley = ((float)height / UI.HEIGHT) * -1f;
            if (!Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, TextureDict))
            {
                Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, TextureDict, true);
            }

            Function.Call(Hash.DRAW_SPRITE, TextureDict, TextureName, x, y, scalex, scaley, rotation, color.R, color.G, color.B, color.A);
        }
    }
}