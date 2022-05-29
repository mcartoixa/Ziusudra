namespace Ziusudra.Desktop
{
    partial class MainForm
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.SplitContainer splitContainer1;
            System.Windows.Forms.SplitContainer splitContainer2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this._TorrentsView = new System.Windows.Forms.DataGridView();
            this.activeTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hashDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.progressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seedingTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DelugeServerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._TorrentDetailsPanel = new Ziusudra.Desktop.View.TorrentDetailsPanel();
            this._MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this._StatusStrip = new System.Windows.Forms.StatusStrip();
            this._ServerVersionToolStripStatusLabel = new Ziusudra.Desktop.Controls.BindableToolStripStatusLabel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._TorrentsView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DelugeServerBindingSource)).BeginInit();
            this._MenuStrip.SuspendLayout();
            this._ToolStrip.SuspendLayout();
            this._StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this._TorrentDetailsPanel);
            splitContainer1.Size = new System.Drawing.Size(800, 379);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(this._TorrentsView);
            splitContainer2.Size = new System.Drawing.Size(800, 266);
            splitContainer2.SplitterDistance = 243;
            splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(243, 266);
            this.treeView1.TabIndex = 0;
            // 
            // _TorrentsView
            // 
            this._TorrentsView.AllowUserToAddRows = false;
            this._TorrentsView.AllowUserToOrderColumns = true;
            this._TorrentsView.AutoGenerateColumns = false;
            this._TorrentsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._TorrentsView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.activeTimeDataGridViewTextBoxColumn,
            this.hashDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.progressDataGridViewTextBoxColumn,
            this.seedingTimeDataGridViewTextBoxColumn});
            this._TorrentsView.DataMember = "Torrents";
            this._TorrentsView.DataSource = this.DelugeServerBindingSource;
            this._TorrentsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TorrentsView.Location = new System.Drawing.Point(0, 0);
            this._TorrentsView.Name = "_TorrentsView";
            this._TorrentsView.ReadOnly = true;
            this._TorrentsView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._TorrentsView.RowTemplate.Height = 25;
            this._TorrentsView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._TorrentsView.ShowEditingIcon = false;
            this._TorrentsView.Size = new System.Drawing.Size(553, 266);
            this._TorrentsView.TabIndex = 0;
            // 
            // activeTimeDataGridViewTextBoxColumn
            // 
            this.activeTimeDataGridViewTextBoxColumn.DataPropertyName = "ActiveTime";
            this.activeTimeDataGridViewTextBoxColumn.HeaderText = "ActiveTime";
            this.activeTimeDataGridViewTextBoxColumn.Name = "activeTimeDataGridViewTextBoxColumn";
            this.activeTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // hashDataGridViewTextBoxColumn
            // 
            this.hashDataGridViewTextBoxColumn.DataPropertyName = "Hash";
            this.hashDataGridViewTextBoxColumn.HeaderText = "Hash";
            this.hashDataGridViewTextBoxColumn.Name = "hashDataGridViewTextBoxColumn";
            this.hashDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // progressDataGridViewTextBoxColumn
            // 
            this.progressDataGridViewTextBoxColumn.DataPropertyName = "Progress";
            this.progressDataGridViewTextBoxColumn.HeaderText = "Progress";
            this.progressDataGridViewTextBoxColumn.Name = "progressDataGridViewTextBoxColumn";
            this.progressDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // seedingTimeDataGridViewTextBoxColumn
            // 
            this.seedingTimeDataGridViewTextBoxColumn.DataPropertyName = "SeedingTime";
            this.seedingTimeDataGridViewTextBoxColumn.HeaderText = "SeedingTime";
            this.seedingTimeDataGridViewTextBoxColumn.Name = "seedingTimeDataGridViewTextBoxColumn";
            this.seedingTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // DelugeServerBindingSource
            // 
            this.DelugeServerBindingSource.AllowNew = false;
            this.DelugeServerBindingSource.DataSource = typeof(Ziusudra.Desktop.ViewModel.DelugeServer);
            // 
            // _TorrentDetailsPanel
            // 
            this._TorrentDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TorrentDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this._TorrentDetailsPanel.Name = "_TorrentDetailsPanel";
            this._TorrentDetailsPanel.Size = new System.Drawing.Size(800, 109);
            this._TorrentDetailsPanel.TabIndex = 0;
            this._TorrentDetailsPanel.Torrent = null;
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(800, 24);
            this._MenuStrip.TabIndex = 0;
            this._MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // _ToolStrip
            // 
            this._ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this._ToolStrip.Location = new System.Drawing.Point(0, 24);
            this._ToolStrip.Name = "_ToolStrip";
            this._ToolStrip.Size = new System.Drawing.Size(800, 25);
            this._ToolStrip.TabIndex = 1;
            this._ToolStrip.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // _StatusStrip
            // 
            this._StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ServerVersionToolStripStatusLabel});
            this._StatusStrip.Location = new System.Drawing.Point(0, 428);
            this._StatusStrip.Name = "_StatusStrip";
            this._StatusStrip.Size = new System.Drawing.Size(800, 22);
            this._StatusStrip.TabIndex = 2;
            // 
            // _ServerVersionToolStripStatusLabel
            // 
            this._ServerVersionToolStripStatusLabel.Name = "_ServerVersionToolStripStatusLabel";
            this._ServerVersionToolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(splitContainer1);
            this.Controls.Add(this._StatusStrip);
            this.Controls.Add(this._ToolStrip);
            this.Controls.Add(this._MenuStrip);
            this.MainMenuStrip = this._MenuStrip;
            this.Name = "MainForm";
            this.Text = "Ziusudra";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._TorrentsView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DelugeServerBindingSource)).EndInit();
            this._MenuStrip.ResumeLayout(false);
            this._MenuStrip.PerformLayout();
            this._ToolStrip.ResumeLayout(false);
            this._ToolStrip.PerformLayout();
            this._StatusStrip.ResumeLayout(false);
            this._StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip _MenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStrip _ToolStrip;
        private ToolStripButton toolStripButton1;
        private StatusStrip _StatusStrip;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private TreeView treeView1;
        private DataGridView _TorrentsView;
        private View.TorrentDetailsPanel _TorrentDetailsPanel;
        private BindingSource DelugeServerBindingSource;
        private Controls.BindableToolStripStatusLabel _ServerVersionToolStripStatusLabel;
        private DataGridViewTextBoxColumn activeTimeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn hashDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn progressDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn seedingTimeDataGridViewTextBoxColumn;
    }
}
