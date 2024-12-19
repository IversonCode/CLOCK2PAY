using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLOCK2PAY
{
    public partial class Users : UserControl
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        private int loginAttempts = 0; // Counter for login attempts
        public Users()
        {
            InitializeComponent();
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

        private void Users_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            // Check if any required field is empty or contains placeholder text
            if (string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text == "ID" || string.IsNullOrWhiteSpace(textBox2.Text) || textBox2.Text == "Password")
            {
                MessageBox.Show("Please Fill All Fields");
                return; // Exit the method if validation fails
            }

            // Establish the connection string to connect to the Access database
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // Open the connection to the database
                conn.Open();

                // Query to check if the entered credentials match an admin account
                string adminQuery = "SELECT ID, Admin, Picture FROM Admin WHERE Admin = @Admin AND [Password] = @password";
                using (OleDbCommand adminCmd = new OleDbCommand(adminQuery, conn))
                {
                    // Bind parameters to prevent SQL injection
                    adminCmd.Parameters.AddWithValue("@Admin", textBox1.Text);
                    adminCmd.Parameters.AddWithValue("@password", textBox2.Text);

                    // Execute the query and retrieve the result
                    using (OleDbDataReader reader = adminCmd.ExecuteReader())
                    {
                        if (reader.Read()) // If admin credentials are valid
                        {
                            // Retrieve admin details
                            string adminID = reader["ID"].ToString();
                            string adminName = reader["Admin"].ToString();
                            byte[] adminPicture = (byte[])reader["Picture"]; // Assuming Picture is stored as binary data

                            // Insert into AdminLoginRecord
                            string insertQuery = "INSERT INTO AdminLoginRecord (ID, Admin, L_Date, L_Time_In, Picture) VALUES (@ID, @Admin, @L_Date, @L_Time, @Picture)";
                            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@ID", adminID);
                                insertCmd.Parameters.AddWithValue("@Admin", adminName);
                                insertCmd.Parameters.AddWithValue("@L_Date", DateTime.Now.Date); // Current date
                                insertCmd.Parameters.AddWithValue("@L_Time_In", DateTime.Now.TimeOfDay); // Current time
                                insertCmd.Parameters.AddWithValue("@Picture", adminPicture);

                                // Execute the insert command
                                insertCmd.ExecuteNonQuery();
                            }

                            // Open the admin dashboard form
                            Clock2Pay adminForm = new Clock2Pay();
                            adminForm.lblLogtype.Text = "Admin"; // Set the label text to "Admin"
                            adminForm.LogID.Text = adminID;
                           
                            adminForm.Show();

                            // Clear the textboxes
                            textBox1.Text = "ID";
                            textBox2.Text = "Password";
                            textBox1.ForeColor = Color.Gray;
                            textBox2.ForeColor = Color.Gray;
                            textBox2.PasswordChar = '\0';
                            return; // Exit the method since login is successful
                        }
                       
                    }
                }









                // Query to check if the entered credentials match an admin account
                string adminsuperQuery = "SELECT COUNT(*) FROM AdminSuper WHERE SuperAdmin = @SuperAdmin AND [Password] = @password";
                using (OleDbCommand adminCmd = new OleDbCommand(adminsuperQuery, conn))
                {
                    // Bind parameters to prevent SQL injection
                    adminCmd.Parameters.AddWithValue("@SuperAdmin", textBox1.Text);
                    adminCmd.Parameters.AddWithValue("@password", textBox2.Text);

                    // Execute the query and retrieve the result
                    int isAdminS = (int)adminCmd.ExecuteScalar();
                    if (isAdminS > 0) // If admin credentials are valid
                    {

                        // Open the admin dashboard form
                        Clock2Pay adminForm = new Clock2Pay();
                        adminForm.lblLogtype.Text = "SuperAdmin"; // Set the label text to "Admin"
                        adminForm.Show();

                        // Clear the textboxes
                        textBox1.Text = "ID";
                        textBox2.Text = "Password";
                        textBox1.ForeColor = Color.Gray;
                        textBox2.ForeColor = Color.Gray;
                        textBox2.PasswordChar = '\0';
                        return; // Exit the method since login is successful
                    }
                }


                // Query to check if the entered credentials match a user account
                string userQuery = "SELECT COUNT(*) FROM MasterList WHERE ID = @id AND [Password] = @password";
                using (OleDbCommand userCmd = new OleDbCommand(userQuery, conn))
                {
                    // Bind parameters to prevent SQL injection
                    userCmd.Parameters.AddWithValue("@id", textBox1.Text);
                    userCmd.Parameters.AddWithValue("@password", textBox2.Text);

                    // Execute the query and retrieve the result
                    int isUser = (int)userCmd.ExecuteScalar();
                    if (isUser > 0) // If user credentials are valid
                    {
                        // Query to retrieve user details from the database
                        string getUserDetailsQuery = "SELECT ID, Firstname, Middlename, Lastname, Email, Contact_No, Designation, Picture FROM MasterList WHERE ID = @id";
                        using (OleDbCommand userDetailsCmd = new OleDbCommand(getUserDetailsQuery, conn))
                        {
                            // Bind parameter to prevent SQL injection
                            userDetailsCmd.Parameters.AddWithValue("@id", textBox1.Text);

                            // Execute the query and process the result
                            using (OleDbDataReader reader = userDetailsCmd.ExecuteReader())
                            {
                                if (reader.Read()) // If user details are found
                                {
                                    // Extract user details from the reader
                                    string idno = reader["ID"].ToString();
                                    string firstname = reader["Firstname"].ToString();
                                    string middlename = reader["Middlename"].ToString();
                                    string lastname = reader["Lastname"].ToString();
                                    string email = reader["Email"].ToString();
                                    string contactNo = reader["Contact_No"].ToString();
                                    string desig = reader["Designation"].ToString();
                                    byte[] pictureData = reader["Picture"] as byte[];

                                    // Pass user details to the UserLogPayslip form
                                    UserLogPayslip ulp = new UserLogPayslip();
                                    ulp.SetUserData(idno, firstname, middlename, lastname, email, contactNo, desig, pictureData);
                                    ulp.Show();

                                    // Clear the textboxes
                                    textBox1.Text = "ID";
                                    textBox2.Text = "Password";
                                    textBox1.ForeColor = Color.Gray;
                                    textBox2.ForeColor = Color.Gray;
                                    textBox2.PasswordChar = '\0';
                                    return; // Exit the method since login is successful
                                }
                            }
                        }
                    }
                    else // If neither admin nor user credentials are valid
                    {


                        if (loginAttempts >= 1)
                        {
                            // Initialize the database connection
                            string connectionString = "Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"; // Replace with your actual connection string
                            using (OleDbConnection myConn = new OleDbConnection(connectionString))
                            {
                                // Open the connection
                                myConn.Open();

                                // Insert into the Attempt table
                                string insertQuery = "INSERT INTO Attempt (Input_1, Input_2, Time_C, Date_C,Log_Type) VALUES (@Insert1, @Insert2, @TimeC, @DateC, @type)";
                                using (OleDbCommand myCmd = new OleDbCommand(insertQuery, myConn))
                                {
                                    // Add parameters
                                    myCmd.Parameters.AddWithValue("@Insert1", textBox1.Text);
                                    myCmd.Parameters.AddWithValue("@Insert2", textBox2.Text);
                                    myCmd.Parameters.AddWithValue("@TimeC", DateTime.Now.ToString("HH:mm:ss")); // Current time
                                    myCmd.Parameters.AddWithValue("@DateC", DateTime.Now.ToString("yyyy-MM-dd")); // Current date
                                    myCmd.Parameters.AddWithValue("@Insert2", label3.Text);

                                    // Execute the query
                                    int rowsAffected = myCmd.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                       
                                    }
                                }
                            }

                            // Reset login attempts
                            loginAttempts = 0;
                        }
                       
                            // Increment login attempts
                            loginAttempts++;
                            MessageBox.Show("Invalid ID or Password", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Question);

                            // Reset input fields
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
