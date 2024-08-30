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
    public class Tile : Sprite
    {
        //Stores position of tile in tile map
        public Vector2 position;

        //Handles player interaction and student updating
        Boolean canPlayerInteract;
        Boolean playerInteracted;

        //Used to see of the player can collide with the tile
        public bool canCollide;

        //Used to check collision
        public Rectangle hitBox;

        //Color it is drawn with
        public Color color;

        public Level level;

        //Contstructor for use with textures NOT in a sprite sheet
        public Tile(int screenWidth, int screenHeight, int x, int y, bool canInteract, ContentManager content, Texture2D texture, float layerDepth) : base(screenWidth, screenHeight, x, y, texture, new Vector2(0, 0), layerDepth)
        {
            position = new Vector2(x, y);
            canPlayerInteract = canInteract;
            hitBox = new Rectangle(Width * x, Height * y, Width, Height);
            color = Color.White;
            LoadContent(content);
        }
        public Tile(int screenWidth, int screenHeight, int x, int y, bool canInteract, ContentManager content, Texture2D texture, SpriteEffects e, float layerDepth) : base(screenWidth, screenHeight, x, y, texture, new Vector2(0, 0), e, layerDepth)
        {
            position = new Vector2(x, y);
            canPlayerInteract = canInteract;
            hitBox = new Rectangle(Width * x, Height * y, Width, Height);
            color = Color.White;
            LoadContent(content);
        }

        private void LoadContent(ContentManager content)
        {
            //Deprecated
        }

        //Draws the tile's texture at the position using the position vector
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, null, color, 0, Vector2.Zero, effect, 0);
        }

        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (canPlayerInteract && playerInteracted)
            {
                playerInteracted = false;
            }
        }

        public void playerInteraction()
        {
            playerInteracted = true;
        }
    }
}
