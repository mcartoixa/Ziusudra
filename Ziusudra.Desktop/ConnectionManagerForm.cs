using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ziusudra.Desktop
{
    public partial class ConnectionManagerForm:
        Form
    {
        public ConnectionManagerForm()
        {
            InitializeComponent();
        }

        private async void _AddButton_Click(object sender, EventArgs e)
        {
            using var form = new DelugeServerEditForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _DelugerServersBindingSource.Add(form.DelugeServer);
                await form.DelugeServer.InitAsync();
            }
        }
    }
}
