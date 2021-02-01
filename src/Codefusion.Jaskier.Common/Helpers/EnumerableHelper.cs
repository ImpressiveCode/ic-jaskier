namespace Codefusion.Jaskier.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableHelper
    {
        public static double Median(this IEnumerable<int> source)
        {
            int[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;

            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }

            if (count % 2 == 0)
            {
                int a = temp[(count / 2) - 1];
                int b = temp[count / 2];
                return (a + b) / 2d;
            }

            return temp[count / 2];
        }
    }
}
