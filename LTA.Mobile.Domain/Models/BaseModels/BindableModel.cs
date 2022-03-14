using System.ComponentModel;
using System.Runtime.CompilerServices;
using LTA.Mobile.Domain.Properties;

namespace LTA.Mobile.Domain.Models.BaseModels
{
    public class BindableModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BindableModel()
        {
            PropertyChanged = delegate { };
        }

        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, val))
                return;

            member = val;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void SetPropertyState<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            member = val;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}