using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ziusudra.Desktop
{
    public partial class DelugeServerEditForm:
        Form
    {

        public DelugeServerEditForm()
        {
            InitializeComponent();

            Data = new ViewModel.DelugeServerEditData();
            _BindingSource.DataSource = Data;
        }

        public DelugeServerEditForm(ViewModel.DelugeServer server):
            this()
        {
            Data = new ViewModel.DelugeServerEditData(server);
            _BindingSource.DataSource = Data;
        }

        private void _SaveButton_Click(object sender, EventArgs e)
        {
            if (Validate())
                Close();
        }

        public ViewModel.DelugeServer DelugeServer => Data.CreateServer();

        public ViewModel.DelugeServerEditData Data { get; init; }
    }
}
