using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using SerialPortListener;

namespace Serial_Communication
{
    public partial class SerialCommunication : Form
    {
        String[] PortNames;
        public static SenderEmulator SNDfrm = new SenderEmulator();

        public SerialCommunication()
        {
            InitializeComponent();
            textBox1.ScrollBars = ScrollBars.Vertical;
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
            GetAvailablePortNames();
            //serialPort1.ReadTimeout = 500;
            //serialPort1.WriteTimeout = 500;
        }

        void GetAvailablePortNames()
        {
            PortNames = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(PortNames);
        }

        void RefreshAvailablePortNames()
        {
            comboBox1.Items.Clear();
            PortNames = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(PortNames);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if(comboBox1.Text == "" || comboBox2.Text == "")
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    textBox1.AppendText("[" + date + "] " + "Please select port settings" + "\r\n");
                }
                else
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Open();
                    string CurrentPornName = comboBox1.Text;
                    label5.Text = "Opened " + CurrentPornName;
                    textBox1.AppendText("[" + date + "] " + "Port " + CurrentPornName + " opened" + "\r\n");
                    panel1.BackColor = ColorTranslator.FromHtml("#42f477");
                    textBox2.Enabled = false;
                    button1.Enabled = true;
                    button3.Enabled = false;
                    button4.Enabled = true;
                    textBox2.Enabled = true;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    textBox1.AppendText("[" + date + "] " + "Error: Unauthorized Access" + "\r\n");
                }
                else if(ex is System.IO.IOException)
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    textBox1.AppendText("[" + date + "] " + "Error: IO Exception" + "\r\n");
                    textBox1.AppendText("[" + date + "] " + "Please plug in your device" + "\r\n");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string date = time.ToString(@"hh\:mm\:ss");
            serialPort1.Close();
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
            string CurrentPornName = comboBox1.Text;
            textBox1.AppendText("[" + date + "] " + "Port " + CurrentPornName + " closed" + "\r\n");
            label5.Text = "Closed " + CurrentPornName; ;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = false;
            textBox2.Enabled = false;
            comboBox2.Enabled = true;
            comboBox1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
            try
            {
                if(textBox2.Text == "logclear")
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    serialPort1.WriteLine(textBox2.Text);
                    textBox1.AppendText("[" + date + "] " + "Sending: " + textBox2.Text + "\r\n");
                    textBox2.Text = "";
                    button2.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                DateTime time = DateTime.Now;
                string date = time.ToString(@"hh\:mm\:ss");
                if (ex is System.IO.IOException)
                {
                    textBox1.AppendText("[" + date + "] " + "Error: IO Exception" + "\r\n");
                    textBox1.AppendText("[" + date + "] " + comboBox1.Text + " Port opened, but device unpluged.." + "\r\n");
                }
                else if(ex is System.InvalidOperationException)
                {
                    textBox1.AppendText("[" + date + "] " + "Error: Invalid Operation Exception" + "\r\n");
                    textBox1.AppendText("[" + date + "] " + "Plese Check settings" + "\r\n");
                }
                else if(ex is System.ObjectDisposedException)
                {
                    textBox1.AppendText("[" + date + "] " + "Error: Object Disposed Exception" + "\r\n");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime time = DateTime.Now;
                string date = time.ToString(@"hh\:mm\:ss");
                if (serialPort1.BytesToRead > 0)
                {
                    textBox3.Text = serialPort1.ReadLine();
                    textBox1.AppendText("[" + date + "] " + "Received: " + textBox3.Text + "\r\n");
                }
                else
                    textBox1.AppendText("[" + date + "] " + "Error: There is no data to read" + "\r\n");
            }
            catch (Exception ex)
            {
                DateTime time = DateTime.Now;
                string date = time.ToString(@"hh\:mm\:ss");
                if (ex is TimeoutException)
                {
                    textBox1.AppendText("[" + date + "] " + "Timuot Exception" + "\r\n");
                }
                else if (ex is InvalidOperationException)
                {
                    textBox1.AppendText("[" + date + "] " + "Error: Invalid Operation Exception" + "\r\n");
                    textBox1.AppendText("[" + date + "] " + "Plese Check settings" + "\r\n");
                }
                else if (ex is System.IO.IOException)
                {
                    textBox1.AppendText("[" + date + "] " + "Error: IO Exception" + "\r\n");
                    textBox1.AppendText("[" + date + "] " + comboBox1.Text + " Port opened, but device unpluged.." + "\r\n");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RefreshAvailablePortNames();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SerialCommunication Serial = new SerialCommunication();
            Serial.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //btnscp.Enabled = false;
            DateTime time = DateTime.Now;
            string date = time.ToString(@"hh\:mm\:ss");
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
            string CurrentPornName = comboBox1.Text;
            if(serialPort1.IsOpen)
            {
                serialPort1.Close();
                textBox1.AppendText("[" + date + "] " + "Port " + CurrentPornName + " closed" + "\r\n");
                textBox1.AppendText("[" + date + "] " + "Before start port listening you have to close port connection" + "\r\n");
            }
            button3.Enabled = true;
            comboBox2.Enabled = true;
            comboBox1.Enabled = true;
            MainForm SCPL = new MainForm();
            SCPL.Show();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            
            SNDfrm.Show();
        }
    }
}
