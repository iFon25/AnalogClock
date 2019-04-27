using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnalogClock
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Thread mainThread = null;

        public MainWindow()
        {
            InitializeComponent();
            mainThread = new Thread(new ThreadStart(StartClock));
            mainThread.Start();
        }

        static void Rotate(Line arrow, double angle)
        {   
            if (arrow != null)
            {
                var rotate = new RotateTransform
                {
                    CenterX = 151,
                    CenterY = 151
                };

                double newAngle = 0;

                newAngle = rotate.Angle + angle;

                if (newAngle >= 360) newAngle -= 360;

                rotate.Angle = newAngle;
                arrow.RenderTransform = rotate;

            }
        }

        private static double MinuteAngle(TimeSpan time)
        {
            double milStep = 360d / (60d * 60d * 1000d);
            double secStep = 360d / (60d * 60d);
            double minStep = 360d / 60d;
            double angle = 0;

            angle = minStep * time.Minutes + secStep * time.Seconds + milStep * time.Milliseconds;

            return angle;
        }

        private static double HourAngle(TimeSpan time)
        {
            double secStep = 360d / (60d * 60d * 12d);
            double minStep = 360d / (60d * 12d);
            double hourStep = 360d / 12d;
            double angle = 0;

            int analogHours = time.Hours >= 12 ? time.Hours - 12: time.Hours;

            var angle1 = hourStep * analogHours;
            var angle2 = minStep * time.Minutes;
            var angle3 = secStep * time.Seconds;

            var newangle = angle1 + angle2 + angle3;

            angle = hourStep * analogHours + minStep * time.Minutes + secStep * time.Seconds;

            return angle;
        }

        private bool SetTime(TimeSpan time)
        {
            try
            {
                var startMinAngle = MinuteAngle(time);
                var startHourAngle = HourAngle(time);

                Dispatcher.Invoke(() => {
                    Rotate(MinArrow, startMinAngle);
                    Rotate(HourArrow, startHourAngle);
                });
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return false;
            }

        }

        public void StartClock()
        {
            var inWork = true;
            while (inWork)
            {
                inWork = SetTime(DateTime.Now.TimeOfDay);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mainThread.IsAlive) mainThread.IsBackground = true;
        }

        private void BtnDiffCalculation_Click(object sender, RoutedEventArgs e)
        {
            var rotateMinArrow = MinArrow.RenderTransform as RotateTransform;
            var rotateHourArrow = HourArrow.RenderTransform as RotateTransform;

            var angleMin = rotateMinArrow.Angle;
            var angleHour = rotateHourArrow.Angle;

            var diff = angleHour >= angleMin ? angleHour - angleMin : angleMin - angleHour;

            AngleDiff.Text = (diff > 180d ? 360d - diff : diff).ToString();
        }
    }
}
