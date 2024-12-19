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
using static System.Net.WebRequestMethods;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging; // For working with images
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CLOCK2PAY
{
    public partial class AttendanceTracker : UserControl
    {

        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls

        public AttendanceTracker()
        {
            InitializeComponent();
        }


        void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=CLOCK2PAYDB.accdb;Persist Security Info=False");
            dt = new DataTable();

            // Format today's date to match the format in MS Access
            string todayDate = DateTime.Now.ToString("MM/dd/yyyy");

            // Modify the SQL query to filter by today's date
            string query = $"SELECT * FROM TIME_IN_OUT WHERE C_Date = #{todayDate}#";

            adapter = new OleDbDataAdapter(query, conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }


        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void AttendanceTracker_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

            GetUsers();

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;  // Set header background color
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;


            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_In_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_Out_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column  
            


            dataGridView1.Columns["In_Schedule"].DefaultCellStyle.Format = "hh:mm tt";
            dataGridView1.Columns["Out_Schedule"].DefaultCellStyle.Format = "hh:mm tt";


            dataGridView1.Columns["Basic_Rate"].Visible = false; // Hide the Photo column  

            dataGridView1.Columns["Worked_Hours"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Overtime"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Gross_Pay"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["SSS"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Pag-Ibig"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Phil-Health"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Deduction"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Total"].Visible = false; // Hide the Photo column  



            dataGridView1.Columns["Time_In"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Time_Out"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM


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
                // Show Picture in pictureBox3
                var pictureCell = dataGridView1.CurrentRow.Cells["Picture"];
                if (pictureCell.Value != DBNull.Value)
                {
                    byte[] imgData = (byte[])pictureCell.Value;
                    using (MemoryStream ms = new MemoryStream(imgData))
                    {
                        pictureBox3.Image = System.Drawing.Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox3.Image = null;
                }

                // Show Time_In_Pic in pictureBox2
                var timeInPicCell = dataGridView1.CurrentRow.Cells["Time_In_Pic"];
                if (timeInPicCell.Value != DBNull.Value)
                {
                    byte[] timeInImgData = (byte[])timeInPicCell.Value;
                    using (MemoryStream ms = new MemoryStream(timeInImgData))
                    {
                        pictureBox2.Image = System.Drawing.Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox2.Image = null;
                }

                // Show Time_Out_Pic in pictureBox1
                var timeOutPicCell = dataGridView1.CurrentRow.Cells["Time_Out_Pic"];
                if (timeOutPicCell.Value != DBNull.Value)
                {
                    byte[] timeOutImgData = (byte[])timeOutPicCell.Value;
                    using (MemoryStream ms = new MemoryStream(timeOutImgData))
                    {
                        pictureBox1.Image = System.Drawing.Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            else
            {
                // Clear all picture boxes if no current row is selected
                pictureBox3.Image = null;
                pictureBox2.Image = null;
                pictureBox1.Image = null;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Determine the column to search based on the ComboBox selection
            string selectedColumn = comboBox1.SelectedItem?.ToString();

            // Check if the text box is empty
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                // If the search box is empty, reload all users
                GetUsers();
            }
            else if (!string.IsNullOrEmpty(selectedColumn))
            {
                // Create a DataView from the original DataTable
                DataView dv = new DataView(dt);

                // Filter rows based on the selected column and entered text
                dv.RowFilter = string.Format("{0} LIKE '%{1}%'", selectedColumn, textBox1.Text);

                // Bind the filtered data to the DataGridView
                dataGridView1.DataSource = dv;
            }
        }

       

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Trigger a search based on the new ComboBox selection
            textBox1_TextChanged(sender, e);
        }
    }
}
