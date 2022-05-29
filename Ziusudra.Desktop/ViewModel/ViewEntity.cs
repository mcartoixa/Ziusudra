using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ziusudra.Desktop.ViewModel
{

    public abstract class ViewEntity:
        IViewEntity
    {

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanging([CallerMemberName] string propertyName = "")
        {
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;
    }
}
