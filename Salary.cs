using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace CLOCK2PAY
{
    public partial class Salary : UserControl
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls


        // Declare PrintDocument, PrintDialog, and PrintPreviewDialog
        private PrintDocument printDoc;
        private PrintDialog printDialog;
        private PrintPreviewDialog previewDialog;

        public Salary()
        {
            InitializeComponent();

            // Initialize PrintDocument and PrintDialog
            printDoc = new PrintDocument();
            printDialog = new PrintDialog();
            previewDialog = new PrintPreviewDialog();

            // Set the PrintPage event handler to define the printing logic
            printDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);


        }


        // Event handler for the PrintPage event
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Create a Bitmap of the entire Panel1
            Bitmap panelBitmap = new Bitmap(panel1.Width, panel1.Height);

            // Render the content of Panel1 into the Bitmap
            panel1.DrawToBitmap(panelBitmap, new Rectangle(0, 0, panel1.Width, panel1.Height));

            // Get the margin bounds (the printable area on the page)
            Rectangle printArea = e.MarginBounds;

            // Draw the Panel1 content (Bitmap) on the printed page
            e.Graphics.DrawImage(panelBitmap, printArea);
        }

        // Button click event for printing (without preview)
        private void printButtonDirect_Click(object sender, EventArgs e)
        {
            // Set the PrintDocument to be used by the PrintDialog
            printDialog.Document = printDoc;

            // Show the PrintDialog to allow the user to select the printer
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Start the printing process
                printDoc.Print();
            }
        }

        private void Printbtn_Click(object sender, EventArgs e)
        {
           // Set the PrintDocument to be used by the PrintPreviewDialog
           
            previewDialog.Document = printDoc;

            // Show the PrintPreviewDialog for the user to preview the print
            previewDialog.ShowDialog();
        }

        void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            // Modify SQL to only select rows where Resign_Date is NULL
            adapter = new OleDbDataAdapter("SELECT * FROM TIME_IN_OUT", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void Salary_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

            GetUsers();

            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_In_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_Out_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_In"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Time_Out"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["In_Schedule"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Out_Schedule"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Basic_Rate"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["Gross_Pay"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["SSS"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["Pag-Ibig"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["Phil-Health"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["Deduction"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["Total"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed


            // Set custom column header styles
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(6, 28, 58);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold); // Set font style

            // Set custom column header styles
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(6, 28, 58);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold); // Set font style

            // Optionally set the height for headers
            dataGridView1.ColumnHeadersHeight = 25;



            // Set alternating row colors for all rows
            dataGridView1.RowsDefaultCellStyle.BackColor = Color.FromArgb(180, 199, 231);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            // Change the background color for selected rows
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Gray;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

        }

        private void ApplyFilters()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                // Get filters from DateTimePickers
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date;

                string dateFilter = string.Format("C_Date >= #{0}# AND C_Date <= #{1}#",
                                                   startDate.ToString("yyyy-MM-dd"),
                                                   endDate.ToString("yyyy-MM-dd"));

                // Get filter from TextBox (ID filter)
                string idFilter = string.IsNullOrEmpty(textBox2.Text)
                    ? ""
                    : string.Format("Convert(ID, 'System.String') LIKE '%{0}%'", textBox2.Text);

                // Combine filters
                string combinedFilter = dateFilter;
                if (!string.IsNullOrEmpty(idFilter))
                {
                    combinedFilter += " AND " + idFilter;
                }

                // Apply the combined filter
                dt.DefaultView.RowFilter = combinedFilter;

                // Update labels for DateTimePickers
                label7.Text = "From: " + startDate.ToString("dd MMMM yyyy");
                label17.Text = "To: " + endDate.ToString("dd MMMM yyyy");

                // Update Employee ID Label
                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    label8.Text = "Employee ID: " + textBox2.Text;
                }
                else
                {
                    label8.Text = "Employee ID:";
                }

                // Update Designation, Basic Rate, and Full Name
                UpdateDesignationAndBasicRate();
                UpdateFullName();

                // Recalculate and update the labels
                UpdateCalculatedLabels();
            }
        }

        private void UpdateDesignationAndBasicRate()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                string designation = "";
                decimal basicRate = 0;

                // Iterate over the visible rows and get the Designation and Basic Rate
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Visible)
                    {
                        // Get the first non-empty designation and basic rate from the filtered rows
                        if (row.Cells["Designation"].Value != null && string.IsNullOrEmpty(designation))
                        {
                            designation = row.Cells["Designation"].Value.ToString();
                        }

                        if (row.Cells["Basic_Rate"].Value != null && basicRate == 0)
                        {
                            // Assuming Basic_Rate is a currency or number field
                            decimal.TryParse(row.Cells["Basic_Rate"].Value.ToString(), out basicRate);
                        }
                    }
                }

                // Set the Designation and Basic Rate to the respective labels
                label9.Text = "Designation: " + designation;
                label10.Text = "Basic Rate: " + string.Format("₱{0:#,0.00}", basicRate);
            }
        }

        // New method to update Full Name in label18
        private void UpdateFullName()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Visible)
                    {
                        // Get Lastname, Firstname, and Middlename from the first visible row
                        string lastname = row.Cells["Lastname"]?.Value?.ToString();
                        string firstname = row.Cells["Firstname"]?.Value?.ToString();
                        string middlename = row.Cells["Middlename"]?.Value?.ToString();

                        // Combine them into a single string
                        label18.Text = $"Employee: {lastname} {firstname} {middlename}".Trim();


                        // Exit after setting the name (use only the first matching row)
                        return;
                    }
                }

                // If no rows are visible, clear the label
                label18.Text = "Employee Name:";
            }
        }

        private void UpdateCalculatedLabels()
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                // Initialize totals for sums
                double totalWorkHours = 0;
                decimal totalGrossPay = 0, totalSSS = 0, totalPagIbig = 0, totalPhilHealth = 0, totalDeduction = 0, totalNetTotal = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Visible)
                    {
                        // Work_Hours
                        if (row.Cells["Worked_Hours"].Value != null && double.TryParse(row.Cells["Worked_Hours"].Value.ToString(), out double workHours))
                            totalWorkHours += workHours;

                        // Gross_Pay
                        if (row.Cells["Gross_Pay"].Value != null && decimal.TryParse(row.Cells["Gross_Pay"].Value.ToString(), out decimal grossPay))
                            totalGrossPay += grossPay;

                        // SSS
                        if (row.Cells["SSS"].Value != null && decimal.TryParse(row.Cells["SSS"].Value.ToString(), out decimal sss))
                            totalSSS += sss;

                        // Pag-Ibig
                        if (row.Cells["Pag-Ibig"].Value != null && decimal.TryParse(row.Cells["Pag-Ibig"].Value.ToString(), out decimal pagIbig))
                            totalPagIbig += pagIbig;

                        // Phil-Health
                        if (row.Cells["Phil-Health"].Value != null && decimal.TryParse(row.Cells["Phil-Health"].Value.ToString(), out decimal philHealth))
                            totalPhilHealth += philHealth;

                        // Deduction
                        if (row.Cells["Deduction"].Value != null && decimal.TryParse(row.Cells["Deduction"].Value.ToString(), out decimal deduction))
                            totalDeduction += deduction;

                        // Net Total
                        if (row.Cells["Total"].Value != null && decimal.TryParse(row.Cells["Total"].Value.ToString(), out decimal netTotal))
                            totalNetTotal += netTotal;
                    }
                }

                // Set the calculated values to the respective labels with formatting
                label11.Text = "Worked_Hours: " + totalWorkHours;
                label12.Text = "Gross_Pay: " + string.Format("₱{0:#,0.00}", totalGrossPay);
                label13.Text = "SSS: " + string.Format("₱{0:#,0.00}", totalSSS);
                label14.Text = "Pag-Ibig: " + string.Format("₱{0:#,0.00}", totalPagIbig);
                label15.Text = "Phil-Health: " + string.Format("₱{0:#,0.00}", totalPhilHealth);
                label16.Text = "Deduction: " + string.Format("₱{0:#,0.00}", totalDeduction);
                label19.Text = "Total: " + string.Format("₱{0:#,0.00}", totalNetTotal);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Check if textBox2 has any value (it's not empty)
            if (!string.IsNullOrEmpty(textBox2.Text))
            {

                // If textBox2 is empty, enable the buttons again
                btnSendEmailAll.Enabled = false;
                PrintbtnAll.Enabled = false;
                label26.Visible = false;
                label25.Visible = false;
                btnSendEmailAll.Visible = false;
                PrintbtnAll.Visible = false;
                // You can add any additional logic here to execute when textBox2 has a value.
                // For example, if you want to trigger some other action:
                // ExecuteSomeFunction();
            }
            else
            {

                label26.Visible = true;
                label25.Visible = true;
                // If there's a value, disable the buttons
                btnSendEmailAll.Enabled = true;
                PrintbtnAll.Enabled = true;

                btnSendEmailAll.Visible = true;
                PrintbtnAll.Visible= true;
                
            }

            ApplyFilters();

            // Additional logic to update personal details and calculations
            UpdateCalculatedLabels();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null) // Check if there is a selected row
            {
                var pictureCell = dataGridView1.CurrentRow.Cells["Picture"]; // Get the cell
                if (pictureCell.Value != DBNull.Value) // Check if the cell value is not DBNull
                {
                    byte[] imgData = (byte[])pictureCell.Value; // Cast the value to byte array
                    using (MemoryStream ms = new MemoryStream(imgData)) // Use a MemoryStream to hold the image data
                    {
                        pictureBox4.Image = System.Drawing.Image.FromStream(ms); // Set the pictureBox image
                    }
                }
                else
                {
                    pictureBox4.Image = null; // Clear the pictureBox image if no data
                }
            }
            else
            {
                pictureBox4.Image = null; // Clear the pictureBox if no current row is selected
            }
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            // Fetch the employee email based on the ID
            string employeeId = textBox2.Text; // Assuming textBox2 contains the Employee ID
            string employeeEmail = GetEmployeeEmailById(employeeId);

            if (string.IsNullOrEmpty(employeeEmail))
            {
                MessageBox.Show("No email found for the given Employee ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Step 1: Capture the print content as an image
            Bitmap panelBitmap = new Bitmap(panel1.Width, panel1.Height);
            panel1.DrawToBitmap(panelBitmap, new Rectangle(0, 0, panel1.Width, panel1.Height));

            // Convert the image to a MemoryStream
            MemoryStream ms = new MemoryStream();
            panelBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin); // Reset the stream position

            // Create an instance of the WaitingNotif form
            WaitingNotif waitingNotif = new WaitingNotif
            {
                StartPosition = FormStartPosition.CenterScreen
            };

            BackgroundWorker bgWorker = new BackgroundWorker();

            // Do the email-sending task in the background
            bgWorker.DoWork += (s, args) =>
            {
                // Compose and send the email
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("iverson.longa@gmail.com"); // Replace with your email
                mail.To.Add(employeeEmail);
                mail.Subject = "Your Salary Details";
                mail.Body = "Please find your salary details attached.";

                // Attach the image as a file
                mail.Attachments.Add(new Attachment(ms, "SalaryDetails.png", "image/png"));

                // Configure the SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Replace with your SMTP server
                smtpClient.Credentials = new System.Net.NetworkCredential("iverson.longa@gmail.com", "ytsr thza ueag jhst"); // Replace with your email credentials
                smtpClient.EnableSsl = true;

                smtpClient.Send(mail); // Send the email
            };

            // Handle completion of the background task
            bgWorker.RunWorkerCompleted += (s, args) =>
            {
                // Close the WaitingNotif form once the task is completed
                waitingNotif.Close();

                if (args.Error != null)
                {
                    MessageBox.Show("Failed to send email: " + args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Call the method to insert the payslip record into the database
                    InsertPayslipRecord(employeeId, ms);

                    // Delete all visible employee records from the database
                    DeleteVisibleEmployeeRecords();

                    // Collect rows to remove
                    List<DataGridViewRow> rowsToRemove = new List<DataGridViewRow>();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Check if the row is visible
                        if (row.Visible)
                        {
                            rowsToRemove.Add(row);
                        }
                    }

                    // Remove collected rows
                    foreach (var row in rowsToRemove)
                    {
                        dataGridView1.Rows.Remove(row);
                    }

                    // Confirm the action
                    MessageBox.Show("Payslip saved, email sent, and records deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            // Show the WaitingNotif form and start the background task
            waitingNotif.Show();
            bgWorker.RunWorkerAsync();
        }

        private void InsertPayslipRecord(string employeeId, MemoryStream payslipImage)
        {
            // Connect to the database
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                string query = "INSERT INTO PaySlipRecord (ID, C_Date, Payslip) VALUES (?, ?, ?)"; // Use ? for parameters

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameters with explicit types
                    cmd.Parameters.Add("ID", OleDbType.VarChar).Value = employeeId; // Use VarChar for employee ID (string)
                    cmd.Parameters.Add("C_Date", OleDbType.Date).Value = DateTime.Today; // Use Date for the current date
                    cmd.Parameters.Add("Payslip", OleDbType.Binary).Value = payslipImage.ToArray(); // Use Binary for the payslip image as a byte array

                    // Open the connection and execute the query
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteVisibleEmployeeRecords()
        {
            // Connect to the database
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                conn.Open();

                // Prepare a command to delete records
                string query = "DELETE FROM TIME_IN_OUT WHERE ID = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Iterate through all rows in the DataGridView
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Check if the row is visible
                        if (row.Visible)
                        {
                            // Get the employee ID from the row
                            string employeeId = row.Cells["ID"].Value.ToString();

                            // Add parameter for employee ID
                            cmd.Parameters.Clear(); // Clear previous parameters
                            cmd.Parameters.Add("ID", OleDbType.VarChar).Value = employeeId; // Use VarChar for employee ID (string)

                            // Execute the delete command
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private string GetEmployeeEmailById(string employeeId)
        {
            string email = null;

            // Debugging: log the employeeId for which the email is being fetched
            Console.WriteLine($"Fetching email for Employee ID: {employeeId}");

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // Use ? instead of @ID for OleDb
                string query = "SELECT Email FROM MasterList WHERE ID = ?";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add the parameter without the @ symbol
                    cmd.Parameters.AddWithValue("?", employeeId);
                    conn.Open();

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        email = result.ToString();
                        Console.WriteLine($"Email found: {email}");
                    }
                    else
                    {
                        // Log if no email is found for the given ID
                        Console.WriteLine($"No email found for Employee ID: {employeeId}");
                    }
                }
            }

            return email;
        }

        private void btnSendEmail_MouseEnter(object sender, EventArgs e)
        {
            btnSendEmail.BackColor = Color.Gray;
        }

        private void btnSendEmail_MouseLeave(object sender, EventArgs e)
        {
            btnSendEmail.BackColor = Color.White;
        }

        private void Printbtn_MouseEnter(object sender, EventArgs e)
        {
            Printbtn.BackColor = Color.Gray;
        }

        private void Printbtn_MouseLeave(object sender, EventArgs e)
        {
            Printbtn.BackColor = Color.White;
        }

        private void btnSendEmailAll_Click(object sender, EventArgs e)
        {
            // Initialize the background worker
            BackgroundWorker bgWorker = new BackgroundWorker();
            WaitingNotif waitingNotif = new WaitingNotif
            {
                StartPosition = FormStartPosition.CenterScreen
            };

            bgWorker.DoWork += (s, args) =>
            {
                List<string> failedEmails = new List<string>();
                HashSet<string> processedIds = new HashSet<string>(); // HashSet to track processed IDs
                List<string> idsToRemove = new List<string>(); // List to track IDs for removal

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    // Only process visible rows
                    if (row.Visible)
                    {
                        string employeeId = row.Cells["ID"]?.Value?.ToString();

                        // Skip if the email for this employee ID has already been sent
                        if (processedIds.Contains(employeeId))
                        {
                            continue;
                        }

                        // Fetch the email using the existing GetEmployeeEmailById method
                        string employeeEmail = GetEmployeeEmailById(employeeId);

                        // Skip if no email is found
                        if (string.IsNullOrEmpty(employeeEmail))
                        {
                            failedEmails.Add($"ID: {employeeId} (No Email Found)");
                            continue;
                        }

                        try
                        {
                            // Set the textbox2 with the current employee's ID
                            if (textBox2.InvokeRequired)
                            {
                                textBox2.Invoke((MethodInvoker)delegate
                                {
                                    textBox2.Text = employeeId; // Simulate input for the current ID
                                });
                            }
                            else
                            {
                                textBox2.Text = employeeId; // Simulate input for the current ID
                            }

                            // Call ApplyFilters to filter the DataGridView based on the current employee ID
                            ApplyFilters(); // This should filter based on textbox2

                            // Update calculated labels based on the filtered employee
                            UpdateCalculatedLabels(); // Ensure this method updates the UI based on the current employee

                            // Capture panel1 as Bitmap (Ensure cross-thread safety)
                            Bitmap panelBitmap = null;
                            if (panel1.InvokeRequired)
                            {
                                panel1.Invoke((MethodInvoker)delegate
                                {
                                    panelBitmap = new Bitmap(panel1.Width, panel1.Height);
                                    panel1.DrawToBitmap(panelBitmap, new Rectangle(0, 0, panel1.Width, panel1.Height));
                                });
                            }
                            else
                            {
                                panelBitmap = new Bitmap(panel1.Width, panel1.Height);
                                panel1.DrawToBitmap(panelBitmap, new Rectangle(0, 0, panel1.Width, panel1.Height));
                            }

                            using (MemoryStream ms = new MemoryStream())
                            {
                                panelBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Seek(0, SeekOrigin.Begin);

                                using (MailMessage mail = new MailMessage())
                                {
                                    mail.From = new MailAddress("iverson.longa@gmail.com");
                                    mail.To.Add(employeeEmail);
                                    mail.Subject = "Your Salary Details";
                                    mail.Body = "Please find your salary details attached.";
                                    mail.Attachments.Add(new Attachment(ms, "SalaryDetails.png", "image/png"));

                                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                                    {
                                        smtpClient.Credentials = new NetworkCredential("iverson.longa@gmail.com", "ytsr thza ueag jhst");
                                        smtpClient.EnableSsl = true;

                                        try
                                        {
                                            smtpClient.Send(mail); // Send the email
                                                                   // Log successful emails
                                            Console.WriteLine($"Email sent to: {employeeId}");

                                            // Insert the payslip record into the database
                                            InsertPayslipRecord(employeeId, new MemoryStream(ms.ToArray())); // Create a new Memory Stream for the payslip record

                                            // Mark the employee ID as processed
                                            processedIds.Add(employeeId); // Add the employee ID to the processed list
                                            idsToRemove.Add(employeeId); // Add the employee ID to the list for removal
                                        }
                                        catch (Exception ex)
                                        {
                                            // Log the specific error for this ID
                                            failedEmails.Add($"ID: {employeeId} (Failed to Send - {ex.Message})");
                                            Console.WriteLine($"Failed to send email to {employeeId}: {ex.Message}");
                                        }
                                    }
                                }
                            }

                            // Reset textBox2 to allow processing of the next ID
                            if (textBox2.InvokeRequired)
                            {
                                textBox2.Invoke((MethodInvoker)delegate
                                {
                                    textBox2.Text = string.Empty; // Clear the textbox for the next iteration
                                });
                            }
                            else
                            {
                                textBox2.Text = string.Empty; // Clear the textbox for the next iteration
                            }

                            // Optionally, add a small delay to avoid rate-limiting issues (adjust as needed)
                            System.Threading.Thread.Sleep(1000); // 1-second delay
                        }
                        catch (Exception ex)
                        {
                            // Log any general exceptions that occur during the process
                            failedEmails.Add($"ID: {employeeId} (Error - {ex.Message})");
                            Console.WriteLine($"Error processing {employeeId}: {ex.Message}");
                        }
                    }
                }

                // After processing all emails, delete all visible records
                DeleteVisibleEmployeeRecords(idsToRemove);

                args.Result = new { FailedEmails = failedEmails }; // Pass failed emails back to the RunWorkerCompleted handler
            };

            bgWorker.RunWorkerCompleted += (s, args) =>
            {
                // Close the waiting notification
                waitingNotif.Close();

                if (args.Error != null)
                {
                    MessageBox.Show("An error occurred: " + args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var result = args.Result as dynamic;
                    List<string> failedEmails = result.FailedEmails;

                    if (failedEmails.Count > 0)
                    {
                        string message = "Emails could not be sent to the following:\n" + string.Join("\n", failedEmails);
                        MessageBox.Show(message, "Failed Emails", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("All emails sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        GetUsers();
                    }
                }
            };

            // Show the waiting notification and start the background worker
            waitingNotif.Show();
            bgWorker.RunWorkerAsync();
        }

        private void DeleteVisibleEmployeeRecords(List<string> idsToRemove)
        {
            // Connect to the database
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                conn.Open();

                // Prepare a command to delete records
                string query = "DELETE FROM TIME_IN_OUT WHERE ID = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Iterate through the IDs to remove
                    foreach (string employeeId in idsToRemove)
                    {
                        // Add parameter for employee ID
                        cmd.Parameters.Clear(); // Clear previous parameters
                        cmd.Parameters.Add("ID", OleDbType.VarChar).Value = employeeId; // Use VarChar for employee ID (string)

                        // Execute the delete command
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


    }
}
