using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf.Pages
{
    public partial class LeaderboardPage : Page
    {
        private RecordService recordService =
            new RecordService();

        public LeaderboardPage()
        {
            InitializeComponent();

            LoadRecords();
        }

        private void LoadRecords()
        {
            RecordsGrid.ItemsSource =
                recordService.GetRecords();
        }

        private void BackButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainWindow.Instance
                .GetFrame()
                .Navigate(
                    new MainMenuPage());
        }
    }
}