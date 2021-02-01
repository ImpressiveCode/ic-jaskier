namespace Codefusion.Jaskier.Client.VS2015.Services
{
    using Microsoft.VisualStudio.Shell.Interop;

    public class VsFrameController
    {
        private readonly IVsWindowFrame vsWindowFrame;

        public VsFrameController(IVsWindowFrame vsWindowFrame)
        {
            this.vsWindowFrame = vsWindowFrame;
        }

        public void Show()
        {
            this.vsWindowFrame.Show();
        }
    }
}
