namespace Ziusudra.Desktop.View
{
    partial class Accordion
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
            _FilterCategoriesBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)_FilterCategoriesBindingSource).BeginInit();
            SuspendLayout();
            // 
            // _FilterCategoriesBindingSource
            // 
            _FilterCategoriesBindingSource.AllowNew = false;
            _FilterCategoriesBindingSource.DataMember = "FilterCategories";
            _FilterCategoriesBindingSource.DataSource = typeof(ViewModel.DelugeServer);
            _FilterCategoriesBindingSource.ListChanged += _FilterCategoriesBindingSource_ListChanged;
            _FilterCategoriesBindingSource.PositionChanged += _FilterCategoriesBindingSource_PositionChanged;
            // 
            // Accordion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            Name = "Accordion";
            Size = new Size(260, 436);
            ((System.ComponentModel.ISupportInitialize)_FilterCategoriesBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private BindingSource _FilterCategoriesBindingSource;
    }
}
