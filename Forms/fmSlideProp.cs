using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class fmSlideProp : Form {

        #region Private Variables

        private Classes.Marker lMarker;
        private Classes.MarkerCollection markers;

        #endregion Private Variables

        #region Constructor

        public fmSlideProp() {
            InitializeComponent();
        }

        public fmSlideProp(Classes.Marker marker, Classes.MarkerCollection sMarkers, String autoName) {
            InitializeComponent();
            this.Marker = marker;
            markers = sMarkers;
            if (!String.IsNullOrEmpty(autoName)) {
                txtName.Text = autoName;
                lMarker.Name = autoName;
                cmdAccept_Click(cmdAccept, new EventArgs());
            }
        }

        #endregion Constructor

        #region Control Events

        private void cmdAccept_Click(object sender, EventArgs e) {
            int sNo = 0;
            if (txtName.Text.Trim() != "" && int.TryParse(txtName.Text.Trim(), out sNo)) {
                lMarker.Name = "s" + txtName.Text;
                if (markers.Count > 0) {
                    foreach (Classes.Marker m in markers) {
                        if (lMarker.Name == m.Name) {
                            MessageBox.Show("Marker already exists", "Slide Markers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        } else {
                            this.DialogResult = DialogResult.OK;
                            Properties.Settings.Default.tempMarker = lMarker.Name;
                            Properties.Settings.Default.Save();
                            this.Close();
                        }
                    }
                } else {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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