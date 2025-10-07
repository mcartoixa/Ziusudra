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
            components = new System.ComponentModel.Container();
            SplitContainer splitContainer1;
            SplitContainer splitContainer2;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            treeView1 = new TreeView();
            _TorrentsView = new DataGridView();
            _QueueDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _NameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _TotalWantedDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _ProgressDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _DownloadPayloadRateDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _UploadPayloadRateDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _ExpectedTimeOfArrivalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            DelugeServerBindingSource = new BindingSource(components);
            _TorrentDetailsPanel = new Ziusudra.Desktop.View.TorrentDetailsPanel();
            _MenuStrip = new MenuStrip();
            _FileToolStripMenuItem = new ToolStripMenuItem();
            _EditToolStripMenuItem = new ToolStripMenuItem();
            _ViewToolStripMenuItem = new ToolStripMenuItem();
            _HelpToolStripMenuItem = new ToolStripMenuItem();
            _ToolStrip = new ToolStrip();
            _ConnectionManagerToolStripButton = new ToolStripButton();
            _StatusStrip = new StatusStrip();
            _ServerVersionToolStripStatusLabel = new Ziusudra.Desktop.Controls.BindableToolStripStatusLabel();
            _RefreshTimer = new System.Windows.Forms.Timer(components);
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_TorrentsView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DelugeServerBindingSource).BeginInit();
            _MenuStrip.SuspendLayout();
            _ToolStrip.SuspendLayout();
            _StatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(_TorrentDetailsPanel);
            splitContainer1.Size = new Size(800, 379);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(treeView1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(_TorrentsView);
            splitContainer2.Size = new Size(800, 266);
            splitContainer2.SplitterDistance = 243;
            splitContainer2.TabIndex = 0;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(243, 266);
            treeView1.TabIndex = 0;
            // 
            // _TorrentsView
            // 
            _TorrentsView.AllowUserToAddRows = false;
            _TorrentsView.AllowUserToOrderColumns = true;
            _TorrentsView.AutoGenerateColumns = false;
            _TorrentsView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _TorrentsView.Columns.AddRange(new DataGridViewColumn[] { _QueueDataGridViewTextBoxColumn, _NameDataGridViewTextBoxColumn, _TotalWantedDataGridViewTextBoxColumn, _ProgressDataGridViewTextBoxColumn, _DownloadPayloadRateDataGridViewTextBoxColumn, _UploadPayloadRateDataGridViewTextBoxColumn, _ExpectedTimeOfArrivalDataGridViewTextBoxColumn });
            _TorrentsView.DataMember = "Torrents";
            _TorrentsView.DataSource = DelugeServerBindingSource;
            _TorrentsView.Dock = DockStyle.Fill;
            _TorrentsView.Location = new Point(0, 0);
            _TorrentsView.Name = "_TorrentsView";
            _TorrentsView.ReadOnly = true;
            _TorrentsView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _TorrentsView.RowTemplate.Height = 25;
            _TorrentsView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _TorrentsView.ShowEditingIcon = false;
            _TorrentsView.Size = new Size(553, 266);
            _TorrentsView.TabIndex = 0;
            // 
            // _QueueDataGridViewTextBoxColumn
            // 
            _QueueDataGridViewTextBoxColumn.DataPropertyName = "Queue";
            _QueueDataGridViewTextBoxColumn.HeaderText = "#";
            _QueueDataGridViewTextBoxColumn.Name = "_QueueDataGridViewTextBoxColumn";
            _QueueDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _NameDataGridViewTextBoxColumn
            // 
            _NameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            _NameDataGridViewTextBoxColumn.HeaderText = "Name";
            _NameDataGridViewTextBoxColumn.Name = "_NameDataGridViewTextBoxColumn";
            _NameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _TotalWantedDataGridViewTextBoxColumn
            // 
            _TotalWantedDataGridViewTextBoxColumn.DataPropertyName = "TotalWanted";
            _TotalWantedDataGridViewTextBoxColumn.HeaderText = "Size";
            _TotalWantedDataGridViewTextBoxColumn.Name = "_TotalWantedDataGridViewTextBoxColumn";
            _TotalWantedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _ProgressDataGridViewTextBoxColumn
            // 
            _ProgressDataGridViewTextBoxColumn.DataPropertyName = "Progress";
            _ProgressDataGridViewTextBoxColumn.HeaderText = "Progress";
            _ProgressDataGridViewTextBoxColumn.Name = "_ProgressDataGridViewTextBoxColumn";
            _ProgressDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _DownloadPayloadRateDataGridViewTextBoxColumn
            // 
            _DownloadPayloadRateDataGridViewTextBoxColumn.DataPropertyName = "DownloadPayloadRate";
            _DownloadPayloadRateDataGridViewTextBoxColumn.HeaderText = "Down Speed";
            _DownloadPayloadRateDataGridViewTextBoxColumn.Name = "_DownloadPayloadRateDataGridViewTextBoxColumn";
            _DownloadPayloadRateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _UploadPayloadRateDataGridViewTextBoxColumn
            // 
            _UploadPayloadRateDataGridViewTextBoxColumn.DataPropertyName = "UploadPayloadRate";
            _UploadPayloadRateDataGridViewTextBoxColumn.HeaderText = "Up Speed";
            _UploadPayloadRateDataGridViewTextBoxColumn.Name = "_UploadPayloadRateDataGridViewTextBoxColumn";
            _UploadPayloadRateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _ExpectedTimeOfArrivalDataGridViewTextBoxColumn
            // 
            _ExpectedTimeOfArrivalDataGridViewTextBoxColumn.DataPropertyName = "ExpectedTimeOfArrival";
            _ExpectedTimeOfArrivalDataGridViewTextBoxColumn.HeaderText = "ETA";
            _ExpectedTimeOfArrivalDataGridViewTextBoxColumn.Name = "_ExpectedTimeOfArrivalDataGridViewTextBoxColumn";
            _ExpectedTimeOfArrivalDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // DelugeServerBindingSource
            // 
            DelugeServerBindingSource.AllowNew = false;
            DelugeServerBindingSource.DataSource = typeof(ViewModel.DelugeServer);
            // 
            // _TorrentDetailsPanel
            // 
            _TorrentDetailsPanel.Dock = DockStyle.Fill;
            _TorrentDetailsPanel.Location = new Point(0, 0);
            _TorrentDetailsPanel.Name = "_TorrentDetailsPanel";
            _TorrentDetailsPanel.Size = new Size(800, 109);
            _TorrentDetailsPanel.TabIndex = 0;
            _TorrentDetailsPanel.Torrent = null;
            // 
            // _MenuStrip
            // 
            _MenuStrip.Items.AddRange(new ToolStripItem[] { _FileToolStripMenuItem, _EditToolStripMenuItem, _ViewToolStripMenuItem, _HelpToolStripMenuItem });
            _MenuStrip.Location = new Point(0, 0);
            _MenuStrip.Name = "_MenuStrip";
            _MenuStrip.Size = new Size(800, 24);
            _MenuStrip.TabIndex = 0;
            _MenuStrip.Text = "menuStrip1";
            // 
            // _FileToolStripMenuItem
            // 
            _FileToolStripMenuItem.Name = "_FileToolStripMenuItem";
            _FileToolStripMenuItem.Size = new Size(37, 20);
            _FileToolStripMenuItem.Text = "&File";
            // 
            // _EditToolStripMenuItem
            // 
            _EditToolStripMenuItem.Name = "_EditToolStripMenuItem";
            _EditToolStripMenuItem.Size = new Size(39, 20);
            _EditToolStripMenuItem.Text = "&Edit";
            // 
            // _ViewToolStripMenuItem
            // 
            _ViewToolStripMenuItem.Name = "_ViewToolStripMenuItem";
            _ViewToolStripMenuItem.Size = new Size(44, 20);
            _ViewToolStripMenuItem.Text = "&View";
            // 
            // _HelpToolStripMenuItem
            // 
            _HelpToolStripMenuItem.Name = "_HelpToolStripMenuItem";
            _HelpToolStripMenuItem.Size = new Size(44, 20);
            _HelpToolStripMenuItem.Text = "&Help";
            // 
            // _ToolStrip
            // 
            _ToolStrip.Items.AddRange(new ToolStripItem[] { _ConnectionManagerToolStripButton });
            _ToolStrip.Location = new Point(0, 24);
            _ToolStrip.Name = "_ToolStrip";
            _ToolStrip.Size = new Size(800, 25);
            _ToolStrip.TabIndex = 1;
            _ToolStrip.Text = "toolStrip1";
            // 
            // _ConnectionManagerToolStripButton
            // 
            _ConnectionManagerToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _ConnectionManagerToolStripButton.Image = (Image)resources.GetObject("_ConnectionManagerToolStripButton.Image");
            _ConnectionManagerToolStripButton.ImageTransparentColor = Color.Magenta;
            _ConnectionManagerToolStripButton.Name = "_ConnectionManagerToolStripButton";
            _ConnectionManagerToolStripButton.Size = new Size(23, 22);
            _ConnectionManagerToolStripButton.Text = "Connection Manager";
            _ConnectionManagerToolStripButton.Click += _ConnectionManagerToolStripButton_Click;
            // 
            // _StatusStrip
            // 
            _StatusStrip.Items.AddRange(new ToolStripItem[] { _ServerVersionToolStripStatusLabel });
            _StatusStrip.Location = new Point(0, 428);
            _StatusStrip.Name = "_StatusStrip";
            _StatusStrip.Size = new Size(800, 22);
            _StatusStrip.TabIndex = 2;
            // 
            // _ServerVersionToolStripStatusLabel
            // 
            _ServerVersionToolStripStatusLabel.Name = "_ServerVersionToolStripStatusLabel";
            _ServerVersionToolStripStatusLabel.Size = new Size(0, 17);
            // 
            // _RefreshTimer
            // 
            _RefreshTimer.Interval = 5000;
            _RefreshTimer.Tick += _RefreshTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(_StatusStrip);
            Controls.Add(_ToolStrip);
            Controls.Add(_MenuStrip);
            MainMenuStrip = _MenuStrip;
            Name = "MainForm";
            Text = "Ziusudra";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_TorrentsView).EndInit();
            ((System.ComponentModel.ISupportInitialize)DelugeServerBindingSource).EndInit();
            _MenuStrip.ResumeLayout(false);
            _MenuStrip.PerformLayout();
            _ToolStrip.ResumeLayout(false);
            _ToolStrip.PerformLayout();
            _StatusStrip.ResumeLayout(false);
            _StatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private MenuStrip _MenuStrip;
        private ToolStripMenuItem _FileToolStripMenuItem;
        private ToolStripMenuItem _EditToolStripMenuItem;
        private ToolStripMenuItem _ViewToolStripMenuItem;
        private ToolStripMenuItem _HelpToolStripMenuItem;
        private ToolStrip _ToolStrip;
        private ToolStripButton _ConnectionManagerToolStripButton;
        private StatusStrip _StatusStrip;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private TreeView treeView1;
        private DataGridView _TorrentsView;
        private View.TorrentDetailsPanel _TorrentDetailsPanel;
        private BindingSource DelugeServerBindingSource;
        private Controls.BindableToolStripStatusLabel _ServerVersionToolStripStatusLabel;
        private System.Windows.Forms.Timer _RefreshTimer;
        private DataGridViewTextBoxColumn _QueueDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _NameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _TotalWantedDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _ProgressDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _DownloadPayloadRateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _UploadPayloadRateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _ExpectedTimeOfArrivalDataGridViewTextBoxColumn;
    }
}
