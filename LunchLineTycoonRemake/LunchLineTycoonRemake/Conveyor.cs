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
    public class Conveyor : Tile
    {
        //True if the conveyer belt has food on it
        public bool hasFood;

        //Current ingredient that is on the station
        public IngredientType currentIngredient;

        //Used to randomly add food to the conveyer belt
        public Random rnd;

        int timer;
        public Conveyor(int screenWidth, int screenHeight, int x, int y, Texture2D texture, Random rnd, Level level, ContentManager content, float layerDepth) : base(screenWidth, screenHeight, x, y, true, content, texture, layerDepth)
        {
            timer = 0;
            this.level = level;
            this.rnd = rnd;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, null, Color.White, 0, Vector2.Zero, effect, 0);
            if (hasFood)
            {
                //spriteBatch.Draw(level.foodTextures[(int)currentIngredient], rectangle, Color.White);
            }
        }

        public void RemoveFood()
        {
            hasFood = false;
        }

        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!hasFood && (rnd.Next(0, 120) == 0 || timer > 300))
            {
                currentIngredient = Ingredient.RandomIngredientType(rnd);

                hasFood = true;
                Texture2D ingredientTexture = level.ingredientTextures[(int)currentIngredient];
                Rectangle ingredientRect = new Rectangle(rectangle.X + rectangle.Width / 4, rectangle.Y,
                    rectangle.Width / 2, rectangle.Height / 2);
                level.items.Add(new Ingredient(ingredientRect, ingredientTexture, new Vector2(ingredientTexture.Width / 2, ingredientTexture.Height / 2), currentIngredient, this, level.layerDepths["Items"]));
                timer = 0;
            }
            timer++;
        }
    }
}
