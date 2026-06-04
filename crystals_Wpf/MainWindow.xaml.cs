using crystals_Wpf.Pages;
using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;

            MainFrame.Navigate(new LoginPage());

            SoundService.Instance.PlayMusic();
        }

        public Frame GetFrame()
        {
            return MainFrame;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                this.WindowState = this.WindowState == WindowState.Maximized ?
                    WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}