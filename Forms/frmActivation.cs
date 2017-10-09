using System;
using System.Windows.Forms;

namespace Teleprompter.Forms {

    public partial class frmActivation : Form {
        public Licensing.License myLicense;

        public frmActivation(Licensing.License license) {
            InitializeComponent();
            myLicense = license;
        }

        private void frmActivation_Load(object sender, EventArgs e) {
            if (myLicense != null) {
                String status;
                Licensing.LicenseGenerator.ValidateLicense(out myLicense, out status);
                lblStatus.Text = status;
            }
            LoadMyLicense();
        }

        private void LoadMyLicense() {
            if (myLicense != null) {
                txtLicensee.Text = myLicense.Licensee;
                txtLicensee.ReadOnly = true;
                txtIssue.Text = myLicense.IssueDate.ToString("yyyy/MM/dd");
                txtIssue.ReadOnly = true;
                txtActivation.Text = myLicense.ActivationKey;
                txtExpiry.Text = myLicense.ExpiryDate.ToString("yyyy/MM/dd");
            } else {
                txtLicensee.Text = "";
                txtIssue.Text = DateTime.Now.ToString("yyyy/MM/dd");
                txtIssue.ReadOnly = true;
                txtActivation.Text = "";
                txtExpiry.Text = "";
            }
        }

        private void btnActivate_Click(object sender, EventArgs e) {
            this.Cursor = Cursors.WaitCursor;
            String licensee = txtLicensee.Text;
            String activation = txtActivation.Text;
            if (String.IsNullOrEmpty(licensee)) {
                lblStatus.Text = "Please enter user name.";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                txtLicensee.Focus();
                return;
            }
            if (String.IsNullOrEmpty(activation)) {
                lblStatus.Text = "Please enter activation code";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                txtActivation.Focus();
                return;
            }
            String status;
            System.Net.ServicePointManager.Expect100Continue = false;
            if (Licensing.LicenseGenerator.GenerateLicense(licensee, DateTime.Now, activation, out status, out myLicense)) {
                LoadMyLicense();
                lblStatus.Text = status;
            } else {
                lblStatus.Text = status;
            }
            this.Cursor = Cursors.Arrow;
        }

        private void btnReset_Click(object sender, EventArgs e) {
            LoadMyLicense();
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();
        }
    }
}