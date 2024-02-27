using System;
using System.IO;
using System.IO.Ports;
using System.Windows;

namespace Uart_App
{
    public partial class MainWindow : Window
    {
        SerialPort _serialPort;
        string timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ");
        string logFilePath = @"D:\Logs\received_data.txt"; // 指定的訊息儲存位置

        public MainWindow()
        {
            InitializeComponent();
            LoadAvailablePorts();
            LoadBaudRates();
            // 訂閱窗口關閉事件
            Closing += MainWindow_Closing;
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

        private void DeleteLogFile()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刪除log檔案時出錯：{ex.Message}");
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
                    DeleteLogFile(); // 關閉串口時刪除log檔案

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
                string timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss.fff] ");
                string timestampedData = $"{timestamp}{dataReceived}";// 將時間戳記添加到接收到的訊息中

                Dispatcher.Invoke(() =>
                {
                    receivedDataTextBox.Text += dataReceived;
                    receivedDataTextBox.ScrollToEnd(); // 將捲動條滾動到最新位置
                    SaveReceivedData(dataReceived); // 儲存接收到的訊息到指定位置
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"處理接收數據時出錯：{ex.Message}");
            }
        }

        private void SaveReceivedData(string data)
        {
            try
            {
                if (!File.Exists(logFilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // 創建儲存位置目錄
                    File.WriteAllText(logFilePath, data); // 寫入接收到的訊息到檔案
                }
                else
                {
                    File.AppendAllText(logFilePath, data); // 追加接收到的訊息到檔案
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"儲存接收數據時出錯：{ex.Message}");
            }
        }

        // 新增按鈕點擊事件，用於打開log檔案位置
        private void OpenLogFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{logFilePath}\"");
                }
                else
                {
                    MessageBox.Show("Log file does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening log file location: {ex.Message}");
            }
        }

        // 窗口关闭时删除log文件
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    File.Delete(logFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting log file: {ex.Message}");
            }
        }
    }
}
