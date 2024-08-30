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
    /* ideas
     * Salad            spin joystick
       Pasta            quick time (boil time)
       MashedPotatoes   spam button
    */
    public class CookingStation : Tile
    {
        public CookingStationType type;
        public Player user;
        public Texture2D inUse;
        public CookingStation(int screenWidth, int screenHeight, int x, int y, ContentManager content, Texture2D texture, float layerDepth) :
            base(screenWidth, screenHeight, x, y, true, content, texture, layerDepth)
        {
            user = null;
            canCollide = true;
            color = Color.White;
        }

        public void ChangeUser(Player player)
        {
            user = player;
            if (user != null)
            {
                switch (type)
                {
                    case CookingStationType.blend:
                        inUse = level.tileTextures[11];
                        break;
                    case CookingStationType.chop:
                        inUse = level.tileTextures[12];
                        break;
                    case CookingStationType.boil:
                        inUse = level.tileTextures[13];
                        break;

                }
            }

        }

        public void MakeFood()
        {
            if (user.heldItem is Ingredient && user.heldItem != null)
            {
                Rectangle rect = new Rectangle(rectangle.X + rectangle.Width / 4, rectangle.Y,
                    rectangle.Width / 2, rectangle.Height / 2);
                switch (type)
                {
                    case (CookingStationType.blend):
                        if (((Ingredient)user.heldItem).type == IngredientType.Potato)
                        {
                            user.ButtonMash();

                            user.waiting = new Food(rect, level.foodTextures[2],
                                new Vector2(level.foodTextures[2].Width / 2, level.foodTextures[2].Height / 2), FoodType.MashedPotatoes, level.layerDepths["Items"]);

                        }
                        break;
                    case (CookingStationType.chop):
                        if (((Ingredient)user.heldItem).type == IngredientType.Lettuce || ((Ingredient)user.heldItem).type == IngredientType.Tomato)
                        {
                            user.ButtonMash();

                            user.waiting = new Food(rect, level.foodTextures[0],
                                new Vector2(level.foodTextures[0].Width / 2, level.foodTextures[0].Height / 2), FoodType.Salad, level.layerDepths["Items"]);

                        }
                        break;
                    case (CookingStationType.boil):
                        if (((Ingredient)user.heldItem).type == IngredientType.Tomato)
                        {
                            user.ButtonMash();

                            user.waiting = new Food(rect, level.foodTextures[1],
                                new Vector2(level.foodTextures[1].Width / 2, level.foodTextures[1].Height / 2), FoodType.Pasta, level.layerDepths["Items"]);

                        }
                        else if (((Ingredient)user.heldItem).type == IngredientType.UncookedNoodle)
                        {
                            user.ButtonMash();

                            user.waiting = new Food(rect, level.foodTextures[1],
                                new Vector2(level.foodTextures[1].Width / 2, level.foodTextures[1].Height / 2), FoodType.Pasta, level.layerDepths["Items"]);
                        }
                        break;
                }
            }
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (user == null)
                spriteBatch.Draw(texture, rectangle, null, color, 0, Vector2.Zero, effect, 0);
            else
            {
                spriteBatch.Draw(inUse, rectangle, null, color, 0, Vector2.Zero, effect, 0);
            }


        }
    }
}
