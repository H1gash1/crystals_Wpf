using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crystals_Wpf.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public bool IsBlocked { get; set; }
    }
}
