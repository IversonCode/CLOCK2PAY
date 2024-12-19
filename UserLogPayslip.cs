using AForge.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLOCK2PAY
{
    public partial class UserLogPayslip : Form
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls

        private bool isImageUploaded = false; // Tracks if an image is uploaded


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,        // x-coordinate of upper-left corner
        int nTopRect,         // y-coordinate of upper-left corner
        int nRightRect,       // x-coordinate of lower-right corner
        int nBottomRect,      // y-coordinate of lower-right corner
        int nWidthEllipse,    // width of ellipse
        int nHeightEllipse  // height of ellipse
            );


        public UserLogPayslip()
        {
            InitializeComponent();

            // Set the form border style to none
            this.FormBorderStyle = FormBorderStyle.None;

            // Create a rounded region for the form
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 80, 80));




        }

        public void SetUserData(string idno, string firstname, string middlename, string lastname, string email, string contactNo,string desig, byte[] pictureData)
        {
            label4.Text = $"Name: {firstname} {middlename} {lastname}"; // Combine Firstname and Lastname
            label2.Text = "Contact No: " + contactNo;
            label3.Text = "ID: " + idno; // Use + for concatenation
            label6.Text = "Email: " + email;
            label5.Text = "Designation: " + desig;

            // Convert byte[] to Image and assign it to the PictureBox
            if (pictureData != null && pictureData.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(pictureData))
                {
                    pb_image.Image = Image.FromStream(ms);
                }
            }
            else
            {
                pb_image.Image = null; // Optionally set a default image or leave it blank
            }
        }

        private void LoadDataForUser(string userId)
        {
            // Create the connection to your Access database
            string connectionString = "Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb";
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                // Prepare the SQL query to get data for the specific user
                string query = "SELECT * FROM TIME_IN_OUT WHERE ID = ?";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("?", userId);

                // Create a DataTable to hold the data
                DataTable dt = new DataTable();
                conn.Open();
                adapter.Fill(dt);
                conn.Close();

                // Bind the DataTable to the DataGridView
                dataGridView1.DataSource = dt;
            }
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

        private void UserLogPayslip_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

            GetUsers();

            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["ID"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Lastname"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Middlename"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Firstname"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Designation"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_In_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_Out_Pic"].Visible = false; // Hide the Photo column

            dataGridView1.Columns["Basic_Rate"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["SSS"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Pag-Ibig"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Phil-Health"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Deduction"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Total"].Visible = false; // Hide the Photo column
            // Set the format for the Basic_Salary column to display the Peso sign
            dataGridView1.Columns["Gross_Pay"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed
            dataGridView1.Columns["In_Schedule"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Out_Schedule"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Time_In"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Time_Out"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
           


            // Optionally load data for the current user when the form loads
            string userId = label3.Text.Replace("ID: ", ""); // Get ID from label3
            LoadDataForUser(userId);

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

        

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicks Yes, hide the form
            if (result == DialogResult.Yes)
            {
                this.Hide();
            }
            // Optionally, if the user clicks No, you can do nothing or show a message
            else
            {
                // Do nothing or show a cancellation message if needed
            }
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Gray;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.FromArgb(8, 26, 60);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // Check if the DataTable is initialized
            if (dt != null)
            {
                // Get the selected date from the DateTimePicker
                DateTime selectedDate = dateTimePicker1.Value.Date;

                // Create a DataView to filter the DataTable
                DataView dv = new DataView(dt);

                // Filter rows based on the selected date and the C_Date column
                // Assuming C_Date is of DateTime type in the database
                dv.RowFilter = string.Format("C_Date = #{0}#", selectedDate.ToString("MM/dd/yyyy"));

                // Update the DataGridView with the filtered data
                dataGridView1.DataSource = dv;
            }
        }
    }
    
}
