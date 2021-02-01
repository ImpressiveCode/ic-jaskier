namespace Codefusion.Jaskier.Client.VS2015
{
    public static class Hacks
    {        
        public static void Apply()
        {
            EnsureAssembly<System.Windows.Interactivity.Behavior>();
        }

        /// <summary>
        /// Makes sure to copy <see cref="T"/> assembly to the output directory and load in runtime.
        /// </summary>
        private static void EnsureAssembly<T>()
        {            
        }
    }
}
