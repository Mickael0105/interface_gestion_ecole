using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MyApp.Interface.Services;

namespace MyApp.Interface
{
    public partial class MainWindow : Window
    {
        private int _failedAttempts = 0;
        private const int MaxFailedAttempts = 4;
        private bool _isPasswordVisible = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            txtMessage.Text = string.Empty;
            var username = txtUsername.Text?.Trim() ?? string.Empty;
            
            // Get password from the visible one if toggled, otherwise from PasswordBox
            var password = _isPasswordVisible ? txtVisiblePassword.Text : pwdPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtMessage.Text = "Nom d'utilisateur et mot de passe sont requis.";
                return;
            }

            var session = AuthenticationService.Authenticate(username, password);
            if (session != null)
            {
                _failedAttempts = 0;

                try
                {
                    // Transition to Welcome/Loading Window
                    var welcome = new WelcomeWindow(session);
                    welcome.Show();

                    this.Close();
                }
                catch (Exception ex)
                {
                    txtMessage.Text = "Erreur : " + ex.Message;
                    Debug.WriteLine(ex.ToString());
                }
            }
            else
            {
                _failedAttempts++;
                var remaining = MaxFailedAttempts - _failedAttempts;

                if (_failedAttempts >= MaxFailedAttempts)
                {
                    MessageBox.Show("Trop de tentatives échouées. L'application va se fermer.", "Verrouillage", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Application.Current.Shutdown();
                    return;
                }

                txtMessage.Text = $"Identifiants incorrects. Restant : {remaining}.";
            }
        }

        private void BtnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text?.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Veuillez entrer votre nom d'utilisateur d'abord.", "Mot de passe oublié", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                 MessageBox.Show("Veuillez contacter le développeur de l'application pour réinitialiser votre accès.", 
                     "Récupération Admin", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Veuillez contacter le responsable pour obtenir votre mot de passe.", 
                    "Récupération Mot de passe", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnTogglePassword_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;

            if (_isPasswordVisible)
            {
                // Show TextBox, Hide PasswordBox
                txtVisiblePassword.Text = pwdPassword.Password;
                txtVisiblePassword.Visibility = Visibility.Visible;
                pwdPassword.Visibility = Visibility.Collapsed;
                // Update icon (strikethrough eye simulation)
                // btnTogglePassword.Content... (Using text emoji for simplicity)
            }
            else
            {
                // Show PasswordBox, Hide TextBox
                pwdPassword.Password = txtVisiblePassword.Text;
                pwdPassword.Visibility = Visibility.Visible;
                txtVisiblePassword.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Êtes-vous sûr de vouloir fermer l'application ?", 
                "Quitter", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // Sync PasswordBox -> TextBox
        private void PwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordVisible)
            {
                if (txtVisiblePassword.Text != pwdPassword.Password)
                    txtVisiblePassword.Text = pwdPassword.Password;
            }
        }

        // Sync TextBox -> PasswordBox
        private void TxtVisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isPasswordVisible)
            {
                if (pwdPassword.Password != txtVisiblePassword.Text)
                    pwdPassword.Password = txtVisiblePassword.Text;
            }
        }
    }
}