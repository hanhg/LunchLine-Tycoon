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
    public class Food : Item
    {
        //The type of food that is used
        public FoodType foodType;
        public bool isPrepared;

        //Creates an array holding the values in FoodType that is used to create a random food.
        public static Array foodTypeArray = Enum.GetValues(typeof(FoodType));

        public Food(Rectangle rect, Texture2D texture, Vector2 origin, FoodType foodType, float layerDepth) : base(rect, texture, origin, layerDepth)
        {
            this.foodType = foodType;
            isPrepared = false;
        }

        public static FoodType RandomFoodType(Random rnd)
        {
            return (FoodType)foodTypeArray.GetValue(rnd.Next(foodTypeArray.Length));
        }

        public bool equals(object obj)
        {
            Food food = (Food)obj;
            return foodType == food.foodType;
        }

        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //TODO:
        }
    }
}
