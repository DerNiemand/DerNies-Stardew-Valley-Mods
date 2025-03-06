using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace StackAttack
{
    internal class BlockLine
    {
        private List<Block> blocks;

        public int BlockCount
        {
            get { return blocks.Count; }
        }

        private float scale;

        private int currentColumn;
        public int CurrentColumn
        {
            get { return currentColumn; }
        }

        private int maxColumn;

        private bool reversing;

        public BlockLine(int blockCount, float scale, int ColumnCount)
        {
            blocks = new List<Block>(blockCount);

            this.scale = scale;
            this.maxColumn = ColumnCount - 1;
        }

        public void StartLine(Vector2 pos)
        {
            blocks.Add(new Block(scale, pos));
        }

        public void Move(Vector2 direction)
        {
            Block? newBlock = null;
            if (blocks.Count != blocks.Capacity)
            {
                newBlock = blocks.Last().ShallowClone();
            }

            if (reversing)
            {
                foreach (Block block in blocks)
                {
                    block.Move(-direction);
                }

                currentColumn--;

                if (currentColumn <= blocks.Count - 1)
                {
                    reversing = false;
                }
            }
            else
            {
                foreach (Block block in blocks)
                {
                    block.Move(direction);
                }

                if (newBlock != null)
                {
                    blocks.Add(newBlock);
                }

                currentColumn++;

                if (currentColumn >= maxColumn)
                {
                    reversing = true;
                }
            }

        }

        public void Draw(SpriteBatch b)
        {
            foreach (var block in blocks)
            {
                block.Draw(b);
            }
        }

        public int GetBlockCount()
        {
            return blocks.Count;
        }

        public List<int> GetBlockColumns()
        {
            List<int> result = new List<int>();
            for (int i = 0; i < blocks.Count; i++)
            {
                result.Add(currentColumn - i);
            }

            return result;
        }

        public bool CheckBlockColumns(List<int> columns)
        {
            List<int> deletionIndicies = new List<int>();
            int columLeftShift = 0;
            for (int i = 0; i <= blocks.Count - 1; i++)
            {

                bool onOtherBlock = columns.Contains(currentColumn - i);
                if (!onOtherBlock)
                {
                    if (i == blocks.Capacity - BlockCount)
                    {
                        columLeftShift++;
                    }
                    deletionIndicies.Add(i);
                }
            }

            deletionIndicies.Sort();
            deletionIndicies.Reverse();

            foreach (int index in deletionIndicies)
            {
                blocks.RemoveAt(index);
            }


            currentColumn -= columLeftShift;

            return GetBlockCount() > 0;
        }

        public void UpdateScale(float scale,float[] xPos, float yPos)
        {
            this.scale = scale;

            for (int i = 0; i < BlockCount; ++i)
            {
                Vector2 pos = new Vector2(xPos[i], yPos);
                blocks[i].UpdateScale(scale, pos);
            }
        }
    }


}
