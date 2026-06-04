using crystals_Wpf.Models;
using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf.Pages
{
    public partial class LoginPage : Page
    {
        private AuthService authService = new AuthService();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = authService.Login(login, password);

            if (user != null)
            {
                if (user.IsBlocked)
                {
                    MessageBox.Show("Ваш аккаунт заблокирован!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Устанавливаем данные текущего пользователя
                CurrentUser.Id = user.Id;
                CurrentUser.Login = user.Login;
                CurrentUser.Role = user.Role;
                CurrentUser.IsLoggedIn = true;

                // Запускаем фоновую музыку
                SoundService.Instance.PlayMusic();

                // Переходим в главное меню
                MainWindow.Instance.GetFrame().Navigate(new MainMenuPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new RegisterPage());
        }
    }
}