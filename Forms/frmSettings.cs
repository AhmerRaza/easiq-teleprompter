using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class frmSettings : Form {

        public frmSettings() {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e) {
            cmbSize.SelectedItem = Properties.Settings.Default.defaultEditor.ToString();
            cmbBackground.SelectedItem = Properties.Settings.Default.defaultBack;
            cmbFore.SelectedItem = Properties.Settings.Default.defaultFore;
            cmbCamera.SelectedItem = Properties.Settings.Default.defaultCamera;
            LoadFonts();
        }

        private void LoadFonts() {
            if (Classes.Controller.fonts == null) { Classes.Controller.LoadFonts(); }
            cmbFont.Items.Clear();
            foreach (String font in Classes.Controller.fonts) {
                cmbFont.Items.Add(font);
            }
            try {
                cmbFont.SelectedItem = Properties.Settings.Default.defaultFont;
            } catch {
                cmbFont.SelectedItem = "Arial";
            }
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            Classes.Controller.pptServerIP = txtPPTIP.Text;
            Properties.Settings.Default.pptServer = Classes.Controller.pptServerIP;
            String msg = "";
            if (!Classes.Controller.canconnect) { Classes.Controller.canconnect = Classes.Controller.tcpClient.ConnectToServer(Classes.Controller.pptServerIP, out msg); }
            if (Classes.Controller.canconnect) {
                btnConnect.BackColor = System.Drawing.Color.Green;
            } else {
                MessageBox.Show(msg, "Powerpoint viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConnect.BackColor = System.Drawing.Color.Red;
            }
        }

        private void btnSave_Click(object sender, EventArgs e) {
            Properties.Settings.Default.defaultBack = cmbBackground.SelectedItem.ToString();
            Properties.Settings.Default.defaultFore = cmbFore.SelectedItem.ToString();
            Properties.Settings.Default.defaultCamera = cmbCamera.SelectedItem.ToString();
            Properties.Settings.Default.Save();
            if (cmbSize.SelectedItem != null) { Classes.Controller.ChangeDefaultFontSizes(int.Parse(cmbSize.SelectedItem.ToString())); }
        }

        private void cmbBackground_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbBackground.SelectedItem != null && cmbFore.SelectedItem != null) {
                if (cmbBackground.SelectedItem.ToString() == "Black" && cmbFore.SelectedItem.ToString() != "White") {
                    cmbFore.SelectedIndexChanged -= new EventHandler(cmbFore_SelectedIndexChanged);
                    cmbFore.SelectedItem = "White";
                    cmbFore.SelectedIndexChanged += new EventHandler(cmbFore_SelectedIndexChanged);
                } else if (cmbBackground.SelectedItem.ToString() == "White" && cmbFore.SelectedItem.ToString() != "Black") {
                    cmbFore.SelectedIndexChanged -= new EventHandler(cmbFore_SelectedIndexChanged);
                    cmbFore.SelectedItem = "Black";
                    cmbFore.SelectedIndexChanged += new EventHandler(cmbFore_SelectedIndexChanged);
                }
            }
        }

        private void cmbFore_SelectedIndexChanged(object sender, EventArgs e) {
            if (cmbBackground.SelectedItem != null && cmbFore.SelectedItem != null) {
                if (cmbFore.SelectedItem.ToString() == "Black" && cmbBackground.SelectedItem.ToString() != "White") {
                    cmbBackground.SelectedIndexChanged -= new EventHandler(cmbBackground_SelectedIndexChanged);
                    cmbBackground.SelectedItem = "White";
                    cmbBackground.SelectedIndexChanged += new EventHandler(cmbBackground_SelectedIndexChanged);
                } else if (cmbFore.SelectedItem.ToString() == "White" && cmbBackground.SelectedItem.ToString() != "Black") {
                    cmbBackground.SelectedIndexChanged -= new EventHandler(cmbBackground_SelectedIndexChanged);
                    cmbBackground.SelectedItem = "Black";
                    cmbBackground.SelectedIndexChanged += new EventHandler(cmbBackground_SelectedIndexChanged);
                }
            }
        }
    }
}