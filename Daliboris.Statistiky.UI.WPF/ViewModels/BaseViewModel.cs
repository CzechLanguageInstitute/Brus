using System.ComponentModel;
using System.Runtime.CompilerServices;
using Daliboris.Statistiky.UI.WPF.Annotations;

namespace Daliboris.Statistiky.UI.WPF.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}