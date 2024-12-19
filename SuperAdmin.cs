using AForge.Controls;
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
using System.Drawing.Imaging; // For working with images

namespace CLOCK2PAY
{
    public partial class SuperAdmin : UserControl
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls
        public SuperAdmin()
        {
            InitializeComponent();
        }
        public void GetUser()
        {

            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            // Modify SQL to only select rows where Resign_Date is NULL
            adapter = new OleDbDataAdapter("SELECT * FROM Admin ", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();

        }
        public void GetUser2()
        {

            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            // Modify SQL to only select rows where Resign_Date is NULL
            adapter = new OleDbDataAdapter("SELECT * FROM AdminLoginRecord ", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView2.DataSource = dt;
            conn.Close();

        }
        private void SCreate_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;
            // Disable default header visual styles
            dataGridView2.EnableHeadersVisualStyles = false;

            GetUser();
            GetUser2();

            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column
            dataGridView2.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView2.Columns["Picture"].Visible = false; // Hide the Photo column
            dataGridView2.Columns["L_Time_In"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView2.Columns["L_Time_Out"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM


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





            // Set custom column header styles
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(6, 28, 58);
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold); // Set font style

            // Set custom column header styles
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(6, 28, 58);
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold); // Set font style

            // Optionally set the height for headers
            dataGridView2.ColumnHeadersHeight = 25;



            // Set alternating row colors for all rows
            dataGridView2.RowsDefaultCellStyle.BackColor = Color.FromArgb(180, 199, 231);
            dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            // Change the background color for selected rows
            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Gray;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ACreate ac = new ACreate();
            ac.ShowDialog();
            GetUser();
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
                        pictureBox1.Image = System.Drawing.Image.FromStream(ms); // Set the pictureBox image
                    }
                }
                else
                {
                    pictureBox1.Image = null; // Clear the pictureBox image if no data
                }
            }
            else
            {
                pictureBox1.Image = null; // Clear the pictureBox if no current row is selected
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AUpdate ap = new AUpdate();
            ap.ShowDialog();
            GetUser();



        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null) // Check if there is a selected row
            {
                var pictureCell = dataGridView2.CurrentRow.Cells["Picture"]; // Get the cell
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Determine the column to search based on the ComboBox selection
            string selectedColumn = comboBox1.SelectedItem?.ToString();

            // Check if the text box is empty
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                // If the search box is empty, reload all users
                GetUser();
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
                    string query = "DELETE FROM Admin WHERE Count = @C"; // Ensure Count is the right column name
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
                        GetUser(); // Refresh the DataGridView
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
    }
}
