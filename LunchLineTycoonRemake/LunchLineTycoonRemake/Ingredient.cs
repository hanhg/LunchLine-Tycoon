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
    class Ingredient : Item
    {
        public IngredientType type;

        //Creates an array holding the values in IngredientType that is used to create a random ingredient.
        public static Array ingredientTypeArray = Enum.GetValues(typeof(IngredientType));

        public Ingredient(Rectangle rect, Texture2D texture, Vector2 origin, IngredientType type, Conveyor c, float layerDepth) : base(rect, texture, origin, layerDepth)
        {
            this.type = type;
            isOnConveyer = true;
            conveyer = c;
        }

        public static IngredientType RandomIngredientType(Random rnd)
        {
            return (IngredientType)ingredientTypeArray.GetValue(rnd.Next(ingredientTypeArray.Length));
        }

    }


}
