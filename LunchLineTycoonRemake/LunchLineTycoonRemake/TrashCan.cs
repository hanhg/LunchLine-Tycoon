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
    public class TrashCan : Tile
    {
        public TrashCan(int screenWidth, int screenHeight, int x, int y, ContentManager content, Texture2D texture, float layerDepth) :
            base(screenWidth, screenHeight, x, y, true, content, texture, layerDepth)
        {

        }

        public void ThrowAway(Player player)
        {
            level.items.Remove(player.heldItem);
            player.heldItem = null;
        }
    }
}
