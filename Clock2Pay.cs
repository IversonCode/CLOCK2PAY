using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLOCK2PAY
{
    public partial class Clock2Pay : Form
    {

        bool sidebarExpand;


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
       int nLeftRect,        // x-coordinate of upper-left corner
       int nTopRect,         // y-coordinate of upper-left corner
       int nRightRect,       // x-coordinate of lower-right corner
       int nBottomRect,      // y-coordinate of lower-right corner
       int nWidthEllipse,    // width of ellipse
       int nHeightEllipse  // height of ellipse
           );
        public Clock2Pay()
        {
            InitializeComponent();

            // Set the form border style to none
            this.FormBorderStyle = FormBorderStyle.None;

            // Create a rounded region for the form
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));


            LoadDashboard();


            

        }

        private void LoadDashboard()
        {
            // Create a new instance of the dashboard
            Dashboard dashboard = new Dashboard
            {
                Location = new Point(49, 42), // Set the desired location
                Size = new Size(1223, 609),   // Optional: Set a fixed size
                Name = "dashboard1"           // Assign a name for identification
            };

            // Add the UserControl to the form
            this.Controls.Add(dashboard);
        }

        private void ReloadDashboard()
        {
            // Check if the dashboard UserControl already exists
            Control existingDashboard = this.Controls["dashboard1"];

            if (existingDashboard != null)
            {
                // If the control exists, remove it before adding a new instance
                this.Controls.Remove(existingDashboard);
            }

            // Load and add the new dashboard UserControl
            LoadDashboard();
        }
        private void Clock2Pay_Load(object sender, EventArgs e)
        {

            if (lblLogtype.Text == "Admin")
            {
                panel9.Enabled = false;
                button6.Enabled = false;
                panel9.Visible = false;
                button6.Visible = false;


            }

            else if (lblLogtype.Text == "SuperAdmin")
            {
                panel9.Enabled = true;
                button6.Enabled = true;
                panel9.Visible = true;
                button6.Visible = true;
            }


            
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Hide();



            //Labels Showssss
            DB.Show();
            AT.Hide();
            MLL.Hide();
            RC.Hide();
            SLR.Hide();
            RL.Hide();
            SuperA.Hide();
            lblatt.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Set button colors as needed
            button1.BackColor = Color.DarkGray;
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);

            // Hide all other UserControls
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Hide();
            // Hide all labels
            DB.Hide();
            AT.Hide();
            MLL.Hide();
            RC.Hide();
            SLR.Hide();
            RL.Hide();
            SuperA.Hide();
            lblatt.Hide();
            // Reload and show the dashboard
            ReloadDashboard();  // This will ensure dashboard1 is shown correctly

            // Show the necessary label for dashboard1
            DB.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
           


            button2.BackColor = Color.DarkGray;
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);
            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Hide();
            attendanceTracker1.Show();
            records1.Hide();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Hide();
            //Labels Showssss
            DB.Hide();
            AT.Show();
            MLL.Hide();
            RC.Hide();
            SLR.Hide();
            RL.Hide();
            SuperA.Hide();
            lblatt.Hide();

        }

        private void button3_Click(object sender, EventArgs e)
        {
         

            masterList1.RefreshDataGridView();

            button3.BackColor = Color.DarkGray;
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);
            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Show();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Hide();

            //Labels Showssss
            DB.Hide();
            AT.Hide();
            MLL.Show();
            RC.Hide();
            SLR.Hide();
            RL.Hide();
            SuperA.Hide();
            lblatt.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.BackColor = Color.DarkGray;
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);

            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Show();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Hide();
            //Labels Showssss
            DB.Hide();
            AT.Hide();
            MLL.Hide();
            RC.Show();
            SLR.Hide();
            RL.Hide();
            SuperA.Hide();
            lblatt.Hide();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.BackColor = Color.DarkGray;
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);

            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Show();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Hide();
            //Labels Showssss
            DB.Hide();
            AT.Hide();
            MLL.Hide();
            RC.Hide();
            SLR.Show();
            RL.Hide();
            SuperA.Hide();
            lblatt.Hide();

        }



        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicks Yes, proceed with logout
            if (result == DialogResult.Yes)
            {
                if (lblLogtype.Text == "Admin")
                {
                    // Establish the connection string to connect to the Access database
                    using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
                    {
                        // Open the connection to the database
                        conn.Open();

                        // Query to update the L_Time_Out column for the current admin and date, where L_Time_Out is still null
                        string updateQuery = "UPDATE AdminLoginRecord SET L_Time_Out = @L_Time_Out WHERE ID = @ID AND L_Date = @L_Date AND L_Time_Out IS NULL";
                        using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, conn))
                        {
                            // Bind parameters to the query
                            updateCmd.Parameters.AddWithValue("@L_Time_Out", DateTime.Now.TimeOfDay); // Current time
                            updateCmd.Parameters.AddWithValue("@ID", LogID.Text); // Admin ID from LogID label
                            updateCmd.Parameters.AddWithValue("@L_Date", DateTime.Now.Date); // Current date

                            // Execute the update command
                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            // Check if the update was successful
                            if (rowsAffected > 0)
                            {

                                this.Hide();
                                
                            }
                            else
                            {
                                MessageBox.Show("No matching record found to update logout time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                this.Hide();

            }
            
           
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            // Ensure MinimumSize and MaximumSize are properly set before this code runs.

            if (sidebarExpand)
            {
                // If the sidebar is expanded, minimize it
                sidebar.Width -= 10;

                // Check if the sidebar width has reached or gone below the minimum size
                if (sidebar.Width <= sidebar.MinimumSize.Width)
                {
                    sidebar.Width = sidebar.MinimumSize.Width; // Ensure it stops exactly at the minimum
                    sidebarExpand = false; // Set to collapsed state
                    timer1.Stop();
                }
            }
            else
            {
                // If the sidebar is collapsed, expand it
                sidebar.Width += 10;

                // Check if the sidebar width has reached or gone above the maximum size
                if (sidebar.Width >= sidebar.MaximumSize.Width)
                {
                    sidebar.Width = sidebar.MaximumSize.Width; // Ensure it stops exactly at the maximum
                    sidebarExpand = true; // Set to expanded state
                    timer1.Stop();
                }
            }
        }

        private void Menubttn_Click(object sender, EventArgs e)
        {
            // Toggle the expand/collapse state and start the timer
            timer1.Start();
            //set timer ineterval to smoothen
        }

        

        private void button8_Click(object sender, EventArgs e)
        {
            resignList1.RefreshDataGridView();

            button8.BackColor = Color.DarkGray;
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);
            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Hide();
            resignList1.Show();
            superAdmin1.Hide();
            attempt1.Hide();

            //Labels Showssss
            DB.Hide();
            AT.Hide();
            MLL.Hide();
            RC.Hide();
            SLR.Hide();
            RL.Show();
            SuperA.Hide();
            lblatt.Hide();
        }



        

        private void button6_Click(object sender, EventArgs e)
        {
            button6.BackColor = Color.DarkGray;
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);
            button9.BackColor = Color.FromArgb(160, 188, 239);

            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Show();
            attempt1.Hide();

            //Labels Showssss
            SuperA.Show();
            DB.Hide();
            AT.Hide();
            MLL.Hide();
            RC.Hide();
            SLR.Hide();
            RL.Hide();
            lblatt.Hide();

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            button9.BackColor = Color.DarkGray;
            button6.BackColor = Color.FromArgb(160, 188, 239);
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button2.BackColor = Color.FromArgb(160, 188, 239);
            button3.BackColor = Color.FromArgb(160, 188, 239);
            button4.BackColor = Color.FromArgb(160, 188, 239);
            button5.BackColor = Color.FromArgb(160, 188, 239);
            button8.BackColor = Color.FromArgb(160, 188, 239);

            //Shows UserControlssss
            dashboard1.Hide();
            masterList1.Hide();
            attendanceTracker1.Hide();
            records1.Hide();
            salary1.Hide();
            resignList1.Hide();
            superAdmin1.Hide();
            attempt1.Show();


            //Labels Showssss
            SuperA.Hide();
            DB.Hide();
            AT.Hide();
            MLL.Hide();
            RC.Hide();
            SLR.Hide();
            RL.Hide();
            lblatt.Show();
        }
    }
}
