using System.ComponentModel;

namespace Ziusudra.Desktop.View
{
    public partial class AccordionItem: UserControl
    {
        public AccordionItem()
        {
            InitializeComponent();
        }

        public override string? Text
        {
            get { return _CategoryCheckBox.Text; }
            set {  _CategoryCheckBox.Text = value ?? string.Empty; }
        }

        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get { return _FilterCategoryBindingSource.DataSource; }
            set { _FilterCategoryBindingSource.DataSource = value; }
        }

    }
}
