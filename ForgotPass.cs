using System;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.ComponentModel;
using AForge.Controls;
using System.Drawing;

namespace CLOCK2PAY
{
    public partial class ForgotPass : Form
    {
        // Define your connection string here
        private string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=CLOCK2PAYDB.accdb"; // Replace with your actual connection string

        public ForgotPass()
        {
            InitializeComponent();

        }

        private void ForgotPass_Load(object sender, EventArgs e)
        {
            // Load logic if necessary
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private string GetEmailById(string id)
        {
            string email = null;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Email FROM MasterList WHERE ID = @ID";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        email = result.ToString();
                    }
                }
            }

            return email;
        }

        private string GetPasswordById(string id)
        {
            string password = null;

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT [Password] FROM MasterList WHERE ID = @ID";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        password = result.ToString();
                    }
                }
            }

            return password;
        }

        private void button1_Click(object sender, EventArgs e)
        {


            string userId = textBoxUserId.Text; // Assuming textBoxUserId is where the ID is input
            string email = GetEmailById(userId); // Get email associated with the ID
            string password = GetPasswordById(userId); // Get password associated with the ID

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // Create an instance of the WaitingNotif form
                WaitingNotif waitingNotif = new WaitingNotif
                {
                    StartPosition = FormStartPosition.CenterScreen
                };

                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += (s, args) =>
                {
                    // Try to send the email in the background
                    SendEmail(email, password); // Send the password to the user's email
                };
                bgWorker.RunWorkerCompleted += (s, args) =>
                {
                    // Close the WaitingNotif form once the task is completed
                    waitingNotif.Close();

                    if (args.Error != null)
                    {
                        MessageBox.Show("Failed to send email: " + args.Error.Message);
                    }
                    else
                    {
                        MessageBox.Show("Password sent to your email successfully!");
                        this.Hide();
                    }
                };

                // Show the WaitingNotif form and start the BackgroundWorker
                waitingNotif.Show();
                bgWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("No user found with this ID.");
            }


        }

        private void SendEmail(string recipientEmail, string password)
        {
            // Configure the email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("iverson.longa@gmail.com"); // your email
            mail.To.Add(recipientEmail);
            mail.Subject = "Your Password";
            mail.Body = $"Thank You for Waiting,Your password is: {password}"; // password in the email body

            // SMTP client configuration
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Update SMTP settings
            smtpClient.Credentials = new NetworkCredential("iverson.longa@gmail.com", "ytsr thza ueag jhst"); //  your credentials
            smtpClient.EnableSsl = true; // Use SSL

            smtpClient.Send(mail);
        }

        private void textBoxUserId_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox6_MouseEnter(object sender, EventArgs e)
        {
            pictureBox6.BackColor = Color.Gray;
        }

        private void pictureBox6_MouseLeave(object sender, EventArgs e)
        {
            pictureBox6.BackColor = Color.DarkSlateGray;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Gray;
            button1.Font = new Font(button1.Font.FontFamily, 8);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button1.Font = new Font(button1.Font.FontFamily, 12);
        }

        private void pictureBox6_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }

        
    }
}
