using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CLOCK2PAY
{
    public partial class ResignList : UserControl
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls
        public ResignList()
        {
            InitializeComponent();
        }

        public void RefreshDataGridView()
        {
            GetUsers();  // Call the GetUsers method to reload the data
        }

        public void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            // Modify SQL to only select rows where Resign_Date is NULL
            adapter = new OleDbDataAdapter("SELECT * FROM MasterList WHERE Resign_Date IS NOT NULL", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }




        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null) // Check if there is a selected row
            {
                // Prompt the user for confirmation before deleting
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this user?",
                                                             "Confirm Deletion",
                                                             MessageBoxButtons.YesNo,
                                                             MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes) // Proceed only if the user confirms
                {
                    string query = "DELETE FROM MasterList WHERE Count = @C"; // Ensure Count is the right column name
                    cmd = new OleDbCommand(query, conn);

                    // Make sure to convert the correct cell value to an integer
                    cmd.Parameters.AddWithValue("@C", Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value));

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User Deleted");
                        }
                        else
                        {
                            MessageBox.Show("No user found with the specified identifier.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting user: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                        GetUsers(); // Refresh the DataGridView
                    }
                }
                else
                {
                    // User clicked No, do nothing
                    MessageBox.Show("Deletion cancelled.");
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }

        private void ResignList_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

            GetUsers();
            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Password"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column

            // Set the format for the Basic_Salary column to display the Peso sign
            dataGridView1.Columns["Basic_Rate"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed



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

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null) // Check if a row is selected
            {
                // Get the selected user's data
                DataGridViewRow selectedRow = dataGridView1.CurrentRow;

                // Retrieve data from the selected row
                string id = selectedRow.Cells["ID"].Value?.ToString() ?? string.Empty;
                string password = selectedRow.Cells["Password"].Value?.ToString() ?? string.Empty;
                string lastname = selectedRow.Cells["Lastname"].Value?.ToString() ?? string.Empty;
                string firstname = selectedRow.Cells["Firstname"].Value?.ToString() ?? string.Empty;
                string middlename = selectedRow.Cells["Middlename"].Value?.ToString() ?? string.Empty;
                string suffix = selectedRow.Cells["Suffix"].Value?.ToString() ?? string.Empty;
                DateTime birthDate = Convert.ToDateTime(selectedRow.Cells["BirthDate"].Value);
                string address = selectedRow.Cells["Address"].Value?.ToString() ?? string.Empty;
                string gender = selectedRow.Cells["Gender"].Value?.ToString() ?? string.Empty;
                string email = selectedRow.Cells["Email"].Value?.ToString() ?? string.Empty;

                // Convert Contact_No safely to double
                double contactNo = 0;
                if (selectedRow.Cells["Contact_No"].Value != DBNull.Value &&
                    double.TryParse(selectedRow.Cells["Contact_No"].Value?.ToString()?.Trim(), out double result))
                {
                    contactNo = result;
                }
                else
                {
                    MessageBox.Show("Invalid or missing Contact_No value. Ensure it is numeric.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string designation = selectedRow.Cells["Designation"].Value?.ToString() ?? string.Empty;
                DateTime hiredDate = DateTime.Now.Date; // Set the new hire date to today's date

                // Retrieve Picture data
                byte[] picture = null;
                if (selectedRow.Cells["Picture"].Value != DBNull.Value)
                {
                    picture = (byte[])selectedRow.Cells["Picture"].Value;
                }
                else
                {
                    MessageBox.Show("Missing or invalid picture data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                double basicRate = Convert.ToDouble(selectedRow.Cells["Basic_Rate"].Value);
                string eStatus = selectedRow.Cells["E_Status"].Value?.ToString() ?? string.Empty;
                DateTime InStats = Convert.ToDateTime(selectedRow.Cells["In_Schedule"].Value);
                DateTime OutStats = Convert.ToDateTime(selectedRow.Cells["Out_Schedule"].Value);
                // Check if the user has already been rehired
                string checkQuery = "SELECT COUNT(*) FROM MasterList WHERE [ID] = @id AND [Resign_Date] IS NULL";
                OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn);
                checkCmd.Parameters.Add("@id", OleDbType.VarChar).Value = id;

                conn.Open();
                int existingUserCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                conn.Close();

                if (existingUserCount > 0)
                {
                    // User is already rehired, show a message
                    MessageBox.Show("The user you're trying to rehire is already rehired.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Prepare the SQL query to insert the user
                string query = "INSERT INTO MasterList ([ID], [Password], [Lastname], [Firstname], [Middlename], [Suffix], [BirthDate], [Address], [Gender], [Email], [Contact_No], [Designation], [Hired_Date], [Picture], [Basic_Rate], [E_Status], [In_Schedule], [Out_Schedule]) " +
                                "VALUES (@id, @password, @lastname, @firstname, @middlename, @suffix, @birthDate, @address, @gender, @email, @contactNo, @designation, @hiredDate, @picture, @basicRate, @eStatus, @ins,@outs)";

                // Initialize the OleDbCommand object
                OleDbCommand cmd = new OleDbCommand(query, conn);

                // Bind parameters explicitly
                cmd.Parameters.Add("@id", OleDbType.VarChar).Value = id;
                cmd.Parameters.Add("@password", OleDbType.VarChar).Value = password;
                cmd.Parameters.Add("@lastname", OleDbType.VarChar).Value = lastname;
                cmd.Parameters.Add("@firstname", OleDbType.VarChar).Value = firstname;
                cmd.Parameters.Add("@middlename", OleDbType.VarChar).Value = middlename;
                cmd.Parameters.Add("@suffix", OleDbType.VarChar).Value = suffix;
                cmd.Parameters.Add("@birthDate", OleDbType.Date).Value = birthDate;
                cmd.Parameters.Add("@address", OleDbType.VarChar).Value = address;
                cmd.Parameters.Add("@gender", OleDbType.VarChar).Value = gender;
                cmd.Parameters.Add("@email", OleDbType.VarChar).Value = email;
                cmd.Parameters.Add("@contactNo", OleDbType.Double).Value = contactNo;
                cmd.Parameters.Add("@designation", OleDbType.VarChar).Value = designation;
                cmd.Parameters.Add("@hiredDate", OleDbType.Date).Value = hiredDate;
                cmd.Parameters.Add("@picture", OleDbType.Binary).Value = picture;
                cmd.Parameters.Add("@basicRate", OleDbType.Double).Value = basicRate;
                cmd.Parameters.Add("@eStatus", OleDbType.VarChar).Value = eStatus;

                cmd.Parameters.Add("@ins", OleDbType.Date).Value = InStats;
                cmd.Parameters.Add("@outs", OleDbType.Date).Value = OutStats;
                // Execute the insert query
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                conn.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Employee Rehired Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    

                }
                else
                {
                    MessageBox.Show("Failed to rehire user. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                GetUsers(); // Refresh DataGridView
            }
            else
            {
                MessageBox.Show("Please select a user to rehire.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }










        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Determine the column to search based on the ComboBox selection
            string selectedColumn = comboBox1.SelectedItem?.ToString();

            // Check if the text box is empty
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                // If the search box is empty, reload all users
                GetUsers();
            }
            else if (!string.IsNullOrEmpty(selectedColumn))
            {
                // Create a DataView from the original DataTable
                DataView dv = new DataView(dt);

                // Filter rows based on the selected column and entered text
                dv.RowFilter = string.Format("{0} LIKE '%{1}%'", selectedColumn, textBox2.Text);

                // Bind the filtered data to the DataGridView
                dataGridView1.DataSource = dv;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Trigger a search based on the new ComboBox selection
            textBox2_TextChanged(sender, e);
        }
    }
}
