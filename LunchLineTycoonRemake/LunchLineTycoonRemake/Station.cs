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
    public class Station : Tile
    {
        //Amount of food levels this station has before it is empty. Max food levels
        public int foodCapacity;

        //Amount of food this station has
        public int amountOfFoodLeft;

        //Is a student waiting at this station?
        public bool studentWaiting;

        //List of students at the station
        public Queue<Child> childrenWaiting;

        //With food, what type of station is it (what food does it hold)
        public Food foodBeingHeld;

        public Texture2D[] textures;

        public Station(int screenWidth, int screenHeight, int x, int y, int foodCapacity, ContentManager content, Texture2D texture, float layerDepth) : base(screenWidth, screenHeight, x, y, true, content, texture, layerDepth)
        {
            this.foodCapacity = foodCapacity;
            canCollide = true;
            childrenWaiting = new Queue<Child>();
            foodBeingHeld = null;
            textures = new Texture2D[4];
            textures[3] = texture;
            LoadContent(content);
        }

        //Add food to the station returns whether or not adding food worked
        public bool AddFood(Food newFood)
        {
            if (foodBeingHeld == null)
            {
                amountOfFoodLeft++;
                foodBeingHeld = newFood;
                texture = textures[(int)(newFood.foodType)];
                return true;
            }
            else if (foodBeingHeld.foodType.Equals(newFood.foodType) && (amountOfFoodLeft < foodCapacity))
            {
                amountOfFoodLeft++;
                return true;
            }
            return false;
        }


        //Remove food from the station
        public bool RemoveFood()
        {
            if (amountOfFoodLeft > 0)
            {
                amountOfFoodLeft--;
                if (amountOfFoodLeft == 0)
                    texture = textures[3];
                return true;
            }
            return false;
        }

        public Boolean equals(Rectangle r)
        {
            if (this.rectangle.X == r.X && this.rectangle.Y == r.Y)
            {
                return true;
            }
            return false;
        }
        public Boolean equals(Vector2 position)
        {
            if (position.X == this.position.X && position.Y == this.position.Y)
            {
                return true;
            }
            return false;
        }
        public Boolean equals(int x, int y)
        {
            if (x == position.X && y == position.Y)
            {
                return true;
            }
            return false;
        }
        public void LoadContent(ContentManager content)
        {
            textures[0] = content.Load<Texture2D>("Tiles/Stations/Full Salad Station");
            textures[1] = content.Load<Texture2D>("Tiles/Stations/Full Pasta Station");
            textures[2] = content.Load<Texture2D>("Tiles/Stations/Full Mashed Potatoes Station");
        }
        public override void Update(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Update(gameTime, spriteBatch);
            if (foodBeingHeld == null)
                texture = level.tileTextures[1];
            Child[] arr = childrenWaiting.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                Child child = arr[i];
                child.rectangle.Y = rectangle.Y - level.tileHeight - ((i + 1) * level.tileWidth / 10);
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            Child[] arr = childrenWaiting.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                Child child = arr[i];
                if (child.order.foodRequirements.Count > 0)
                {
                    Food food = child.order.foodRequirements[0];
                    Texture2D texture = level.foodTextures[(int)food.foodType];
                    Rectangle rect = new Rectangle(child.rectangle.Right - Width / 2, child.rectangle.Top - Width / 4, child.rectangle.Width / 2, child.rectangle.Height / 2);
                    if (i == 0)
                        spriteBatch.Draw(texture, rect, null, Color.White, 0, Vector2.Zero, effect, 0);
                }
            }
        }
    }
}
