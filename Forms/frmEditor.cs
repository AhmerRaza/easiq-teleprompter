using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Teleprompter {

    public partial class frmEditor : Form {
        private ElementHost ctrlHost;
        public Wpf_Controls.Editor wpfViewBox;
        private bool isFlipped = false;
        private bool switchMe = false;
        private Screen myScreen;
        private bool newForm = false;

        public frmEditor(bool display) {
            InitializeComponent();
            if (display) { LoadControls(); }
        }

        private void frmEditor_Load(object sender, EventArgs e) {
            Classes.Controller.PercentageChangedEvent += new EventHandler<EventArgs>(Controller_PercentageChangedEvent);
            newForm = true;
            lblSpeed.BringToFront();
            lblStatus.BringToFront();
        }

        private void LoadControls() {
            myScreen = (Screen.AllScreens.Length > 1 ? Screen.AllScreens[0] : Screen.AllScreens[0]);
            if (myScreen.WorkingArea.Width > 2000) {
                WiderScreen(myScreen);
            } else {
                NormalScreen(myScreen);
            }
            LoadWPF();
            lblSpeed.Text = Classes.Controller.playSpeed.ToString();
            if (Classes.Controller.playSpeed >= 0) {
                picUp.Visible = false;
                picDown.Visible = true;
            } else {
                picUp.Visible = true;
                picDown.Visible = false;
            }
            int eyeline = Classes.Controller.eyeline;
            if (eyeline == 0) {
                picUp.Top = (30 * (panel1.Height / 100));
                picDown.Top = (30 * (panel1.Height / 100));
            } else {
                picUp.Top = (eyeline * (panel1.Height / 100));
                picDown.Top = (eyeline * (panel1.Height / 100));
            }
            if (picUp.Top >= panel1.Height) {
                picUp.Top -= 50;
                picDown.Top -= 50;
            }

            picUp.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);
            //picDown.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipX);
            LoadText();
            HookMouseControl(this.Controls);
            if (Classes.Controller.flipVisible) { Classes.Controller.flipViewer.wpfViewBox.ExtentEvent += new EventHandler<EventArgs>(wpfViewBox_ExtentEvent); }
            lblSpeed.Left = panel1.Right - lblSpeed.Width;
            lblStatus.Left = panel1.Right - lblStatus.Width;
            ShowNotification();
            SpeedReset();
            wpfViewBox_ToggleSwitchEvent(this, new EventArgs());
            ChangeViewerWidth(Classes.Controller.lMargin, Classes.Controller.rMargin, true);
        }

        public void ChangeViewerWidth(double lP, double rP, bool reload) {
            double maxWidth = 974;
            double minWidth = maxWidth / 3;
            double maxMargin = minWidth * 2;

            int left = 0;
            int right = 0;
            try {
                left = (int)lP;
            } catch { }
            try {
                right = (int)rP;
            } catch { }
            double leftMargin = (left > 0 ? maxMargin * left / 100 : 0);
            double rightMargin = (right > 0 ? maxMargin * right / 100 : 0);
            double newWidth = maxWidth - leftMargin - rightMargin;

            panel1.Width = (int)newWidth;

            int margin = (this.ClientSize.Width == 1024 ? 50 : (this.ClientSize.Width - 974) / 2);
            //panel1.Left = margin;

            int myWidth = (myScreen.WorkingArea.Width > 2000 ? myScreen.WorkingArea.Width / 2 : myScreen.WorkingArea.Width);
            panel1.Left = margin + (int)leftMargin;
            lblSpeed.Left = panel1.Right - lblSpeed.Width;
            lblStatus.Left = panel1.Right - lblStatus.Width;

            picUp.Left = panel1.Left - 50;
            picDown.Left = picUp.Left;
            if (reload) {
                LoadWPF();
                LoadText();
            }
        }

        private void Controller_PercentageChangedEvent(object sender, EventArgs e) {
            try {
                wpfViewBox.Perc = Classes.Controller.percentage;
            } catch { }
        }

        private void playlist_ActiveStreamChangedEvent(object sender, Classes.ActiveStreamChangedArgs e) {
            wpfViewBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "editor");
        }

        public int myHeight {
            get { return this.Height; }
        }

        private void Controller_FontSizeChangedEvent(object sender, EventArgs e) {
            wpfViewBox.ChangeDefaultFontSize();
        }

        private void Controller_ViewerStreamChangedEvent(object sender, Classes.ViewerChangedArgs e) {
            wpfViewBox.LoadText(Classes.Controller.playlist.ViewerStream.RTFContent, "editor");
            wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false, true);
        }

        private void frmViewer_Resize(object sender, EventArgs e) {
        }

        private void HookMouseControl(Control.ControlCollection controls) {
            foreach (Control c in controls) {
                //c.MouseDoubleClick += new MouseEventHandler(c_MouseDoubleClick);
                c.MouseClick += new MouseEventHandler(c_MouseClick);
                HookMouseControl(c.Controls);
            }
        }

        private void c_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                Classes.Controller.TogglePlay();
            }
        }

        private void c_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && !donotscroll) {
                Classes.Controller.TogglePlay();
            }
        }

        private void LoadText() {
            wpfViewBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "Editor");
            System.Drawing.Size flipperSize = (Classes.Controller.flipVisible ? Classes.Controller.flipViewer.GetPanelSize() : new Size(974, 768));
            wpfViewBox.SetPageWidth(flipperSize.Width);

            //wpfViewBox.ResizeEditor(flipperSize.Height, flipperSize.Width);
            //wpfViewBox.ResizeEditor(Classes.Controller.flipViewer.wpfViewBox.Height, Classes.Controller.flipViewer.wpfViewBox.Width);
            wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false, true);
            //wpfViewBox.SetPageWidth(974);
            wpfViewBox.SetPercentage(Classes.Controller.editorPerc);
            wpfViewBox.Focus();
        }

        private void NormalScreen(Screen myScreen) {
            this.Left = myScreen.WorkingArea.Left;
            this.Top = myScreen.WorkingArea.Top;
            this.Width = myScreen.WorkingArea.Width;
            this.Height = myScreen.WorkingArea.Height;
            int margin = (this.ClientSize.Width == 1024 ? 50 : (this.ClientSize.Width - panel1.Width) / 2);
            panel1.Left = margin;

            picUp.Left = panel1.Left - 50;
            picDown.Left = panel1.Left - 50;

            picUp.Top = (30 * (panel1.Height / 100));
            picDown.Top = (30 * (panel1.Height / 100));
            this.BringToFront();
        }

        private void WiderScreen(Screen myScreen) {
            this.Left = myScreen.WorkingArea.Left;
            this.Top = myScreen.WorkingArea.Top;
            this.Width = 1024;
            this.Height = myScreen.WorkingArea.Height;
            panel1.Left = 50;
            picUp.Left = panel1.Left - 50;
            picDown.Left = panel1.Left - 50;
            picUp.Top = (30 * (panel1.Height / 100));
            picDown.Top = (30 * (panel1.Height / 100));
        }

        private void LoadWPF() {
            ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(ctrlHost);

            wpfViewBox = new Wpf_Controls.Editor(true);
            wpfViewBox.ScrollChanged += new Wpf_Controls.Editor.ScrollEventHandler(wpfViewBox_ScrollChanged);
            wpfViewBox.TextChangedEvent += new EventHandler<Wpf_Controls.TextArgs>(wpfViewBox_TextChangedEvent);
            wpfViewBox.EscapeEvent += new EventHandler<EventArgs>(wpfViewBox_EscapeEvent);
            wpfViewBox.ToggleSwitchEvent += new EventHandler<EventArgs>(wpfViewBox_ToggleSwitchEvent);
            wpfViewBox.ForceToggleSwitchEvent += new EventHandler<EventArgs>(wpfViewBox_ForceToggleSwitchEvent);
            wpfViewBox.ForceScrollOffEvent += new EventHandler<EventArgs>(wpfViewBox_ForceScrollOffEvent);
            wpfViewBox.MarkerAddedEvent += new EventHandler<Wpf_Controls.MarkerAddArgs>(wpfEditorBox_MarkerAddedEvent);
            wpfViewBox.PrevMarkerEvent += new EventHandler<EventArgs>(wpfEditorBox_PrevMarkerEvent);
            wpfViewBox.NextMarkerEvent += new EventHandler<EventArgs>(wpfEditorBox_NextMarkerEvent);
            wpfViewBox.BigEditUpdateEvent += new EventHandler<EventArgs>(wpfViewBox_BigEditUpdateEvent);
            wpfViewBox.AdjustFontUpEvent += new EventHandler<EventArgs>(wpfViewBox_AdjustFontUpEvent);
            wpfViewBox.AdjustFontDownEvent += new EventHandler<EventArgs>(wpfViewBox_AdjustFontDownEvent);
            wpfViewBox.InternalMarkerEvent += wpfViewBox_InternalMarkerEvent;
            wpfViewBox.InitializeComponent();
            wpfViewBox.changescrollbar();

            wpfViewBox.AddHotkeys();
            ctrlHost.Child = wpfViewBox;
        }

        private void wpfViewBox_InternalMarkerEvent(object sender, Wpf_Controls.IMarkerArgs e) {
            Classes.Controller.mainForm.GotoInternalMarker(e.markerName);
        }

        private void wpfViewBox_BigEditUpdateEvent(object sender, EventArgs e) {
            wpfViewBox.GetContent();
        }

        private bool donotscroll = false;

        private void wpfViewBox_ForceScrollOffEvent(object sender, EventArgs e) {
            donotscroll = true;
            RecolorImage(false);
            this.MouseWheel -= new MouseEventHandler(frmEditor_MouseWheel);
            if (Classes.Controller.isPlaying) { Classes.Controller.TogglePlay(); }
        }

        private void SetLineScroll() {
        }

        private void wpfViewBox_AdjustFontDownEvent(object sender, EventArgs e) {
            Classes.Controller.mainForm.AdjustFontSize(-1);
        }

        private void wpfViewBox_AdjustFontUpEvent(object sender, EventArgs e) {
            Classes.Controller.mainForm.AdjustFontSize(1);
        }

        private void wpfEditorBox_MarkerAddedEvent(object sender, Wpf_Controls.MarkerAddArgs e) {
            Classes.Controller.mainForm.AddMarker(e.mType);
            Classes.Controller.mainForm.SetMarkerNames();
        }

        private void wpfEditorBox_NextMarkerEvent(object sender, EventArgs e) {
            Classes.Controller.mainForm.nextMarkerToolStripMenuItem_Click(sender, e);
        }

        private void wpfEditorBox_PrevMarkerEvent(object sender, EventArgs e) {
            Classes.Controller.mainForm.prevMarkerToolStripMenuItem_Click(sender, e);
        }

        private void wpfViewBox_ToggleSwitchEvent(object sender, EventArgs e) {
            donotscroll = false;
            Classes.Controller.playlist.ActiveStreamChangedEvent -= playlist_ActiveStreamChangedEvent;
            Classes.Controller.playlist.ActiveStream.RTFContent = wpfViewBox.UpdateContent();
            Classes.Controller.playlist.ActiveStreamChangedEvent += playlist_ActiveStreamChangedEvent;
            updateViewer(true);
        }

        private MemoryStream ms;

        private void wpfViewBox_TextChangedEvent(object sender, Wpf_Controls.TextArgs e) {
            ms = e.memorystream;
            Classes.Controller.MatchHandled = false;
        }

        private void wpfViewBox_EscapeEvent(object sender, EventArgs e) {
            if (Classes.Controller.isPlaying) { Classes.Controller.TogglePlay(); }
            Classes.Controller.playlist.ActiveStream.RTFContent = wpfViewBox.UpdateContent();
            Classes.Controller.mainForm.wpfEditorBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "editor");
            Classes.Controller.editorPerc = wpfViewBox.GetPercentage();
            Classes.Controller.mainForm.wpfEditorBox.SetPercentage(Classes.Controller.editorPerc);
            Classes.Controller.ShowBigEditor(false);
        }

        public delegate void LoadTextDelegate(List<String> markerNames, List<String> smarkerNames, double percentage);

        public void LoadText(List<String> markerNames, List<String> smarkerNames, double percentage) {
            if (this.InvokeRequired) {
                this.Invoke(new LoadTextDelegate(LoadText), markerNames, smarkerNames, percentage);
            } else {
                wpfViewBox.LoadText(Classes.Controller.playlist.ViewerStream.RTFContent, "editor");
                wpfViewBox.SetMarkerNames(markerNames, 1);
                wpfViewBox.SetMarkerNames(smarkerNames, 2);
                wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false, true);
                wpfViewBox.Perc = percentage;
            }
        }

        private void wpfViewBox_ScrollChanged(object sender, Wpf_Controls.ScrollArgs args) {
            Classes.Controller.ChangeBigEditorPercentage(args.perc);
            //Classes.Controller.ChangePercentage(args.perc);
        }

        public void LoadText(MemoryStream ms) {
        }

        public void ResizeText(double fSize) {
        }

        public void SetPercentage(int perc) {
            wpfViewBox.Perc = perc;
        }

        public void SetPercentage(double perc) {
            wpfViewBox.Perc = perc;
        }

        public void ScrollFromController(double pos) {
            wpfViewBox.ScrollDirect(pos);
        }

        private void frmEditor_KeyDown(object sender, KeyEventArgs e) {
            bool updatetheviewer = false;
            if (e.KeyCode == Keys.F2) {
                wpfViewBox.GetContent();
                Classes.Controller.playlist.ActiveStream.RTFContent = ms;
                updatetheviewer = true;
            } else if (e.KeyCode == Keys.Escape) {
                if (Classes.Controller.isPlaying) { Classes.Controller.TogglePlay(); }
                Classes.Controller.playlist.ActiveStream.RTFContent = wpfViewBox.UpdateContent();
                Classes.Controller.mainForm.wpfEditorBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "editor");

                Classes.Controller.ShowBigEditor(false);
            }
            if (updatetheviewer) { updateViewer(true); }
        }

        public void updateViewer(bool playMe) {
            DoSomething();
            Classes.Controller.ChangeSpeed(0);
            ChangeImageFlip(Classes.Controller.playSpeed);
            SpeedReset();
            this.MouseWheel += new MouseEventHandler(frmEditor_MouseWheel);
            panel1.Focus();
            RecolorImage(playMe);
            if (playMe) { if (!Classes.Controller.isPlaying) { Classes.Controller.TogglePlay(); } }
        }

        private void RecolorImage(bool playing) {
            Bitmap bmp = (Bitmap)picUp.Image;
            for (int x = 0; x < bmp.Width; x++) {
                for (int y = 0; y < bmp.Height; y++) {
                    Color bmpPx = bmp.GetPixel(x, y);
                    int r, g, b;
                    r = bmpPx.R;
                    g = bmpPx.G;
                    b = bmpPx.B;

                    if (playing && r == 255 && g < 255 && b < 255) {
                        bmp.SetPixel(x, y, Color.White);
                    } else if (!playing && r >= 200 && g >= 200 && b >= 200) {
                        bmp.SetPixel(x, y, Color.Red);
                    }
                }
            }
            picUp.Image = bmp;
            Bitmap bmp1 = (Bitmap)picDown.Image;
            for (int x = 0; x < bmp1.Width; x++) {
                for (int y = 0; y < bmp1.Height; y++) {
                    Color bmpPx = bmp1.GetPixel(x, y);
                    int r, g, b;
                    r = bmpPx.R;
                    g = bmpPx.G;
                    b = bmpPx.B;

                    if (playing && r == 255 && g < 255 && b < 255) {
                        bmp1.SetPixel(x, y, Color.White);
                    } else if (!playing && r >= 200 && g >= 200 && b >= 200) {
                        bmp1.SetPixel(x, y, Color.Red);
                    }
                }
            }
            picDown.Image = bmp1;
        }

        private void DoSomething() {
            if (newForm) {
                Classes.Controller.playlist.ActiveStream.RTFContent = wpfViewBox.UpdateContent();
                double percentage = 0;
                percentage = Classes.Controller.getViewerPercentage();
                Classes.Controller.playlist.ViewerStream = Classes.Controller.playlist.ActiveStream;
                Classes.Controller.mainForm.UpdateMarkers();
                Classes.Controller.SendStreamToViewer(percentage);
                System.Windows.Forms.Application.DoEvents();
                Classes.Controller.SetPercentage(percentage);
            }
        }

        private void wpfViewBox_ExtentEvent(object sender, EventArgs e) {
            if ((Classes.Controller.mainForm.GetSpeed < 0 && Classes.Controller.flipViewer.wpfViewBox.Perc == 0) || (Classes.Controller.mainForm.GetSpeed > 0 && Classes.Controller.flipViewer.wpfViewBox.IsAt100())) {
                Classes.Controller.mainForm.GetSpeed = 0;
                if (Classes.Controller.isPlaying) { Classes.Controller.TogglePlay(); }
            }
        }

        private int repeatCount = 0;

        private void frmEditor_MouseWheel(object sender, MouseEventArgs e) {
            repeatCount++;
            if (repeatCount < 2) {
                Classes.Controller.mainForm.c_MouseWheel(sender, e);
                int speed = Classes.Controller.mainForm.ChangeSpeedFromBigEditor();
                ChangeImageFlip(speed);
                lblSpeed.Text = speed.ToString();
                lblSpeed.Left = panel1.Right - lblSpeed.Width;
                HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
                ee.Handled = true;
            } else {
                repeatCount = 0;
            }
        }

        private void SpeedReset() {
            ChangeImageFlip(0);
            lblSpeed.Text = "0";
            lblSpeed.Left = panel1.Right - lblSpeed.Width;
        }

        public void ChangeImageFlip(int speed) {
            if (speed < 0) {
                picUp.Visible = true;
                picDown.Visible = false;
            } else {
                picUp.Visible = false;
                picDown.Visible = true;
            }
        }

        private void wpfViewBox_ForceToggleSwitchEvent(object sender, EventArgs e) {
            picUp.Visible = false;
            picDown.Visible = false;
            switchMe = false;
            Classes.Controller.mainForm.BigEditorSwitch(switchMe);
        }

        private void blackoutToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.BigEditorShortcuts(1);
            ShowNotification();
        }

        private void flipVToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.BigEditorShortcuts(3);
            ShowNotification();
        }

        private void flipHToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.BigEditorShortcuts(2);
            ShowNotification();
        }

        private void ShowNotification() {
            if (Classes.Controller.showLogo) {
                lblStatus.Text = "Logo";
                lblStatus.Visible = true;
            } else if (Classes.Controller.blackout) {
                lblStatus.Text = "Blackout";
                lblStatus.Visible = true;
            } else if (Classes.Controller.flipB) {
                lblStatus.Text = "Flip H && V";
                lblStatus.Visible = true;
            } else if (Classes.Controller.flipX) {
                lblStatus.Text = "Flip H";
                lblStatus.Visible = true;
            } else if (Classes.Controller.flipY) {
                lblStatus.Text = "Flip V";
                lblStatus.Visible = true;
            } else {
                lblStatus.Text = "";
            }
            Application.DoEvents();
            lblStatus.Left = panel1.Right - lblStatus.Width;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e) {
            DoSomething();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            wpfViewBox.GetContent();
            Classes.Controller.SavePlaylist(false);
        }

        private void logoToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.BigEditorShortcuts(4);
            ShowNotification();
        }

        private void fontUpToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.AdjustFontSize(1);
        }

        private void fontDownToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.AdjustFontSize(-1);
        }

        private void nextMarkerToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.ChangeMarkers(1);
        }

        private void prevMarkerToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.ChangeMarkers(-1);
        }

        private void allCapsToolStripMenuItem_Click(object sender, EventArgs e) {
            wpfViewBox.ChangeCase(true);
            wpfViewBox.GetContent();
        }

        private void sentenceToolStripMenuItem_Click(object sender, EventArgs e) {
            wpfViewBox.ChangeCase(false);
            wpfViewBox.GetContent();
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.UpdateColor(1);
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.UpdateColor(2);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.UpdateColor(3);
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.UpdateColor(4);
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.UpdateColor(5);
        }

        private void nextSlideToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void prevSlideToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void nextStreamToolStripMenuItem_Click(object sender, EventArgs e) {
            SpeedReset();
            Classes.Controller.mainForm.ChangeStream(1);
        }

        private void prevStreamToolStripMenuItem_Click(object sender, EventArgs e) {
            SpeedReset();
            Classes.Controller.mainForm.ChangeStream(-1);
        }

        private void increaseSpeedToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeSpeedFromMenu(1);
        }

        private void decreaseSpeedToolStripMenuItem_Click(object sender, EventArgs e) {
            ChangeSpeedFromMenu(-1);
        }

        public void ChangeSpeedFromController(int speed) {
            ChangeImageFlip(speed);
            lblSpeed.Text = speed.ToString();
            lblSpeed.Left = panel1.Right - lblSpeed.Width;
        }

        public void ChangeSpeedFromMenu(int direction) {
            int speed = Classes.Controller.mainForm.ChangeSpeedFromBigEditor(direction);
            ChangeImageFlip(speed);
            lblSpeed.Text = speed.ToString();
            lblSpeed.Left = panel1.Right - lblSpeed.Width;
        }

        private void imageTestToolStripMenuItem_Click(object sender, EventArgs e) {
            wpfViewBox.SaveAsImage("test.jpg");
        }

        private void picUp_Click(object sender, EventArgs e) {
            FireClick();
        }

        private void picDown_Click(object sender, EventArgs e) {
            FireClick();
        }

        private void FireClick() {
            wpfViewBox.GetContent();
            Classes.Controller.playlist.ActiveStream.RTFContent = ms;
            updateViewer(true);
        }

        private void leftMargIncreaseToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.LeftFromBig(1);
        }

        private void leftMargDecreaseToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.LeftFromBig(-1);
        }

        private void rightMargIncreaseToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.RightFromBig(1);
        }

        private void rightMargDecreaseToolStripMenuItem_Click(object sender, EventArgs e) {
            Classes.Controller.mainForm.RightFromBig(-1);
        }
    }
}