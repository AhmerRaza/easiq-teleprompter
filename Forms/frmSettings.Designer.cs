namespace Teleprompter {
    partial class frmSettings {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmbBackground = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbCamera = new System.Windows.Forms.ComboBox();
            this.cmbFore = new System.Windows.Forms.ComboBox();
            this.txtPPTIP = new System.Windows.Forms.TextBox();
            this.cmbFont = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default font size:";
            // 
            // cmbSize
            // 
            this.cmbSize.DisplayMember = "127.0.0.1";
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.cmbSize.Location = new System.Drawing.Point(118, 9);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(38, 21);
            this.cmbSize.TabIndex = 59;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 60;
            this.label2.Text = "PPT Server IP:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(250, 190);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 62;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(169, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 63;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(250, 219);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 64;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmbBackground
            // 
            this.cmbBackground.DisplayMember = "127.0.0.1";
            this.cmbBackground.FormattingEnabled = true;
            this.cmbBackground.Items.AddRange(new object[] {
            "Black",
            "White"});
            this.cmbBackground.Location = new System.Drawing.Point(118, 38);
            this.cmbBackground.Name = "cmbBackground";
            this.cmbBackground.Size = new System.Drawing.Size(119, 21);
            this.cmbBackground.TabIndex = 66;
            this.cmbBackground.ValueMember = "Black";
            this.cmbBackground.SelectedIndexChanged += new System.EventHandler(this.cmbBackground_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 65;
            this.label3.Text = "Default back colour";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 67;
            this.label4.Text = "Default fore colour";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 69;
            this.label5.Text = "Default cam colour";
            // 
            // cmbCamera
            // 
            this.cmbCamera.DataBindings.Add(new System.Windows.Forms.Binding("ValueMember", global::Teleprompter.Properties.Settings.Default, "defaultCamera", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cmbCamera.DisplayMember = "127.0.0.1";
            this.cmbCamera.FormattingEnabled = true;
            this.cmbCamera.Items.AddRange(new object[] {
            "Yellow",
            "Navy",
            "LimeGreen",
            "Blue",
            "White"});
            this.cmbCamera.Location = new System.Drawing.Point(118, 96);
            this.cmbCamera.Name = "cmbCamera";
            this.cmbCamera.Size = new System.Drawing.Size(119, 21);
            this.cmbCamera.TabIndex = 70;
            this.cmbCamera.ValueMember = global::Teleprompter.Properties.Settings.Default.defaultCamera;
            // 
            // cmbFore
            // 
            this.cmbFore.DataBindings.Add(new System.Windows.Forms.Binding("ValueMember", global::Teleprompter.Properties.Settings.Default, "defaultFore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cmbFore.DisplayMember = "127.0.0.1";
            this.cmbFore.FormattingEnabled = true;
            this.cmbFore.Items.AddRange(new object[] {
            "White",
            "Black"});
            this.cmbFore.Location = new System.Drawing.Point(118, 67);
            this.cmbFore.Name = "cmbFore";
            this.cmbFore.Size = new System.Drawing.Size(119, 21);
            this.cmbFore.TabIndex = 68;
            this.cmbFore.ValueMember = global::Teleprompter.Properties.Settings.Default.defaultFore;
            this.cmbFore.SelectedIndexChanged += new System.EventHandler(this.cmbFore_SelectedIndexChanged);
            // 
            // txtPPTIP
            // 
            this.txtPPTIP.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Teleprompter.Properties.Settings.Default, "pptServer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtPPTIP.Location = new System.Drawing.Point(118, 190);
            this.txtPPTIP.Name = "txtPPTIP";
            this.txtPPTIP.Size = new System.Drawing.Size(119, 20);
            this.txtPPTIP.TabIndex = 61;
            this.txtPPTIP.Text = global::Teleprompter.Properties.Settings.Default.pptServer;
            // 
            // cmbFont
            // 
            this.cmbFont.DataBindings.Add(new System.Windows.Forms.Binding("ValueMember", global::Teleprompter.Properties.Settings.Default, "defaultCamera", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cmbFont.DisplayMember = "127.0.0.1";
            this.cmbFont.FormattingEnabled = true;
            this.cmbFont.Items.AddRange(new object[] {
            "Yellow",
            "Navy",
            "LimeGreen",
            "Blue",
            "White"});
            this.cmbFont.Location = new System.Drawing.Point(118, 123);
            this.cmbFont.Name = "cmbFont";
            this.cmbFont.Size = new System.Drawing.Size(119, 21);
            this.cmbFont.TabIndex = 72;
            this.cmbFont.ValueMember = global::Teleprompter.Properties.Settings.Default.defaultCamera;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 71;
            this.label6.Text = "Default font";
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 249);
            this.Controls.Add(this.cmbFont);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbCamera);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbFore);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbBackground);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtPPTIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSize);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.frmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPPTIP;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbBackground;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbFore;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbCamera;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbFont;
        private System.Windows.Forms.Label label6;
    }
}