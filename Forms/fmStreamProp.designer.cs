namespace Teleprompter {
    partial class fmStreamProp {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fmStreamProp));
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdAccept = new System.Windows.Forms.Button();
            this.txtPPT = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkPowerpoint = new System.Windows.Forms.CheckBox();
            this.chkHide = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Stream Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(164, 9);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(222, 21);
            this.txtName.TabIndex = 1;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Image = global::Teleprompter.Properties.Resources.cancel;
            this.cmdCancel.Location = new System.Drawing.Point(314, 115);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdAccept
            // 
            this.cmdAccept.Image = global::Teleprompter.Properties.Resources.accept;
            this.cmdAccept.Location = new System.Drawing.Point(233, 115);
            this.cmdAccept.Name = "cmdAccept";
            this.cmdAccept.Size = new System.Drawing.Size(75, 23);
            this.cmdAccept.TabIndex = 3;
            this.cmdAccept.Text = "Accept";
            this.cmdAccept.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdAccept.UseVisualStyleBackColor = true;
            this.cmdAccept.Click += new System.EventHandler(this.cmdAccept_Click);
            // 
            // txtPPT
            // 
            this.txtPPT.Location = new System.Drawing.Point(164, 36);
            this.txtPPT.Name = "txtPPT";
            this.txtPPT.Size = new System.Drawing.Size(222, 21);
            this.txtPPT.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Presentation Name";
            // 
            // chkPowerpoint
            // 
            this.chkPowerpoint.AutoSize = true;
            this.chkPowerpoint.Location = new System.Drawing.Point(10, 66);
            this.chkPowerpoint.Name = "chkPowerpoint";
            this.chkPowerpoint.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkPowerpoint.Size = new System.Drawing.Size(164, 17);
            this.chkPowerpoint.TabIndex = 6;
            this.chkPowerpoint.Text = "Powerpoint Presentation";
            this.chkPowerpoint.UseVisualStyleBackColor = true;
            // 
            // chkHide
            // 
            this.chkHide.AutoSize = true;
            this.chkHide.Location = new System.Drawing.Point(10, 89);
            this.chkHide.Name = "chkHide";
            this.chkHide.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkHide.Size = new System.Drawing.Size(133, 17);
            this.chkHide.TabIndex = 7;
            this.chkHide.Text = "Hide Slide Markers";
            this.chkHide.UseVisualStyleBackColor = true;
            // 
            // fmStreamProp
            // 
            this.AcceptButton = this.cmdAccept;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(395, 148);
            this.Controls.Add(this.chkHide);
            this.Controls.Add(this.chkPowerpoint);
            this.Controls.Add(this.txtPPT);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdAccept);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "fmStreamProp";
            this.Text = "Stream Properties";
            this.Load += new System.EventHandler(this.fmStreamProp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdAccept;
        private System.Windows.Forms.TextBox txtPPT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkPowerpoint;
        private System.Windows.Forms.CheckBox chkHide;
    }
}