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
    public abstract class Sprite
    {
        public Rectangle rectangle;
        public Texture2D texture;
        public Vector2 origin;

        public int Width;
        public int Height;

        public SpriteEffects effect;

        public float layerDepth;

        public Sprite(int tileWidth, int tileHeight, int x, int y, Texture2D texture, Vector2 origin, float layerDepth)
        {
            rectangle = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
            Width = tileWidth;
            Height = tileHeight;
            this.texture = texture;
            this.origin = origin;
            this.layerDepth = layerDepth;
            effect = SpriteEffects.None;
        }
        public Sprite(int tileWidth, int tileHeight, int x, int y, Texture2D texture, Vector2 origin, SpriteEffects e, float layerDepth)
        {
            rectangle = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
            Width = tileWidth;
            Height = tileHeight;
            this.texture = texture;
            this.origin = origin;
            this.layerDepth = layerDepth;
            effect = e;
        }

        public Sprite(Rectangle rectangle, Texture2D texture, Vector2 origin, float layerDepth)
        {
            this.rectangle = rectangle;
            this.texture = texture;
            this.origin = origin;
            this.layerDepth = layerDepth;
            effect = SpriteEffects.None;
        }
        public Sprite(Rectangle rectangle, Texture2D texture, Vector2 origin, SpriteEffects e, float layerDepth)
        {
            this.rectangle = rectangle;
            this.texture = texture;
            this.origin = origin;
            this.layerDepth = layerDepth;
            effect = e;
        }

        public Vector2 getCenter()
        {
            return new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2); ;
        }

        //It draws
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        //Updates
        public abstract void Update(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
