namespace crystals_Wpf.Models
{
    public static class AppSettings
    {
        public static bool MusicEnabled = true;
        public static bool SoundEffectsEnabled = true;
        public static bool ConfirmExit = true;

        // Громкость (0.0 - 1.0)
        public static double MusicVolume = 0.5;
        public static double SoundEffectsVolume = 0.5;

        // Усилители
        public static int ExtraMovesCount = 3;
        public static int TimeFreezeCount = 2;
        public static int ShuffleCount = 2;
        public static int DefaultMoves = 30;
    }
}