using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace ex2
{
    public partial class Form1 : Form
    {
        //Queue<Int32> dataQueue = new Queue<Int32>();
        ConcurrentQueue<Int32> dataQueue = new ConcurrentQueue<Int32>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button_enqueue_Click(object sender, EventArgs e)
        {
            if(int.TryParse(txtEnqueue.Text, out int valueToAdd))
            {
                // if conversion success, add value
                dataQueue.Enqueue(valueToAdd);

                // clear textbox
                txtEnqueue.Clear();
                txtEnqueue.Focus();
            }

            else
            {
                //MessageBox.Show("Why u enter text?!?!");
                MessageBox.Show("Please enter a valid integer.", "Invalid Input", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                txtEnqueue.Clear();
            }
        }

        private void buttonDequeue_Click(object sender, EventArgs e)
        {
            if (dataQueue.TryDequeue(out int valueToRemove))
            {
                txtDequeue.Text = valueToRemove.ToString();
            }
            else
            {
                MessageBox.Show("The queue is empty, cannot dequeue", "Queue Empty", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateQueue()
        {
            txtQueueAmount.Text = dataQueue.Count.ToString();
            txtContent.Text = string.Join(", ", dataQueue);
        }

        private void buttonAverage_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtDequeAverage.Text, out int n) || n <= 0)
            {
                MessageBox.Show("Please enter a positive integer", "Invalid N", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dataQueue.Count < n)
            {
                MessageBox.Show("Not enough data, aborting", "Not enough N", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double sum = 0.0;

            for (int i = 0; i < n; i++)
            {
                dataQueue.TryDequeue(out int value);
                sum += value;
            }

            double average = sum / n;
            textAvgResult.Text = average.ToString("F3");


        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            UpdateQueue();
        }

        private void txtEnqueue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_enqueue.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
    }
}
