namespace Ziusudra.Desktop.View
{
    partial class AccordionItem
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _CategoryCheckBox = new CheckBox();
            _FilterCategoryBindingSource = new BindingSource(components);
            _FiltersDataGridView = new DataGridView();
            _ValueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _CountDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)_FilterCategoryBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_FiltersDataGridView).BeginInit();
            SuspendLayout();
            // 
            // _CategoryCheckBox
            // 
            _CategoryCheckBox.AutoSize = true;
            _CategoryCheckBox.Dock = DockStyle.Top;
            _CategoryCheckBox.Location = new Point(0, 0);
            _CategoryCheckBox.Name = "_CategoryCheckBox";
            _CategoryCheckBox.Size = new Size(150, 14);
            _CategoryCheckBox.TabIndex = 0;
            _CategoryCheckBox.UseVisualStyleBackColor = true;
            // 
            // _FilterCategoryBindingSource
            // 
            _FilterCategoryBindingSource.AllowNew = false;
            _FilterCategoryBindingSource.DataSource = typeof(ViewModel.FilterCategory);
            // 
            // _FiltersDataGridView
            // 
            _FiltersDataGridView.AllowUserToAddRows = false;
            _FiltersDataGridView.AllowUserToDeleteRows = false;
            _FiltersDataGridView.AllowUserToResizeColumns = false;
            _FiltersDataGridView.AllowUserToResizeRows = false;
            _FiltersDataGridView.AutoGenerateColumns = false;
            _FiltersDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _FiltersDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            _FiltersDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _FiltersDataGridView.ColumnHeadersVisible = false;
            _FiltersDataGridView.Columns.AddRange(new DataGridViewColumn[] { _ValueDataGridViewTextBoxColumn, _CountDataGridViewTextBoxColumn });
            _FiltersDataGridView.DataSource = _FilterCategoryBindingSource;
            _FiltersDataGridView.Dock = DockStyle.Fill;
            _FiltersDataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            _FiltersDataGridView.Location = new Point(0, 14);
            _FiltersDataGridView.MultiSelect = false;
            _FiltersDataGridView.Name = "_FiltersDataGridView";
            _FiltersDataGridView.RowHeadersVisible = false;
            _FiltersDataGridView.ScrollBars = ScrollBars.None;
            _FiltersDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _FiltersDataGridView.Size = new Size(150, 26);
            _FiltersDataGridView.TabIndex = 2;
            // 
            // _ValueDataGridViewTextBoxColumn
            // 
            _ValueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            _ValueDataGridViewTextBoxColumn.HeaderText = "Value";
            _ValueDataGridViewTextBoxColumn.Name = "_ValueDataGridViewTextBoxColumn";
            _ValueDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _CountDataGridViewTextBoxColumn
            // 
            _CountDataGridViewTextBoxColumn.DataPropertyName = "Count";
            _CountDataGridViewTextBoxColumn.FillWeight = 10F;
            _CountDataGridViewTextBoxColumn.HeaderText = "Count";
            _CountDataGridViewTextBoxColumn.Name = "_CountDataGridViewTextBoxColumn";
            _CountDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // AccordionItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_FiltersDataGridView);
            Controls.Add(_CategoryCheckBox);
            MinimumSize = new Size(100, 40);
            Name = "AccordionItem";
            Size = new Size(150, 40);
            ((System.ComponentModel.ISupportInitialize)_FilterCategoryBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)_FiltersDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox _CategoryCheckBox;
        private BindingSource _FilterCategoryBindingSource;
        private DataGridView _FiltersDataGridView;
        private DataGridViewTextBoxColumn _ValueDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _CountDataGridViewTextBoxColumn;
    }
}
