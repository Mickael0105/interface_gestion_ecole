using System;
using System.Windows;
using System.Windows.Threading;
using MyApp.Interface.Services;

namespace MyApp.Interface
{
    public partial class WelcomeWindow : Window
    {
        private readonly UserSession _session;
        private readonly DispatcherTimer _timer;

        public WelcomeWindow(UserSession session)
        {
            InitializeComponent();
            _session = session;

            txtWelcome.Text = $"Bienvenue, {session.Username}";
            txtRole.Text = $"Accès {session.Role}";

            // Simulate loading
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _timer.Stop();
            
            // Open HomeWindow
            var home = new HomeWindow(_session);
            home.Show();
            
            this.Close();
        }
    }
}
