using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CLOCK2PAY
{
    public partial class LoginLimit : Form
    {

        private Timer countdownTimer; // Timer to manage the countdown
        private int countdownTime; // Countdown time in seconds

       

        public LoginLimit()
        {
            InitializeComponent();

            

            countdownTime = 5; // Set countdown time in seconds
            countdownTimer = new Timer();
            countdownTimer.Interval = 1000; // Set interval to 1 second (1000 milliseconds)
            countdownTimer.Tick += new EventHandler(timer1_Tick); // Subscribe to the Tick event
            countdownTimer.Start(); // Start the timer

        }

        public void SetUserData(string firstname, string lastname, string designation)
        {
            label1.Text = $"{firstname} {lastname} You Haved Reach the Limit of attendace for today"; // Combine Firstname and Lastname
            label4.Text = "Position:  " + designation;
        }


        private void LoginLimit_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            countdownTime--; // Decrease the countdown time by 1 second

            // Update any UI element, like a Label, to show remaining time
            label3.Text = countdownTime.ToString() + " Seconds Remaining"; // Assuming labelTime is a Label control

            if (countdownTime <= 0)
            {
                countdownTimer.Stop(); // Stop the timer
                countdownTimer.Dispose(); // Dispose of the timer if no longer needed

                // Show the new form (assuming Form3 is the next form to display)

                this.Close();




            }
        }
    }
}
