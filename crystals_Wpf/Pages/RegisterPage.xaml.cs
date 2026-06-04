using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf.Pages
{
    public partial class RegisterPage : Page
    {
        private AuthService authService =
            new AuthService();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string login =
                LoginTextBox.Text.Trim();

            string password =
                PasswordBox.Password.Trim();

            if (login == "" || password == "")
            {
                MessageBox.Show(
                    "Заполните все поля.");
                return;
            }

            bool result =
                authService.Register(
                    login,
                    password);

            if (result)
            {
                MessageBox.Show(
                    "Аккаунт успешно создан.");

                MainWindow.Instance
                    .GetFrame()
                    .Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show(
                    "Пользователь уже существует.");
            }
        }

        private void BackButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainWindow.Instance
                .GetFrame()
                .Navigate(new LoginPage());
        }
    }
}