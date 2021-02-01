namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using System;
    using System.IO;
    using Codefusion.Jaskier.Common.Services;
    using EnvDTE;

    using Microsoft.VisualStudio.Shell.Interop;

    public interface IVsBridge
    {
        event EventHandler SolutionOpened;

        event EventHandler SolutionClosed;

        event EventHandler<DocumentEventArgs> DocumentSaved;        

        string SolutionFileName { get; }

        void OpenSolutionDocument(string fileRelativePath);

        VsFrameController CreateSingleInstanceToolWindow(string caption, string guid, object content);
    }

    public class VsBridge : IVsBridge
    {        
        private static Func<Type, object> provider;        

        private static DTE dte;
        private static SolutionEvents solutionEvents;    
        private static DocumentEvents documentEvents;

        private readonly IErrorHandler errorHandler;

        public VsBridge(IErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        private static event EventHandler MySolutionOpened;

        private static event EventHandler MySolutionClosed;

        private static event EventHandler<DocumentEventArgs> MyDocumentSaved;

        public string SolutionFileName => dte.Solution?.FullName;        

        public static void Initialize(Func<Type, object> serviceProvider)
        {
            provider = serviceProvider;

            dte = GetService<DTE>();

            // Store references because event objects may be GC'ed.
            solutionEvents = dte.Events.SolutionEvents;
            documentEvents = dte.Events.DocumentEvents;

            solutionEvents.Opened += OnSolutionOpened;
            solutionEvents.AfterClosing += OnSolutionClosed;

            documentEvents.DocumentSaved += OnDocumentSaved;
        }     

        public event EventHandler SolutionOpened
        {
            add
            {
                MySolutionOpened += value;
            }

            remove
            {
                MySolutionOpened -= value;
            }
        }

        public event EventHandler<DocumentEventArgs> DocumentSaved
        {
            add
            {
                MyDocumentSaved += value;
            }

            remove
            {
                MyDocumentSaved -= value;
            }
        }

        public event EventHandler SolutionClosed
        {
            add
            {
                MySolutionClosed += value;
            }

            remove
            {
                MySolutionClosed -= value;
            }
        }

        public void OpenSolutionDocument(string fileRelativePath)
        {
            var solutionPath = Path.GetDirectoryName(this.SolutionFileName);
            if (string.IsNullOrEmpty(solutionPath))
            {
                return;
            }

            var fullPath = Path.Combine(solutionPath, fileRelativePath);
            if (!File.Exists(fullPath))
            {
                return;
            }

            try
            {
                dte.Documents.Open(fullPath);
            }
            catch (ArgumentException argumentException)
            {
                this.errorHandler.Handle(argumentException.Message);
            }
        }

        public VsFrameController CreateSingleInstanceToolWindow(string caption, string guid, object content)
        {
            const int ToolWindowInstanceId = 0; // Single-instance toolwindow

            Guid guidNull = Guid.Empty;
            int[] position = new int[1];
            IVsWindowFrame windowFrame;

            var uiShell = (IVsUIShell)GetService<SVsUIShell>();

            var toolWindowPersistenceGuid = new Guid(guid);

            var result = uiShell.CreateToolWindow(
                (uint)__VSCREATETOOLWIN.CTW_fInitNew,
                ToolWindowInstanceId,
                content, 
                ref guidNull, 
                ref toolWindowPersistenceGuid,
                ref guidNull, 
                null, 
                caption, 
                position, 
                out windowFrame);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(result);

            return new VsFrameController(windowFrame);
        }

        private static void OnSolutionOpened()
        {
            MySolutionOpened?.Invoke(null, EventArgs.Empty);
        }

        private static void OnSolutionClosed()
        {
            MySolutionClosed?.Invoke(null, EventArgs.Empty);
        }

        private static void OnDocumentSaved(Document document)
        {
            var args = new DocumentEventArgs(document?.FullName ?? String.Empty);

            MyDocumentSaved?.Invoke(dte, args);
        }

        private static T GetService<T>()
        {
            return (T)provider(typeof(T));
        }
    }
}
