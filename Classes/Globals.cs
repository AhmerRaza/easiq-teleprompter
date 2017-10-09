using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Teleprompter.Classes {

    public class Globals {
        private static String fileLocation = String.Empty;
        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        #region Licensing

        private static bool CheckLicense() {
            //first check drive
            DriveType dType = CheckDriveType();
            String serial = (dType == DriveType.Removable ? GetUSBSerial() : GetMacAddress());
            fileLocation = (dType == DriveType.Removable ? AppDomain.CurrentDomain.BaseDirectory : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Metathought", "EasiQ", "Teleprompter"));
            String licenseFile = Path.Combine(fileLocation, "license.xml");
            bool licensed = false;
            if (!Directory.Exists(fileLocation) && dType != DriveType.Removable) {
                Directory.CreateDirectory(fileLocation);
            } else if (File.Exists(licenseFile)) {
                licensed = validateLicense(licenseFile, serial);
            }
            if (!licensed && IsOnline()) {
                Forms.frmLicense licensing = new Forms.frmLicense();
                if (licensing.ShowDialog() == DialogResult.Cancel) {
                    Application.Exit();
                    Environment.Exit(0);
                } else {
                    LicenseServer.TPLicense server = new LicenseServer.TPLicense();
                    String activationCode = licensing.LicenseCode;
                    int expDays = -1;
                    if (server.ValidateLicense(activationCode, out expDays)) {
                        DateTime expDate = DateTime.Now;
                        try {
                            expDate = DateTime.Now.AddDays(expDays);
                        } catch {
                            expDate = new DateTime(9999, 12, 31);
                        }
                        String pubKey = String.Empty;
                        String data = String.Format("{0}|{1}", serial, expDate.ToString("yyyy/MM/dd"));
                        String encrypted = Encrypt(data, out pubKey);
                        if (CreateLicenseFile(licenseFile, serial, expDate.ToString("yyyy/MM/dd"), encrypted, pubKey)) {
                            return CheckLicense();
                        }
                    } else {
                        if (MessageBox.Show("You have entered an invalid activation code! Try again?", "Licensing", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question) == DialogResult.Retry) {
                            return CheckLicense();
                        } else {
                            Application.Exit();
                            Environment.Exit(0);
                        }
                    }
                }
            } else if (!IsOnline()) {
                MessageBox.Show("Cannot activate software. Please connect your computer to the internet and try again!", "Licensing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return licensed;
        }

        private static string Encrypt(string data, out String _privateKey) {
            var rsa = new RSACryptoServiceProvider();
            _privateKey = rsa.ToXmlString(true);
            String _publicKey = rsa.ToXmlString(false);
            rsa.FromXmlString(_publicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray) {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        private static bool CreateLicenseFile(String licenseFile, String serial, String expDate, String encrypted, String pubKey) {
            try {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlNode rootNode = doc.CreateElement("root");

                XmlNode serialNode = doc.CreateElement("serial");
                serialNode.AppendChild(doc.CreateTextNode(serial));
                rootNode.AppendChild(serialNode);
                XmlNode expiryNode = doc.CreateElement("expiryDate");
                expiryNode.AppendChild(doc.CreateTextNode(expDate));
                rootNode.AppendChild(expiryNode);
                XmlNode encryptedNode = doc.CreateElement("encrypted");
                encryptedNode.AppendChild(doc.CreateTextNode(encrypted));
                rootNode.AppendChild(encryptedNode);
                XmlNode pubNode = doc.CreateElement("pub");
                pubNode.AppendChild(doc.CreateTextNode(pubKey));
                rootNode.AppendChild(pubNode);

                doc.AppendChild(rootNode);

                doc.Save(licenseFile);
                return File.Exists(licenseFile);
            } catch {
                return false;
            }
        }

        private static bool validateLicense(String licenseFile, String appSerial) {
            bool success = false;
            try {
                String serial = String.Empty;
                DateTime expiryDate = DateTime.Now.AddYears(-1);
                String encrypted = String.Empty;
                String decrypted = String.Empty;
                String pubKey = String.Empty;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(licenseFile);
                foreach (XmlNode xNode in xmlDoc.ChildNodes) {
                    if (xNode.Name == "root") {
                        foreach (XmlElement xmlElement in xNode.ChildNodes) {
                            switch (xmlElement.Name) {
                                case "serial": serial = xmlElement.InnerText; break;
                                case "expiryDate": expiryDate = (DateTime.TryParse(xmlElement.InnerText, out expiryDate) ? expiryDate : DateTime.Now.AddYears(-1)); break;
                                case "encrypted": encrypted = xmlElement.InnerText; break;
                                case "pub": pubKey = xmlElement.InnerText; break;
                            }
                        }
                    }
                }
                if (Decrypt(encrypted, pubKey, out decrypted)) {
                    String[] decryptedArray = decrypted.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (decryptedArray.Length == 2) {
                        String dSerial = decryptedArray[0];
                        DateTime dExpDate = (DateTime.TryParse(decryptedArray[1], out dExpDate) ? dExpDate : DateTime.Now.AddYears(-1));
                        if (dSerial == serial && dSerial == appSerial && dExpDate == expiryDate && dExpDate >= DateTime.Now) {
                            success = true;
                        } else {
                            MessageBox.Show("Invalid or expired license. Please enter new activation code", "Licensing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            xmlDoc = null;
                            File.Delete(licenseFile);
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Invalid license file! " + ex.Message, "Licensing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                Environment.Exit(0);
            }
            return success;
        }

        public static bool Decrypt(string data, String pubKey, out String decrypted) {
            bool success = false;
            try {
                var rsa = new RSACryptoServiceProvider();
                var dataArray = data.Split(new char[] { ',' });
                byte[] dataByte = new byte[dataArray.Length];
                for (int i = 0; i < dataArray.Length; i++) {
                    dataByte[i] = Convert.ToByte(dataArray[i]);
                }

                rsa.FromXmlString(pubKey);
                var decryptedByte = rsa.Decrypt(dataByte, false);
                decrypted = _encoder.GetString(decryptedByte);
                success = true;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
                decrypted = ex.Message;
            }
            return success;
        }

        private static DriveType CheckDriveType() {
            string path = Process.GetCurrentProcess().MainModule.FileName;
            FileInfo fileInfo = new FileInfo(path);
            string driveRoot = fileInfo.Directory.Root.Name;
            DriveInfo driveInfo = new DriveInfo(driveRoot);
            return driveInfo.DriveType;
        }

        private static String GetUSBSerial() {
            string driveLetter = Environment.SystemDirectory.Substring(0, 2);
            return new System.Management.ManagementObject("Win32_LogicalDisk.DeviceID=\"" + driveLetter + "\"").GetPropertyValue("VolumeSerialNumber").ToString();
        }

        private static string GetMacAddress() {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed && !string.IsNullOrEmpty(tempMac) && tempMac.Length >= MIN_MAC_ADDR_LENGTH) {
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                    break;
                }
            }
            return macAddress;
        }

        #endregion Licensing

        private static bool IsOnline() {
            Ping myPing = new Ping();
            String host = "metathought.buddhalounge.co.za";
            byte[] buffer = new byte[32];
            int timeout = 1000;
            PingOptions pingOptions = new PingOptions();
            PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
            if (reply.Status == IPStatus.Success) {
                return true;
            } else {
                return false;
            }
        }

        #region FileVersion

        private static void GetFileVersion() {
            if (IsOnline()) {
                using (WebClient client = new WebClient()) {
                    client.DownloadFile("http://metathought.buddhalounge.co.za/easiq/teleprompter/version.txt", Path.Combine(fileLocation, "version.txt"));
                }
                if (File.Exists(Path.Combine(fileLocation, "version.txt"))) {
                    StreamReader sr = new StreamReader(Path.Combine(fileLocation, "version.txt"));
                    String vText = sr.ReadToEnd();
                    Version version = Assembly.GetEntryAssembly().GetName().Version;
                    String assVersion = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString() + "." + version.Revision.ToString();
                    if (vText != assVersion) {
                        DialogResult updateBox = MessageBox.Show("You are not using the latest version of the software. New activation code is required to run updated software. Click yes to send request. Click no to download latest software. Click cancel to continue on current version.", "Teleprompter", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                        if (updateBox == DialogResult.Yes) {
                            Forms.frmMailer mailer = new Forms.frmMailer();
                            mailer.ShowDialog();
                        } else if (updateBox == DialogResult.No) {
                            DriveType dType = CheckDriveType();
                            fileLocation = (dType == DriveType.Removable ? AppDomain.CurrentDomain.BaseDirectory : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Metathought", "EasiQ", "Teleprompter"));
                            String licenseFile = Path.Combine(fileLocation, "license.xml");
                            File.Delete(licenseFile);
                            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloader.exe"));
                            Application.Exit();
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }

        #endregion FileVersion
    }

    #region Event Args

    public class StreamAddedArgs : EventArgs {
        private QStream lStream;

        public StreamAddedArgs(QStream stream) {
            this.lStream = stream;
        }

        public QStream Stream { get { return lStream; } }
    }

    public class StreamRemovedArgs : EventArgs {
        private string lGUID = "";

        public StreamRemovedArgs(string guid) {
            lGUID = guid;
        }

        public string GUID { get { return lGUID; } }
    }

    public class MarkerChangedArgs : EventArgs {
        private string lGUID = "";
        private bool active = true;
        private int markerType = 1;

        public MarkerChangedArgs(String guid, bool isActiveStream, int mType) {
            lGUID = guid;
            active = isActiveStream;
            markerType = mType;
        }

        public String GUID { get { return lGUID; } }

        public bool isActive { get { return active; } }

        public int MType { get { return markerType; } }
    }

    public class StreamDetailsChangedArgs : EventArgs {
        private QStream lStream;

        public StreamDetailsChangedArgs(QStream stream) {
            lStream = stream;
        }

        public QStream Stream { get { return lStream; } }
    }

    public class ActiveStreamChangedArgs : EventArgs {
        private QStream lStream;
        private QStream lOldStream;

        public ActiveStreamChangedArgs(QStream stream, QStream oldstream) {
            lStream = stream;
            lOldStream = oldstream;
        }

        public QStream Stream { get { return lStream; } }

        public QStream OldStream { get { return lOldStream; } }
    }

    public class ViewerChangedArgs : EventArgs {

        public double percentage { get; set; }

        public bool reset { get; set; }

        public bool changePos { get; set; }

        public ViewerChangedArgs(double perc, bool Reset, bool Change) {
            percentage = perc;
            reset = Reset;
            changePos = Change;
        }
    }

    public class ViewerTextChangedArgs : EventArgs {
    }

    public class StreamWordCountChangedArgs : EventArgs {
        private int lCount = 0;
        private int lWordsPerSecond;

        public StreamWordCountChangedArgs(int count, int wps) {
            lCount = count;
            lWordsPerSecond = wps;
        }

        public int Count { get { return lCount; } }

        public int WordsPerSecond { get { return lWordsPerSecond; } }
    }

    #endregion Event Args

    [Flags]
    public enum FindOptions {

        /// <summary>
        /// Perform case-insensitive non-word search.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Perform case-sensitive search.
        /// </summary>
        MatchCase = 0x00000001,

        /// <summary>
        /// Perform the search against whole word.
        /// </summary>
        MatchWholeWord = 0x00000002,
    }

    public class StreamItems {

        public String guid { get; set; }

        public String name { get; set; }
    }

    public class Controls {

        public static Image MarkerImage() {
            Image image = null;
            String thisDir = AppDomain.CurrentDomain.BaseDirectory;
            String markerImgFile = System.IO.Path.Combine(thisDir, "Images", "blue.png");// "white.png");
            BitmapImage bitmap = null;
            bitmap = new BitmapImage(new Uri(markerImgFile, UriKind.Absolute));
            if (bitmap != null) {
                image = new Image();
                image.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                image.Source = bitmap;
                image.Width = bitmap.Width;
                image.Height = bitmap.Width;
                image.Name = "img" + Guid.NewGuid().ToString().Substring(0, 8);
            }
            return image;
        }

        public static Image SlideImage() {
            Image image = null;
            String thisDir = AppDomain.CurrentDomain.BaseDirectory;
            String slideImgFile = System.IO.Path.Combine(thisDir, "Images", "blue.png");
            BitmapImage bitmap = null;
            bitmap = new BitmapImage(new Uri(slideImgFile, UriKind.Absolute));
            if (bitmap != null) {
                image = new Image();
                image.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                image.Source = bitmap;
                image.Width = bitmap.Width;
                image.Height = bitmap.Width;
            }
            return image;
        }

        public static Image CameraImage() {
            Image image = null;
            String thisDir = AppDomain.CurrentDomain.BaseDirectory;
            String cameraImgFile = "";
            BitmapImage bitmap = null;
            bitmap = new BitmapImage(new Uri(cameraImgFile, UriKind.Absolute));
            if (bitmap != null) {
                image = new Image();
                image.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                image.Source = bitmap;
                image.Width = bitmap.Width;
                image.Height = 17;
            }
            return image;
        }
    }

    public static class Extensions {

        public static string GetDataFormat(this string text) {
            // First validate the text
            if (string.IsNullOrEmpty(text)) return System.Windows.DataFormats.Text;

            // Return right data
            if (text.StartsWith(@"{\rtf")) return System.Windows.DataFormats.Rtf;

            // Return default
            return System.Windows.DataFormats.Text;
        }
    }
}