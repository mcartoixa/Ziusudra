using System.ComponentModel;

namespace Ziusudra.Desktop.View
{
    public partial class Accordion:
        UserControl
    {
        public Accordion()
        {
            InitializeComponent();
        }

        private void _FilterCategoriesBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        var category = (ViewModel.FilterCategory?)_FilterCategoriesBindingSource.List[e.NewIndex];
                        if (category is not null)
                        {
                            AccordionItem item = new AccordionItem();
                            item.DataSource = category;
                            item.Dock = category == (ViewModel.FilterCategory)_FilterCategoriesBindingSource.Current ? DockStyle.Fill : DockStyle.Top;
                            Controls.Add(item);
                        }
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    Controls.RemoveAt(e.OldIndex);
                    break;
                case ListChangedType.Reset:
                    {
                        SuspendLayout();
                        Controls.Clear();
                        foreach (ViewModel.FilterCategory category in _FilterCategoriesBindingSource)
                        {
                            AccordionItem item = new AccordionItem();
                            item.Text = category.Category;
                            item.DataSource = category;
                            Controls.Add(item);
                        }
                        if (_FilterCategoriesBindingSource.Current == null)
                            _FilterCategoriesBindingSource.Position = 0;
                        ResumeLayout();
                    }
                    break;
            }
        }

        private void _FilterCategoriesBindingSource_PositionChanged(object sender, EventArgs e)
        {
            SuspendLayout();
            for (int i = 0; i < Controls.Count; i++)
            {
                if (i < _FilterCategoriesBindingSource.Position)
                    Controls[i].Dock = DockStyle.Top;
                else if (i > _FilterCategoriesBindingSource.Position)
                    Controls[i].Dock = DockStyle.Bottom;
                else
                    Controls[i].Dock = DockStyle.Fill;
            }
            ResumeLayout();
        }

        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get { return _FilterCategoriesBindingSource.DataSource; }
            set { _FilterCategoriesBindingSource.DataSource = value; }
        }
    }
}
