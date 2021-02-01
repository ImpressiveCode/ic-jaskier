namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;

    public class DocumentEventArgs : EventArgs
    {
        public DocumentEventArgs(string fullFileName)
        {
            this.FullFileName = fullFileName;
        }

        public string FullFileName { get; }
    }
}
