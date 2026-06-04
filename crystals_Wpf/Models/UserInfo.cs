using System;

namespace crystals_Wpf.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Role { get; set; }

        public DateTime RegistrationDate { get; set; }

        public bool IsBlocked { get; set; }
    }
}