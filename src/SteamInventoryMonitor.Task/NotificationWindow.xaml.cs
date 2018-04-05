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
        bool pressed;

        public event Action Clicked;

        public string NotificationTitle
        {
            get { return GetValue(NotificationTitleProperty) as string; }
            set { SetValue(NotificationTitleProperty, value); }
        }
        public static readonly DependencyProperty NotificationTitleProperty = DependencyProperty.Register("NotificationTitle", typeof(string), typeof(NotificationWindow), null);

        public string NotificationMsg
        {
            get { return GetValue(NotificationMsgProperty) as string; }
            set { SetValue(NotificationMsgProperty, value); }
        }
        public static readonly DependencyProperty NotificationMsgProperty = DependencyProperty.Register("NotificationMsg", typeof(string), typeof(NotificationWindow), null);

        public string NotificationIcon
        {
            get { return GetValue(NotificationIconProperty) as string; }
            set { SetValue(NotificationIconProperty, value); }
        }
        public static readonly DependencyProperty NotificationIconProperty = DependencyProperty.Register("NotificationIcon", typeof(string), typeof(NotificationWindow), null);

        public NotificationWindow()
        {
            InitializeComponent();
            DataContext = this;

            player.Load();
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, App.NOTIFICATION_DELAY_S);
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
        private void gridMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => pressed = true;
        private void gridMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (pressed)
            {
                pressed = false;
                Clicked?.Invoke();
            }
        }
        private void gridMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => pressed = false;
    }
}
