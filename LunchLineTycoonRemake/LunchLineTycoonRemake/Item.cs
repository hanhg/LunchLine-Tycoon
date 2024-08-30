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

    public class Item : Sprite
    {
        //Used to see if this item is currently being held by the player
        public bool isBeingHeld;

        //Is this food on the conveyer?
        public bool isOnConveyer;

        //If its on the conveyer which conveyer is it on?
        public Conveyor conveyer;

        //Shadow of texture

        public Item(Rectangle rect, Texture2D texture, Vector2 origin, float layerDepth) : base(rect, texture, origin, layerDepth) //instance of food (child order)
        {

        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Rectangle adjusted because it gets messed up with rotation
            spriteBatch.Draw(texture, new Rectangle(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Width / 2, rectangle.Width, rectangle.Height),
                null, Color.White, 0, origin, SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
        public void removeFromConveyor()
        {
            isOnConveyer = false;
            conveyer.hasFood = false;
        }
    }
}
