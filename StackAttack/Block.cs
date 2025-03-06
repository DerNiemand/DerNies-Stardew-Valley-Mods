using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StackAttack;

namespace StackAttack
{
    class Block
    {
        private readonly Texture2D blockTexture;
        private float scale;

        private Vector2 position;

        public Block(float scale, Vector2 position)
        {
            blockTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/Block.png");
            this.scale = scale;
            this.position = position;
        }

        public void Draw(SpriteBatch b)
        {
            b.Draw(blockTexture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public void Move(Vector2 direction)
        {
            position += direction;
        }

        public void UpdateScale(float scale, Vector2 pos)
        {
            this.scale = scale;

            position = pos;
        }
    }
}
