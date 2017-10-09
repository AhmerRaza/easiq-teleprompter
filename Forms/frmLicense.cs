using System;
using System.Windows.Forms;

namespace Teleprompter.Forms {

    public partial class frmLicense : Form {
        public String LicenseCode;

        public frmLicense() {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, EventArgs e) {
            LicenseCode = txtCode.Text;
            if (!String.IsNullOrEmpty(LicenseCode)) {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            } else {
                if (MessageBox.Show("Please enter an activation code", "Licensing", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel) {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private void frmLicense_Load(object sender, EventArgs e) {
            LicenseCode = String.Empty;
        }
    }
}