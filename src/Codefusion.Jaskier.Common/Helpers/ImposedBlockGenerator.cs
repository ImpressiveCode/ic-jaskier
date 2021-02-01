namespace Codefusion.Jaskier.Common.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Returns characters in specified order.
    /// </summary>
    public sealed class ImposedBlockGenerator
    {
        private readonly List<char> block = new List<char>();

        private int position = -1;

        public int Position => this.position;

        public void SetBlock(List<char> newBlock)
        {
            this.block.Clear();

            if (newBlock != null)
            {
                this.block.AddRange(newBlock);
            }            
        }

        public void SetPosition(int newPosition)
        {
            this.position = newPosition;
        }

        public char? Next()
        {
            this.position++;
            return this.GetForIndex(this.position);
        }

        public char? GetForIndex(int index)
        {
            if (this.block.Count == 0)
            {
                return null;
            }

            if (index < 0)
            {
                index += this.block.Count;
            }

            if (index >= this.block.Count)
            {
                index = index % this.block.Count;
            }

            return this.block[index];
        }
    }
}
