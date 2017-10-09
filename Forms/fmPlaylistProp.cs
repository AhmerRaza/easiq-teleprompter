using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class fmPlaylistProp : Form {

        #region Private Variables

        private Classes.Playlist PList = new Classes.Playlist();

        #endregion Private Variables

        #region Constructor

        public fmPlaylistProp() {
            InitializeComponent();
        }

        #endregion Constructor

        #region Form Events

        private void fmPlaylistProp_Load(object sender, EventArgs e) {
            this.Text = "Playlist Properties";
        }

        #endregion Form Events

        #region Control Events

        private void cmdCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmdAccept_Click(object sender, EventArgs e) {
            PList.Name = txtName.Text;
            PList.Client = "";
            PList.Production = txtProduction.Text;
            PList.ProductionCompany = txtCompany.Text;
            PList.Season = "";
            PList.Episode = "";
            foreach (Classes.QStream stream in PList.Streams) { stream.Trans = chkTrans.Checked; }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion Control Events

        #region Public Exposure

        public Classes.Playlist PlayList {
            get { return PList; }
            set {
                PList = value;
                txtName.Text = PList.Name;
                txtProduction.Text = PList.Production;
                txtCompany.Text = PList.ProductionCompany;
                bool trans = false;
                foreach (Classes.QStream stream in PList.Streams) {
                    if (stream.Trans) {
                        trans = true;
                        break;
                    }
                }
                chkTrans.Checked = trans;
            }
        }

        #endregion Public Exposure
    }
}