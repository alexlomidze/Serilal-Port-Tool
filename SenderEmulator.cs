using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace Serial_Communication
{
    public partial class SenderEmulator : Form
    {
        public SenderEmulator()
        {
            InitializeComponent();
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
            panel2.BackColor = ColorTranslator.FromHtml("#f44147");
            textBox2.ScrollBars = ScrollBars.Vertical;
            GetAvailablePortNames();
        }
        String[] PortNames;
        string filePath = "";
        string psnd = "";
        bool psndFile;
        bool breakState = true;
        private BackgroundWorker _worker = null;
        

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
        
        void Delay(int a)
        {
            Thread.Sleep(a);
        }
        string getDataTime()
        {
            DateTime time = DateTime.Now;
            string date = time.ToString(@"hh\:mm\:ss");
            return date;
        }

        public static Task Delay(double milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) =>
            {
                tcs.TrySetResult(true);
            };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }
        
        private static Task<int> SleepAsync(int ms)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(ms);
                return ms / 1000;
            });
        }
         
            

        private void button1_Click(object sender, EventArgs e)
        {
            PSND_FILE.Filter ="PSND File|*.PSND";
            if (PSND_FILE.ShowDialog() == DialogResult.OK)
            {
                filePath = PSND_FILE.FileName;
            }
            textBox1.Text = filePath;
        }

        public async void button3_Click(object sender, EventArgs e)
        {
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            label2.Text = "Sending";
            if(serialPort1.IsOpen)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
            }
            panel1.BackColor = ColorTranslator.FromHtml("#42f477");
            psndFile = true;
            DateTime time = DateTime.Now;
            string date = time.ToString(@"hh\:mm\:ss");
            try
            {
                using (StreamReader file = new StreamReader(filePath))
                {
                    var lineCount = File.ReadAllLines(filePath).Length;
                    if(serialPort1.IsOpen)
                    {
                        for (int i = 0; i < lineCount; i++)
                        {
                            if (i == 0 && serialPort1.IsOpen)
                            {
                                if (psndFile)
                                {
                                    //while (psnd != null)
                                    {
                                        psnd = file.ReadLine();
                                        if (psnd != null)
                                        {
                                            if (psnd == "#STARTPSND Start Sending")
                                            {
                                                psndFile = true;
                                                textBox2.AppendText(psnd + "\r\n");
                                            }
                                            
                                            else
                                            {
                                                psndFile = false;
                                                textBox2.AppendText("[" + date + "] " + "Damaged PSND file\r\n");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else Dispose();
                            if (psndFile && i != lineCount - 1 && serialPort1.IsOpen)
                            {
                                if (psndFile)
                                {
                                    while (psnd != null)
                                    {
                                        psnd = file.ReadLine();
                                        if (psnd != null)
                                        {
                                            string h = h = getDataTime();
                                            Thread delayTread = new Thread(() =>
                                            {
                                                h = getDataTime();
                                            });
                                            delayTread.Start();
                                            try
                                            {
                                                if (serialPort1.IsOpen && psnd != "#ENDPSND End Sending")
                                                {
                                                    textBox2.AppendText("[" + h + "] " + "Sending: " + psnd + "\r\n");
                                                    serialPort1.WriteLine(psnd);
                                                }
                                                else if (psnd == "#ENDPSND End Sending")
                                                {
                                                    textBox2.AppendText(psnd + "\r\n");
                                                    comboBox1.Enabled = true;
                                                    comboBox2.Enabled = true;
                                                    comboBox3.Enabled = true;
                                                    comboBox4.Enabled = true;
                                                    comboBox5.Enabled = true;
                                                    button5.Enabled = true;
                                                    button6.Enabled = true;
                                                    label2.Text = "Sending Finished";
                                                    groupBox1.Enabled = true;
                                                    groupBox2.Enabled = true;
                                                }
                                                else break;
                                                
                                            }
                                            catch (Exception ex)
                                            {
                                                if (ex is InvalidOperationException)
                                                    MessageBox.Show("Invalid Operation");
                                            }
                                            int delay = Convert.ToInt32(comboBox6.Text);
                                            int seconds = await SleepAsync(delay);
                                        }
                                    }
                                }
                            }
                            else break;
                            if (i == lineCount - 1 && serialPort1.IsOpen)
                            {
                                groupBox1.Enabled = true;
                                groupBox2.Enabled = true;
                                label2.Text = "Sending Finished";
                            }
                            else break;
                            if (!breakState)
                                break;
                        
                        }//for
                    }
                    else
                    {
                        textBox2.AppendText("[" + date + "] " + "Open the port!\r\n");
                    }
                }//using
            }
            catch(ArgumentException)
            {
                textBox2.AppendText("[" + date + "] " + "Please open file" + "\r\n");
            }
        }
        

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "" || comboBox5.Text == "")
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    textBox2.AppendText("[" + date + "] " + "Please select port settings" + "\r\n");
                }
                else
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    switch(comboBox3.Text)
                    {
                        case "5":
                            serialPort1.DataBits = 5;
                            break;
                        case "6":
                            serialPort1.DataBits = 6;
                            break;
                        case "7":
                            serialPort1.DataBits = 7;
                            break;
                        case "8":
                            serialPort1.DataBits = 8;
                            break;
                        default:
                            serialPort1.DataBits = 5;
                            break;
                    }
                    switch(comboBox4.Text)
                    {
                        case "None":
                            serialPort1.Parity = Parity.None;
                            break;
                        case "Odd":
                            serialPort1.Parity = Parity.Odd;
                            break;
                        case "Even":
                            serialPort1.Parity = Parity.Even;
                            break;
                        case "Mark":
                            serialPort1.Parity = Parity.Mark;
                            break;
                        case "Space":
                            serialPort1.Parity = Parity.Space;
                            break;
                    }
                    switch(comboBox5.Text)
                    {
                        case "One":
                            serialPort1.StopBits = StopBits.One;
                            break;
                        case "Two":
                            serialPort1.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            serialPort1.StopBits = StopBits.OnePointFive;
                            break;
                    }
                    serialPort1.Open();
                    string CurrentPornName = comboBox1.Text;
                    label11.Text = "Opened " + CurrentPornName;
                    panel2.BackColor = ColorTranslator.FromHtml("#42f477");
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    button5.Enabled = false;
                    button6.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    textBox2.AppendText("[" + date + "] " + "Error: Unauthorized Access" + "\r\n");
                }
                else if (ex is System.IO.IOException)
                {
                    DateTime time = DateTime.Now;
                    string date = time.ToString(@"hh\:mm\:ss");
                    textBox2.AppendText("[" + date + "] " + "Error: IO Exception" + "\r\n");
                    textBox2.AppendText("[" + date + "] " + "Please plug in your device" + "\r\n");
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RefreshAvailablePortNames();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel2.BackColor = ColorTranslator.FromHtml("#f44147");
            string CurrentPornName = comboBox1.Text;
            label11.Text = "Closed " + CurrentPornName; ;
            serialPort1.Close();
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
            comboBox5.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
            comboBox5.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            label2.Text = "Sending Stopped";
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
            panel2.BackColor = ColorTranslator.FromHtml("#f44147");
            string CurrentPornName = comboBox1.Text;
            textBox2.AppendText("Closed " + CurrentPornName + "\r\n");
            label11.Text = "Closed " + CurrentPornName; ;
            breakState = false;
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void SenderEmulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            this.Parent = null;
            e.Cancel = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "PSND File|*.PSND";
            saveFileDialog1.Title = "Create Sender Emulator File PSND";
            string path = "";
            BinaryWriter _Bw = null;
            string start = "#STARTPSND Start Sending";
            string content1 = "//>>";
            string content2 = "//>> Please clear this 3 line and type your data/commands";
            string content3 = "//>>";
            string end = "#ENDPSND End Sending";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog1.FileName;
                _Bw = new BinaryWriter(File.Create(path));
                _Bw.Close();
            }
            File.AppendAllText(path, start + "\r\n");
            File.AppendAllText(path, content1 + "\r\n");
            File.AppendAllText(path, content2 + "\r\n");
            File.AppendAllText(path, content3 + "\r\n");
            File.AppendAllText(path, end + "\r\n");
            textBox3.Text = path;
        }
    }
}
