using crystals_Wpf.Models;
using crystals_Wpf.Services;
using System.Windows;
using System.Windows.Controls;

namespace crystals_Wpf.Pages
{
    public partial class AdminPage : Page
    {
        private AdminService adminService =
            new AdminService();

        public AdminPage()
        {
            InitializeComponent();

            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersGrid.ItemsSource =
                adminService.GetUsers();
        }

        private UserInfo SelectedUser()
        {
            return UsersGrid.SelectedItem
                as UserInfo;
        }

        private void BlockButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            UserInfo user =
                SelectedUser();

            if (user == null)
                return;

            adminService.BlockUser(
                user.Id);

            LoadUsers();
        }

        private void UnblockButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            UserInfo user =
                SelectedUser();

            if (user == null)
                return;

            adminService.UnblockUser(
                user.Id);

            LoadUsers();
        }

        private void MakeAdminButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            UserInfo user =
                SelectedUser();

            if (user == null)
                return;

            adminService.MakeAdmin(
                user.Id);

            LoadUsers();
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