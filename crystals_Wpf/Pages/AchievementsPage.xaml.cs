using crystals_Wpf.Models;
using crystals_Wpf.Services;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace crystals_Wpf.Pages
{
    public partial class AchievementsPage : Page
    {
        private AchievementService achievementService = new AchievementService();

        public AchievementsPage()
        {
            InitializeComponent();
            LoadAchievements();
        }

        private void LoadAchievements()
        {
            var achievements = achievementService.GetUserAchievements(CurrentUser.Id);
            AchievementsGrid.ItemsSource = achievements;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SoundService.Instance.PlayClickSound();
            MainWindow.Instance.GetFrame().Navigate(new MainMenuPage());
        }
    }

    // Конвертеры - должны быть в том же пространстве имен
    public class UnlockedToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUnlocked = value is bool && (bool)value;

            var style = new Style(typeof(Border));

            if (isUnlocked)
            {
                style.Setters.Add(new Setter(Border.MarginProperty, new Thickness(8)));
                style.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(15)));
                style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(12)));
                style.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromArgb(230, 255, 255, 255))));
                style.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(78, 205, 196))));
                style.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(2)));
            }
            else
            {
                style.Setters.Add(new Setter(Border.MarginProperty, new Thickness(8)));
                style.Setters.Add(new Setter(Border.PaddingProperty, new Thickness(15)));
                style.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(12)));
                style.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));
                style.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(204, 204, 204))));
                style.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1)));
                style.Setters.Add(new Setter(Border.OpacityProperty, 0.6));
            }

            return style;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUnlocked = value is bool && (bool)value;
            return isUnlocked ? new SolidColorBrush(Color.FromRgb(51, 51, 51)) : new SolidColorBrush(Color.FromRgb(153, 153, 153));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUnlocked = value is bool && (bool)value;
            return isUnlocked ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isUnlocked = value is bool && (bool)value;
            return isUnlocked ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}