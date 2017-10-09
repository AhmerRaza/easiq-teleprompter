using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class fmStreamProp : Form {

        #region Private Variables

        private Classes.QStream lStream = new Classes.QStream();

        #endregion Private Variables

        #region Constructor

        public fmStreamProp() {
            InitializeComponent();
        }

        #endregion Constructor

        #region Usercontrol Events

        private void fmStreamProp_Load(object sender, EventArgs e) {
        }

        #endregion Usercontrol Events

        #region Control events

        private void cmdCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmdAccept_Click(object sender, EventArgs e) {
            if (txtName.Text.Trim() != "") {
                lStream.Name = txtName.Text;
                lStream.PPTName = txtPPT.Text;
                lStream.IsPPT = chkPowerpoint.Checked;
                lStream.HideSlides = chkHide.Checked;
                //lStream.Trans = chkTrans.Checked;
                this.DialogResult = DialogResult.OK;
            }
            this.Close();
        }

        #endregion Control events

        #region Public Exposure

        public Classes.QStream Stream {
            get { return lStream; }
            set {
                lStream = value;
                txtName.Text = lStream.Name;
                txtPPT.Text = lStream.PPTName;
                chkPowerpoint.Checked = lStream.IsPPT;
                chkHide.Checked = lStream.HideSlides;
                //chkTrans.Checked = lStream.Trans;
            }
        }

        #endregion Public Exposure
    }
}