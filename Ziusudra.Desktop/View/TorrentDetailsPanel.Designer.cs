namespace Ziusudra.Desktop.View
{
    partial class TorrentDetailsPanel
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
            this._TrackerControl = new System.Windows.Forms.TabControl();
            this._StatusTabPage = new System.Windows.Forms.TabPage();
            this._DetailsTabPage = new System.Windows.Forms.TabPage();
            this._OptionsTabPage = new System.Windows.Forms.TabPage();
            this._FilesTabPage = new System.Windows.Forms.TabPage();
            this._PeersTabPage = new System.Windows.Forms.TabPage();
            this._TrackersTabPage = new System.Windows.Forms.TabPage();
            this._TrackerControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // _TrackerControl
            // 
            this._TrackerControl.Controls.Add(this._StatusTabPage);
            this._TrackerControl.Controls.Add(this._DetailsTabPage);
            this._TrackerControl.Controls.Add(this._OptionsTabPage);
            this._TrackerControl.Controls.Add(this._FilesTabPage);
            this._TrackerControl.Controls.Add(this._PeersTabPage);
            this._TrackerControl.Controls.Add(this._TrackersTabPage);
            this._TrackerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TrackerControl.Location = new System.Drawing.Point(0, 0);
            this._TrackerControl.Name = "_TrackerControl";
            this._TrackerControl.SelectedIndex = 0;
            this._TrackerControl.Size = new System.Drawing.Size(716, 150);
            this._TrackerControl.TabIndex = 1;
            // 
            // _StatusTabPage
            // 
            this._StatusTabPage.Location = new System.Drawing.Point(4, 24);
            this._StatusTabPage.Name = "_StatusTabPage";
            this._StatusTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._StatusTabPage.Size = new System.Drawing.Size(708, 122);
            this._StatusTabPage.TabIndex = 0;
            this._StatusTabPage.Text = "Status";
            this._StatusTabPage.UseVisualStyleBackColor = true;
            // 
            // _DetailsTabPage
            // 
            this._DetailsTabPage.Location = new System.Drawing.Point(4, 24);
            this._DetailsTabPage.Name = "_DetailsTabPage";
            this._DetailsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._DetailsTabPage.Size = new System.Drawing.Size(708, 122);
            this._DetailsTabPage.TabIndex = 1;
            this._DetailsTabPage.Text = "Details";
            this._DetailsTabPage.UseVisualStyleBackColor = true;
            // 
            // _OptionsTabPage
            // 
            this._OptionsTabPage.Location = new System.Drawing.Point(4, 24);
            this._OptionsTabPage.Name = "_OptionsTabPage";
            this._OptionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._OptionsTabPage.Size = new System.Drawing.Size(708, 122);
            this._OptionsTabPage.TabIndex = 2;
            this._OptionsTabPage.Text = "Options";
            this._OptionsTabPage.UseVisualStyleBackColor = true;
            // 
            // _FilesTabPage
            // 
            this._FilesTabPage.Location = new System.Drawing.Point(4, 24);
            this._FilesTabPage.Name = "_FilesTabPage";
            this._FilesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._FilesTabPage.Size = new System.Drawing.Size(708, 122);
            this._FilesTabPage.TabIndex = 3;
            this._FilesTabPage.Text = "Files";
            this._FilesTabPage.UseVisualStyleBackColor = true;
            // 
            // _PeersTabPage
            // 
            this._PeersTabPage.Location = new System.Drawing.Point(4, 24);
            this._PeersTabPage.Name = "_PeersTabPage";
            this._PeersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._PeersTabPage.Size = new System.Drawing.Size(708, 122);
            this._PeersTabPage.TabIndex = 4;
            this._PeersTabPage.Text = "Peers";
            this._PeersTabPage.UseVisualStyleBackColor = true;
            // 
            // _TrackersTabPage
            // 
            this._TrackersTabPage.Location = new System.Drawing.Point(4, 24);
            this._TrackersTabPage.Name = "_TrackersTabPage";
            this._TrackersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._TrackersTabPage.Size = new System.Drawing.Size(708, 122);
            this._TrackersTabPage.TabIndex = 5;
            this._TrackersTabPage.Text = "Trackers";
            this._TrackersTabPage.UseVisualStyleBackColor = true;
            // 
            // TorrentDetailsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._TrackerControl);
            this.Name = "TorrentDetailsPanel";
            this.Size = new System.Drawing.Size(716, 150);
            this._TrackerControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl _TrackerControl;
        private TabPage _StatusTabPage;
        private TabPage _DetailsTabPage;
        private TabPage _OptionsTabPage;
        private TabPage _FilesTabPage;
        private TabPage _PeersTabPage;
        private TabPage _TrackersTabPage;
    }
}
