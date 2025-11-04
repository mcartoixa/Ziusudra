using System.Windows.Forms.Design;

namespace Ziusudra.Desktop.Controls
{

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.StatusStrip)]
    public partial class BindableToolStripStatusLabel:
        ToolStripStatusLabel,
        IBindableComponent
    {

        public BindableToolStripStatusLabel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
