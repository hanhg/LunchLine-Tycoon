using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunchLineTycoonRemake
{
    public class Counter : Tile
    {
        public Item held;
        public Counter(int screenWidth, int screenHeight, int x, int y, ContentManager content, Texture2D texture, float layerDepth) :
            base(screenWidth, screenHeight, x, y, true, content, texture, layerDepth)
        {

        }

        public void addItem(Player player)
        {
            if (held == null)
            {
                held = player.heldItem;
                player.heldItem = null;
                held.rectangle.X = rectangle.X + rectangle.Width / 4;
                held.rectangle.Y = rectangle.Y;
                player.playerState = PlayerState.Carrying;
            }
        }
    }
}
