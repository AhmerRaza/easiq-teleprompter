using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Teleprompter {

    public partial class frmViewer : Form {
        private ElementHost ctrlHost;
        public Wpf_Controls.FlipViewer wpfViewBox;
        private bool isFlipped = false;
        private Screen myScreen;
        private Stopwatch sw = new Stopwatch();
        private bool runWatch = false;

        public frmViewer() {
            InitializeComponent();
            //Classes.Controller.ViewerStreamChangedEvent += new EventHandler<Classes.ViewerChangedArgs>(Controller_ViewerStreamChangedEvent);
            Classes.Controller.BigEditorPercentageChangedEvent += Controller_BigEditorPercentageChangedEvent;
            Classes.Controller.FontSizeChangedEvent += new EventHandler<EventArgs>(Controller_FontSizeChangedEvent);
            Classes.Controller.PlayStateChangedEvent += new EventHandler<EventArgs>(Controller_PlayStateChangedEvent);
            Classes.Controller.SpeedChangedEvent += new EventHandler<EventArgs>(Controller_SpeedChangedEvent);
            Classes.Controller.ChangeEyelineEvent += new EventHandler<EventArgs>(Controller_ChangeEyelineEvent);
            Classes.Controller.LogoChangedEvent += new EventHandler<EventArgs>(Controller_LogoChangedEvent);
            Classes.Controller.ShowCountEvent += new EventHandler<EventArgs>(Controller_ShowCountEvent);
            Classes.Controller.CountChangedEvent += new EventHandler<EventArgs>(Controller_CountChangedEvent);
        }

        private void Controller_BigEditorPercentageChangedEvent(object sender, EventArgs e) {
            double editorPercentage = Classes.Controller.bigEditPercentage;
            SetPercentage(editorPercentage);
        }

        private void Controller_CountChangedEvent(object sender, EventArgs e) {
            lblCounter.Text = Classes.Controller.CountText;
        }

        public void ToggleLayout(bool show) {
            if (show) {
                panel1.ResumeLayout();
            } else {
                panel1.SuspendLayout();
            }
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
                this.Invalidate();
            } else {
                picLogo.Visible = false;
                panel1.Visible = true;
                SetEyeline(isFlipped, wpfViewBox.fy);
            }
        }

        private void Controller_ChangeEyelineEvent(object sender, EventArgs e) {
            FlipEyeline();
        }

        public void SetEyeline(bool flipActive, bool flipVertical) {
            isFlipped = flipActive || Classes.Controller.flipX;

            if (!picLogo.Visible) {
                if (Classes.Controller.playSpeed >= 0) {
                    picUp.Visible = wpfViewBox.fb || wpfViewBox.fx;
                    picDown.Visible = !wpfViewBox.fb && !wpfViewBox.fx;
                } else {
                    picUp.Visible = !wpfViewBox.fb && !wpfViewBox.fx;
                    picDown.Visible = wpfViewBox.fb || wpfViewBox.fx;
                }
            }
            if (isFlipped) {
                if (this.ClientSize.Width - panel1.Right < 50) {
                    int diff = this.ClientSize.Width - panel1.Right - 50;
                    panel1.Left += diff;
                }
                picUp.Left = rightImage;
                int wherearewe = picUp.Right;
                picDown.Left = rightImage;
            } else {
                picUp.Left = leftImage;
                picDown.Left = leftImage;
            }
            FlipEyeline();
        }

        private void FlipEyeline() {
            int eyeline = Classes.Controller.eyeline;
            if (wpfViewBox.fy || wpfViewBox.fb) {
                int eyeMargin = (eyeline * (panel1.Height / 100));
                picUp.Top = panel1.Height - eyeMargin - 50;
                picDown.Top = panel1.Height - eyeMargin - 50;

                if (picUp.Top >= panel1.Height) {
                    picUp.Top -= 50;
                    picDown.Top -= 50;
                } else if (picUp.Top < 0) {
                    picUp.Top = 0;
                    picDown.Top = 0;
                }
            } else {
                picUp.Top = (eyeline * (panel1.Height / 100));
                picDown.Top = (eyeline * (panel1.Height / 100));
                if (picUp.Top >= panel1.Height) {
                    picUp.Top -= 50;
                    picDown.Top -= 50;
                }
            }
        }

        public int myHeight {
            get { return this.Height; }
        }

        private void Controller_SpeedChangedEvent(object sender, EventArgs e) {
            //int speed = (Classes.Controller.playSpeed < 0 ? Classes.Controller.playSpeed * -1 : Classes.Controller.playSpeed);
            //int interval = (60 - (speed * 5));
            //tmrScroll.Interval = 10;
            Classes.Controller.scrollComplete = true;
            wpfViewBox.offset = Classes.Controller.playSpeed;
            if (!picLogo.Visible) {
                if (Classes.Controller.playSpeed >= 0) {
                    picUp.Visible = wpfViewBox.fb || wpfViewBox.fx;
                    picDown.Visible = !wpfViewBox.fb && !wpfViewBox.fx;
                } else {
                    picUp.Visible = !wpfViewBox.fb && !wpfViewBox.fx;
                    picDown.Visible = wpfViewBox.fb || wpfViewBox.fx;
                }
            }
        }

        private void Controller_PlayStateChangedEvent(object sender, EventArgs e) {
            tmrScroll.Enabled = Classes.Controller.isPlaying;
            //ToggleStopWatch(Classes.Controller.isPlaying);
        }

        private void Controller_FontSizeChangedEvent(object sender, EventArgs e) {
            wpfViewBox.ChangeDefaultFontSize();
        }

        private void Controller_ViewerStreamChangedEvent(object sender, Classes.ViewerChangedArgs e) {
            wpfViewBox.LoadText(Classes.Controller.playlist.ViewerStream.RTFContent, "editor", true);
            wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false);
        }

        private void frmViewer_Resize(object sender, EventArgs e) {
        }

        private void frmViewer_Load(object sender, EventArgs e) {
            picUp.Size = new System.Drawing.Size(45, 45);
            picDown.Size = new System.Drawing.Size(45, 45);
            myScreen = (Screen.AllScreens.Length > 1 ? Screen.AllScreens[1] : Screen.AllScreens[0]);
            picUp.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);
            //picDown.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipX);

            if (myScreen.WorkingArea.Width > 2000) {
                WiderScreen(myScreen);
            } else {
                NormalScreen(myScreen);
            }
            //this.ClientSize = new System.Drawing.Size(1024, 768);
            LoadWPF();
            ChangeViewerWidth(0, 0);

            if (Classes.Controller.playSpeed >= 0) {
                picUp.Visible = false;
                picDown.Visible = true;
            } else {
                picUp.Visible = true;
                picDown.Visible = false;
            }

            picLogo.Visible = false;
            lblCounter.Visible = false;
            wpfViewBox.ChangePageWidth(panel1.Width);
        }

        public int leftImage, rightImage;

        private void NormalScreen(Screen myScreen) {
            this.Left = myScreen.WorkingArea.Left;
            this.Top = myScreen.WorkingArea.Top;
            this.Width = myScreen.WorkingArea.Width;
            this.Height = myScreen.WorkingArea.Height;
            int margin = (this.ClientSize.Width == 1024 ? 50 : (this.ClientSize.Width - panel1.Width) / 2);
            picUp.Top = (30 * (panel1.Height / 100));
            picDown.Top = (30 * (panel1.Height / 100));
            picUp.Left = picDown.Left = leftImage;

            int lMargin = (this.ClientSize.Width - lblCounter.Width) / 2;
            lblCounter.Top = (this.Height - lblCounter.Height) / 2;
            lblCounter.Left = lMargin;
            picLogo.Top = 0;
            picLogo.Left = 0;
            picLogo.Width = this.Width;
            picLogo.Height = this.Height;
        }

        private void WiderScreen(Screen myScreen) {
            this.Left = myScreen.WorkingArea.Left;
            this.Top = myScreen.WorkingArea.Top;
            this.Width = 1024;
            this.Height = myScreen.WorkingArea.Height;
            picUp.Top = (30 * (panel1.Height / 100));
            picDown.Top = (30 * (panel1.Height / 100));
            picUp.Left = picDown.Left = leftImage;

            picLogo.Top = 0;
            picLogo.Left = 0;
            picLogo.Width = this.Width;
            picLogo.Height = this.Height;
            lblCounter.Top = (this.Height - lblCounter.Height) / 2;
            lblCounter.Left = (this.Width - lblCounter.Width) / 2;
        }

        private void LoadWPF() {
            ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Fill;

            panel1.Controls.Add(ctrlHost);
            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(panel1, true, null);

            wpfViewBox = new Wpf_Controls.FlipViewer();
            wpfViewBox.ScrollChanged += new Wpf_Controls.FlipViewer.ScrollEventHandler(wpfViewBox_ScrollChanged);
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
            rightImage = panel1.Right - (this.ClientSize.Width - panel1.Right < 50 ? 50 : 0);

            bool fx = Classes.Controller.flipX;
            bool fy = Classes.Controller.flipY;
            bool fb = Classes.Controller.flipB;

            try {
                wpfViewBox.Flipper(false, false, false, this.Height);
                wpfViewBox.ChangePageWidth(newWidth);
                wpfViewBox.Flipper(fx, fy, fb, this.Height);
                SetEyeline(wpfViewBox.fx, wpfViewBox.fy);
            } catch {
                SetEyeline(false, false);
            }
            wpfViewBox.SetPageWidth(panel1.Width);
        }

        public System.Drawing.Size GetPanelSize() {
            return panel1.Size;
        }

        public delegate void LoadTextDelegate(List<String> markerNames, List<String> smarkerNames, double percentage);

        public void LoadText(List<String> markerNames, List<String> smarkerNames, double percentage) {
            if (this.InvokeRequired) {
                this.Invoke(new LoadTextDelegate(LoadText), markerNames, smarkerNames, percentage);
            } else {
                wpfViewBox.LoadText(Classes.Controller.playlist.ViewerStream.RTFContent, "editor", true);
                wpfViewBox.SetMarkerNames(markerNames, 1);
                wpfViewBox.SetMarkerNames(smarkerNames, 2);
                wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false);
                wpfViewBox.Perc = percentage;
            }
        }

        private void wpfViewBox_ScrollChanged(object sender, Wpf_Controls.ScrollArgs args) {
            Classes.Controller.ChangePercentage(args.perc, wpfViewBox.isFromFont);
        }

        public void TogglePlay() {
            tmrScroll.Enabled = !tmrScroll.Enabled;
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

        public void SetSpeed(int speed) {
            tmrScroll.Interval = 30 - (speed - 1);
            wpfViewBox.offset = (speed == 0 ? 0 : speed >= 1 ? 1 : -1);// speed;
        }

        private Thread stopwatchThread;

        private void ToggleStopWatch(bool startstop) {
            runWatch = startstop;
            if (runWatch) {
                ThreadStart ts = new ThreadStart(RunStopWatch);
                stopwatchThread = new Thread(ts);
                stopwatchThread.Start();
            } else {
                stopwatchThread.Abort();
                stopwatchThread = null;
            }
        }

        private void RunStopWatch() {
            sw.Reset();
            sw.Start();
            if (Stopwatch.IsHighResolution) {
                Console.WriteLine("Operations timed using the system's high-resolution performance counter.");
            } else {
                Console.WriteLine("Operations timed using the DateTime class.");
            }
            long frequency = Stopwatch.Frequency;
            Console.WriteLine("  Timer frequency in ticks per second = {0}",
                frequency);
            long msFreq = (frequency / 1000) * 30;
            long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
            while (runWatch) {
                if (sw.ElapsedTicks >= msFreq && Classes.Controller.scrollComplete) {
                    sw.Reset();
                    Classes.Controller.scrollComplete = false;
                    ScrollMe();
                    sw.Start();
                }
            }
        }

        private delegate void ScrollMeDelegate();

        private void ScrollMe() {
            if (InvokeRequired) {
                Invoke(new ScrollMeDelegate(ScrollMe));
            } else {
                wpfViewBox.Scroll();
            }
        }

        private void tmrScroll_Tick(object sender, EventArgs e) {
            if (Classes.Controller.scrollComplete) {
                Classes.Controller.scrollComplete = false;
                wpfViewBox.Scroll();
            }
        }

        public void SetPercentage(double perc) {
            wpfViewBox.Perc = perc;
        }
    }
}