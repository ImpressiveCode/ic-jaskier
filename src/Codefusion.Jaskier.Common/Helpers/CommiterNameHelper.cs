namespace Codefusion.Jaskier.Common.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    /// <summary>
    /// Responsible for normalizing commiter names.
    /// </summary>
    public static class CommiterNameHelper
    {
        private static readonly Dictionary<char, char> CharReplaceMap = new Dictionary<char, char>
        {
            { 'Ę', 'E' },
            { 'Ó', 'O' },
            { 'Ą', 'A' },
            { 'Ś', 'S' },
            { 'Ł', 'L' },
            { 'Ż', 'Z' },
            { 'Ź', 'Z' },
            { 'Ć', 'C' },
            { 'Ń', 'N' }
        };

        public static string Normalize(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = TrimAndNormalize(name);
            name = RemoveDomain(name);
            name = RemoveDiactricts(name);
            name = SortWords(name);

            return name;
        }

        private static string TrimAndNormalize(string s)
        {
            return s.Trim().ToUpperInvariant();
        }

        private static string RemoveDiactricts(string s)
        {
            s = s.Normalize(NormalizationForm.FormD);

            var builder = new StringBuilder();
            foreach (char c in s)
            {
                if (CharReplaceMap.ContainsKey(c))
                {
                    builder.Append(CharReplaceMap[c]);
                    continue;
                }

                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        private static string SortWords(string s)
        {
            var words = s.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var orderedWords = words.OrderBy(g => g);
            return string.Join(" ", orderedWords);
        }

        private static string RemoveDomain(string s)
        {
            var domainIndex = s.IndexOf('\\');
            if (domainIndex > -1)
            {
                s = s.Substring(domainIndex + 1, s.Length - (domainIndex + 1));
            }

            return s;
        }
    }
}
