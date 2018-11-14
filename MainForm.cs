using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
        SaveFileDialog _saveToLogFile = new SaveFileDialog();
        public MainForm()
        {
            InitializeComponent();
            UserInitialization();
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
            tbData.Font = new Font(tbData.Font.FontFamily, 12);
        }

        SaveFileDialog saveLog = new SaveFileDialog();
      
        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            DateTime time = DateTime.Now;
            string date = time.ToString(@"hh\:mm\:ss");
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            //if (tbData.TextLength > maxTextLength)
                //tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);
            tbData.AppendText("\n[" + date + "] Received: " + str);
            tbData.ScrollToCaret();

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
            if(_spManager._serialPort.IsOpen)
            {
                panel1.BackColor = ColorTranslator.FromHtml("#42f477");
                label2.Text = "Listening " + _spManager._serialPort.PortName;
            }
            if(_spManager._serialPort.IsOpen)
            {
                portNameComboBox.Enabled = false;
                baudRateComboBox.Enabled = false;
                parityComboBox.Enabled = false;
                dataBitsComboBox.Enabled = false;
                stopBitsComboBox.Enabled = false;
                btnStart.Enabled = false;
            }
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            portNameComboBox.Enabled = true;
            baudRateComboBox.Enabled = true;
            parityComboBox.Enabled = true;
            dataBitsComboBox.Enabled = true;
            stopBitsComboBox.Enabled = true;
            btnStart.Enabled = true;
            _spManager.StopListening();
            label2.Text = "Not Listening";
            panel1.BackColor = ColorTranslator.FromHtml("#f44147");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbData.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime time = DateTime.Now;
                string txt = tbData.Text;
                String[] lst = txt.Split(new Char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int lenght = lst.Length;
                string date = time.ToString(@"yyyy-MM-dd-HH-mm-ss");
                string fileName = date + ".log";
                //var myfile = File.Create(fileName);
                //myfile.Close();
               // string log = tbData.Text;
                _saveToLogFile.Filter = "Log File|*.log";
                _saveToLogFile.Title = "Export to log file";
                _saveToLogFile.FileName = date;
                string path = "";
                BinaryWriter _Bw = null;
                if (_saveToLogFile.ShowDialog() == DialogResult.OK)
                {
                    
                    path = _saveToLogFile.FileName;
                    _Bw = new BinaryWriter(File.Create(path));
                    _Bw.Close();
                }
                for (int i=0; i<lenght; i++)
                {
                    File.AppendAllText(path, lst[i] + "\r\n");
                   // _Bw.Write(lst[i] + "\r\n");
                }
            }
            catch (Exception ex)
            {
                if(ex is System.IO.IOException)
                {
                    MessageBox.Show("Program folder is being used by another process.");
                }
            }
        }
    }
}
