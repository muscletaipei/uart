using System;
using System.IO.Ports;
using System.Windows;

namespace Uart_App
{
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();
            LoadAvailablePorts();
            LoadBaudRates();
        }

        private void LoadAvailablePorts()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length > 0)
                {
                    portsComboBox.ItemsSource = ports;
                    portsComboBox.SelectedIndex = 0; // 預設選擇第一個端口
                }
                else
                {
                    MessageBox.Show("未檢測到可用的串口！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取可用端口時出錯：{ex.Message}");
            }
        }

        private void LoadBaudRates()
        {
            int[] baudRates = { 9600, 19200, 38400, 57600, 115200, 1500000 }; // 可用的波特率列表
            baudRateComboBox.ItemsSource = baudRates;
            baudRateComboBox.SelectedIndex = 0; // 預設選擇第一個波特率
        }

        private void OpenPortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedPort = portsComboBox.SelectedItem as string;
                int selectedBaudRate = (int)baudRateComboBox.SelectedItem;
                if (!string.IsNullOrEmpty(selectedPort))
                {
                    _serialPort = new SerialPort(selectedPort, selectedBaudRate);
                    _serialPort.Open();
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    openPortButton.IsEnabled = false;
                    closePortButton.IsEnabled = true;
                    receivedDataTextBox.Text += "Success to open！\n";
                }
                else
                {
                    MessageBox.Show("請選擇一個端口！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打開串口時出錯：{ex.Message}");
            }
        }

        private void ClosePortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    openPortButton.IsEnabled = true;
                    closePortButton.IsEnabled = false;
                    receivedDataTextBox.Text += "Close port！\n";
                }
                else
                {
                    MessageBox.Show("串口未打開！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"關閉串口時出錯：{ex.Message}");
            }
        }

        private void CommandTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendCommand();
                commandTextBox.Text = ""; // 清空發送欄文字
            }
        }

        private void SendCommand()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    string command = commandTextBox.Text;
                    _serialPort.WriteLine(command);
                    receivedDataTextBox.ScrollToEnd(); // 將捲動條滾動到最新位置
                }
                else
                {
                    MessageBox.Show("串口未打開！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"發送指令時出錯：{ex.Message}");
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string dataReceived = sp.ReadExisting();
                Dispatcher.Invoke(() =>
                {
                    receivedDataTextBox.Text += dataReceived;
                    receivedDataTextBox.ScrollToEnd(); // 將捲動條滾動到最新位置
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"處理接收數據時出錯：{ex.Message}");
            }
        }

    }
}
