using crystals_Wpf.Models;
using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf.Pages
{
    public partial class SettingsPage : Page
    {
        private int _movesCount = 30;

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            MusicCheckBox.IsChecked = AppSettings.MusicEnabled;
            SoundCheckBox.IsChecked = AppSettings.SoundEffectsEnabled;
            ConfirmExitCheckBox.IsChecked = AppSettings.ConfirmExit;

            MusicVolumeSlider.Value = AppSettings.MusicVolume;
            SoundVolumeSlider.Value = AppSettings.SoundEffectsVolume;

            _movesCount = AppSettings.DefaultMoves;
            MovesCountText.Text = _movesCount.ToString();
        }

        private void MusicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.MusicEnabled = true;
            SoundService.Instance.MusicEnabled = true;
        }

        private void MusicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.MusicEnabled = false;
            SoundService.Instance.MusicEnabled = false;
        }

        private void SoundCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AppSettings.SoundEffectsEnabled = true;
            SoundService.Instance.SoundEffectsEnabled = true;
        }

        private void SoundCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.SoundEffectsEnabled = false;
            SoundService.Instance.SoundEffectsEnabled = false;
        }

        private void MusicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MusicVolumeSlider != null)
            {
                SoundService.Instance.MusicVolume = MusicVolumeSlider.Value;
            }
        }

        private void SoundVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SoundVolumeSlider != null)
            {
                SoundService.Instance.SoundEffectsVolume = SoundVolumeSlider.Value;
            }
        }

        private void DecreaseMoves_Click(object sender, RoutedEventArgs e)
        {
            if (_movesCount > 5)
            {
                _movesCount -= 5;
                MovesCountText.Text = _movesCount.ToString();
            }
        }

        private void IncreaseMoves_Click(object sender, RoutedEventArgs e)
        {
            if (_movesCount < 100)
            {
                _movesCount += 5;
                MovesCountText.Text = _movesCount.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.MusicEnabled = MusicCheckBox.IsChecked ?? true;
            AppSettings.SoundEffectsEnabled = SoundCheckBox.IsChecked ?? true;
            AppSettings.ConfirmExit = ConfirmExitCheckBox.IsChecked ?? true;
            AppSettings.MusicVolume = MusicVolumeSlider.Value;
            AppSettings.SoundEffectsVolume = SoundVolumeSlider.Value;
            AppSettings.DefaultMoves = _movesCount;

            SoundService.Instance.SaveSettings();

            MessageBox.Show("Настройки сохранены!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            MainWindow.Instance.GetFrame().Navigate(new MainMenuPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new MainMenuPage());
        }
    }
}