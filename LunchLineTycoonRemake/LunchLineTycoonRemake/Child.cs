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
    public class Child : Sprite
    {
        //Order keeps track of how many stations a student is still going to, current station is the current station the
        //student is waiting at
        public Task order;
        //StationType currentStation = StationType.pasta;
        //Goes in order of base, skin, clothes, face, hair
        Texture2D[] textures;
        //Not Implemented
        public int satisfaction;

        public int timeWaiting;

        public const double maxMoney = 15;
        public const int maxWaitTime = (int)(60 * 2.5 * 60); //2.5 minutes in frames

        //Go into level class and add a this.MemberwiseClone to the children loop
        public Child(Task task, Level level, ContentManager Content, int screenWidth, int screenHeight, int x, int y, float layerDepth) :
            base(screenWidth, screenHeight, x, y, null, new Vector2(0, 0), layerDepth)
        {
            timeWaiting = 0;
            order = task;
            //currentStation = StationType.pasta;
            textures = new Texture2D[5];
            LoadContent(Content);
        }

        //Loads textures non-randomly
        public void LoadContent(ContentManager content)
        {
            //Randomize later always load textures for children in this order
            textures[0] = content.Load<Texture2D>("Children Pieces/Base/Child_Base");
            textures[1] = content.Load<Texture2D>("Children Pieces/Skin/Skin");
            textures[2] = content.Load<Texture2D>("Children Pieces/Clothes/Clothes1");
            textures[3] = content.Load<Texture2D>("Children Pieces/Faces/Face1");
            textures[4] = content.Load<Texture2D>("Children Pieces/Hair/Hair1");
        }

        //Possibly change later, meant to update children according to their orders being completed
        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //order.Update(gameTime, spriteBatch);
            timeWaiting++;
        }

        //Calls the method to draw the child
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawChild(gameTime, spriteBatch);
        }


        public double pay()
        {
            if (timeWaiting > maxWaitTime)
                return 0;
            else
            {
                double paid = maxMoney * ((maxWaitTime - timeWaiting) / (double)maxWaitTime);
                double dollars = Math.Round(paid * 100) / 100.0;
                return dollars;
            }
        }
        //Draws the child
        private void DrawChild(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                if (i == 1)
                    spriteBatch.Draw(textures[i], rectangle, null, Color.PeachPuff, 0, Vector2.Zero, effect, 0);
                else
                    spriteBatch.Draw(textures[i], rectangle, null, Color.White, 0, Vector2.Zero, effect, 0);
            }
        }
    }
}
