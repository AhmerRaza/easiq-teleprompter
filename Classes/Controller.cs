using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Teleprompter.Classes
{
    public class Controller
    {
        #region Variables

        #region Public Variables

        public static volatile bool viewerLoaded = false;
        public static frmMain mainForm;
        public static frmViewer flipViewer;
        public static frmExViewer exViewer;
        public static Tcpclient tcpClient = new Tcpclient();
        public static Encoding iso = Encoding.GetEncoding(1252);
        public static bool scrollComplete;
        public static double prevlinkPerc = 0;
        public static double editorPerc = 0;
        public static frmEditor bigEditor;
        public static CommsClient client;
        public static YellowMonkey.Licensor.Classes.License myLicense;
        public static Dictionary<String, String> streamItems;
        public static double eFSize = Properties.Settings.Default.efsize;
        public static double vFSize = Properties.Settings.Default.vfsize;
        public static bool flipX = false;
        public static bool flipY = false;
        public static bool flipB = false;
        public static bool showSaveMessage = false;
        public static bool linkText = false;
        public static String pptServerIP = Properties.Settings.Default.pptServer;
        public static bool canconnect = false;
        public static int eyeline = 0;
        public static Image imgViewer = null;
        public static Image imgExViewer = null;
        public static bool showLogo = false;
        public static bool isPlaying = false;
        public static int playSpeed = 0;
        public static double vpercentage = 0;
        public static double bigEditPercentage = 0;
        public static bool countupPlaying = false;
        public static bool countdownPlaying = false;
        public static bool ShowCounter = false;
        public static String CountText = String.Empty;
        public static List<String> fonts;
        public static bool flipVisible = false;

        #endregion Public Variables

        #region Private Variables

        private static IMReceivedEventHandler receivedHandler;
        private static ServiceHost host;
        private static BackgroundWorker worker;
        private static String viewerText;
        public static bool blackout = false;
        private static bool autotext;
        private static Playlist plist;
        private static double oflineHeight = 0;
        private static List<String> markerNames;
        private static List<String> smarkerNames;

        #endregion Private Variables

        #endregion Variables

        #region Properties

        public static double percentage
        {
            get
            {
                vpercentage = (flipVisible ? flipViewer.wpfViewBox.Perc : 0);
                return vpercentage;
            }
            set { vpercentage = value; }
        }

        public static Playlist playlist
        {
            get { return plist; }
            set { plist = value; }
        }

        public static bool Autotext
        {
            get { return autotext; }
            set
            {
                autotext = value;
                if (autotext)
                {
                    worker.CancelAsync();
                    bool cp = worker.CancellationPending;
                    worker.RunWorkerAsync();
                }
                else
                {
                    if (worker.IsBusy) { worker.CancelAsync(); }
                }
            }
        }

        public static String MarkerRTF
        {
            get
            {
                String mrtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000017352474200aece1ce90000000467414d410000b18f0bfc6105000000097048597300002e2300002e230178a53f7600000034494441542853636460f8ff998181810788b1818f4c";//50064e0052f017c2c40e8832012f185500014429608630b103022630300000d07c040f0baf09460000000049454e44ae426082";
                return mrtf;
            }
        }

        public static String WhiteRTF
        {
            get
            {
                String wrtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000017352474200aece1ce90000000467414d410000b18f0bfc6105000000097048597300002e2300002e230178a53f760000001b49444154285363fc0f040c78001394c609461540c07050c0c000005152041cc3bda7bd0000000049454e44ae426082";
                return wrtf;
            }
        }

        public static String XPRtf
        {
            get
            {
                String rtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000097048597300002e2200002e2201aae2dd920000002c494441542853636460f8ff998181810788b1818f40419082ffff71e00f2005406254c120090782918557010080cbf0480a6ea2890000000049454e44ae426082";
                //String rtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000097048597300002e2200002e2201aae2dd920000001a49444154285363fc0f040cf80048013ec030aa001c02c3231c008d64e22dcc8607890000000049454e44ae426082";
                return rtf;
            }
        }

        public static String XPRtf2
        {
            get
            {
                //String rtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000097048597300002e2100002e2101075bfcff0000001a49444154285363fc0f040cf80048013ec030aa001c02c3231c008d64e22dcc8607890000000049454e44ae426082";
                String rtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000097048597300002e2100002e2101075bfcff0000002c494441542853636460f8ff998181810788b1818f40419082ffff71e00f2005406254c120090782918557010080cbf0480a6ea2890000000049454e44ae426082";
                return rtf;
            }
        }

        public static String MarkerRTF2
        {
            get
            {
                String mrtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000017352474200aece1ce90000000467414d410000b18f0bfc6105000000097048597300002e2300002e230178a53f7600000030494441542853636460f8ff998181810788b1818f4c";//3824e0c220057ff12922ca04bcb68c9a00091ea2c28199a2b80000d07c040f270d6b3f0000000049454e44ae426082";
                return mrtf;
            }
        }

        public static String SlideMarkerRTF
        {
            get
            {
                String srtf = "89504e470d0a1a0a0000000d49484452000000080000001008060000002b8a3e7d000000017352474200aece1ce90000000467414d410000b18f0bfc6105000000097048597300002e2300002e230178a53f760000004149444154285363fc3883e7331fef171e86bf0cd8c047262803270029c0ae170a98f8b8bee05542941578c14851f0e91b0f030333948705804cc0234dd00a0606004e620ab1a49399af0000000049454e44ae426082";
                return srtf;
            }
        }

        public static bool ShowLogo
        {
            set
            {
                showLogo = value;
                if (imgViewer != null) { RaiseNewLogoEvent(); }
            }
        }

        #endregion Properties

        #region Application Functions

        public static void RunProgram(String[] args)
        {
            //args = new String[] {"/u"};
            List<String> actArgs = new List<string>();
            for (int i = 0; i < args.Length; i++) { actArgs.Add(args[i]); }
            if (!actArgs.Contains("/u"))
            {
                CheckActivation();
            }
            else
            {
                MessageBox.Show("Skipping license");
            }

            //GetFileVersion();
            if (!GetRegisteredApplication())
            {
                SetApplication();
            }
            mainForm = new frmMain();
            String flipMessage = (Screen.AllScreens.Length > 1 ? "Attached screen detected. " : "No attached screen detected. ");
            flipMessage += "Open flip viewer?";
            if (MessageBox.Show(flipMessage, "Teleprompter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                flipViewer = new frmViewer();
                flipVisible = true;
            }

            bigEditor = new frmEditor(false);
            scrollComplete = true;
            mainForm.Show();
            if (Screen.AllScreens.Length > 1 && Screen.AllScreens[1].WorkingArea.Width > 2000 && flipVisible)
            {
                flipMessage = "Extra viewer available. Open extra viewer?";
                if (MessageBox.Show(flipMessage, "Teleprompter", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    exViewer = new frmExViewer();
                    exViewer.Show();
                    exViewer.ChangeViewerWidth(0, 0);
                }
            }
            if (flipVisible)
            {
                flipViewer.Show();
                flipViewer.wpfViewBox.TransitionEvent += new EventHandler<Wpf_Controls.TransitionArgs>(wpfViewBox_TransitionEvent);
            }
            if (actArgs.Count > 0)
            {
                try
                {
                    foreach (String arg in actArgs)
                    {
                        if (arg != "/u")
                        {
                            String defaultFile = arg;
                            //MessageBox.Show(defaultFile);
                            playlist = DefaultPlaylist(defaultFile);
                            RaiseNewPlaylistEvent();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (playlist == null)
                {
                    playlist = new Playlist();
                    RaiseNewPlaylistEvent();
                }
            }
            else
            {
                playlist = new Playlist();
                RaiseNewPlaylistEvent();
            }
            if (Screen.AllScreens.Length == 1) { mainForm.BringToFront(); }
            markerPositions = new Dictionary<string, Inline>();
            //client = new CommsClient();
            //receivedHandler = new IMReceivedEventHandler(im_MessageReceived);
            //client.MessageReceived += receivedHandler;
        }

        public static bool Exit()
        {
            if (!showSaveMessage)
            {
                DialogResult dr = MessageBox.Show("Do you wish to save the current playlist?", "Teleprompter", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    SavePlaylist(false);
                }
                else if (dr == DialogResult.Cancel)
                {
                    return false;
                }
                showSaveMessage = true;
            }
            return true;
        }

        private static bool GetRegisteredApplication()
        {
            String FileExtension = "esq";
            //Return registered application based on the filename extension
            String strExtension;
            String strProgramName;
            String strEXEFilename;
            RegistryKey regkey_HKEY_CLASSES_ROOT;
            RegistryKey regkey_ProgID;
            RegistryKey regkey_OpenCommand;
            try
            {
                //Add starting dot to extension
                if (FileExtension.StartsWith(".")) { strExtension = FileExtension; } else { strExtension = "." + FileExtension; }
                //Get Programmatic Identifier for this extension
                try
                {
                    regkey_HKEY_CLASSES_ROOT = Registry.ClassesRoot;
                    regkey_ProgID = regkey_HKEY_CLASSES_ROOT.OpenSubKey(strExtension);
                    strProgramName = regkey_ProgID.GetValue(null).ToString();
                    regkey_ProgID.Close();
                }
                catch
                {
                    //Nothing found
                    return false;
                }
                //Get  application associated with the file extension
                try
                {
                    regkey_OpenCommand = regkey_HKEY_CLASSES_ROOT.OpenSubKey(strExtension + "\\shell\\open\\command");
                    strEXEFilename = regkey_OpenCommand.GetValue(null).ToString();
                    regkey_OpenCommand.Close();
                }
                catch
                {
                    //No default application is registered
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void SetApplication()
        {
            string ext = ".esq";
            RegistryKey key = Registry.ClassesRoot.CreateSubKey(ext);
            key.SetValue("", "esq_auto_file");
            key.Close();

            key = Registry.ClassesRoot.CreateSubKey("esq_auto_file" + "\\shell\\Open\\Command");

            key.SetValue("", "\"" + Application.ExecutablePath + "\" \"%L\"");
            key.Close();
        }

        private static void CheckActivation()
        {
            String status;
            System.Net.ServicePointManager.Expect100Continue = false;
            bool activeLicense = YellowMonkey.Licensor.Classes.LicenseGenerator.ValidateLicense(AppDomain.CurrentDomain.BaseDirectory, out myLicense);
            if (!activeLicense)
            {
                MessageBox.Show("No valid license found, closing application", "Licensing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                Environment.Exit(0);
            }
            /*
            while (!activeLicense) {
                System.Net.ServicePointManager.Expect100Continue = false;
                if (!Licensing.LicenseGenerator.ValidateLicense(out myLicense, out status)) {
                    Forms.frmActivation activationF = new Forms.frmActivation(null);
                    if (activationF.ShowDialog() != DialogResult.OK) {
                        Application.Exit();
                        Environment.Exit(0);
                    } else {
                        System.Net.ServicePointManager.Expect100Continue = false;
                        activeLicense = Licensing.LicenseGenerator.ValidateLicense(out myLicense, out status);
                    }
                }
            }
            */
        }

        private static void im_MessageReceived(object sender, IMReceivedEventArgs e)
        {
            String from = e.From;
            String msg = e.Message;
        }

        #endregion Application Functions

        #region Events

        public static event EventHandler<EventArgs> NewPlaylistEvent;

        public static event EventHandler<EventArgs> StreamChangedEvent;

        public static event EventHandler<EventArgs> FontSizeChangedEvent;

        public static event EventHandler<ViewerChangedArgs> ViewerStreamChangedEvent;

        public static event EventHandler<EventArgs> ActiveStreamChangedEvent;

        public static event EventHandler<EventArgs> SpeedChangedEvent;

        public static event EventHandler<EventArgs> PlayStateChangedEvent;

        public static event EventHandler<EventArgs> PercentageChangedEvent;

        public static event EventHandler<EventArgs> BigEditorPercentageChangedEvent;

        public static event EventHandler<EventArgs> ChangeEyelineEvent;

        public static event EventHandler<EventArgs> LogoChangedEvent;

        public static event EventHandler<EventArgs> ShowCountEvent;

        public static event EventHandler<EventArgs> CountChangedEvent;

        private static void wpfViewBox_TransitionEvent(object sender, Wpf_Controls.TransitionArgs e)
        {
            mainForm.TransitionStream(e.forward);
        }

        private static void playlist_NewPlaylistEvent(object sender, EventArgs e)
        {
            RaiseNewPlaylistEvent();
        }

        private static void playlist_ViewerStreamChanged(object sender, ActiveStreamChangedArgs e)
        {
            if (ViewerStreamChangedEvent != null) { ViewerStreamChangedEvent(null, new ViewerChangedArgs(0, true, true)); }
        }

        private static void playlist_StreamRemovedEvent(object sender, StreamRemovedArgs e)
        {
            RaiseNewPlaylistEvent();
        }

        private static void playlist_StreamDetailsChangedEvent(object sender, StreamDetailsChangedArgs e)
        {
            RaiseNewPlaylistEvent();
        }

        private static void playlist_StreamAddedevent(object sender, StreamAddedArgs e)
        {
            RaiseNewPlaylistEvent();
        }

        private static void playlist_ForceRefreshEvent(object sender)
        {
        }

        private static void playlist_ActiveStreamChangedEvent(object sender, ActiveStreamChangedArgs e)
        {
        }

        private static void wpfViewBox_LoadedCompleteEvent(object sender, EventArgs e)
        {
            viewerLoaded = true;
        }

        private static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public static volatile bool MatchHandled = false;

        #endregion Events

        #region Play list Functions

        public static void ChangeActiveStream(String streamGUID)
        {
            playlist.ActiveStream = GetStreamByGUID(streamGUID);
            if (ActiveStreamChangedEvent != null) { ActiveStreamChangedEvent(null, new EventArgs()); }
        }

        public static Playlist DefaultPlaylist(String fileName)
        {
            if (playlist != null) { DisconnectFromPlaylist(); }
            if (!String.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                playlist = new Playlist();
                playlist.FileName = fileName;
                playlist.Name = Path.GetFileNameWithoutExtension(fileName);
                FileInfo fi = new FileInfo(fileName);
                FileStream fs = fi.OpenRead();
                byte[] buffer = new byte[fi.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                playlist.XML = new ASCIIEncoding().GetString(buffer);
                if (playlist.Streams.Count > 0)
                {
                    playlist.ActiveStream = playlist.Streams[0];
                }
            }
            RaiseNewPlaylistEvent();

            if (playlist.ActiveStream == null) { playlist.ActiveStream = new QStream(); }
            if (playlist.ViewerStream == null) { playlist.ViewerStream = new QStream(); }
            ConnectToPlaylist();
            return playlist;
        }

        public static Playlist NewPlaylist(bool hasFile)
        {
            if (playlist != null)
            {
                DisconnectFromPlaylist();
            }
            if (!hasFile)
            {
                playlist = new Playlist();
                fmPlaylistProp pplaylist = new fmPlaylistProp();
                pplaylist.PlayList = playlist;
                if (pplaylist.ShowDialog() == DialogResult.OK) { playlist = pplaylist.PlayList; }
                if (playlist.Streams.Count > 1) { playlist.Add(new QStream()); }
                RaiseNewPlaylistEvent();
            }
            else
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                String fileName = String.Empty;
                ofd.Filter = "EasiQ Scripts (*.esq)|*.esq|Text Files (*.txt)|*.txt";
                ofd.CheckFileExists = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    fileName = ofd.FileName;
                    if (!String.IsNullOrEmpty(fileName) && File.Exists(fileName))
                    {
                        playlist = new Playlist();
                        playlist.FileName = fileName;
                        playlist.Name = Path.GetFileNameWithoutExtension(fileName);
                        FileInfo fi = new FileInfo(fileName);
                        FileStream fs = fi.OpenRead();
                        byte[] buffer = new byte[fi.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();
                        playlist.XML = new ASCIIEncoding().GetString(buffer);
                        if (playlist.Streams.Count > 0)
                        {
                            playlist.ActiveStream = playlist.Streams[0];
                        }
                    }
                    RaiseNewPlaylistEvent();
                }
            }

            if (playlist.ActiveStream == null) { playlist.ActiveStream = new QStream(); }
            if (playlist.ViewerStream == null) { playlist.ViewerStream = new QStream(); }
            ConnectToPlaylist();
            return playlist;
        }

        private static void DisconnectFromPlaylist()
        {
            playlist.ActiveStreamChangedEvent -= new EventHandler<ActiveStreamChangedArgs>(playlist_ActiveStreamChangedEvent);
            playlist.ForceRefreshEvent -= new Playlist.ForceRefreshDelegate(playlist_ForceRefreshEvent);
            playlist.StreamAddedevent -= new EventHandler<StreamAddedArgs>(playlist_StreamAddedevent);
            playlist.StreamDetailsChangedEvent -= new EventHandler<StreamDetailsChangedArgs>(playlist_StreamDetailsChangedEvent);
            playlist.StreamRemovedEvent -= new EventHandler<StreamRemovedArgs>(playlist_StreamRemovedEvent);
            playlist.ViewerStreamChanged -= new EventHandler<ActiveStreamChangedArgs>(playlist_ViewerStreamChanged);
        }

        private static void ConnectToPlaylist()
        {
            playlist.ActiveStreamChangedEvent += new EventHandler<ActiveStreamChangedArgs>(playlist_ActiveStreamChangedEvent);
            playlist.ForceRefreshEvent += new Playlist.ForceRefreshDelegate(playlist_ForceRefreshEvent);
            playlist.StreamAddedevent += new EventHandler<StreamAddedArgs>(playlist_StreamAddedevent);
            playlist.StreamDetailsChangedEvent += new EventHandler<StreamDetailsChangedArgs>(playlist_StreamDetailsChangedEvent);
            playlist.StreamRemovedEvent += new EventHandler<StreamRemovedArgs>(playlist_StreamRemovedEvent);
            playlist.ViewerStreamChanged += new EventHandler<ActiveStreamChangedArgs>(playlist_ViewerStreamChanged);
        }

        public static void SavePlaylist(bool newfile)
        {
            if (String.IsNullOrEmpty(playlist.FileName) || !File.Exists(playlist.FileName) || newfile)
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "EasiQ Scripts (*.esq) | *.esq";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    playlist.FileName = sfd.FileName;
                }
                else
                {
                    return;
                }
            }
            if (!String.IsNullOrEmpty(playlist.FileName) && (playlist.FileName != "") && (playlist.FileName != ".esq"))
            {
                playlist.Name = Path.GetFileNameWithoutExtension(playlist.FileName);
                if (!playlist.FileName.EndsWith(".esq"))
                {
                    int dotIdx = playlist.FileName.IndexOf(".");
                    String ext = playlist.FileName.Substring(dotIdx, playlist.FileName.Length - dotIdx);
                    playlist.FileName = playlist.FileName.Replace(ext, ".esq");
                }
                String content = playlist.XML;
                if (!String.IsNullOrEmpty(content))
                {
                    FileStream fs = new FileStream(playlist.FileName, FileMode.Create);
                    byte[] buffer = new ASCIIEncoding().GetBytes(content);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                    if (!File.Exists(playlist.FileName))
                    {
                        MessageBox.Show("File not saved", "File Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //MessageBox.Show("Playlist " + playlist.Name + " saved as " + playlist.FileName, "File Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No content", "File Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (MessageBox.Show("No filename entered", "File Save", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Retry)
                {
                    SavePlaylist(newfile);
                }
                else
                {
                    return;
                }
            }
        }

        public static void ExportToTXT()
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "Text File (*.txt) | *.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string data = "";
                if (playlist.Name.Trim() != "") { data += "Name: " + playlist.Name + "\r\n"; }
                if (!String.IsNullOrEmpty(playlist.Client)) { data += "Client: " + playlist.Client + "\r\n"; }
                if (!String.IsNullOrEmpty(playlist.ProductionCompany)) { data += "Production Company: " + playlist.ProductionCompany + "\r\n"; }
                if (!String.IsNullOrEmpty(playlist.Production)) { data += "Production: " + playlist.Production + "\r\n"; }
                if (!String.IsNullOrEmpty(playlist.Season)) { data += "Season: " + playlist.Season + "\r\n"; }
                if (!String.IsNullOrEmpty(playlist.Episode)) { data += "Episode: " + playlist.Episode + "\r\n"; }
                foreach (QStream stream in playlist.Streams)
                {
                    data += "\r\n" + stream.Name + "\r\n";
                    for (int i = 0; i <= stream.Name.Length; i++) { data += "-"; }
                    data += "\r\n";
                    String rtfText = GetText(CreateRichTextBoxWithText(stream.RTFContent));
                    data += rtfText;
                }
                TextWriter txtWriter = new StreamWriter(sfd.FileName, false);
                txtWriter.Write(data);
                txtWriter.Close();
                System.Diagnostics.Process.Start(sfd.FileName);
            }
        }

        public static Playlist ImportStream()
        {
            if (playlist == null) { playlist = new Playlist(); }
            DisconnectFromPlaylist();
            ConnectToPlaylist();
            String filename = String.Empty;
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt";
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK) { filename = ofd.FileName; }
            if (!String.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                int removePath = filename.LastIndexOf(@"\");
                String streamName = CleanContents(filename.Substring(removePath, filename.Length - removePath).Replace(@"\", "").Replace(".txt", ""));
                String contents = "";
                Encoding ansi = Encoding.GetEncoding(1252);
                StreamReader objReader = new StreamReader(filename, iso);
                contents = objReader.ReadToEnd();
                contents = CleanContents(contents);
                contents = contents.Replace("\t", String.Empty);//.Replace("\r", "\r\n");
                contents = Regex.Replace(contents, "\n", "");
                contents = Regex.Replace(contents, "\r", "\n");
                contents = Regex.Replace(contents, "\n", "\r\n");

                System.Windows.Controls.RichTextBox rtfControl = CreateRichTextBoxWithText(contents);

                contents = GetTextFromRichTextBox(rtfControl);
                string encodedXml = "<text>" + System.Security.SecurityElement.Escape(contents) + "</text>";

                objReader.Close();
                String guid = System.Guid.NewGuid().ToString();
                String xmlString = "<stream><wps>3</wps><guid>";
                xmlString += guid + "</guid><name>" + streamName + "</name>";
                xmlString += "<trans>false</trans>";
                contents = "<text>" + contents + "</text>"; //System.Security.SecurityElement.Escape
                xmlString += encodedXml + "</stream>";
                QStream stream = new QStream();
                stream.StreamFontSize = (int)((eFSize - 20) / 3) + 1;
                stream.XML = xmlString;

                playlist.Add(stream);
            }
            return playlist;
        }

        private static System.Windows.Controls.RichTextBox CreateRichTextBoxWithText(String text)
        {
            System.Windows.Controls.RichTextBox rtf = new System.Windows.Controls.RichTextBox();
            rtf.Foreground = (Properties.Settings.Default.defaultFore == "White" ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black);

            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            byte[] a = enc.GetBytes(text);

            MemoryStream m = new System.IO.MemoryStream(a);
            StreamWriter sw = new StreamWriter(m, enc);
            sw.Write(text);
            sw.Flush();

            m.Position = 0;
            StreamReader sr = new StreamReader(m, enc);
            String myStr = sr.ReadToEnd();

            TextRange textRange = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
            textRange.Text = text;
            double size = vFSize;
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, "Arial");
            return rtf;
        }

        private static System.Windows.Controls.RichTextBox CreateRichTextBoxWithText(MemoryStream ms)
        {
            System.Windows.Controls.RichTextBox rtf = new System.Windows.Controls.RichTextBox();
            rtf.Foreground = (Properties.Settings.Default.defaultFore == "White" ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black);

            rtf.Selection.Load(ms, System.Windows.DataFormats.Rtf);
            TextRange textRange = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
            double size = vFSize;
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, "Arial");
            return rtf;
        }

        private static String GetTextFromRichTextBox(System.Windows.Controls.RichTextBox rtf)
        {
            TextRange tr = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, System.Windows.DataFormats.Rtf);
            string myText = iso.GetString(ms.ToArray());
            return myText;
        }

        private static String GetText(System.Windows.Controls.RichTextBox rtf)
        {
            TextRange tr = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            return tr.Text;
        }

        public static String CleanContents(String content)
        {
            content = content.Replace("â€¦", "!").Replace("â€™", "'").Replace("â€˜n", "'").Replace("â€˜", "'").Replace("â€œ", "\"");
            content = content.Replace("â€", "\"").Replace("Ã´", "ô").Replace("Ãª", "ê").Replace("‘", "'").Replace("’", "'");
            content = content.Replace("Ã«", "ë").Replace("Ã‰", "É").Replace("Õ", "'").Replace("&", "&amp;");
            return content;
        }

        public static QStream GetStreamByGUID(String guid)
        {
            QStream returnStream = null;
            foreach (QStream qs in playlist.Streams)
            {
                if (qs.GUID == guid)
                {
                    returnStream = qs;
                    break;
                }
            }
            return returnStream;
        }

        public static void SetStreamByGuid(QStream stream)
        {
            for (int i = 0; i < playlist.Streams.Count; i++)
            {
                if (playlist.Streams[i].GUID == stream.GUID)
                {
                    playlist.Streams[i] = stream;
                    break;
                }
            }
            RaiseNewPlaylistEvent();
        }

        public static void AddNewStream()
        {
            if (playlist == null) { playlist = new Playlist(); }
            DisconnectFromPlaylist();
            ConnectToPlaylist();
            playlist.Add(new QStream("NEW STREAM"));
        }

        #endregion Play list Functions

        public static void SendStreamToViewer(double percentage)
        {
            try
            {
                viewerLoaded = false;
                flipViewer.wpfViewBox.LoadedCompleteEvent -= new EventHandler<EventArgs>(wpfViewBox_LoadedCompleteEvent);
                flipViewer.wpfViewBox.LoadedCompleteEvent += new EventHandler<EventArgs>(wpfViewBox_LoadedCompleteEvent);
                flipViewer.wpfViewBox.LoadText(playlist.ViewerStream, true);
                if (exViewer != null) { exViewer.wpfViewBox.LoadText(playlist.ViewerStream.RTFContent, "editor", false); }
                markerNames = (markerNames == null ? new List<string>() : markerNames);
                smarkerNames = (smarkerNames == null ? new List<string>() : smarkerNames);
                List<String> tmarkerNames = new List<string>();
                List<String> tsmarkerNames = new List<string>();
                foreach (Marker m in Classes.Controller.playlist.ViewerStream.Markers) { tmarkerNames.Add(m.InlineName); }
                foreach (Marker m in Classes.Controller.playlist.ViewerStream.SlideMarkers) { tsmarkerNames.Add(m.InlineName); }

                //if (markerNames != tmarkerNames) {
                markerNames = tmarkerNames;
                //if (markerNames.Count > 0) { MessageBox.Show("Markers exist = " + markerNames.Count.ToString()); }
                flipViewer.wpfViewBox.SetMarkerNames(markerNames, 1);
                if (exViewer != null) { exViewer.wpfViewBox.SetMarkerNames(markerNames, 1); }
                // } if (smarkerNames != tsmarkerNames) {
                smarkerNames = tsmarkerNames;
                flipViewer.wpfViewBox.SetMarkerNames(smarkerNames, 2);
                if (exViewer != null) { exViewer.wpfViewBox.SetMarkerNames(smarkerNames, 2); }
                // }
                if (exViewer != null)
                {
                    exViewer.wpfViewBox.ResizeViewer(vFSize, false);
                }
            }
            catch { }
        }

        public static void UpdateViewerMarkers()
        {
            try
            {
                markerNames = (markerNames == null ? new List<string>() : markerNames);
                smarkerNames = (smarkerNames == null ? new List<string>() : smarkerNames);
                List<String> tmarkerNames = new List<string>();
                List<String> tsmarkerNames = new List<string>();
                foreach (Marker m in Classes.Controller.playlist.ViewerStream.Markers) { tmarkerNames.Add(m.InlineName); }
                foreach (Marker m in Classes.Controller.playlist.ViewerStream.SlideMarkers) { tsmarkerNames.Add(m.InlineName); }

                //if (markerNames != tmarkerNames) {
                markerNames = tmarkerNames;
                //if (markerNames.Count > 0) { MessageBox.Show("Markers exist = " + markerNames.Count.ToString()); }
                flipViewer.wpfViewBox.SetMarkerNames(markerNames, 1);
                if (exViewer != null) { exViewer.wpfViewBox.SetMarkerNames(markerNames, 1); }
                // } if (smarkerNames != tsmarkerNames) {
                smarkerNames = tsmarkerNames;
                flipViewer.wpfViewBox.SetMarkerNames(smarkerNames, 2);
                if (exViewer != null) { exViewer.wpfViewBox.SetMarkerNames(smarkerNames, 2); }
                // }
                if (exViewer != null)
                {
                    exViewer.wpfViewBox.ResizeViewer(vFSize, false);
                }
            }
            catch { }
        }

        public static bool BigEditorLoaded = false;

        public static void ShowBigEditor(bool showMe)
        {
            if (showMe)
            {
                BigEditorLoaded = false;
                editorPerc = mainForm.wpfEditorBox.GetPercentage();
                bigEditor = new frmEditor(true);
                mainForm.SetMarkerNames();
                bigEditor.wpfViewBox.ResizeViewer(vFSize, false, true);
                bigEditor.SetPercentage((int)editorPerc);
                bigEditor.updateViewer(false);
                System.Windows.Forms.Application.DoEvents();
                BigEditorLoaded = true;
                mainForm.Hide();
                bigEditor.Show();
                bigEditor.BringToFront();
            }
            else
            {
                editorPerc = bigEditor.wpfViewBox.GetPercentage2();
                percentage = editorPerc;
                bigEditor.SendToBack();
                bigEditor.Close();
                bigEditor = null;
                mainForm.Show();
                mainForm.changed = false;
                mainForm.toggleBigBox();
                mainForm.wpfEditorBox.SetPercentage(editorPerc);
            }
        }

        public static Dictionary<String, Inline> markerPositions;

        public static void ToggleCounter(bool showCounter)
        {
            ShowCounter = showCounter;
            if (ShowCounter && isPlaying) { TogglePlay(); }
            Blackout(ShowCounter);
            if (ShowCountEvent != null) { ShowCountEvent(null, new EventArgs()); }
        }

        public static void ChangeCounter(String countText)
        {
            CountText = countText;
            if (CountChangedEvent != null) { CountChangedEvent(null, new EventArgs()); }
        }

        public static void SetStreamItems()
        {
            if (streamItems == null)
            {
                streamItems = new Dictionary<string, string>();
            }
            else
            {
                streamItems.Clear();
            }
            foreach (QStream qstream in playlist.Streams)
            {
                if (qstream.GUID != null && qstream.Name != null)
                {
                    streamItems.Add(qstream.GUID, qstream.Name);
                }
            }
        }

        public static void RemoveStream()
        {
            if (playlist.ActiveStream != null)
            {
                if (MessageBox.Show("Confirm delete stream " + playlist.ActiveStream.Name + "?", "Delete stream", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    playlist.Remove(playlist.ActiveStream);
                    if (StreamChangedEvent != null) { StreamChangedEvent(null, new EventArgs()); }
                    RaiseNewPlaylistEvent();
                }
            }
        }

        public static void ReorderPlaylist(List<String> tags)
        {
            List<QStream> newOrder = new List<QStream>();
            for (int i = 0; i < tags.Count; i++)
            {
                QStream stream = playlist[tags[i]];
                newOrder.Add(stream);
            }
            for (int i = 0; i < playlist.Streams.Count; i++)
            {
                if (playlist[i] != newOrder[i]) { playlist[i] = newOrder[i]; }
            }
            RaiseNewPlaylistEvent();
        }

        public static void SetImage(Image img)
        {
            imgViewer = (Image)img.Clone();
            imgExViewer = (Image)img.Clone();
            if (showLogo) { RaiseNewLogoEvent(); }
        }

        public static void SetFontSizes(int selectedSize)
        {
            double baseSize = 20;
            eFSize = baseSize + ((selectedSize - 1) * 3);
            if (eFSize == 41)
            {
                eFSize = 40;
            }
            vFSize = eFSize * 3;
            double currentPercentage = 0;
            if (flipVisible)
            {
                currentPercentage = flipViewer.wpfViewBox.Perc;
                flipViewer.wpfViewBox.ResizeViewer(vFSize, true);
            }
            if (exViewer != null) { exViewer.wpfViewBox.ResizeViewer(vFSize, true); }
            Application.DoEvents();
            foreach (QStream stream in playlist)
            {
                if (stream != playlist.ViewerStream)
                {
                    List<String> markerNames = new List<string>();
                    List<String> smarkerNames = new List<string>();
                    foreach (Classes.Marker m in stream.Markers) { markerNames.Add(m.InlineName); }
                    foreach (Classes.Marker m in stream.SlideMarkers) { smarkerNames.Add(m.InlineName); }
                    SetMarkerNames(stream, markerNames, 1);
                    SetMarkerNames(stream, smarkerNames, 2);
                }
            }
            Application.DoEvents();
            if (flipVisible)
            {
                flipViewer.wpfViewBox.Perc = currentPercentage;
                if (exViewer != null) { exViewer.wpfViewBox.Perc = currentPercentage; }
            }
        }

        public static void ChangeDefaultFontSizes(int selectedSize)
        {
            double baseSize = 20;
            double eSize = baseSize + (selectedSize * 3);
            double vSize = eSize * 3;
            Properties.Settings.Default.efsize = eSize;
            Properties.Settings.Default.vfsize = vSize;
            Properties.Settings.Default.Save();
            if (FontSizeChangedEvent != null) { FontSizeChangedEvent(null, new EventArgs()); }
        }

        public static double getViewerPercentage()
        {
            if (flipVisible)
            {
                return flipViewer.wpfViewBox.Perc;
            }
            else
            {
                return mainForm.wpfEditorBox.Perc;
            }
        }

        public static void SetFlipper(bool x, bool y)
        {
            flipX = x;
            flipY = y;
            if (flipX && flipY)
            {
                flipB = true;
            }
            else
            {
                flipB = false;
            }
            flipViewer.wpfViewBox.Flipper(flipX, flipY, flipB, flipViewer.myHeight);
            flipViewer.SetEyeline(flipX, flipY);
        }

        public static void Blackout(bool blackoutSelected)
        {
            blackout = blackoutSelected;
            flipViewer.ToggleBlackout(blackout);
            if (exViewer != null) { exViewer.ToggleBlackout(blackout); }
        }

        public static void ChangeSpeed(int speed)
        {
            playSpeed = speed;
            if (SpeedChangedEvent != null) { SpeedChangedEvent(null, new EventArgs()); }
        }

        public static void TogglePlay()
        {
            isPlaying = !isPlaying;
            if (PlayStateChangedEvent != null) { PlayStateChangedEvent(null, new EventArgs()); }
        }

        public static void ChangeBigEditorPercentage(double perc)
        {
            bigEditPercentage = perc;
            if (BigEditorPercentageChangedEvent != null) { BigEditorPercentageChangedEvent(null, new EventArgs()); }
        }

        public static void ChangePercentage(double perc, bool fromFont)
        {
            if (!fromFont)
            {
                percentage = perc;
                if (PercentageChangedEvent != null) { PercentageChangedEvent(null, new EventArgs()); }
            }
        }

        public static void SetPercentage(double perc)
        {
            percentage = perc;
            if (flipViewer != null && flipViewer.wpfViewBox != null)
            {
                flipViewer.wpfViewBox.Perc = perc;
                double vp = flipViewer.wpfViewBox.Perc;
                if (exViewer != null) { exViewer.wpfViewBox.Perc = perc; }
            }
        }

        public static void SetOffset(double offset)
        {
            if (flipViewer.wpfViewBox != null) { flipViewer.wpfViewBox.VOffset = offset; }
            if (exViewer != null) { exViewer.wpfViewBox.VOffset = offset; }
        }

        public static void ChangeEyeline(int eye)
        {
            eyeline = eye;
            if (ChangeEyelineEvent != null) { ChangeEyelineEvent(null, new EventArgs()); }
        }

        public static void ChangeMarker(String markerName)
        {
            Inline i;
            //MessageBox.Show("sel marker = " + markerName);

            if ((i = flipViewer.wpfViewBox.GotoMarker(markerName)) != null)
            {
                flipViewer.wpfViewBox.ScrollToMarker(i);
                Application.DoEvents();
                if (exViewer != null)
                {
                    exViewer.wpfViewBox.Perc = flipViewer.wpfViewBox.Perc;
                    //exViewer.wpfViewBox.ScrollToMarker(i);
                }
                if (bigEditor != null && bigEditor.Visible) { bigEditor.wpfViewBox.ScrollToMarker(i); }
            }
            else
            {
                // MessageBox.Show("No marker found");
            }
            i = null;
        }

        public static double lMargin, rMargin;

        public static void ChangeViewerWidth(double lP, double rP)
        {
            lMargin = lP;
            rMargin = rP;
            flipViewer.ChangeViewerWidth(lP, rP);
            if (bigEditor != null && bigEditor.Visible)
            {
                bigEditor.ChangeViewerWidth(lP, rP, true);
                bigEditor.wpfViewBox.ScrollDirect(flipViewer.wpfViewBox.cpos);
            }
            if (exViewer != null)
            {
                exViewer.ChangeViewerWidth(lP, rP);
                exViewer.wpfViewBox.ScrollDirect(flipViewer.wpfViewBox.cpos);
            }
        }

        public static void LoadFonts()
        {
            if (fonts == null)
            {
                fonts = new List<string>();
            }
            else
            {
                fonts.Clear();
            }
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            FontFamily[] fontFamilies;
            fontFamilies = installedFontCollection.Families;
            int count = fontFamilies.Length;
            for (int j = 0; j < count; ++j) { fonts.Add(fontFamilies[j].Name); }
        }

        private static void SetMarkerNames(QStream stream, List<String> names, int markerType)
        {
            List<String> intMarkerNames = names;
            int nI = 0;
            if (stream.Document != null)
            {
                foreach (var block in stream.Document.Blocks)
                {
                    if (block is Paragraph)
                    {
                        Paragraph paragraph = (Paragraph)block;
                        foreach (Inline inline in paragraph.Inlines)
                        {
                            if (inline is InlineUIContainer)
                            {
                                InlineUIContainer uiContainer = (InlineUIContainer)inline;
                                if (uiContainer.Child is System.Windows.Controls.Image && nI < names.Count)
                                {
                                    int mType = GetElementRTF(uiContainer);
                                    if (markerType == mType)
                                    {
                                        inline.Name = names[nI];
                                        nI++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static int GetElementRTF(InlineUIContainer ic)
        {
            TextRange tr = new TextRange(ic.ContentStart, ic.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Rtf);
            string rtfText = ASCIIEncoding.Default.GetString(ms.ToArray());
            return (rtfText.Contains(MarkerRTF) ? 1 : (rtfText.Contains(SlideMarkerRTF)) ? 2 : 0);
        }

        private static void RaiseNewLogoEvent()
        {
            if (LogoChangedEvent != null) { LogoChangedEvent(null, new EventArgs()); }
        }

        private static void RaiseNewPlaylistEvent()
        {
            if (streamItems == null)
            {
                streamItems = new Dictionary<string, string>();
            }
            else
            {
                streamItems.Clear();
            }
            foreach (QStream qstream in playlist.Streams)
            {
                if (qstream.GUID != null && qstream.Name != null)
                {
                    streamItems.Add(qstream.GUID, qstream.Name);
                }
            }
            if (NewPlaylistEvent != null) { NewPlaylistEvent(null, new EventArgs()); }
        }
    }
}