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


namespace CLOCK2PAY
{
    public partial class Records : UserControl
    {

        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls


        public Records()
        {
            InitializeComponent();
        }

        void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM TIME_IN_OUT", conn);
            conn.Open();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            conn.Close();
        }


        private void Records_Load(object sender, EventArgs e)
        {
            // Disable default header visual styles
            dataGridView1.EnableHeadersVisualStyles = false;

            GetUsers();

            dataGridView1.Columns["Count"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_In_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Time_Out_Pic"].Visible = false; // Hide the Photo column
            dataGridView1.Columns["Picture"].Visible = false; // Hide the Photo column  
            dataGridView1.Columns["Time_In"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM
            dataGridView1.Columns["Time_Out"].DefaultCellStyle.Format = "hh:mm tt"; // 12-hour format with AM/PM

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
                    string query = "DELETE FROM TIME_IN_OUT WHERE Count = @C"; // Ensure Count is the right column name
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

        private void button4_Click(object sender, EventArgs e)
        {

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

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }

}
