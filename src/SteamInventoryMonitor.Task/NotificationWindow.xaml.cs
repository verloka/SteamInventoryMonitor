using System;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace SteamInventoryMonitor.Task
{
    public partial class NotificationWindow : Window
    {
        SoundPlayer player = new SoundPlayer("Sounds/open-ended.wav");
        DispatcherTimer timer;

        public NotificationWindow()
        {
            InitializeComponent();
            player.Load();
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 4);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (cbHide.IsChecked.Value)
                Close();
            else
            {
                cbHide.IsChecked = true;
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Start();
            }
        }
        private void btnCloseNotificationClick(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            cbHide.IsChecked = true;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            Top = SystemParameters.VirtualScreenHeight / 2 - 55;
            Left = SystemParameters.VirtualScreenWidth - 400;

            timer.Start();
            player.Play();
        }
    }
}
