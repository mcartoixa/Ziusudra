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

        protected virtual string Error => string.Empty;

        protected virtual string this[string columnName] => string.Empty;

        string IDataErrorInfo.Error => Error;

        string IDataErrorInfo.this[string columnName] => this[columnName];

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;
    }
}
