namespace Ziusudra.Desktop
{
    partial class DelugeServerEditForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.ErrorProvider _ErrorProvider;
            this._BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._HostnameTextBox = new System.Windows.Forms.TextBox();
            this._UsernameTextBox = new System.Windows.Forms.TextBox();
            this._PasswordTextBox = new System.Windows.Forms.TextBox();
            this._CancelButton = new System.Windows.Forms.Button();
            this._SaveButton = new System.Windows.Forms.Button();
            this._PortUpDown = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            _ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(_ErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._PortUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(65, 15);
            label2.TabIndex = 0;
            label2.Text = "Hostname:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 67);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(63, 15);
            label1.TabIndex = 4;
            label1.Text = "Username:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 96);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(60, 15);
            label3.TabIndex = 6;
            label3.Text = "Password:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 37);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(32, 15);
            label4.TabIndex = 2;
            label4.Text = "Port:";
            // 
            // _ErrorProvider
            // 
            _ErrorProvider.ContainerControl = this;
            _ErrorProvider.DataSource = this._BindingSource;
            // 
            // _BindingSource
            // 
            this._BindingSource.DataSource = typeof(Ziusudra.Desktop.ViewModel.DelugeServerEditData);
            // 
            // _HostnameTextBox
            // 
            this._HostnameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._HostnameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._BindingSource, "HostName", true));
            this._HostnameTextBox.Location = new System.Drawing.Point(83, 6);
            this._HostnameTextBox.Name = "_HostnameTextBox";
            this._HostnameTextBox.PlaceholderText = "127.0.01";
            this._HostnameTextBox.Size = new System.Drawing.Size(189, 23);
            this._HostnameTextBox.TabIndex = 1;
            // 
            // _UsernameTextBox
            // 
            this._UsernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._UsernameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._BindingSource, "Username", true));
            this._UsernameTextBox.Location = new System.Drawing.Point(83, 64);
            this._UsernameTextBox.Name = "_UsernameTextBox";
            this._UsernameTextBox.Size = new System.Drawing.Size(189, 23);
            this._UsernameTextBox.TabIndex = 5;
            // 
            // _PasswordTextBox
            // 
            this._PasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._PasswordTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._BindingSource, "Password", true));
            this._PasswordTextBox.Location = new System.Drawing.Point(83, 93);
            this._PasswordTextBox.Name = "_PasswordTextBox";
            this._PasswordTextBox.Size = new System.Drawing.Size(189, 23);
            this._PasswordTextBox.TabIndex = 7;
            this._PasswordTextBox.UseSystemPasswordChar = true;
            // 
            // _CancelButton
            // 
            this._CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._CancelButton.Location = new System.Drawing.Point(197, 131);
            this._CancelButton.Name = "_CancelButton";
            this._CancelButton.Size = new System.Drawing.Size(75, 23);
            this._CancelButton.TabIndex = 9;
            this._CancelButton.Text = "Cancel";
            this._CancelButton.UseVisualStyleBackColor = true;
            // 
            // _SaveButton
            // 
            this._SaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._SaveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._SaveButton.Location = new System.Drawing.Point(116, 131);
            this._SaveButton.Name = "_SaveButton";
            this._SaveButton.Size = new System.Drawing.Size(75, 23);
            this._SaveButton.TabIndex = 8;
            this._SaveButton.Text = "Save";
            this._SaveButton.UseVisualStyleBackColor = true;
            this._SaveButton.Click += new System.EventHandler(this._SaveButton_Click);
            // 
            // _PortUpDown
            // 
            this._PortUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this._BindingSource, "Port", true));
            this._PortUpDown.Location = new System.Drawing.Point(189, 35);
            this._PortUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._PortUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._PortUpDown.Name = "_PortUpDown";
            this._PortUpDown.Size = new System.Drawing.Size(83, 23);
            this._PortUpDown.TabIndex = 3;
            this._PortUpDown.Value = new decimal(new int[] {
            58846,
            0,
            0,
            0});
            // 
            // DelugeServerEditForm
            // 
            this.AcceptButton = this._SaveButton;
            this.CancelButton = this._CancelButton;
            this.ClientSize = new System.Drawing.Size(284, 166);
            this.Controls.Add(this._PortUpDown);
            this.Controls.Add(label4);
            this.Controls.Add(this._CancelButton);
            this.Controls.Add(this._SaveButton);
            this.Controls.Add(this._PasswordTextBox);
            this.Controls.Add(label3);
            this.Controls.Add(this._UsernameTextBox);
            this.Controls.Add(label1);
            this.Controls.Add(this._HostnameTextBox);
            this.Controls.Add(label2);
            this.Name = "DelugeServerEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Deluge Server";
            ((System.ComponentModel.ISupportInitialize)(_ErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._PortUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label2;
        private TextBox _HostnameTextBox;
        private TextBox _UsernameTextBox;
        private TextBox _PasswordTextBox;
        private Button _CancelButton;
        private Button _SaveButton;
        private NumericUpDown _PortUpDown;
        private BindingSource _BindingSource;
        private ErrorProvider _ErrorProvider;
    }
}
