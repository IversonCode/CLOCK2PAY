using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using System.Globalization;
using System.Drawing.Imaging; // For working with images

namespace CLOCK2PAY
{
    public partial class Mupdate : Form
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
        public Mupdate()
        {
            InitializeComponent();

            // Set the form border style to none
            this.FormBorderStyle = FormBorderStyle.None;

            // Create a rounded region for the form
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 80, 80));


        }

        void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM MasterList WHERE Resign_Date IS NULL", conn);
            conn.Open();
            adapter.Fill(dt);
            dgvUser.DataSource = dt;
            conn.Close();

        }



        private void Mupdate_Load(object sender, EventArgs e)
        {
            GetUsers();

            // Set the DateTimePicker to show only time
            dtpIN.Format = DateTimePickerFormat.Custom;
            dtpIN.CustomFormat = "hh:mm tt"; // For 12-hour format with AM/PM
                                             // dateTimePicker1.CustomFormat = "HH:mm"; // For 24-hour format

            dtpIN.ShowUpDown = true; // Use up/down arrows to pick time

            // Set the DateTimePicker to show only time
            dtpOUT.Format = DateTimePickerFormat.Custom;
            dtpOUT.CustomFormat = "hh:mm tt"; // For 12-hour format with AM/PM
                                              // dateTimePicker1.CustomFormat = "HH:mm"; // For 24-hour format

            dtpOUT.ShowUpDown = true; // Use up/down arrows to pick time



            dgvUser.Columns["Password"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Picture"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Count"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Firstname"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Middlename"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Lastname"].Visible = false; // Hide the Photo column
            dgvUser.Columns["BirthDate"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Address"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Gender"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Email"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Contact_No"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Designation"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Hired_Date"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Resign_Date"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Basic_Rate"].Visible = false; // Hide the Photo column
            dgvUser.Columns["E_Status"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Suffix"].Visible = false; // Hide the Photo column
            dgvUser.Columns["In_Schedule"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Out_Schedule"].Visible = false; // Hide the Photo column
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dgvUser.CurrentRow == null || dgvUser.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show("Please select a row first.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if ID already exists for a different record
            string checkQuery = "SELECT COUNT(*) FROM MasterList WHERE ID = @id AND Count <> @ct AND Resign_Date IS NULL";
            OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@id", tbid.Text);
            checkCmd.Parameters.AddWithValue("@ct", Convert.ToInt32(dgvUser.CurrentRow.Cells[0].Value));

            conn.Open();
            int idCount = (int)checkCmd.ExecuteScalar();
            if (idCount > 0)
            {
                MessageBox.Show("The ID you entered is already taken by another record. Please choose a different ID.",
                                "ID Already Exists",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                conn.Close();
                isUpdating = false; // Reset flag if update is canceled
                return;
            }
            conn.Close(); // Close connection after check

            // Check for required fields
            if (tbpass.Text == "Password" || tbfn.Text == "Firstname" || tbln.Text == "Lastname" || cbsf.Text == "Suffix" || tbcn.Text == "Contact No" || cbgn.Text == "Gender" || tbem.Text == "Email" || tbdsnt.Text == "Designation" || tbadd.Text == "Address" || tbid.Text == "ID" || tbsr.Text == "B.Rate" || tbmn.Text == "Middlename" ||
               string.IsNullOrWhiteSpace(tbpass.Text) || string.IsNullOrWhiteSpace(tbfn.Text) ||
               string.IsNullOrWhiteSpace(tbln.Text) || string.IsNullOrWhiteSpace(cbsf.Text) ||
               string.IsNullOrWhiteSpace(tbcn.Text) || string.IsNullOrWhiteSpace(cbgn.Text) ||
               string.IsNullOrWhiteSpace(tbem.Text) || string.IsNullOrWhiteSpace(tbdsnt.Text) ||
               string.IsNullOrWhiteSpace(tbadd.Text) || string.IsNullOrWhiteSpace(tbid.Text) ||
               string.IsNullOrWhiteSpace(tbsr.Text) || string.IsNullOrWhiteSpace(tbmn.Text) ||
               pb_image.Image == null || dtphire.Value == null || DTbd.Value == null)
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            isUpdating = true; // Set flag to prevent SelectionChanged event execution

            // Update query
            string query = isImageUploaded
                ? "UPDATE MasterList SET ID=@id, [Password]=@ps, Lastname=@ls, Middlename=@mn, Firstname=@fn, Suffix=@sf, BirthDate=@bd, Address=@add, Gender=@gd, Email=@em, Contact_No=@cn, Designation=@dst, Hired_Date=@hd, Basic_Rate=@br, E_Status=@es, Picture=@i, In_Schedule=@inSchedule, Out_Schedule=@outSchedule WHERE Count=@ct"
                : "UPDATE MasterList SET ID=@id, [Password]=@ps, Lastname=@ls, Middlename=@mn, Firstname=@fn, Suffix=@sf, BirthDate=@bd, Address=@add, Gender=@gd, Email=@em, Contact_No=@cn, Designation=@dst, Hired_Date=@hd, Basic_Rate=@br, E_Status=@es, In_Schedule=@inSchedule, Out_Schedule=@outSchedule WHERE Count=@ct";

            OleDbCommand cmd = new OleDbCommand(query, conn);

            // Add parameters from textboxes and controls
            cmd.Parameters.Add("@id", OleDbType.VarChar).Value = tbid.Text;
            cmd.Parameters.Add("@ps", OleDbType.VarChar).Value = tbpass.Text;
            cmd.Parameters.Add("@ls", OleDbType.VarChar).Value = tbln.Text;
            cmd.Parameters.Add("@mn", OleDbType.VarChar).Value = tbmn.Text;
            cmd.Parameters.Add("@fn", OleDbType.VarChar).Value = tbfn.Text;
            cmd.Parameters.Add("@sf", OleDbType.VarChar).Value = cbsf.Text;
            cmd.Parameters.Add("@bd", OleDbType.Date).Value = DTbd.Value.Date;
            cmd.Parameters.Add("@add", OleDbType.VarChar).Value = tbadd.Text;
            cmd.Parameters.Add("@gd", OleDbType.VarChar).Value = cbgn.Text;
            cmd.Parameters.Add("@em", OleDbType.VarChar).Value = tbem.Text;
            cmd.Parameters.Add("@cn", OleDbType.Numeric).Value = Convert.ToInt64(tbcn.Text);
            cmd.Parameters.Add("@dst", OleDbType.VarChar).Value = tbdsnt.Text;
            cmd.Parameters.Add("@hd", OleDbType.Date).Value = dtphire.Value.Date;

            

            // Handle Basic Rate (Currency)
            if (decimal.TryParse(tbsr.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal basicRate))
            {
                cmd.Parameters.Add("@br", OleDbType.Currency).Value = basicRate;
            }
            else
            {
                MessageBox.Show("Please enter a valid currency value for Basic Rate.");
                isUpdating = false; // Reset flag if update is canceled
                return;
            }

            cmd.Parameters.Add("@es", OleDbType.VarChar).Value = cbes.Text;
            // Adding In_Schedule and Out_Schedule from dtpIN and dtpOUT
            cmd.Parameters.Add("@inSchedule", OleDbType.Date).Value = dtpIN.Value;  // In_Schedule (dtpIN)
            cmd.Parameters.Add("@outSchedule", OleDbType.Date).Value = dtpOUT.Value; // Out_Schedule (dtpOUT)

            if (isImageUploaded)
            {
                // Convert the image to a byte array
                using (MemoryStream ms = new MemoryStream())
                {
                    pb_image.Image.Save(ms, pb_image.Image.RawFormat);
                    byte[] imageBytes = ms.ToArray();
                    cmd.Parameters.Add("@i", OleDbType.Binary).Value = imageBytes; // Use OleDbType.Binary
                }
            }

            // Adding Count (to specify which record to update)
            cmd.Parameters.Add("@ct", OleDbType.Integer).Value = Convert.ToInt32(dgvUser.CurrentRow.Cells[0].Value);

            // Execute the command
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            MessageBox.Show("User Updated Successfully");

            GetUsers(); // Refresh data to reflect updates

            isUpdating = false; // Reset flag after update is complete

            this.Hide();
        }



        // Add this field at the class level
        private bool isUpdating = false;

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Ensure that the DataGridView has a selected row
            if (dgvUser.CurrentRow == null) return; // Exit if no row is selected

            if (isUpdating) return; // Skip execution if an update is in progress

            // Proceed with populating the textboxes from the selected row
            tbid.Text = dgvUser.CurrentRow.Cells[1].Value.ToString();
            tbpass.Text = dgvUser.CurrentRow.Cells[2].Value.ToString();
            tbln.Text = dgvUser.CurrentRow.Cells[3].Value.ToString();
            tbmn.Text = dgvUser.CurrentRow.Cells[4].Value.ToString();
            tbfn.Text = dgvUser.CurrentRow.Cells[5].Value.ToString();
            cbsf.Text = dgvUser.CurrentRow.Cells[6].Value.ToString();
            DTbd.Value = Convert.ToDateTime(dgvUser.CurrentRow.Cells[7].Value);
            tbadd.Text = dgvUser.CurrentRow.Cells[8].Value.ToString();
            cbgn.Text = dgvUser.CurrentRow.Cells[9].Value.ToString();
            tbem.Text = dgvUser.CurrentRow.Cells[10].Value.ToString();
            tbcn.Text = dgvUser.CurrentRow.Cells[11].Value.ToString();
            tbdsnt.Text = dgvUser.CurrentRow.Cells[12].Value.ToString();
            dtphire.Value = Convert.ToDateTime(dgvUser.CurrentRow.Cells[13].Value);

            if (dgvUser.CurrentRow.Cells["Picture"].Value != DBNull.Value)
            {
                byte[] imgData = (byte[])dgvUser.CurrentRow.Cells["Picture"].Value;
                using (MemoryStream ms = new MemoryStream(imgData))
                {
                    pb_image.Image = System.Drawing.Image.FromStream(ms);
                }
            }
            else
            {
                pb_image.Image = null;
            }

            decimal currencyValue = (decimal)dgvUser.CurrentRow.Cells[16].Value;
            tbsr.Text = currencyValue.ToString("C2"); // Format as currency with two decimal places

            cbes.Text = dgvUser.CurrentRow.Cells[17].Value.ToString();

            // Handle Time In and Time Out assignments
            object timeInValue = dgvUser.CurrentRow.Cells[18].Value;
            object timeOutValue = dgvUser.CurrentRow.Cells[19].Value;

            // Check if the value is DBNull or an invalid DateTime
            if (timeInValue != DBNull.Value && timeInValue is DateTime)
            {
                DateTime timeIn = (DateTime)timeInValue;
                if (timeIn != DateTime.MinValue && timeIn != new DateTime(1899, 12, 30))
                {
                    // If the time is valid, update dtpIN
                    dtpIN.Value = new DateTime(dtpIN.Value.Year, dtpIN.Value.Month, dtpIN.Value.Day, timeIn.Hour, timeIn.Minute, timeIn.Second);
                }
                else
                {
                    // Handle default value case
                    dtpIN.Value = DateTime.Now; // Default to current time if invalid
                }
            }
            else
            {
                // Handle DBNull or invalid time
                dtpIN.Value = DateTime.Now; // Default to current time if invalid
            }

            // Check if the value is DBNull or an invalid DateTime for timeOut
            if (timeOutValue != DBNull.Value && timeOutValue is DateTime)
            {
                DateTime timeOut = (DateTime)timeOutValue;
                if (timeOut != DateTime.MinValue && timeOut != new DateTime(1899, 12, 30))
                {
                    // If the time is valid, update dtpOUT
                    dtpOUT.Value = new DateTime(dtpOUT.Value.Year, dtpOUT.Value.Month, dtpOUT.Value.Day, timeOut.Hour, timeOut.Minute, timeOut.Second);
                }
                else
                {
                    // Handle default value case
                    dtpOUT.Value = DateTime.Now; // Default to current time if invalid
                }
            }
            else
            {
                // Handle DBNull or invalid time
                dtpOUT.Value = DateTime.Now; // Default to current time if invalid
            }
        }


        private void pictureBox14_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pb_image.Image = new Bitmap(openFileDialog.FileName);
                isImageUploaded = true;
            }
        }

        private void cbes_Enter(object sender, EventArgs e)
        {
            if (cbes.Text == "E.Status")
            {
                cbes.Text = "";
                cbes.ForeColor = Color.Black;
            }
        }

        private void cbes_Leave(object sender, EventArgs e)
        {
            if (cbes.Text == "")
            {
                cbes.Text = "E.Status";
                cbes.ForeColor = Color.Gray;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Check if the text box is empty
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                // If the search box is empty, reload all users
                GetUsers();
            }
            else
            {
                // Create a DataView from the original DataTable
                DataView dv = new DataView(dt);

                // Filter rows where the ID matches the entered text
                dv.RowFilter = string.Format("ID LIKE '%{0}%'", textBox1.Text);

                // Bind the filtered data to the DataGridView
                dgvUser.DataSource = dv;
            }
        }

        

        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cbDesignation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (dt != null)
            {
                string selectedStatus = cbStatus.SelectedItem?.ToString() ?? "All";
                string selectedGender = cbGender.SelectedItem?.ToString() ?? "All";
                string selectedDesignation = cbDesignation.SelectedItem?.ToString() ?? "All";

                DataView dv = new DataView(dt);

                // Build filter conditionally based on selections
                string filter = "";

                if (selectedStatus != "All")
                {
                    filter += string.Format("[E_Status] = '{0}'", selectedStatus);
                }

                if (selectedGender != "All")
                {
                    if (filter.Length > 0) filter += " AND ";
                    filter += string.Format("[Gender] = '{0}'", selectedGender);
                }

                if (selectedDesignation != "All")
                {
                    if (filter.Length > 0) filter += " AND ";
                    filter += string.Format("[Designation] = '{0}'", selectedDesignation);
                }

                dv.RowFilter = filter;
                dgvUser.DataSource = dv;
            }
        }

        private void tbdsnt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbdsnt.Text == "Manager")
            {
                tbsr.Text = "120";

            }
            else if (tbdsnt.Text == "Worker")
            {
                tbsr.Text = "100";
            }
            else if (tbdsnt.Text == "Janitor")
            {
                tbsr.Text = "80";
            }
        }
    }
}
