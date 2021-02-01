namespace Codefusion.Jaskier.Common.Tests
{
    using System.IO;
    using NUnit.Framework;

    internal static class Resources
    {
        public static string GetPath(string fileName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Resources", fileName);
        }
    }
}
