using System;

namespace crystals_Wpf.Models
{
    public class UserAchievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool IsUnlocked { get; set; }
        public string UnlockDate { get; set; }
        public string DateVisibility => IsUnlocked ? "Visible" : "Collapsed";
    }
}