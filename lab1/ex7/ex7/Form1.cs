using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1_ex7
{
    public partial class Form1 : Form
    {
        int current_state = 0;
        int timeout_counter = 0;

        const int THRESHOLD = 160;
        const int TIMEOUT_LIMIT = 3;
        const int DISPLAY_TIME = 10;

        public Form1()
        {
            InitializeComponent();
            this.AcceptButton = process_btn;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int ax = 0, ay= 0, az = 0;
            int.TryParse(ax_tb.Text, out ax);
            int.TryParse(ay_tb.Text, out ay);
            int.TryParse(az_tb.Text, out az);
            
            string history = $"({ax}, {ay}, {az}, {current_state})" +  Environment.NewLine;
            history_tb.AppendText(history);

            switch (current_state)
            {
                case 0:
                    if (az > THRESHOLD)
                    {
                        current_state = 1;
                        timeout_counter = 0;
                    }

                    else if (ax > THRESHOLD)
                    {
                        current_state = 2;
                        timeout_counter = 0;
                    }
                    break;
                
                case 1:
                    if (ax > THRESHOLD)
                    {
                        current_state = 11; // High Punch
                        timeout_counter = 0;
                    }
                    else if (timeout_counter > TIMEOUT_LIMIT)
                        current_state = 0;
                    else
                        timeout_counter++;
                    break;
                
                case 2:
                    if (ay > THRESHOLD)
                    {
                        current_state = 3;
                        timeout_counter = 0;
                    }
                    else if (timeout_counter > TIMEOUT_LIMIT)
                    { current_state = 10; 
                      timeout_counter = 0;
                    }
                    else
                    {
                        timeout_counter++;
                    }
                    break;

                case 3:
                    if (az > THRESHOLD)
                    {
                        current_state = 12;
                        timeout_counter = 0;
                    }
                    else if (timeout_counter > TIMEOUT_LIMIT)
                    {
                        current_state = 0;
                        timeout_counter = 0;
                    }
                    else
                        timeout_counter++;
                    break;

                case 10: // Display "Simple Punch"  
                    //if (timeout_counter == 0)
                    history_tb.AppendText("Simple Punch\n");
                    // This state just waits for its display time to run out, then resets.
                    if (timeout_counter > DISPLAY_TIME) { current_state = 0; } else { timeout_counter++; }
                    break;

                case 11: // Display "High Punch"
                    //if (timeout_counter == 0)
                    history_tb.AppendText("HIGH PUNCH\n");
                    if (timeout_counter > DISPLAY_TIME) { current_state = 0; } else { timeout_counter++; }
                    break;

                case 12: // Display "Right-hook"
                    //if (timeout_counter == 0)
                    history_tb.AppendText("RIGHT-HOOK\n");
                    if (timeout_counter > DISPLAY_TIME) { current_state = 0; } else { timeout_counter++; }
                    break;
            }

            state_tb.Text = current_state.ToString();
        }
        }
    }
    

