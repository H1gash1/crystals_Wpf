using System;

namespace crystals_Wpf.Models
{
    public static class CurrentUser
    {
        public static int Id { get; set; }
        public static string Login { get; set; }
        public static string Role { get; set; }
        public static bool IsLoggedIn { get; set; }

        public static void Clear()
        {
            Id = 0;
            Login = null;
            Role = null;
            IsLoggedIn = false;
        }
    }
}