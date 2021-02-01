namespace Codefusion.Jaskier.Common.Services
{
    using System;
    using System.Linq;

    public interface ICommandLineParametersParser
    {
        string ParseParameter(string name);
    }

    public class CommandLineParametersParser : ICommandLineParametersParser
    {
        private readonly string[] args;        

        public CommandLineParametersParser()
            : this(Environment.GetCommandLineArgs())
        {            
        }

        private CommandLineParametersParser(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            this.args = args;
        }

        /// <summary>
        /// Parses paremeter with specified name.
        /// <para>Returns null if parameter was not found.</para>
        /// </summary>
        public string ParseParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            
            var result = this.args.FirstOrDefault(p => p.IndexOf(name, StringComparison.OrdinalIgnoreCase) > -1);
            if (result == null)
            {
                return null;
            }

            int length = name.Length;
            if (result.Length >= length)
            {
                result = result.Substring(length, result.Length - length);

                if (result.StartsWith("=", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Remove(0, 1);
                }
            }

            return result;
        }
    }
}
