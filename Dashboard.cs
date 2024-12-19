using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CLOCK2PAY
{
    public partial class Dashboard : UserControl
    {
        OleDbConnection conn; // Manages the connection to the Access database
        OleDbCommand cmd; // Executes SQL commands
        OleDbDataAdapter adapter; // Bridges data between Access and the application
        DataTable dt; // Stores data in-memory for binding to controls

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            // Load users and update the resignation date count on form load
            GetUsers();
            UpdateResignDateCount();
            UpdateTimeInCount();
            UpdateTimeOutCount();
            UpdateLateCount();
            UpdateOnTimeCount();
            UpdateOvertimeCount();

            // Set current date and time
            label11.Text = DateTime.Now.ToString("MMMM dd, yyyy");
            timer1.Start();

            // Initialize the chart after label4 is updated
            InitializeChart();
        }

        private void InitializeChart()
        {
            // Clear any existing series
            chart1.Series.Clear();

            // Create a new series
            Series series = new Series
            {
                Name = "SampleSeries",
                ChartType = SeriesChartType.Column, // Use 'Column' for vertical bars
                XValueType = ChartValueType.String, // X-Axis value type
                YValueType = ChartValueType.Double  // Y-Axis value type
            };

            // Add the series to the chart
            chart1.Series.Add(series);

            

            // Ensure the label's text is numeric and assign it to the "Employees" bar
            int employeesValue = 0;
            if (int.TryParse(label4.Text, out employeesValue))
            {
                // Add "Employees" with dynamic value from Label4
                series.Points.AddXY("Employees", employeesValue);
            }
            else
            {
                // Default to 0 if the label doesn't contain a valid number
                series.Points.AddXY("Employees", 0);
            }

            // Ensure the label's text is numeric and assign it to the "Employees" bar
            int timeinvalue = 0;
            if (int.TryParse(label2.Text, out timeinvalue))
            {
                // Add "Employees" with dynamic value from Label4
                series.Points.AddXY("Time_In", timeinvalue);
            }
            else
            {
                // Default to 0 if the label doesn't contain a valid number
                series.Points.AddXY("Time_In", 0);
            }

            // Ensure the label's text is numeric and assign it to the "Employees" bar
            int timeoutvalue = 0;
            if (int.TryParse(label5.Text, out timeoutvalue))
            {
                // Add "Employees" with dynamic value from Label4
                series.Points.AddXY("Time_Out", timeoutvalue);
            }
            else
            {
                // Default to 0 if the label doesn't contain a valid number
                series.Points.AddXY("Time_Out", 0);
            }

            // Ensure the label's text is numeric and assign it to the "Employees" bar
            int otvalue = 0;
            if (int.TryParse(label14.Text, out otvalue))
            {
                // Add "Employees" with dynamic value from Label4
                series.Points.AddXY("On Time", otvalue);
            }
            else
            {
                // Default to 0 if the label doesn't contain a valid number
                series.Points.AddXY("OnTIme", 0);
            }

            // Ensure the label's text is numeric and assign it to the "Employees" bar
            int latevalue = 0;
            if (int.TryParse(label7.Text, out latevalue))
            {
                // Add "Employees" with dynamic value from Label4
                series.Points.AddXY("Late", latevalue);
            }
            else
            {
                // Default to 0 if the label doesn't contain a valid number
                series.Points.AddXY("Late", 0);
            }

            // Ensure the label's text is numeric and assign it to the "Employees" bar
            int overvalue = 0;
            if (int.TryParse(label9.Text, out overvalue))
            {
                // Add "Employees" with dynamic value from Label4
                series.Points.AddXY("Overtime", overvalue);
            }
            else
            {
                // Default to 0 if the label doesn't contain a valid number
                series.Points.AddXY("Overtime", 0);
            }



           

            // Set the color of the "Employees" bar to DarkGray
            series.Points[0].Color = Color.DarkGray;

            // Set the colors of the other bars
            series.Points[1].Color = Color.FromArgb(84, 130, 53);
            series.Points[2].Color = Color.FromArgb(197, 90, 17);  // Example for Time_Out
            series.Points[3].Color = Color.SeaGreen;  // Example for Overtime
            series.Points[4].Color = Color.FromArgb(204, 51, 0);  // Example for Late
            series.Points[5].Color = Color.DarkGray;  // Example for Overtime

            // Customize the chart appearance (optional)
            chart1.ChartAreas[0].AxisX.Title = "Categories";
            chart1.ChartAreas[0].AxisY.Title = "Values";

            // Force refresh the chart to ensure updates are shown
            chart1.Invalidate();
        }

        public void GetUsers()
        {
            conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb");
            dt = new DataTable();
            adapter = new OleDbDataAdapter("SELECT * FROM MasterList", conn);
            conn.Open();
            adapter.Fill(dt);
            conn.Close();
        }

        private void UpdateResignDateCount()
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // SQL query to count rows where Resign_Date is NULL
                string query = "SELECT COUNT(*) FROM MasterList WHERE Resign_Date IS NULL";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    // Execute the query and get the count
                    var result = cmd.ExecuteScalar();

                    // Set the result to Label4 (this will be displayed on the form)
                    label4.Text = result.ToString();
                }
            }
        }

        private void UpdateOvertimeCount()
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // SQL query to count rows where Overtime is not NULL and the date is today
                string query = "SELECT COUNT(*) FROM Time_In_Out WHERE C_Date = @TodayDate AND Out_Status = 'Overtime'";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameter to filter by today's date
                    cmd.Parameters.AddWithValue("@Today", DateTime.Now.Date);

                    conn.Open();
                    // Execute the query and get the count
                    var result = cmd.ExecuteScalar();

                    // Set the result to Label4 (this will be displayed on the form)
                    label9.Text = result.ToString();
                }
            }
        }



        private void UpdateOnTimeCount()
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // Get today's date formatted for comparison with the C_Date column (assuming C_Date is Date/Time type)
                string todayDate = DateTime.Now.ToString("MM/dd/yyyy"); // Adjust format based on your database

                // SQL query to count rows where C_Date matches today's date and In_Status is "On time"
                string query = "SELECT COUNT(*) FROM Time_In_Out WHERE C_Date = @TodayDate AND In_Status = 'On time'";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@TodayDate", todayDate);

                    conn.Open();
                    // Execute the query and get the count of records where C_Date equals today's date and In_Status is "On time"
                    var result = cmd.ExecuteScalar();

                    // Set the result to Label3 (this will be displayed on the form)
                    label14.Text = result.ToString();
                }
            }
        }

        private void UpdateLateCount()
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // Get today's date formatted for comparison with the C_Date column (assuming C_Date is Date/Time type)
                string todayDate = DateTime.Now.ToString("MM/dd/yyyy"); // Adjust format based on your database

                // SQL query to count rows where C_Date matches today's date and In_Status is "Late"
                string query = "SELECT COUNT(*) FROM Time_In_Out WHERE C_Date = @TodayDate AND In_Status = 'Late'";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@TodayDate", todayDate);

                    conn.Open();
                    // Execute the query and get the count of records where C_Date equals today's date and In_Status is "Late"
                    var result = cmd.ExecuteScalar();

                    // Set the result to Label2 (this will be displayed on the form)
                    label7.Text = result.ToString();
                }
            }
        }

        private void UpdateTimeInCount()
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // Get today's date formatted for comparison with the C_Date column (assuming C_Date is Date/Time type)
                string todayDate = DateTime.Now.ToString("MM/dd/yyyy"); // Adjust format based on your database

                // SQL query to count rows where C_Date matches today's date
                string query = "SELECT COUNT(*) FROM Time_In_Out WHERE C_Date = @TodayDate";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@TodayDate", todayDate);

                    conn.Open();
                    // Execute the query and get the count of records where C_Date equals today's date
                    var result = cmd.ExecuteScalar();

                    // Set the result to Label2 (this will be displayed on the form)
                    label2.Text = result.ToString();
                }
            }
        }

        private void UpdateTimeOutCount()
        {
            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OleDb.12.0;Data Source=CLOCK2PAYDB.accdb"))
            {
                // Get today's date formatted for comparison with the C_Date column (assuming C_Date is Date/Time type)
                string todayDate = DateTime.Now.ToString("MM/dd/yyyy"); // Adjust format based on your database

                // SQL query to count rows where C_Date matches today's date and Time_Out is not NULL
                string query = "SELECT COUNT(*) FROM Time_In_Out WHERE C_Date = @TodayDate AND Time_Out IS NOT NULL";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameter to prevent SQL injection
                    cmd.Parameters.AddWithValue("@TodayDate", todayDate);

                    conn.Open();
                    // Execute the query and get the count of records where C_Date equals today's date and Time_Out is not NULL
                    var result = cmd.ExecuteScalar();

                    // Set the result to Label5 (this will be displayed on the form)
                    label5.Text = result.ToString();
                }
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update time every tick (e.g., on a timer)
            label12.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Event for label4 click (can be used if needed)
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            // Event for chart1 click (can be used if needed)
        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
