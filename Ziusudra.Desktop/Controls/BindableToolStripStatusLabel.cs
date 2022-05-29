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

            BindingContext = new BindingContext();
            DataBindings = new ControlBindingsCollection(this);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        public BindingContext BindingContext { get; set; }

        public ControlBindingsCollection DataBindings { get; private set; }
    }
}
