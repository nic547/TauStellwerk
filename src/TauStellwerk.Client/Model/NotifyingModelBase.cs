using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TauStellwerk.Client.Model
{
    public abstract class NotifyingModelBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public event PropertyChangingEventHandler? PropertyChanging;

        protected void SetAndNotifyIfChanged<T>(ref T field, T value, [CallerMemberName] string caller = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(caller));
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}