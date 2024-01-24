using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form , rm
    {
        public static List<string> files = new List<string>();
        DataTable dataTable = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            emailcontant.Multiline = true;
            guna2TextBox1.Multiline = true;
      
        }
        public DataTable ImportExcelToDataTable(string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook excelWorkbook = excelApp.Workbooks.Open(filePath);
            Excel.Worksheet excelWorksheet = excelWorkbook.Sheets[1];
            Excel.Range excelRange = excelWorksheet.UsedRange;

            dataTable = new DataTable();

            // Adding columns to DataTable
            for (int col = 1; col <= excelRange.Columns.Count; col++)
            {
                string columnName = excelRange.Cells[1, col].Value.ToString();
                dataTable.Columns.Add(columnName);
            }

            // Adding rows to DataTable
            for (int row = 2; row <= excelRange.Rows.Count; row++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int col = 1; col <= excelRange.Columns.Count; col++)
                {
                    dataRow[col - 1] = excelRange.Cells[row, col].Value;
                }
                dataTable.Rows.Add(dataRow);
            }

            // Clean up Excel objects
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelRange);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

            return dataTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                DataTable dataTable = ImportExcelToDataTable(filePath);
                dataGridView1.DataSource = dataTable;

            }
          

        }

        private void sendemail_event_click(object sender, EventArgs e)
        {
            bool F = false;
            foreach (RadioButton item in groupchoose.Controls)
            {
                if (item.Checked && item.Text == "Filed text")
                {
                    errorProvider1.Clear();
                    if (lebel_smtp_user.Text.Length == 0)
                    {
                        errorProvider1.SetError(lebel_smtp_user , "required");
                    }
                    else if(lebel_smtp_password.Text.Length == 0)
                    {
                        errorProvider1.SetError(lebel_smtp_password, "required");
                    }
                    else if(lebel_smtp_email.Text.Length == 0)
                    {
                        errorProvider1.SetError(lebel_smtp_email, "required");
                    }
                    else
                    {
                        errorProvider1.Clear();
                        F = send(lebel_smtp_email.Text);
                        if(F)
                            notifydone(lebel_smtp_email.Text);
                    }

                }
                else if (item.Checked && item.Text == "File exel") {
                    if (txtindex.Text.Length == 0)
                    {
                        MessageBox.Show("index column invalid");
                        return;
                    }
                        
                    if (dataTable.Rows.Count != 0)
                    {
                        for (int i = 1; i < dataTable.Rows.Count; i++)
                        {
                            if (dataTable.Rows[i][Convert.ToInt32(txtindex.Text)].ToString().Length != 0)
                            {
                                send(dataTable.Rows[i][Convert.ToInt32(txtindex.Text)].ToString());
                            }
                        }
                        notifydone((dataTable.Rows.Count -1) +" email");
                    }
                    else
                    {
                        MessageBox.Show("empty list");
                    }
                }
                
        }
        }

        private void notifydone(string ml)
        {
                notifyIcon.Icon = Icon.FromHandle(Properties.Resources.icons_send_mail.GetHicon());
                notifyIcon.Text = "Notification Example";
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = "Done";
                notifyIcon.BalloonTipText = "Task completed : mail sent successfully to " + ml;

            notifyIcon.ShowBalloonTip(100);
        }
        Attachment attachment ;
        public Boolean send(string email)
        {
            try
            {
                // Set your SMTP server details
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587; // Use the appropriate port for your SMTP server
                string smtpUsername = lebel_smtp_user.Text;
                string smtpPassword = lebel_smtp_password.Text;

                // Create a new MailMessage
                MailMessage mail;
                SmtpClient smtpClient;

                mail = new MailMessage();
                mail.From = new MailAddress(email);
                mail.To.Add(email);
                mail.Subject = Subjecttxt.Text;
                // Set the HTML-formatted body
                mail.Body = emailcontant.Text;

                if (guna2CheckBox1.Checked)
                    mail.IsBodyHtml = true;
                if(attachment != null)
                {
                    foreach (string filePath in files)
                    {
                        Attachment attachment = new Attachment(filePath);
                        mail.Attachments.Add(attachment);
                    }
                }

                smtpClient = new SmtpClient(smtpServer);
                smtpClient.Port = smtpPort;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true; // Set to true if your SMTP server requires SSL

                // Send the email
                smtpClient.Send(mail);
                logstxt.Text += "Completed sending email to : " + email + "\n";
                tabControl1.SelectedIndex = 1;
                return true;
            }
            catch(Exception ex)
            {
                labelerror.Text += ex.Message;
                labelerror.ForeColor = Color.Red;
                tabControl1.SelectedIndex = 2;
                return false;
            }
           
        }
        private void guna2ImageButton1_Click(object sender, EventArgs e)
        { // Use an OpenFileDialog to allow the user to select a file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select File to Attach";
                openFileDialog.Filter = "All Files (*.*)|*.*";
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Attach the selected file
                    try
                    {
                        attachment = new Attachment(openFileDialog.FileName);
                       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    files.Add(openFileDialog.FileName);

                }
            }
            display_attchfiles();

        }
        void display_attchfiles()
        {
            // Clear existing labels in the panel
            panel_files.Controls.Clear();

            // Add labels for each file path to the panel
            foreach (string filePath in files)
            {
                fileitem label = new fileitem(this);
                label.filename = filePath;
                label.AutoSize = true;
                label.Dock = DockStyle.Top;
                panel_files.Controls.Add(label);
            }

        }
        public void removeitem(string filename)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] == filename)
                {
                    files.RemoveAt(i);
                }
            }
            display_attchfiles();
        }

        private void lebel_smtp_email_TextChanged(object sender, EventArgs e)
        {

        }

        private void emailcontant_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void emailcontant_Validating(object sender, CancelEventArgs e)
        {

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string htmlContent = guna2TextBox1.Text;

            // Display HTML content in the WebBrowser control
            webBrowser1.DocumentText = htmlContent;
        }
    }
}
