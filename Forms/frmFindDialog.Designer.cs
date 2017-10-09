namespace Teleprompter {
    partial class frmFindDialog {
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
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.optDown = new System.Windows.Forms.RadioButton();
            this.optUp = new System.Windows.Forms.RadioButton();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkWholeWord = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtFindWhat = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnReplace = new System.Windows.Forms.Button();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.optDown);
            this.GroupBox1.Controls.Add(this.optUp);
            this.GroupBox1.Location = new System.Drawing.Point(152, 79);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(103, 40);
            this.GroupBox1.TabIndex = 11;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Direction";
            // 
            // optDown
            // 
            this.optDown.AutoSize = true;
            this.optDown.Checked = true;
            this.optDown.Location = new System.Drawing.Point(46, 17);
            this.optDown.Name = "optDown";
            this.optDown.Size = new System.Drawing.Size(53, 17);
            this.optDown.TabIndex = 8;
            this.optDown.TabStop = true;
            this.optDown.Text = "Down";
            this.optDown.UseVisualStyleBackColor = true;
            this.optDown.CheckedChanged += new System.EventHandler(this.optDown_CheckedChanged);
            // 
            // optUp
            // 
            this.optUp.AutoSize = true;
            this.optUp.Location = new System.Drawing.Point(7, 17);
            this.optUp.Name = "optUp";
            this.optUp.Size = new System.Drawing.Size(39, 17);
            this.optUp.TabIndex = 7;
            this.optUp.Text = "Up";
            this.optUp.UseVisualStyleBackColor = true;
            this.optUp.CheckedChanged += new System.EventHandler(this.optUp_CheckedChanged);
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(9, 104);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(82, 17);
            this.chkMatchCase.TabIndex = 6;
            this.chkMatchCase.Text = "Match case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            this.chkMatchCase.CheckedChanged += new System.EventHandler(this.chkMatchCase_CheckedChanged);
            // 
            // chkWholeWord
            // 
            this.chkWholeWord.AutoSize = true;
            this.chkWholeWord.Location = new System.Drawing.Point(9, 81);
            this.chkWholeWord.Name = "chkWholeWord";
            this.chkWholeWord.Size = new System.Drawing.Size(135, 17);
            this.chkWholeWord.TabIndex = 5;
            this.chkWholeWord.Text = "Match whole word only";
            this.chkWholeWord.UseVisualStyleBackColor = true;
            this.chkWholeWord.CheckedChanged += new System.EventHandler(this.chkWholeWord_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(261, 77);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(71, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(261, 12);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(71, 23);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "Find next";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtFindWhat
            // 
            this.txtFindWhat.Location = new System.Drawing.Point(84, 12);
            this.txtFindWhat.Name = "txtFindWhat";
            this.txtFindWhat.Size = new System.Drawing.Size(171, 20);
            this.txtFindWhat.TabIndex = 1;
            this.txtFindWhat.TextChanged += new System.EventHandler(this.txtFindWhat_TextChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(6, 15);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(56, 13);
            this.Label1.TabIndex = 8;
            this.Label1.Text = "Find what:";
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(262, 41);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(71, 23);
            this.btnReplace.TabIndex = 4;
            this.btnReplace.Text = "Replace Next";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(85, 41);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(171, 20);
            this.txtReplace.TabIndex = 3;
            this.txtReplace.TextChanged += new System.EventHandler(this.txtReplace_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Replace With:";
            // 
            // frmFindDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 129);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.chkMatchCase);
            this.Controls.Add(this.chkWholeWord);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.txtFindWhat);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmFindDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find";
            this.Load += new System.EventHandler(this.frmFindDialog_Load);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.RadioButton optDown;
        internal System.Windows.Forms.RadioButton optUp;
        internal System.Windows.Forms.CheckBox chkMatchCase;
        internal System.Windows.Forms.CheckBox chkWholeWord;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnFind;
        internal System.Windows.Forms.TextBox txtFindWhat;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnReplace;
        internal System.Windows.Forms.TextBox txtReplace;
        internal System.Windows.Forms.Label label2;
    }
}