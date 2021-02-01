namespace Codefusion.Jaskier.Client.VS2015
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using Codefusion.Jaskier.Client.VS2015.Services;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F", PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class Vs2015Package : AsyncPackage
    {        
        /// <summary>
        /// Vs2015Package GUID string.
        /// </summary>
        public const string PackageGuidString = "79434214-9642-44da-b46f-4431a5c90205";

        /// <summary>
        /// Initializes a new instance of the <see cref="Vs2015Package"/> class.
        /// </summary>
        public Vs2015Package()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            Hacks.Apply();
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await ServiceLocator.Instance.Resolve<IPluginInitializer>()
                        .Initialize(this.GetService);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Failed to initialize extension {Strings.PluginName}. Details: {exception}");
            }            
        }      
        #endregion
    }
}
