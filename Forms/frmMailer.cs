using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace Teleprompter.Forms {

    public partial class frmMailer : Form {

        public frmMailer() {
            InitializeComponent();
        }

        private void frmMailer_Load(object sender, EventArgs e) {
        }

        private void btnSend_Click(object sender, EventArgs e) {
            String name = txtName.Text;
            String email = txtEmail.Text;
            String period = txtPeriod.Text;
            String notes = txtNotes.Text;
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(email)) {
                String message = "New upgrade / license request received." + Environment.NewLine + Environment.NewLine;
                message += "From: " + name + Environment.NewLine;
                message += "Email address: " + email + Environment.NewLine;
                message += "Period requested: " + period + Environment.NewLine;
                message += "Other notes: " + notes + Environment.NewLine;
                String status = String.Empty;
                if (SendMail(message, email, out status)) {
                    MessageBox.Show("Request sent", "Licensing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                } else {
                    MessageBox.Show("Request error: " + status, "Licensing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else if (String.IsNullOrEmpty(name)) {
                MessageBox.Show("Please enter name", "License Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtName.Focus();
            } else if (String.IsNullOrEmpty(email)) {
                MessageBox.Show("Please enter email address", "License Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtEmail.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        public bool SendMail(String message, String ccAddress, out String status) {
            String mailBody = message;
            status = String.Empty;
            try {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage objMail = new MailMessage();
                MailAddress objMail_fromaddress = new MailAddress("info@metathought.co.za");
                MailAddress objMail_toaddress = new MailAddress("bookings@easiq.co.za");
                objMail.To.Add(objMail_toaddress);
                objMail.From = objMail_fromaddress;
                objMail.IsBodyHtml = false;
                objMail.Body = mailBody;
                objMail.Priority = MailPriority.High;
                MailAddress cc = new MailAddress(ccAddress);
                objMail.CC.Add(cc);
                smtpClient.Host = "mail.npsa.co.za";
                smtpClient.Credentials = new NetworkCredential("info@metathought.co.za", "info01");

                try {
                    objMail.Subject = "Upgrade / License Request";
                    smtpClient.Send(objMail);
                } catch (Exception ex) {
                    status = ex.Message;
                    return false;
                }
            } catch (Exception ex) {
                status = ex.Message;
                return false;
            }
            return true;
        }
    }
}