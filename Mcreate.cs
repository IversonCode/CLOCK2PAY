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
    public partial class Mcreate : Form
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


        public Mcreate()
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
            adapter = new OleDbDataAdapter("SELECT * FROM MasterList", conn);
            conn.Open();
            adapter.Fill(dt);

            conn.Close();

        }

       
        private void Mcreate_Load(object sender, EventArgs e)
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
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

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
            // Check for required fields
            if (tbpass.Text == "Password" || tbfn.Text == "Firstname" || tbln.Text == "Lastname" || cbsf.Text == "Suffix" || tbcn.Text == "Contact No" || cbgn.Text == "Gender" || tbem.Text == "Email" || tbdsnt.Text == "Designation"|| tbadd.Text == "Address"|| tbid.Text == "ID" || tbsr.Text == "B.Rate"|| tbmn.Text == "Middlename"||
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

            // Validate that contact number is numeric
            if (!long.TryParse(tbcn.Text, out _))
            {
                MessageBox.Show("Please enter a valid numeric value for the contact number.");
                return;
            }

            // Validate that basic salary is numeric
            if (!decimal.TryParse(tbsr.Text, out _))
            {
                MessageBox.Show("Please enter a valid numeric value for the Basic Salary.");
                return;
            }

            // Open the database connection
            conn.Open();

            // Check if ID already exists
            string checkQuery = "SELECT COUNT(*) FROM MasterList WHERE [ID] = @id";
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
            string query = "INSERT INTO MasterList ([ID],[Password], [Lastname], [Firstname],[Middlename], [Suffix], [BirthDate], [Address], [Gender], [Email], [Contact_No], [Designation], [Hired_Date], [Picture], [Basic_Rate], [E_Status],[In_Schedule], [Out_Schedule]) " +
                           "VALUES (@id, @p, @ln, @fn,@mn, @sf, @bd, @ad, @ge, @eml, @cn, @ds, @hd, @i, @bs,@es,@is,@os)";

            cmd = new OleDbCommand(query, conn);

            // Add parameters from textboxes and controls with explicit types
            cmd.Parameters.Add("@id", OleDbType.VarChar).Value = tbid.Text;
            cmd.Parameters.Add("@p", OleDbType.VarChar).Value = tbpass.Text;
            cmd.Parameters.Add("@ln", OleDbType.VarChar).Value = tbln.Text;
            cmd.Parameters.Add("@fn", OleDbType.VarChar).Value = tbfn.Text;
            cmd.Parameters.Add("@mn", OleDbType.VarChar).Value = tbmn.Text;
            cmd.Parameters.Add("@sf", OleDbType.VarChar).Value = cbsf.Text;
            cmd.Parameters.Add("@bd", OleDbType.Date).Value = DTbd.Value.Date;
            cmd.Parameters.Add("@ad", OleDbType.VarChar).Value = tbadd.Text;
            cmd.Parameters.Add("@ge", OleDbType.VarChar).Value = cbgn.Text;
            cmd.Parameters.Add("@eml", OleDbType.VarChar).Value = tbem.Text;
            cmd.Parameters.Add("@cn", OleDbType.Numeric).Value = Convert.ToInt64(tbcn.Text);
            cmd.Parameters.Add("@ds", OleDbType.VarChar).Value = tbdsnt.Text;
            cmd.Parameters.Add("@hd", OleDbType.Date).Value = dtphire.Value.Date;

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

            // Add Basic Rate parameter as Currency
            cmd.Parameters.Add("@bs", OleDbType.Currency).Value = Convert.ToDecimal(tbsr.Text);
            cmd.Parameters.Add("@es", OleDbType.VarChar).Value = cbes.Text;
            // Format the time as 12-hour format with AM/PM
            cmd.Parameters.Add("@is", OleDbType.VarChar).Value = dtpIN.Value.ToString("hh:mm tt");
            cmd.Parameters.Add("@os", OleDbType.VarChar).Value = dtpOUT.Value.ToString("hh:mm tt");


            // Insert the new user
            cmd.ExecuteNonQuery();
            MessageBox.Show("User Inserted Successfully", "User Inserted", MessageBoxButtons.OK, MessageBoxIcon.Information);
           


            conn.Close();

            this.Hide();




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

        private void tbfn_Enter(object sender, EventArgs e)
        {
            if (tbfn.Text == "Firstname")
            {
                tbfn.Text = "";
                tbfn.ForeColor = Color.Black;
            }
        }

        private void tbfn_Leave(object sender, EventArgs e)
        {
            if (tbfn.Text == "")
            {
                tbfn.Text = "Firstname";
                tbfn.ForeColor = Color.Gray;
            }
        }

        private void tbln_Enter(object sender, EventArgs e)
        {
            if (tbln.Text == "Lastname")
            {
                tbln.Text = "";
                tbln.ForeColor = Color.Black;
            }
        }

        private void tbln_Leave(object sender, EventArgs e)
        {
            if (tbln.Text == "")
            {
                tbln.Text = "Lastname";
                tbln.ForeColor = Color.Gray;
            }
        }

        private void cbsf_Enter(object sender, EventArgs e)
        {
            if (cbsf.Text == "Suffix")
            {
                cbsf.Text = "";
                cbsf.ForeColor = Color.Black;
            }
        }

        private void cbsf_Leave(object sender, EventArgs e)
        {
            if (cbsf.Text == "")
            {
                cbsf.Text = "Suffix";
                cbsf.ForeColor = Color.Gray;
            }
        }

        private void cbgn_Enter(object sender, EventArgs e)
        {
            if (cbgn.Text == "Gender")
            {
                cbgn.Text = "";
                cbgn.ForeColor = Color.Black;
            }
        }

        private void cbgn_Leave(object sender, EventArgs e)
        {
            if (cbgn.Text == "")
            {
                cbgn.Text = "Gender";
                cbgn.ForeColor = Color.Gray;
            }
        }

        private void tbem_Enter(object sender, EventArgs e)
        {
            if (tbem.Text == "Email")
            {
                tbem.Text = "";
                tbem.ForeColor = Color.Black;
            }
        }

        private void tbem_Leave(object sender, EventArgs e)
        {
            if (tbem.Text == "")
            {
                tbem.Text = "Email";
                tbem.ForeColor = Color.Gray;
            }
        }

        private void tbcn_Enter(object sender, EventArgs e)
        {
            if (tbcn.Text == "Contact No")
            {
                tbcn.Text = "";
                tbcn.ForeColor = Color.Black;
            }
        }

        private void tbcn_Leave(object sender, EventArgs e)
        {
            if (tbcn.Text == "")
            {
                tbcn.Text = "Contact No";
                tbcn.ForeColor = Color.Gray;
            }
        }

        

        private void tbsr_Enter(object sender, EventArgs e)
        {
            if (tbsr.Text == "B.Rate")
            {
                tbsr.Text = "";
                tbsr.ForeColor = Color.Black;
            }
        }

        private void tbsr_Leave(object sender, EventArgs e)
        {
            if (tbsr.Text == "")
            {
                tbsr.Text = "B.Rate";
                tbsr.ForeColor = Color.Gray;
            }
        }

        private void tbadd_Enter(object sender, EventArgs e)
        {
            if (tbadd.Text == "Address")
            {
                tbadd.Text = "";
                tbadd.ForeColor = Color.Black;
            }
        }

        private void tbadd_Leave(object sender, EventArgs e)
        {
            if (tbadd.Text == "")
            {
                tbadd.Text = "Address";
                tbadd.ForeColor = Color.Gray;
            }
        }

        private void tbmn_Enter(object sender, EventArgs e)
        {
            if (tbmn.Text == "Middlename")
            {
                tbmn.Text = "";
                tbmn.ForeColor = Color.Black;
            }
        }

        private void tbmn_Leave(object sender, EventArgs e)
        {
            if (tbmn.Text == "")
            {
                tbmn.Text = "Middlename";
                tbmn.ForeColor = Color.Gray;
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

        private void tbdsnt_Enter(object sender, EventArgs e)
        {
            if (tbdsnt.Text == "Designation")
            {
                tbdsnt.Text = "";
                tbdsnt.ForeColor = Color.Black;
            }
        }

        private void tbdsnt_Leave(object sender, EventArgs e)
        {
            if (tbdsnt.Text == "")
            {
                tbdsnt.Text = "Designation";
                tbdsnt.ForeColor = Color.Gray;
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

        private void tbsr_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
