using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class fmMarkerProp : Form {

        #region Private Variables

        private Classes.Marker lMarker;
        private String autoName;

        #endregion Private Variables

        #region Constructor

        public fmMarkerProp() {
            InitializeComponent();
        }

        public fmMarkerProp(Classes.Marker marker, String autoname) {
            InitializeComponent();
            this.Marker = marker;
            autoName = autoname;
            if (!String.IsNullOrEmpty(autoName)) {
                txtName.Text = autoName;
                lMarker.Name = autoName;
                cmdAccept_Click(cmdAccept, new EventArgs());
            }
        }

        private bool newName = false;
        private int mIdx = -1;

        public fmMarkerProp(Classes.Marker marker, int markIdx, String autoname, bool rename) {
            mIdx = markIdx;
            newName = rename;
            InitializeComponent();
            this.Marker = marker;
            autoName = autoname;
            if (!String.IsNullOrEmpty(autoName)) {
                txtName.Text = autoName;
                lMarker.Name = autoName;
                //cmdAccept_Click(cmdAccept, new EventArgs());
            }
        }

        #endregion Constructor

        #region Control Events

        private void cmdAccept_Click(object sender, EventArgs e) {
            if (txtName.Text.Trim() != "") {
                lMarker.Name = txtName.Text;
                bool nameExists = false;
                for (int mi = 0; mi < Classes.Controller.playlist.ActiveStream.Markers.Count; mi++) {
                    Classes.Marker m = Classes.Controller.playlist.ActiveStream.Markers[mi];

                    if (m.Name == lMarker.Name && mi != mIdx) {
                        nameExists = true;
                        break;
                    }
                }
                if (!nameExists) {
                    this.DialogResult = DialogResult.OK;
                    Properties.Settings.Default.tempMarker = lMarker.Name;
                    Properties.Settings.Default.Save();
                    this.Close();
                } else {
                    MessageBox.Show("Marker name already exists...Please try again", "Markers", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion Control Events

        #region Public Exposure

        public Classes.Marker Marker {
            get { return lMarker; }
            set {
                lMarker = value;
                txtName.Text = lMarker.Name;
            }
        }

        #endregion Public Exposure
    }
}