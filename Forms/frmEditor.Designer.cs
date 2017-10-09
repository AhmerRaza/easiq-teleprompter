namespace Teleprompter {
    partial class frmEditor {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.picDown = new System.Windows.Forms.PictureBox();
            this.picUp = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.blackoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allCapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sentenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontColoursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yellowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.greenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextSlideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevSlideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.increaseSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftMargIncreaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftMargDecreaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightMargIncreaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightMargDecreaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUp)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(974, 768);
            this.panel1.TabIndex = 0;
            // 
            // picDown
            // 
            this.picDown.Image = global::Teleprompter.Properties.Resources.marker;
            this.picDown.Location = new System.Drawing.Point(487, 395);
            this.picDown.Name = "picDown";
            this.picDown.Size = new System.Drawing.Size(50, 50);
            this.picDown.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDown.TabIndex = 7;
            this.picDown.TabStop = false;
            this.picDown.Click += new System.EventHandler(this.picDown_Click);
            // 
            // picUp
            // 
            this.picUp.Image = global::Teleprompter.Properties.Resources.marker;
            this.picUp.Location = new System.Drawing.Point(487, 324);
            this.picUp.Name = "picUp";
            this.picUp.Size = new System.Drawing.Size(50, 50);
            this.picUp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUp.TabIndex = 6;
            this.picUp.TabStop = false;
            this.picUp.Click += new System.EventHandler(this.picUp_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blackoutToolStripMenuItem,
            this.flipVToolStripMenuItem,
            this.flipHToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.logoToolStripMenuItem,
            this.editorControlsToolStripMenuItem,
            this.imageTestToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // blackoutToolStripMenuItem
            // 
            this.blackoutToolStripMenuItem.Name = "blackoutToolStripMenuItem";
            this.blackoutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
            this.blackoutToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.blackoutToolStripMenuItem.Text = "Blackout";
            this.blackoutToolStripMenuItem.Click += new System.EventHandler(this.blackoutToolStripMenuItem_Click);
            // 
            // flipVToolStripMenuItem
            // 
            this.flipVToolStripMenuItem.Name = "flipVToolStripMenuItem";
            this.flipVToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
            this.flipVToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.flipVToolStripMenuItem.Text = "Flip V";
            this.flipVToolStripMenuItem.Click += new System.EventHandler(this.flipVToolStripMenuItem_Click);
            // 
            // flipHToolStripMenuItem
            // 
            this.flipHToolStripMenuItem.Name = "flipHToolStripMenuItem";
            this.flipHToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
            this.flipHToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.flipHToolStripMenuItem.Text = "Flip H";
            this.flipHToolStripMenuItem.Click += new System.EventHandler(this.flipHToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // logoToolStripMenuItem
            // 
            this.logoToolStripMenuItem.Name = "logoToolStripMenuItem";
            this.logoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.logoToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.logoToolStripMenuItem.Text = "Logo";
            this.logoToolStripMenuItem.Click += new System.EventHandler(this.logoToolStripMenuItem_Click);
            // 
            // editorControlsToolStripMenuItem
            // 
            this.editorControlsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontUpToolStripMenuItem,
            this.fontDownToolStripMenuItem,
            this.nextMarkerToolStripMenuItem,
            this.prevMarkerToolStripMenuItem,
            this.allCapsToolStripMenuItem,
            this.sentenceToolStripMenuItem,
            this.fontColoursToolStripMenuItem,
            this.nextSlideToolStripMenuItem,
            this.prevSlideToolStripMenuItem,
            this.nextStreamToolStripMenuItem,
            this.prevStreamToolStripMenuItem,
            this.increaseSpeedToolStripMenuItem,
            this.decreaseSpeedToolStripMenuItem,
            this.leftMargIncreaseToolStripMenuItem,
            this.leftMargDecreaseToolStripMenuItem,
            this.rightMargIncreaseToolStripMenuItem,
            this.rightMargDecreaseToolStripMenuItem});
            this.editorControlsToolStripMenuItem.Name = "editorControlsToolStripMenuItem";
            this.editorControlsToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.editorControlsToolStripMenuItem.Text = "Editor Controls";
            // 
            // fontUpToolStripMenuItem
            // 
            this.fontUpToolStripMenuItem.Name = "fontUpToolStripMenuItem";
            this.fontUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
            this.fontUpToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.fontUpToolStripMenuItem.Text = "Font Up";
            this.fontUpToolStripMenuItem.Click += new System.EventHandler(this.fontUpToolStripMenuItem_Click);
            // 
            // fontDownToolStripMenuItem
            // 
            this.fontDownToolStripMenuItem.Name = "fontDownToolStripMenuItem";
            this.fontDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
            this.fontDownToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.fontDownToolStripMenuItem.Text = "Font Down";
            this.fontDownToolStripMenuItem.Click += new System.EventHandler(this.fontDownToolStripMenuItem_Click);
            // 
            // nextMarkerToolStripMenuItem
            // 
            this.nextMarkerToolStripMenuItem.Name = "nextMarkerToolStripMenuItem";
            this.nextMarkerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.nextMarkerToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.nextMarkerToolStripMenuItem.Text = "Next Marker";
            this.nextMarkerToolStripMenuItem.Click += new System.EventHandler(this.nextMarkerToolStripMenuItem_Click);
            // 
            // prevMarkerToolStripMenuItem
            // 
            this.prevMarkerToolStripMenuItem.Name = "prevMarkerToolStripMenuItem";
            this.prevMarkerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.prevMarkerToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.prevMarkerToolStripMenuItem.Text = "Prev Marker";
            this.prevMarkerToolStripMenuItem.Click += new System.EventHandler(this.prevMarkerToolStripMenuItem_Click);
            // 
            // allCapsToolStripMenuItem
            // 
            this.allCapsToolStripMenuItem.Name = "allCapsToolStripMenuItem";
            this.allCapsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D6)));
            this.allCapsToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.allCapsToolStripMenuItem.Text = "All Caps";
            this.allCapsToolStripMenuItem.Click += new System.EventHandler(this.allCapsToolStripMenuItem_Click);
            // 
            // sentenceToolStripMenuItem
            // 
            this.sentenceToolStripMenuItem.Name = "sentenceToolStripMenuItem";
            this.sentenceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D7)));
            this.sentenceToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.sentenceToolStripMenuItem.Text = "Sentence";
            this.sentenceToolStripMenuItem.Click += new System.EventHandler(this.sentenceToolStripMenuItem_Click);
            // 
            // fontColoursToolStripMenuItem
            // 
            this.fontColoursToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.whiteToolStripMenuItem,
            this.yellowToolStripMenuItem,
            this.greenToolStripMenuItem,
            this.redToolStripMenuItem,
            this.blueToolStripMenuItem});
            this.fontColoursToolStripMenuItem.Name = "fontColoursToolStripMenuItem";
            this.fontColoursToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.fontColoursToolStripMenuItem.Text = "Font colours";
            // 
            // whiteToolStripMenuItem
            // 
            this.whiteToolStripMenuItem.Name = "whiteToolStripMenuItem";
            this.whiteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.whiteToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.whiteToolStripMenuItem.Text = "White";
            this.whiteToolStripMenuItem.Click += new System.EventHandler(this.whiteToolStripMenuItem_Click);
            // 
            // yellowToolStripMenuItem
            // 
            this.yellowToolStripMenuItem.Name = "yellowToolStripMenuItem";
            this.yellowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.yellowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.yellowToolStripMenuItem.Text = "Yellow";
            this.yellowToolStripMenuItem.Click += new System.EventHandler(this.yellowToolStripMenuItem_Click);
            // 
            // greenToolStripMenuItem
            // 
            this.greenToolStripMenuItem.Name = "greenToolStripMenuItem";
            this.greenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.greenToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.greenToolStripMenuItem.Text = "Green";
            this.greenToolStripMenuItem.Click += new System.EventHandler(this.greenToolStripMenuItem_Click);
            // 
            // redToolStripMenuItem
            // 
            this.redToolStripMenuItem.Name = "redToolStripMenuItem";
            this.redToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.redToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.redToolStripMenuItem.Text = "Red";
            this.redToolStripMenuItem.Click += new System.EventHandler(this.redToolStripMenuItem_Click);
            // 
            // blueToolStripMenuItem
            // 
            this.blueToolStripMenuItem.Name = "blueToolStripMenuItem";
            this.blueToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
            this.blueToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.blueToolStripMenuItem.Text = "Blue";
            this.blueToolStripMenuItem.Click += new System.EventHandler(this.blueToolStripMenuItem_Click);
            // 
            // nextSlideToolStripMenuItem
            // 
            this.nextSlideToolStripMenuItem.Name = "nextSlideToolStripMenuItem";
            this.nextSlideToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Down)));
            this.nextSlideToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.nextSlideToolStripMenuItem.Text = "Next Slide";
            this.nextSlideToolStripMenuItem.Click += new System.EventHandler(this.nextSlideToolStripMenuItem_Click);
            // 
            // prevSlideToolStripMenuItem
            // 
            this.prevSlideToolStripMenuItem.Name = "prevSlideToolStripMenuItem";
            this.prevSlideToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Up)));
            this.prevSlideToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.prevSlideToolStripMenuItem.Text = "Prev Slide";
            this.prevSlideToolStripMenuItem.Click += new System.EventHandler(this.prevSlideToolStripMenuItem_Click);
            // 
            // nextStreamToolStripMenuItem
            // 
            this.nextStreamToolStripMenuItem.Name = "nextStreamToolStripMenuItem";
            this.nextStreamToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Next)));
            this.nextStreamToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.nextStreamToolStripMenuItem.Text = "Next Stream";
            this.nextStreamToolStripMenuItem.Click += new System.EventHandler(this.nextStreamToolStripMenuItem_Click);
            // 
            // prevStreamToolStripMenuItem
            // 
            this.prevStreamToolStripMenuItem.Name = "prevStreamToolStripMenuItem";
            this.prevStreamToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.PageUp)));
            this.prevStreamToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.prevStreamToolStripMenuItem.Text = "Prev Stream";
            this.prevStreamToolStripMenuItem.Click += new System.EventHandler(this.prevStreamToolStripMenuItem_Click);
            // 
            // increaseSpeedToolStripMenuItem
            // 
            this.increaseSpeedToolStripMenuItem.Name = "increaseSpeedToolStripMenuItem";
            this.increaseSpeedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.increaseSpeedToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.increaseSpeedToolStripMenuItem.Text = "Increase Speed";
            this.increaseSpeedToolStripMenuItem.Click += new System.EventHandler(this.increaseSpeedToolStripMenuItem_Click);
            // 
            // decreaseSpeedToolStripMenuItem
            // 
            this.decreaseSpeedToolStripMenuItem.Name = "decreaseSpeedToolStripMenuItem";
            this.decreaseSpeedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.decreaseSpeedToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.decreaseSpeedToolStripMenuItem.Text = "Decrease Speed";
            this.decreaseSpeedToolStripMenuItem.Click += new System.EventHandler(this.decreaseSpeedToolStripMenuItem_Click);
            // 
            // leftMargIncreaseToolStripMenuItem
            // 
            this.leftMargIncreaseToolStripMenuItem.Name = "leftMargIncreaseToolStripMenuItem";
            this.leftMargIncreaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.leftMargIncreaseToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.leftMargIncreaseToolStripMenuItem.Text = "Left Marg Increase";
            this.leftMargIncreaseToolStripMenuItem.Click += new System.EventHandler(this.leftMargIncreaseToolStripMenuItem_Click);
            // 
            // leftMargDecreaseToolStripMenuItem
            // 
            this.leftMargDecreaseToolStripMenuItem.Name = "leftMargDecreaseToolStripMenuItem";
            this.leftMargDecreaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.L)));
            this.leftMargDecreaseToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.leftMargDecreaseToolStripMenuItem.Text = "Left Marg Decrease";
            this.leftMargDecreaseToolStripMenuItem.Click += new System.EventHandler(this.leftMargDecreaseToolStripMenuItem_Click);
            // 
            // rightMargIncreaseToolStripMenuItem
            // 
            this.rightMargIncreaseToolStripMenuItem.Name = "rightMargIncreaseToolStripMenuItem";
            this.rightMargIncreaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
            this.rightMargIncreaseToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.rightMargIncreaseToolStripMenuItem.Text = "Right Marg Increase";
            this.rightMargIncreaseToolStripMenuItem.Click += new System.EventHandler(this.rightMargIncreaseToolStripMenuItem_Click);
            // 
            // rightMargDecreaseToolStripMenuItem
            // 
            this.rightMargDecreaseToolStripMenuItem.Name = "rightMargDecreaseToolStripMenuItem";
            this.rightMargDecreaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.R)));
            this.rightMargDecreaseToolStripMenuItem.Size = new System.Drawing.Size(248, 22);
            this.rightMargDecreaseToolStripMenuItem.Text = "Right Marg Decrease";
            this.rightMargDecreaseToolStripMenuItem.Click += new System.EventHandler(this.rightMargDecreaseToolStripMenuItem_Click);
            // 
            // imageTestToolStripMenuItem
            // 
            this.imageTestToolStripMenuItem.Name = "imageTestToolStripMenuItem";
            this.imageTestToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.T)));
            this.imageTestToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.imageTestToolStripMenuItem.Text = "Image Test";
            this.imageTestToolStripMenuItem.Visible = false;
            this.imageTestToolStripMenuItem.Click += new System.EventHandler(this.imageTestToolStripMenuItem_Click);
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.BackColor = System.Drawing.Color.Transparent;
            this.lblSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpeed.ForeColor = System.Drawing.Color.Red;
            this.lblSpeed.Location = new System.Drawing.Point(950, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(20, 24);
            this.lblSpeed.TabIndex = 10;
            this.lblSpeed.Text = "0";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(950, 25);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 24);
            this.lblStatus.TabIndex = 11;
            // 
            // frmEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.picDown);
            this.Controls.Add(this.picUp);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editor";
            this.Load += new System.EventHandler(this.frmEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmEditor_KeyDown);
            this.Resize += new System.EventHandler(this.frmViewer_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUp)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox picDown;
        private System.Windows.Forms.PictureBox picUp;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem blackoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editorControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prevMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allCapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sentenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontColoursToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yellowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem greenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextSlideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prevSlideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextStreamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prevStreamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem increaseSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decreaseSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageTestToolStripMenuItem;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripMenuItem leftMargIncreaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftMargDecreaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightMargIncreaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightMargDecreaseToolStripMenuItem;
    }
}