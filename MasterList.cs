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

namespace CLOCK2PAY
{

    public partial class MasterList : UserControl
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls
        public MasterList()
        {
            InitializeComponent();

        }
        public void RefreshDataGridView()
        {
            GetUser();  // Call the GetUsers method to reload the data
        }
        public void GetUser()
        {

            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            // Modify SQL to only select rows where Resign_Date is NULL
            adapter = new OleDbDataAdapter("SELECT * FROM MasterList WHERE Resign_Date IS NULL", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();

        }
        private void MasterList_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

            GetUser();



            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Resign_Date"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Password"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column

           // Set the format for the Basic_Salary column to display the Peso sign
            dataGridView1.Columns["Basic_Rate"].DefaultCellStyle.Format = "₱#,0.00"; // Adjust as needed

            dataGridView1.Columns["In_Schedule"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Out_Schedule"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM

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

        private void button4_Click(object sender, EventArgs e)
        {
            Mcreate MC = new Mcreate();
            MC.ShowDialog();

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

        private void button5_Click(object sender, EventArgs e)
        {
            Mupdate mu = new Mupdate();
            mu.ShowDialog();
            GetUser();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Ensure a row is selected
            if (dataGridView1.CurrentRow != null)
            {
                // Get the ID or unique identifier for the selected row (adjust "ID" if your column name is different)
                var userId = dataGridView1.CurrentRow.Cells["ID"].Value.ToString();

                // Set the Resign_Date cell in the DataGridView to the current date (no time)
                dataGridView1.CurrentRow.Cells["Resign_Date"].Value = DateTime.Now.Date; // Use only the date part

                // Update the database with the new Resign_Date value (only date, no time)
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
                {
                    conn.Open();

                    // Prepare the SQL UPDATE command
                    string query = "UPDATE MasterList SET Resign_Date = ? WHERE ID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        // Add the parameters with the correct data types
                        cmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now.Date; // Set Resign_Date as Date (only the date part)
                        cmd.Parameters.Add("?", OleDbType.VarWChar).Value = userId; // ID as String

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
                MessageBox.Show("Employee Resigned Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
              

                // Optionally, refresh the DataGridView to show the updated data
                GetUser();
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

       

        private void ApplyFilters()
        {
            if (dt == null)
                return;

            DataView dv = new DataView(dt);

            // Collect all filters into a single string
            List<string> filters = new List<string>();

            // TextBox filter
            string selectedColumn = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedColumn) && !string.IsNullOrEmpty(textBox2.Text))
            {
                filters.Add(string.Format("{0} LIKE '%{1}%'", selectedColumn, textBox2.Text));
            }

            // Status filter
            string selectedStatus = cbStatus.SelectedItem?.ToString() ?? "All";
            if (selectedStatus != "All")
            {
                filters.Add(string.Format("[E_Status] = '{0}'", selectedStatus));
            }

            // Gender filter
            string selectedGender = cbGender.SelectedItem?.ToString() ?? "All";
            if (selectedGender != "All")
            {
                filters.Add(string.Format("[Gender] = '{0}'", selectedGender));
            }

            // Designation filter
            string selectedDesignation = cbDesignation.SelectedItem?.ToString() ?? "All";
            if (selectedDesignation != "All")
            {
                filters.Add(string.Format("[Designation] = '{0}'", selectedDesignation));
            }

            // Date filter
            if (!checkBox1.Checked) // If the checkbox is not checked, apply the date filter
            {
                DateTime selectedDate = dateTimePicker1.Value.Date;
                filters.Add(string.Format("Hired_Date = #{0}#", selectedDate.ToString("MM/dd/yyyy")));
            }

            // Combine all filters using AND
            dv.RowFilter = string.Join(" AND ", filters);

            // Update the DataGridView
            dataGridView1.DataSource = dv;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Trigger filter update when checkbox state changes
            ApplyFilters();
        }
    }
}
