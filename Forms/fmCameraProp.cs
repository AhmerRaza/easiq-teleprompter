using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class frmCameraProp : Form {

        #region Private Variables

        private String camName = String.Empty;

        #endregion Private Variables

        #region Constructor

        public frmCameraProp() {
            InitializeComponent();
        }

        #endregion Constructor

        #region Control Events

        private void cmdAccept_Click(object sender, EventArgs e) {
            if (txtName.Text.Trim() != "") {
                camName = txtName.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion Control Events

        #region Public Exposure

        public String CameraName {
            get { return camName; }
        }

        #endregion Public Exposure

        private void frmCameraProp_Load(object sender, EventArgs e) {
            txtName.Focus();
        }
    }
}