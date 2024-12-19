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
    public partial class AUpdate : Form
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
        public AUpdate()
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
            adapter = new OleDbDataAdapter("SELECT * FROM Admin", conn);
            conn.Open();
            adapter.Fill(dt);
            dgvUser.DataSource = dt;
            conn.Close();

        }
        private void AUpdate_Load(object sender, EventArgs e)
        {

            GetUsers();
            
            dgvUser.Columns["Picture"].Visible = false; // Hide the Photo column
            dgvUser.Columns["Count"].Visible = false; // Hide the Photo column
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Check if a row is selected in the DataGridView
            if (dgvUser.CurrentRow == null || dgvUser.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show("Please select a row first.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Check if ID already exists
            string checkIdQuery = "SELECT COUNT(*) FROM Admin WHERE ID = @id AND Count <> @ct";
            cmd = new OleDbCommand(checkIdQuery, conn);
            cmd.Parameters.Add("@id", OleDbType.VarChar).Value = tbid.Text;
            cmd.Parameters.Add("@ct", OleDbType.Integer).Value = Convert.ToInt32(dgvUser.CurrentRow.Cells[0].Value);

            conn.Open();
            int existingIdCount = Convert.ToInt32(cmd.ExecuteScalar());
            conn.Close();

            // If the ID already exists, show a message and return
            if (existingIdCount > 0)
            {
                MessageBox.Show("ID is already taken.");
                return;
            }


            if (tbpass.Text == "Password" || tbid.Text == "ID" || tbam.Text == "Admin")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            isUpdating = true; // Set flag to prevent SelectionChanged event execution


            string query = isImageUploaded
     ? "UPDATE Admin SET ID=@id, Admin=@am, [Password]=@ps, Picture=@i WHERE [Count]=@ct"
     : "UPDATE Admin SET ID=@id, Admin=@am, [Password]=@ps WHERE [Count]=@ct";


            cmd = new OleDbCommand(query, conn);

            // Add parameters from textboxes and controls
            cmd.Parameters.Add("@id", OleDbType.VarChar).Value = tbid.Text;
            cmd.Parameters.Add("@am", OleDbType.VarChar).Value = tbam.Text;
            cmd.Parameters.Add("@ps", OleDbType.VarChar).Value = tbpass.Text;

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


            cmd.Parameters.Add("@ct", OleDbType.Integer).Value = Convert.ToInt32(dgvUser.CurrentRow.Cells[0].Value);

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

        private void dgvUser_SelectionChanged(object sender, EventArgs e)
        {
            // Ensure that the DataGridView has a selected row
            if (dgvUser.CurrentRow == null) return; // Exit if no row is selected

            if (isUpdating) return; // Skip execution if an update is in progress

            // Proceed with populating the textboxes from the selected row
            tbid.Text = dgvUser.CurrentRow.Cells[1].Value.ToString();
            tbam.Text = dgvUser.CurrentRow.Cells[2].Value.ToString();
            tbpass.Text = dgvUser.CurrentRow.Cells[3].Value.ToString();
            

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
    }
}
