using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StackAttack
{
    class DropBlock : Block
    {
        private int dropAmount;

        public DropBlock(float scale, Vector2 position, int dropAmount) : base(scale, position)
        {
            this.dropAmount = dropAmount;
            blockTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/BlockDropped.png");
        }

        public DropBlock(Block block, int dropAmount) : base(block)
        {
            this.dropAmount = dropAmount;
            blockTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/BlockDropped.png");
        }

        public DropBlock(DropBlock dropBlock) : base(dropBlock)
        {
            dropAmount = dropBlock.dropAmount;
            blockTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/BlockDropped.png");
        }

        public new bool Move(Vector2 direction)
        {
            base.Move(direction);
            dropAmount--;
            return dropAmount < 0;
        }
    }
}
