using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Teleprompter {

    public partial class frmExViewer : Form {
        private ElementHost ctrlHost;
        public Wpf_Controls.FlipViewer wpfViewBox;
        private bool isFlipped = false;
        private Screen myScreen;

        public frmExViewer() {
            InitializeComponent();
            //Classes.Controller.ViewerStreamChangedEvent += new EventHandler<Classes.ViewerChangedArgs>(Controller_ViewerStreamChangedEvent);
            Classes.Controller.FontSizeChangedEvent += new EventHandler<EventArgs>(Controller_FontSizeChangedEvent);
            Classes.Controller.SpeedChangedEvent += new EventHandler<EventArgs>(Controller_SpeedChangedEvent);
            Classes.Controller.ChangeEyelineEvent += new EventHandler<EventArgs>(Controller_ChangeEyelineEvent);
            Classes.Controller.LogoChangedEvent += new EventHandler<EventArgs>(Controller_LogoChangedEvent);
            Classes.Controller.ShowCountEvent += new EventHandler<EventArgs>(Controller_ShowCountEvent);
            Classes.Controller.CountChangedEvent += new EventHandler<EventArgs>(Controller_CountChangedEvent);
            Classes.Controller.PercentageChangedEvent += new EventHandler<EventArgs>(Controller_PercentageChangedEvent);
        }

        private void Controller_PercentageChangedEvent(object sender, EventArgs e) {
            try {
                wpfViewBox.Perc = Classes.Controller.percentage;
            } catch { }
        }

        private void Controller_CountChangedEvent(object sender, EventArgs e) {
            lblCounter.Text = Classes.Controller.CountText;
        }

        private void Controller_ShowCountEvent(object sender, EventArgs e) {
            lblCounter.Visible = Classes.Controller.ShowCounter;
        }

        private void Controller_LogoChangedEvent(object sender, EventArgs e) {
            if (Classes.Controller.showLogo) {
                picLogo.Visible = true;
                panel1.Visible = false;
                picUp.Visible = false;
                picDown.Visible = false;
                picLogo.Image = Classes.Controller.imgViewer;
            } else {
                picLogo.Visible = false;
                panel1.Visible = true;
                SetEyeline();
            }
        }

        public void ToggleLayout(bool show) {
            if (show) {
                panel1.ResumeLayout();
            } else {
                panel1.SuspendLayout();
            }
        }

        private void Controller_ChangeEyelineEvent(object sender, EventArgs e) {
            int eyeline = Classes.Controller.eyeline;
            picUp.Top = (eyeline * (panel1.Height / 100));
            picDown.Top = (eyeline * (panel1.Height / 100));
            if (picUp.Top >= panel1.Height) {
                picUp.Top -= 50;
                picDown.Top -= 50;
            }
        }

        public void WhichImage(bool flipped) {
            if (Classes.Controller.playSpeed >= 0) {
                picUp.Visible = false;
                picDown.Visible = true;
            } else {
                picUp.Visible = true;
                picDown.Visible = false;
            }
            if (flipped) {
            } else {
            }
        }

        public void SetEyeline() {
            if (Classes.Controller.playSpeed >= 0) {
                picUp.Visible = false;
                picDown.Visible = true;
            } else {
                picUp.Visible = true;
                picDown.Visible = false;
            }

            picUp.Left = Classes.Controller.flipViewer.leftImage;
            picDown.Left = Classes.Controller.flipViewer.leftImage;

            FlipEyeline();
        }

        private void FlipEyeline() {
            int eyeline = Classes.Controller.eyeline;
            picUp.Top = (eyeline * (panel1.Height / 100));
            picDown.Top = (eyeline * (panel1.Height / 100));
            if (picUp.Top >= panel1.Height) {
                picUp.Top -= 50;
                picDown.Top -= 50;
            }
        }

        public int myHeight {
            get { return this.Height; }
        }

        private void Controller_SpeedChangedEvent(object sender, EventArgs e) {
            if (Classes.Controller.playSpeed >= 0) {
                picUp.Visible = false;
                picDown.Visible = true;
            } else {
                picUp.Visible = true;
                picDown.Visible = false;
            }
            wpfViewBox.offset = Classes.Controller.playSpeed;
        }

        private void Controller_FontSizeChangedEvent(object sender, EventArgs e) {
            wpfViewBox.ChangeDefaultFontSize();
        }

        private void Controller_ViewerStreamChangedEvent(object sender, Classes.ViewerChangedArgs e) {
            wpfViewBox.LoadText(Classes.Controller.playlist.ViewerStream.RTFContent, "editor", false);
            wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false);
        }

        private void frmViewer_Resize(object sender, EventArgs e) {
        }

        private void frmViewer_Load(object sender, EventArgs e) {
            myScreen = (Screen.AllScreens.Length > 1 ? Screen.AllScreens[1] : Screen.AllScreens[0]);
            if (myScreen.WorkingArea.Width > 2000) {
                WiderScreen(myScreen);
            } else {
                NormalScreen(myScreen);
            }
            LoadWPF();
            if (Classes.Controller.playSpeed >= 0) {
                picUp.Visible = false;
                picDown.Visible = true;
            } else {
                picUp.Visible = true;
                picDown.Visible = false;
            }
            picUp.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);
            picLogo.Visible = false;
            lblCounter.Visible = false;
            ChangeViewerWidth(0, 0);
            wpfViewBox.ChangePageWidth(panel1.Width);
        }

        private int leftImage;
        private int rightImage;

        private void NormalScreen(Screen myScreen) {
            this.Left = myScreen.WorkingArea.Left;
            this.Top = myScreen.WorkingArea.Top;
            this.Width = myScreen.WorkingArea.Width;
            this.Height = myScreen.WorkingArea.Height;
            panel1.Left = 50;
            leftImage = 0;
            picUp.Top = (30 * (panel1.Height / 100));
            picDown.Top = (30 * (panel1.Height / 100));
            picUp.Left = leftImage;
            picDown.Left = leftImage;
            int lMargin = (this.ClientSize.Width - lblCounter.Width) / 2;
            lblCounter.Top = (this.Height - lblCounter.Height) / 2;
            lblCounter.Left = lMargin;
            picLogo.Left = panel1.Left;
        }

        private void WiderScreen(Screen myScreen) {
            this.Left = myScreen.WorkingArea.Left + 1024;
            this.Top = myScreen.WorkingArea.Top;
            this.Width = 1024;
            this.Height = myScreen.WorkingArea.Height;
            panel1.Left = 50;

            leftImage = 0;
            picUp.Top = (30 * (panel1.Height / 100));
            picDown.Top = (30 * (panel1.Height / 100));
            picUp.Left = leftImage;
            picDown.Left = leftImage;

            picLogo.Left = 0;
            picLogo.Top = 0;
            picLogo.Width = this.Width;
            picLogo.Height = this.Height;
            lblCounter.Top = (this.Height - lblCounter.Height) / 2;
            lblCounter.Left = (this.Width - lblCounter.Width) / 2;
        }

        private void LoadWPF() {
            ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Fill;

            panel1.Controls.Add(ctrlHost);

            wpfViewBox = new Wpf_Controls.FlipViewer();
            wpfViewBox.InitializeComponent();
            ctrlHost.Child = wpfViewBox;
        }

        public void ChangeViewerWidth(double lP, double rP) {
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
            leftImage = panel1.Left - 50;
            rightImage = panel1.Right - 50;

            bool fx = Classes.Controller.flipX;
            bool fy = Classes.Controller.flipY;
            bool fb = Classes.Controller.flipB;

            try {
                wpfViewBox.Flipper(false, false, false, this.Height);
                wpfViewBox.ChangePageWidth(newWidth);
                SetEyeline();
            } catch {
                SetEyeline();
            }
            wpfViewBox.SetPageWidth(panel1.Width);
        }

        public void LoadText(MemoryStream ms) {
        }

        public void ResizeText(double fSize) {
        }

        public void ToggleBlackout(bool blackout) {
            wpfViewBox.IsVisible = !blackout;
            //wpfViewBox.Visibility = (blackout ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible);
        }

        private void frmViewer_MouseEnter(object sender, EventArgs e) {
            Cursor.Hide();
        }

        private void frmViewer_MouseLeave(object sender, EventArgs e) {
            Cursor.Show();
        }

        public void SetPercentage(int perc) {
            wpfViewBox.Perc = perc;
        }
    }
}