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
    public static class RectangleExtensions
    {
        //Returns amount of overlap between 2 intersecting recangles.
        //Allows pushes to objects to resolve collision
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            //Calculate half size.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            //Get centers
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            //Calculate centers and minimum-non-intersecting distance between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            //If not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            //Calculate and return intersection depths.
            float depthX;
            float depthY;
            if (distanceX > 0)
            {
                depthX = minDistanceX - distanceX;
            }
            else
            {
                depthX = -minDistanceX - distanceX;
            }
            if (distanceY > 0)
            {
                depthY = minDistanceY - distanceY;
            }
            else
            {
                depthY = -minDistanceY - distanceY;
            }
            return new Vector2(depthX, depthY);
        }
    }
}
