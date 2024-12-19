using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
//AForge.Video dll
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge;
using System.Data.OleDb;
using AForge.Controls;

namespace CLOCK2PAY
{
    public partial class Attendance : UserControl
    {
        // Connection string to MS Access database
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=CLOCK2PAYDB.accdb;Persist Security Info=False;";

        private int loginAttempts = 0; // Counter for login attempts


        // Camera Variables
        FilterInfoCollection fic;
        VideoCaptureDevice vcd;
        public Attendance()
        {
            InitializeComponent();

            comboBox1.Visible = false;


        }



        private void StartCamera()
        {
            // Stop the camera if it's already running
            if (vcd != null && vcd.IsRunning)
            {
                vcd.SignalToStop();
                vcd.WaitForStop();
            }

            // Initialize the VideoCaptureDevice using the selected device
            vcd = new VideoCaptureDevice(fic[comboBox1.SelectedIndex].MonikerString);
            vcd.NewFrame += FinalFrame_NewFrame; // Subscribe to the NewFrame event
            vcd.Start(); // Start the video capture
        }
        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Display the current frame in PictureBox
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }





        private byte[] CaptureImage()
        {
            // Capture the current frame from the PictureBox (camera feed)
            if (pictureBox1.Image != null)
            {
                // Make sure to create a new Bitmap from the current image
                Bitmap capturedFrame = new Bitmap(pictureBox1.Image);

                // Convert the captured frame to a byte array
                using (MemoryStream ms = new MemoryStream())
                {
                    // Save the image in JPEG format
                    capturedFrame.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();  // Return the byte array
                }
            }
            return null; // Return null if no image is available
        }











        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "Password")
            {
                return;
            }
            else if (textBox2.PasswordChar == '•')
            {
                textBox2.PasswordChar = '\0';
            }
            else
            {
                textBox2.PasswordChar = '•';
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "ID")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "ID";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Password")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
                textBox2.PasswordChar = '•';
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Password";
                textBox2.ForeColor = Color.Gray;
                textBox2.PasswordChar = '\0';
            }
        }

        private void Attendance_Load(object sender, EventArgs e)
        {
            // Check if the control is in design mode
            if (!DesignMode)
            {
                // Get available video devices (webcams)
                fic = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                // Populate the ComboBox with the list of video devices
                comboBox1.Items.Clear(); // Clear ComboBox before adding devices

                foreach (FilterInfo dev in fic)
                {
                    comboBox1.Items.Add(dev.Name); // Add the device names to the ComboBox
                }

                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0; // Select the first device by default
                    StartCamera(); // Automatically start the camera after adding devices
                }
                else
                {
                    MessageBox.Show("No video capture device found.");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "ID" || textBox2.Text == "Password")
            {
                MessageBox.Show("Please Fill All Fields");
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // Check if the user ID and password are valid
                string query = "SELECT COUNT(*) FROM MasterList WHERE ID = @ID AND [Password] = @Password";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", textBox1.Text);
                    cmd.Parameters.AddWithValue("@Password", textBox2.Text);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0) // If credentials are valid
                    {
                        // Check if the ID has any record with a null Resign_Date
                        string checkResignDateQuery = "SELECT COUNT(*) FROM MasterList WHERE ID = @ID AND Resign_Date IS NULL";
                        using (OleDbCommand cmdResign = new OleDbCommand(checkResignDateQuery, conn))
                        {
                            cmdResign.Parameters.AddWithValue("@ID", textBox1.Text);
                            int activeRecordCount = Convert.ToInt32(cmdResign.ExecuteScalar());

                            if (activeRecordCount > 0)
                            {
                                // Proceed with the process, as there is at least one active record for this ID
                            }
                            else
                            {
                                // Block the process, as all records for this ID have a Resign_Date
                                MessageBox.Show("This ID does not have any active record. The user has resigned.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                textBox1.Text = string.Empty;
                                textBox2.Text = string.Empty;
                                textBox1.ForeColor = Color.Gray;
                                textBox2.ForeColor = Color.Gray;
                                textBox2.PasswordChar = '\0';

                                return; // Exit the process
                            }
                        }

                        // Retrieve user details (Firstname, Middlename, Lastname, Designation, and Picture) based on ID
                        string getUserDetailsQuery = "SELECT Firstname, Middlename, Lastname, Designation,Basic_Rate, Picture,In_Schedule,Out_Schedule FROM MasterList WHERE ID = @ID";
                        using (OleDbCommand cmdDetails = new OleDbCommand(getUserDetailsQuery, conn))
                        {
                            cmdDetails.Parameters.AddWithValue("@ID", textBox1.Text);
                            using (OleDbDataReader reader = cmdDetails.ExecuteReader())
                            {
                                if (reader.Read()) // If user details are found
                                {
                                    // Get the Firstname, Lastname, Middlename, Designation, and Picture for the logged-in user
                                    string firstname = reader["Firstname"].ToString();
                                    string lastname = reader["Lastname"].ToString();
                                    string middlename = reader["Middlename"].ToString();
                                    string designation = reader["Designation"].ToString(); // Get the Designation
                                    decimal basicRate = Convert.ToDecimal(reader["Basic_Rate"]); // Get the Basic_Rate as a decimal
                                    byte[] pictureData = reader["Picture"] as byte[];
                                    TimeSpan InSchedule = Convert.ToDateTime(reader["In_Schedule"]).TimeOfDay;
                                    TimeSpan OutSchedule = Convert.ToDateTime(reader["Out_Schedule"]).TimeOfDay;


                                    // Check if the user has an active Time_In session (Time_Out is NULL) for today
                                    string checkActiveSessionQuery = "SELECT COUNT(*) FROM Time_IN_OUT WHERE ID = @ID AND C_Date = @C_Date AND Time_Out IS NULL";
                                    using (OleDbCommand cmdActiveSession = new OleDbCommand(checkActiveSessionQuery, conn))
                                    {
                                        cmdActiveSession.Parameters.AddWithValue("@ID", textBox1.Text);
                                        cmdActiveSession.Parameters.AddWithValue("@C_Date", DateTime.Now.Date);
                                        int activeSessionCount = (int)cmdActiveSession.ExecuteScalar();

                                        // Check if the user has any Time_In record for today (not necessarily a Time_Out)
                                        string checkLogQuery = "SELECT COUNT(*) FROM Time_IN_OUT WHERE ID = @ID AND C_Date = @C_Date";
                                        using (OleDbCommand cmdCheckLog = new OleDbCommand(checkLogQuery, conn))
                                        {
                                            cmdCheckLog.Parameters.AddWithValue("@ID", textBox1.Text);
                                            cmdCheckLog.Parameters.AddWithValue("@C_Date", DateTime.Now.Date);
                                            int logCount = (int)cmdCheckLog.ExecuteScalar();

                                            if (logCount == 0) // No record for today
                                            {
                                                // No active session, insert Time_In record
                                                string insertTimeInQuery = "INSERT INTO Time_IN_OUT (ID, Lastname, Middlename, Firstname, Designation, Picture, C_Date, Time_In, Time_In_Pic,Basic_Rate,In_Schedule,Out_Schedule) " +
                                                    "VALUES (@ID, @Lastname, @Middlename, @Firstname, @Designation, @Picture, @C_Date, @Time_In, @Time_In_Pic, @Basic_Rate,@In,@Out)";

                                                using (OleDbCommand cmdInsertTimeIn = new OleDbCommand(insertTimeInQuery, conn))
                                                {
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@ID", textBox1.Text);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Lastname", lastname);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Middlename", middlename);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Firstname", firstname);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Designation", designation);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Picture", pictureData ?? (object)DBNull.Value);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@C_Date", DateTime.Now.Date);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Time_In", DateTime.Now.ToString("HH:mm:ss"));
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Time_In_Pic", CaptureImage() ?? (object)DBNull.Value);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Basic_Rate", basicRate);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@In", InSchedule);
                                                    cmdInsertTimeIn.Parameters.AddWithValue("@Out", OutSchedule);

                                                    int insertResult = cmdInsertTimeIn.ExecuteNonQuery();
                                                    if (insertResult > 0)
                                                    {
                                                        // Create TimeInNotif instance and set user data
                                                        TimeInNotif tin = new TimeInNotif();
                                                        tin.SetUserData(firstname, lastname, designation); // Pass the user data
                                                        tin.ShowDialog();
                                                        textBox1.Text = "ID";
                                                        textBox2.Text = "Password";
                                                        textBox1.ForeColor = Color.Gray;
                                                        textBox2.ForeColor = Color.Gray;
                                                        textBox2.PasswordChar = '\0';
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Failed to record Time In.");
                                                    }
                                                }
                                            }
                                            else if (activeSessionCount > 0) // User has an active Time_In session
                                            {
                                                // Update Time_Out for the active session
                                                string updateTimeOutQuery = "UPDATE Time_IN_OUT SET Time_Out = @TimeOut, Time_Out_Pic = @Time_Out_Pic WHERE ID = @ID AND Time_Out IS NULL";
                                                using (OleDbCommand cmdTimeOut = new OleDbCommand(updateTimeOutQuery, conn))
                                                {
                                                    cmdTimeOut.Parameters.AddWithValue("@TimeOut", DateTime.Now.ToString("HH:mm:ss"));
                                                    cmdTimeOut.Parameters.AddWithValue("@Time_Out_Pic", CaptureImage() ?? (object)DBNull.Value);
                                                    cmdTimeOut.Parameters.AddWithValue("@ID", textBox1.Text);

                                                    int rowsAffected = cmdTimeOut.ExecuteNonQuery();
                                                    if (rowsAffected > 0)
                                                    {
                                                        


                                                        // Show the TimeOut notification form
                                                        TimeOutNotif tout = new TimeOutNotif();
                                                        tout.SetUserData(firstname, lastname, designation); // Pass the user data
                                                        tout.ShowDialog();
                                                        textBox1.Text = "ID";
                                                        textBox2.Text = "Password";
                                                        textBox1.ForeColor = Color.Gray;
                                                        textBox2.ForeColor = Color.Gray;
                                                        textBox2.PasswordChar = '\0';
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Failed to record Time Out.");
                                                    }
                                                }
                                            }
                                            else // User has already logged in and out for today
                                            {
                                                // Show the LoginLimit notification form
                                                LoginLimit ll = new LoginLimit();
                                                ll.SetUserData(firstname, lastname, designation); // Pass the user data
                                                ll.ShowDialog();
                                                textBox1.Text = "ID";
                                                textBox2.Text = "Password";
                                                textBox1.ForeColor = Color.Gray;
                                                textBox2.ForeColor = Color.Gray;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {


                        if (loginAttempts >= 1)
                        {
                            // Capture the image from the camera
                            byte[] attemptPicture = CaptureImage();

                            OleDbConnection myConn = new OleDbConnection(connectionString); // Initialize connection
                            OleDbCommand myCmd = null;

                            // Open the connection
                            myConn.Open();

                            // Insert into the Attempt table
                            string insertQuery = "INSERT INTO Attempt (Input_1, Input_2, Attempt_Picture, Time_C, Date_C,Log_type) VALUES (@Insert1, @Insert2, @AttemptPicture, @TimeC, @DateC,@type)";
                            myCmd = new OleDbCommand(insertQuery, myConn); // Use 'myConn' for the command

                            // Add parameters
                            myCmd.Parameters.AddWithValue("@Insert1", textBox1.Text);
                            myCmd.Parameters.AddWithValue("@Insert2", textBox2.Text);
                            myCmd.Parameters.AddWithValue("@AttemptPicture", attemptPicture ?? (object)DBNull.Value);
                            myCmd.Parameters.AddWithValue("@TimeC", DateTime.Now.ToShortTimeString()); // Current time only
                            myCmd.Parameters.AddWithValue("@DateC", DateTime.Now.Date); // Current date only
                            myCmd.Parameters.AddWithValue("@Insert2", label1.Text);

                            // Execute the query
                            int rowsAffected = myCmd.ExecuteNonQuery(); // Execute the command
                            if (rowsAffected > 0)
                            {
                                
                            }

                            // Dispose of the command and close the connection
                            if (myCmd != null)
                            {
                                myCmd.Dispose();
                            }
                            if (myConn.State == ConnectionState.Open)
                            {
                                myConn.Close();
                            }

                            loginAttempts = 0;

                        }

                        loginAttempts++; // Increment login attempts
                        MessageBox.Show("Invalid ID or Password", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Question);

                        textBox1.Text = "ID";
                        textBox2.Text = "Password";
                        textBox1.ForeColor = Color.Gray;
                        textBox2.ForeColor = Color.Gray;
                        textBox2.PasswordChar = '\0';

                        

                    }
                }

               



            }
            













        }

        private void label2_Click(object sender, EventArgs e)
        {


            ForgotPass fp = new ForgotPass();
            fp.ShowDialog();
            
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Gray;
            button1.Font = new Font(button1.Font.FontFamily, 14);

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(160, 188, 239);
            button1.Font = new Font(button1.Font.FontFamily, 20);
        }
    }
}