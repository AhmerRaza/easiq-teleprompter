namespace Teleprompter {
    partial class frmExViewer {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblCounter = new System.Windows.Forms.Label();
            this.picUp = new System.Windows.Forms.PictureBox();
            this.picDown = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDown)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(50, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(974, 768);
            this.panel1.TabIndex = 0;
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(0, 0);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(974, 768);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 5;
            this.picLogo.TabStop = false;
            // 
            // lblCounter
            // 
            this.lblCounter.AutoSize = true;
            this.lblCounter.Font = new System.Drawing.Font("Arial", 80F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCounter.ForeColor = System.Drawing.Color.White;
            this.lblCounter.Location = new System.Drawing.Point(0, 0);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(470, 119);
            this.lblCounter.TabIndex = 6;
            this.lblCounter.Text = "00:00:00";
            // 
            // picUp
            // 
            this.picUp.Image = global::Teleprompter.Properties.Resources.marker;
            this.picUp.Location = new System.Drawing.Point(487, 395);
            this.picUp.Name = "picUp";
            this.picUp.Size = new System.Drawing.Size(45, 45);
            this.picUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUp.TabIndex = 8;
            this.picUp.TabStop = false;
            // 
            // picDown
            // 
            this.picDown.Image = global::Teleprompter.Properties.Resources.marker;
            this.picDown.Location = new System.Drawing.Point(487, 323);
            this.picDown.Name = "picDown";
            this.picDown.Size = new System.Drawing.Size(45, 45);
            this.picDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDown.TabIndex = 9;
            this.picDown.TabStop = false;
            // 
            // frmExViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.picUp);
            this.Controls.Add(this.picDown);
            this.Controls.Add(this.lblCounter);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmExViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Extended Viewer";
            this.Load += new System.EventHandler(this.frmViewer_Load);
            this.MouseEnter += new System.EventHandler(this.frmViewer_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.frmViewer_MouseLeave);
            this.Resize += new System.EventHandler(this.frmViewer_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.PictureBox picUp;
        private System.Windows.Forms.PictureBox picDown;
    }
}