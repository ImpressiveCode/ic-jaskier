namespace Codefusion.Jaskier.Client.VS2015.UserInterface
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Codefusion.Jaskier.Client.VS2015.Properties;

    public class NotifyPropertyChangedObject : IViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
