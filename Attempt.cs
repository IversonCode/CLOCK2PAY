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
    public partial class Attempt : UserControl
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls
        public Attempt()
        {
            InitializeComponent();
        }

        private void Attempt_Load(object sender, EventArgs e)
        {
            GetUsers();
            dataGridView1.Columns["Attempt_Picture"].Visible = false; // Hide the Photo column 
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

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

        public void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            // Modify SQL to only select rows where Resign_Date is NULL
            adapter = new OleDbDataAdapter("SELECT * FROM Attempt", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null) // Check if there is a selected row
            {
                var pictureCell = dataGridView1.CurrentRow.Cells["Attempt_Picture"]; // Get the cell
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
                dv.RowFilter = string.Format("Date_C = #{0}#", selectedDate.ToString("MM/dd/yyyy"));

                // Update the DataGridView with the filtered data
                dataGridView1.DataSource = dv;
            }
        }
    }
}
