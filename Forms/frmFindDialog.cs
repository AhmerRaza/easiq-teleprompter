using System;
using System.Windows.Forms;

namespace Teleprompter {

    public partial class frmFindDialog : Form {

        public frmFindDialog() {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> FindEvent;

        public event EventHandler<EventArgs> ReplaceEvent;

        public event EventHandler<EventArgs> TextChangedEvent;

        public String findString = String.Empty;
        public String replaceString = String.Empty;
        public bool wholeWord = false;
        public bool matchCase = false;
        public bool checkUp = false;

        private void txtFindWhat_TextChanged(object sender, EventArgs e) {
            findString = txtFindWhat.Text;
            if (TextChangedEvent != null) { TextChangedEvent(this, new EventArgs()); }
        }

        private void txtReplace_TextChanged(object sender, EventArgs e) {
            replaceString = txtReplace.Text;
            if (TextChangedEvent != null) { TextChangedEvent(this, new EventArgs()); }
        }

        private void chkWholeWord_CheckedChanged(object sender, EventArgs e) {
            wholeWord = chkWholeWord.Checked;
            if (TextChangedEvent != null) { TextChangedEvent(this, new EventArgs()); }
        }

        private void chkMatchCase_CheckedChanged(object sender, EventArgs e) {
            matchCase = chkMatchCase.Checked;
            if (TextChangedEvent != null) { TextChangedEvent(this, new EventArgs()); }
        }

        private void optUp_CheckedChanged(object sender, EventArgs e) {
            if (optUp.Checked) {
                optDown.Checked = false;
                checkUp = true;
                if (TextChangedEvent != null) { TextChangedEvent(this, new EventArgs()); }
            }
        }

        private void optDown_CheckedChanged(object sender, EventArgs e) {
            if (optDown.Checked) {
                optUp.Checked = false;
                checkUp = false;
                if (TextChangedEvent != null) { TextChangedEvent(this, new EventArgs()); }
            }
        }

        private void btnFind_Click(object sender, EventArgs e) {
            if (FindEvent != null) { FindEvent(this, new EventArgs()); }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void frmFindDialog_Load(object sender, EventArgs e) {
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 50;
            this.Top = 0;
        }

        private void btnReplace_Click(object sender, EventArgs e) {
            if (ReplaceEvent != null) { ReplaceEvent(this, new EventArgs()); }
        }
    }
}