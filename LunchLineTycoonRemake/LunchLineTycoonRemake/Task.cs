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
    public class Task : Sprite
    {
        //The requirements of food for completing the task. Unfinished food
        public List<Food> foodRequirements;

        //Creating a new Task. Food will need to be added
        public Task(Rectangle rect, Texture2D texture, Vector2 origin, float layerDepth) : base(rect, texture, origin, layerDepth)
        {
            foodRequirements = new List<Food>();
        }

        //Creates a task that already contains food requirements
        public Task(Rectangle rect, Texture2D texture, Vector2 origin, List<Food> foodRequirements, float layerDepth) : base(rect, texture, origin, layerDepth)
        {
            this.foodRequirements = foodRequirements;
        }

        //Adds food to the food requirements
        public void AddFood(Food food)
        {
            foodRequirements.Add(food);
        }

        //Completes a food item on the task
        public void CompeleteFood(Food itemCompleted)
        {
            if (itemCompleted.Equals(foodRequirements[0]))
                foodRequirements.Remove(itemCompleted);
        }

        public bool IsComplete()
        {
            return foodRequirements.Count == 0;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, null, Color.White, 0, Vector2.Zero, effect, 0);
            for (int i = 0; i < foodRequirements.Count; i++)
            {
                foodRequirements[i].Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //TODO:
        }

    }
}
