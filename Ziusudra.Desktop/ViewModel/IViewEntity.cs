using System.ComponentModel;

namespace Ziusudra.Desktop.ViewModel
{

    public interface IViewEntity:
        IDataErrorInfo,
        INotifyPropertyChanged,
        INotifyPropertyChanging
    { }
}
