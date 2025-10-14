using System.ComponentModel;
using Ziusudra.Desktop.ViewModel;

namespace Ziusudra.Desktop
{
    public partial class ConnectionManagerForm:
        Form
    {
        public ConnectionManagerForm()
        {
            InitializeComponent();

            _DelugerServersBindingSource.Disposed += _DelugerServersBindingSource_Disposed;
        }

        private async void _AddButton_Click(object sender, EventArgs e)
        {
            using var form = new DelugeServerEditForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                var server = form.DelugeServer;
                await server.InitAsync();
                _DelugerServersBindingSource.Add(server);
            }
        }

        private void _DelugerServersBindingSource_Disposed(object? sender, EventArgs e)
        {
            foreach (DelugeServer ds in _DelugerServersBindingSource)
                if (ds != _DelugerServersBindingSource.Current)
                    ds.Dispose();
        }

        private void _DelugerServersBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    break;
                case ListChangedType.ItemChanged:
                    break;
                case ListChangedType.ItemDeleted:
                    break;
                case ListChangedType.Reset:
                    break;
            }
        }

        public DelugeServer? Current => (DelugeServer?)_DelugerServersBindingSource.Current;
    }
}
