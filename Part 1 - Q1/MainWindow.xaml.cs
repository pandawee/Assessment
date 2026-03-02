using System;
using System.Windows;
using System.Windows.Threading;


namespace AutomationMonitor
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private Random random = new Random();

        private string machineStatus = "STOPPED";
        private bool sensorOn = false;
        private bool motorOn = false;

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
            // Simulate sensor randomly
            sensorOn = random.Next(0, 2) == 1;
            motorOn = machineStatus == "RUNNING";

            // Random error simulation
            if (random.Next(0, 20) == 5)
            {
                machineStatus = "ERROR";
                timer.Stop();
                LogEvent("Machine entered ERROR state");
            }

            UpdateUI();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (machineStatus != "ERROR")
            {
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
            LogEvent("Machine Reset");
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
