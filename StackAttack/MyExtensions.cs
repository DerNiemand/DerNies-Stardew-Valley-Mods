using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static Vector2 HalfBounds(this Texture2D v)
        {
            float halfX = v.Width / 2;
            float halfY = v.Height / 2;

            return new Vector2(halfX, halfY);
        }
    }
}
