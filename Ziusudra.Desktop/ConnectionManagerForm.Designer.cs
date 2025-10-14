namespace Ziusudra.Desktop
{
    partial class ConnectionManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            GroupBox _ServersGroupBox;
            Panel panel1;
            Panel panel2;
            button5 = new Button();
            _RefreshButton = new Button();
            _RemoveButton = new Button();
            _EditButton = new Button();
            _AddButton = new Button();
            _DataGridView = new DataGridView();
            _HostNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _VersionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            _DelugerServersBindingSource = new BindingSource(components);
            _CancelButton = new Button();
            _ConnectButton = new Button();
            _ServersGroupBox = new GroupBox();
            panel1 = new Panel();
            panel2 = new Panel();
            _ServersGroupBox.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_DataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_DelugerServersBindingSource).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // _ServersGroupBox
            // 
            _ServersGroupBox.Controls.Add(panel1);
            _ServersGroupBox.Controls.Add(_DataGridView);
            _ServersGroupBox.Dock = DockStyle.Top;
            _ServersGroupBox.Location = new Point(0, 0);
            _ServersGroupBox.Name = "_ServersGroupBox";
            _ServersGroupBox.Size = new Size(512, 161);
            _ServersGroupBox.TabIndex = 0;
            _ServersGroupBox.TabStop = false;
            _ServersGroupBox.Text = "Servers";
            // 
            // panel1
            // 
            panel1.Controls.Add(button5);
            panel1.Controls.Add(_RefreshButton);
            panel1.Controls.Add(_RemoveButton);
            panel1.Controls.Add(_EditButton);
            panel1.Controls.Add(_AddButton);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(3, 129);
            panel1.Name = "panel1";
            panel1.Size = new Size(506, 29);
            panel1.TabIndex = 1;
            // 
            // button5
            // 
            button5.Location = new Point(422, 3);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 4;
            button5.Text = "Stop";
            button5.UseVisualStyleBackColor = true;
            // 
            // _RefreshButton
            // 
            _RefreshButton.Location = new Point(252, 3);
            _RefreshButton.Name = "_RefreshButton";
            _RefreshButton.Size = new Size(75, 23);
            _RefreshButton.TabIndex = 3;
            _RefreshButton.Text = "Refresh";
            _RefreshButton.UseVisualStyleBackColor = true;
            // 
            // _RemoveButton
            // 
            _RemoveButton.Location = new Point(171, 3);
            _RemoveButton.Name = "_RemoveButton";
            _RemoveButton.Size = new Size(75, 23);
            _RemoveButton.TabIndex = 2;
            _RemoveButton.Text = "Remove";
            _RemoveButton.UseVisualStyleBackColor = true;
            // 
            // _EditButton
            // 
            _EditButton.Location = new Point(90, 3);
            _EditButton.Name = "_EditButton";
            _EditButton.Size = new Size(75, 23);
            _EditButton.TabIndex = 1;
            _EditButton.Text = "Edit";
            _EditButton.UseVisualStyleBackColor = true;
            // 
            // _AddButton
            // 
            _AddButton.Location = new Point(9, 3);
            _AddButton.Name = "_AddButton";
            _AddButton.Size = new Size(75, 23);
            _AddButton.TabIndex = 0;
            _AddButton.Text = "Add";
            _AddButton.UseVisualStyleBackColor = true;
            _AddButton.Click += _AddButton_Click;
            // 
            // _DataGridView
            // 
            _DataGridView.AllowUserToAddRows = false;
            _DataGridView.AllowUserToDeleteRows = false;
            _DataGridView.AutoGenerateColumns = false;
            _DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridView.Columns.AddRange(new DataGridViewColumn[] { _HostNameDataGridViewTextBoxColumn, _VersionDataGridViewTextBoxColumn });
            _DataGridView.DataSource = _DelugerServersBindingSource;
            _DataGridView.Dock = DockStyle.Fill;
            _DataGridView.Location = new Point(3, 19);
            _DataGridView.Name = "_DataGridView";
            _DataGridView.ReadOnly = true;
            _DataGridView.RowTemplate.Height = 25;
            _DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridView.Size = new Size(506, 139);
            _DataGridView.TabIndex = 0;
            // 
            // _HostNameDataGridViewTextBoxColumn
            // 
            _HostNameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _HostNameDataGridViewTextBoxColumn.DataPropertyName = "Hostname";
            _HostNameDataGridViewTextBoxColumn.HeaderText = "Host";
            _HostNameDataGridViewTextBoxColumn.Name = "_HostNameDataGridViewTextBoxColumn";
            _HostNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _VersionDataGridViewTextBoxColumn
            // 
            _VersionDataGridViewTextBoxColumn.DataPropertyName = "Version";
            _VersionDataGridViewTextBoxColumn.HeaderText = "Version";
            _VersionDataGridViewTextBoxColumn.Name = "_VersionDataGridViewTextBoxColumn";
            _VersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _DelugerServersBindingSource
            // 
            _DelugerServersBindingSource.DataSource = typeof(ViewModel.DelugeServer);
            _DelugerServersBindingSource.ListChanged += _DelugerServersBindingSource_ListChanged;
            // 
            // panel2
            // 
            panel2.Controls.Add(_CancelButton);
            panel2.Controls.Add(_ConnectButton);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 157);
            panel2.Name = "panel2";
            panel2.Size = new Size(512, 30);
            panel2.TabIndex = 3;
            // 
            // _CancelButton
            // 
            _CancelButton.CausesValidation = false;
            _CancelButton.DialogResult = DialogResult.Cancel;
            _CancelButton.Location = new Point(425, 3);
            _CancelButton.Name = "_CancelButton";
            _CancelButton.Size = new Size(75, 23);
            _CancelButton.TabIndex = 2;
            _CancelButton.Text = "Cancel";
            _CancelButton.UseVisualStyleBackColor = true;
            // 
            // _ConnectButton
            // 
            _ConnectButton.DialogResult = DialogResult.OK;
            _ConnectButton.Location = new Point(344, 3);
            _ConnectButton.Name = "_ConnectButton";
            _ConnectButton.Size = new Size(75, 23);
            _ConnectButton.TabIndex = 1;
            _ConnectButton.Text = "Connect";
            _ConnectButton.UseVisualStyleBackColor = true;
            // 
            // ConnectionManagerForm
            // 
            AcceptButton = _ConnectButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _CancelButton;
            ClientSize = new Size(512, 187);
            Controls.Add(panel2);
            Controls.Add(_ServersGroupBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ConnectionManagerForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Connection Manager";
            _ServersGroupBox.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_DataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)_DelugerServersBindingSource).EndInit();
            panel2.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private DataGridView _DataGridView;
        private DataGridViewTextBoxColumn loggerDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn methodsDataGridViewTextBoxColumn;
        private BindingSource _DelugerServersBindingSource;
        private Button button5;
        private Button _RefreshButton;
        private Button _RemoveButton;
        private Button _EditButton;
        private Button _AddButton;
        private Button _ConnectButton;
        private Button _CancelButton;
        private DataGridViewTextBoxColumn _HostNameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn _VersionDataGridViewTextBoxColumn;
    }
}
