using System;
using System.Windows;
using System.Windows.Threading;

namespace _1
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private Random random = new Random();

        private string machineStatus = "STOPPED";
        private bool sensorOn = false;
        private bool motorOn = false;

        private bool emergencyStop = false;
        private int sensorWaitCounter = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSystem();
        }

        private void InitializeSystem()
        {
            StatusText.Text = machineStatus;
            UpdateIO();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Emergency Stop Check
                if (emergencyStop)
                    throw new Exception("Emergency Stop Activated");

                // Simulate Communication Failure randomly
                if (random.Next(0, 40) == 10)
                    throw new Exception("Communication Failure");

                // Simulate sensor randomly
                sensorOn = random.Next(0, 2) == 1;
                motorOn = machineStatus == "RUNNING";

                // Sensor timeout simulation
                if (machineStatus == "RUNNING")
                {
                    if (!sensorOn)
                    {
                        sensorWaitCounter++;
                        if (sensorWaitCounter >= 3) // 3 seconds timeout
                            throw new Exception("Sensor Timeout - No Trigger Detected");
                    }
                    else
                    {
                        sensorWaitCounter = 0; // Reset if sensor triggered
                    }
                }

                UpdateUI();
            }
            catch (Exception ex)
            {
                HandleError(ex.Message);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (machineStatus != "ERROR")
            {
                emergencyStop = false;
                machineStatus = "RUNNING";
                timer.Start();
                LogEvent("Machine Started");
                UpdateUI();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            machineStatus = "STOPPED";
            timer.Stop();
            motorOn = false;
            LogEvent("Machine Stopped");
            UpdateUI();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            machineStatus = "STOPPED";
            timer.Stop();
            sensorOn = false;
            motorOn = false;
            emergencyStop = false;
            sensorWaitCounter = 0;
            LogEvent("Machine Reset");
            UpdateUI();
        }

        // Emergency Stop Button (Add this button in XAML)
        private void Emergency_Click(object sender, RoutedEventArgs e)
        {
            emergencyStop = true;
        }

        private void HandleError(string message)
        {
            machineStatus = "ERROR";
            motorOn = false;
            timer.Stop();

            LogEvent("ERROR: " + message);

            MessageBox.Show(message,
                "System Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            UpdateUI();
        }

        private void UpdateUI()
        {
            StatusText.Text = machineStatus;

            switch (machineStatus)
            {
                case "RUNNING":
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                    break;
                case "STOPPED":
                    StatusText.Foreground = System.Windows.Media.Brushes.Blue;
                    break;
                case "ERROR":
                    StatusText.Foreground = System.Windows.Media.Brushes.Red;
                    break;
            }

            UpdateIO();
        }

        private void UpdateIO()
        {
            SensorText.Text = "Sensor: " + (sensorOn ? "ON" : "OFF");
            MotorText.Text = "Motor: " + (motorOn ? "ON" : "OFF");
        }

        private void LogEvent(string message)
        {
            string logEntry = $"{DateTime.Now:HH:mm:ss} - {message}";
            EventLog.Items.Insert(0, logEntry);
        }
    }
}
