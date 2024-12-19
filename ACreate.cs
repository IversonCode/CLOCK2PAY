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
using System.Drawing.Imaging; // For working with images

namespace CLOCK2PAY
{
    public partial class ACreate : Form
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
        public ACreate()
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

            conn.Close();

        }

        private void tbid_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void tbfn_TextChanged(object sender, EventArgs e)
        {

        }

        private void ACreate_Load(object sender, EventArgs e)
        {
            GetUsers();
        }

        private void tbid_Enter(object sender, EventArgs e)
        {
            if (tbid.Text == "ID")
            {
                tbid.Text = "";
                tbid.ForeColor = Color.Black;
            }
        }

        private void tbid_Leave(object sender, EventArgs e)
        {
            if (tbid.Text == "")
            {
                tbid.Text = "ID";
                tbid.ForeColor = Color.Gray;
            }
        }

        private void tbpass_Enter(object sender, EventArgs e)
        {
            if (tbpass.Text == "Password")
            {
                tbpass.Text = "";
                tbpass.ForeColor = Color.Black;
            }
        }

        private void tbpass_Leave(object sender, EventArgs e)
        {
            if (tbpass.Text == "")
            {
                tbpass.Text = "Password";
                tbpass.ForeColor = Color.Gray;
            }
        }

        private void tbam_Enter(object sender, EventArgs e)
        {
            if (tbam.Text == "Admin")
            {
                tbam.Text = "";
                tbam.ForeColor = Color.Black;
            }
        }

        private void tbam_Leave(object sender, EventArgs e)
        {
            if (tbam.Text == "")
            {
                tbam.Text = "Admin";
                tbam.ForeColor = Color.Gray;
            }
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

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbpass.Text == "Password" || tbid.Text == "ID" || tbam.Text == "Admin")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            else if (pb_image.Image == null)
            {
                MessageBox.Show("Please Input A Image!");
            }

            // Open the database connection
            conn.Open();


            // Check if ID already exists
            string checkQuery = "SELECT COUNT(*) FROM Admin WHERE [ID] = @id";
            OleDbCommand checkCmd = new OleDbCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@id", tbid.Text);

            int count = (int)checkCmd.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("The ID you entered is already taken. Please choose a different ID.");
                conn.Close();
                return; // Stop further execution if ID already exists
            }


            // SQL query to insert a new user
            string query = "INSERT INTO Admin ([ID], [Password] , [Admin], [Picture]) " +
                           "VALUES (@id, @am, @p, @i)";

            cmd = new OleDbCommand(query, conn);

            // Add parameters from textboxes and controls with explicit types
            cmd.Parameters.Add("@id", OleDbType.VarChar).Value = tbid.Text;
            cmd.Parameters.Add("@p", OleDbType.VarChar).Value = tbpass.Text;
            cmd.Parameters.Add("@am", OleDbType.VarChar).Value = tbam.Text;


            // Convert the image to a byte array and add it to the parameters
            using (MemoryStream ms = new MemoryStream())
            {
                if (pb_image.Image != null)
                {
                    pb_image.Image.Save(ms, pb_image.Image.RawFormat);
                    cmd.Parameters.Add("@i", OleDbType.Binary).Value = ms.ToArray();
                }
                else
                {
                    cmd.Parameters.Add("@i", OleDbType.Binary).Value = DBNull.Value;
                }
            }

            // Insert the new user
            cmd.ExecuteNonQuery();
            MessageBox.Show("User Inserted Successfully", "User Inserted", MessageBoxButtons.OK, MessageBoxIcon.Information);


            conn.Close();

            this.Hide();


        }
    }
}
