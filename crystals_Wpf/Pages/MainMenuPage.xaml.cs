using crystals_Wpf.Models;
using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf.Pages
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            Loaded += MainMenuPage_Loaded;
        }

        private void MainMenuPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.IsLoggedIn && !string.IsNullOrEmpty(CurrentUser.Login))
            {
                WelcomeText.Text = $"Добро пожаловать, {CurrentUser.Login}!";
            }
            else
            {
                WelcomeText.Text = "Добро пожаловать!";
            }

            // Показываем кнопку администрирования только для админов
            if (CurrentUser.Role == "Admin")
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new GamePage());
        }

        private void AchievementsButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new AchievementsPage());
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new LeaderboardPage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new SettingsPage());
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new AdminPage());
        }

        // Новый метод для выхода из аккаунта
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();

            var result = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Останавливаем музыку
                SoundService.Instance.StopMusic();

                // Очищаем данные текущего пользователя
                CurrentUser.Clear();

                // Переходим на страницу входа
                MainWindow.Instance.GetFrame().Navigate(new LoginPage());
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();

            var result = MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}