namespace Codefusion.Jaskier.Common.Helpers
{
    using System;

    public static class PathHelper
    {
        /// <summary>
        /// "C:\Dev\Project\A\B\C.txt", "C:\Dev\Project" => "A\B\C.txt" 
        /// </summary>
        public static string ReplaceStart(string path, string valueToReplace)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(valueToReplace))
                return path;                

            var index = path.IndexOf(valueToReplace, 0, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return path;

            path = path.Substring(valueToReplace.Length, path.Length - valueToReplace.Length);

            if (path.StartsWith("\\"))
            {
                path = path.Substring(1, path.Length - 1);
            }

            return path;
        }
    }
}
