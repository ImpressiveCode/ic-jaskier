namespace Codefusion.Jaskier.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class RandomBlockGenerator
    {
        private readonly List<char> block = new List<char>();
        private readonly Func<int, int, int> getNextRandomNumber;
        private readonly List<char> chars;

        private RandomBlockGenerator(IEnumerable<char> chars, Func<int, int, int> getNextRandomNumber)
        {
            this.chars = new List<char>(chars);
            this.getNextRandomNumber = getNextRandomNumber;
        }

        /// <summary>
        /// Creates a new <see cref="RandomBlockGenerator"/>.
        /// </summary>
        /// <param name="chars">Characters to use for generating blocks.</param>
        /// <returns>New instance of <see cref="RandomBlockGenerator"/></returns>
        public static RandomBlockGenerator Create(IEnumerable<char> chars)
        {
            var random = new Random();
            return new RandomBlockGenerator(chars, (a, b) => random.Next(a, b));
        }

        /// <summary>
        /// Creates a new <see cref="RandomBlockGenerator"/> and allows to use custom function for generating random numbers.
        /// </summary>
        /// <param name="chars">Characters to use for generating blocks.</param>
        /// <param name="getNextRandomNumber">Function for getting random numbers.</param>
        /// <returns>New instance of <see cref="RandomBlockGenerator"/></returns>
        public static RandomBlockGenerator Create(IEnumerable<char> chars, Func<int, int, int> getNextRandomNumber)
        {
            return new RandomBlockGenerator(chars, getNextRandomNumber);
        }

        /// <summary>
        /// Sets the current block state.
        /// </summary>
        /// <param name="newBlock">The block characters.</param>
        public void SetBlock(params char[] newBlock)
        {
            this.block.Clear();
            this.block.AddRange(newBlock);
        }

        /// <summary>
        /// Gets a copy of current block.
        /// </summary>
        /// <returns>Current block copy.</returns>
        public List<char> GetBlock()
        {
            return this.block.ToList();
        }

        /// <summary>
        /// Get last character in the current block.
        /// </summary>
        /// <returns>The last character in the current block.</returns>
        public char? GetLast()
        {
            if (this.block.Any())
            {
                return this.block.Last();
            }

            return null;
        }

        /// <summary>
        /// Appends new random character to the block.
        /// </summary>
        /// <param name="max">Maxiumum occurrences of the character.</param>
        /// <returns>Character appended to the block.</returns>
        public char Next(int max = 1)
        {
            if (this.block.Count == 0)
            {
                this.block.Add(this.ChooseRandom());
                return this.block.Last();
            }

            var next = this.ChooseRandom();

            if (TailEquals(this.block, next, max))
            {
                next = this.ChooseAnother(next);
            }

            this.block.Add(next);

            return next;
        }

        private char ChooseRandom()
        {
            var randomNumber = this.getNextRandomNumber(0, 1000);
            var hash = randomNumber % this.chars.Count;
            return this.chars[hash];
        }

        private char ChooseAnother(char current)
        {
            var index = this.chars.IndexOf(current);
            index++;
            if (index > this.chars.Count - 1)
            {
                index = 0;
            }

            return this.chars[index];
        }

        private static bool TailEquals(List<char> list, char c, int count)
        {
            var lastNElements = list.Skip(Math.Max(0, list.Count - count)).ToList();
            if (lastNElements.Count != count)
            {
                return false;
            }

            return lastNElements.All(n => n == c);
        }
    }
}
