using crystals_Wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace crystals_Wpf.Services
{
    public class SoundService
    {
        private static SoundService _instance;
        private MediaPlayer _musicPlayer;
        private bool _musicEnabled;
        private bool _soundEffectsEnabled;
        private double _musicVolume = 0.5;
        private double _soundEffectsVolume = 0.5;
        private Dictionary<string, MediaPlayer> _soundEffects;
        private string _soundsPath;

        private SoundService()
        {
            _musicPlayer = new MediaPlayer();
            _soundEffects = new Dictionary<string, MediaPlayer>();
            InitializeSoundsPath();
            LoadSettings();
        }

        public static SoundService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SoundService();
                return _instance;
            }
        }

        private void InitializeSoundsPath()
        {
            string[] possiblePaths = new string[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sounds"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "assets", "sounds"),
            };

            foreach (var path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (Directory.Exists(fullPath))
                {
                    _soundsPath = fullPath;
                    System.Diagnostics.Debug.WriteLine($"Найдена папка со звуками: {_soundsPath}");
                    break;
                }
            }

            if (string.IsNullOrEmpty(_soundsPath))
            {
                _soundsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sounds");
            }
        }

        private void LoadSettings()
        {
            _musicEnabled = AppSettings.MusicEnabled;
            _soundEffectsEnabled = AppSettings.SoundEffectsEnabled;
            _musicVolume = AppSettings.MusicVolume;
            _soundEffectsVolume = AppSettings.SoundEffectsVolume;

            _musicPlayer.Volume = _musicVolume;
        }

        public void SaveSettings()
        {
            AppSettings.MusicEnabled = _musicEnabled;
            AppSettings.SoundEffectsEnabled = _soundEffectsEnabled;
            AppSettings.MusicVolume = _musicVolume;
            AppSettings.SoundEffectsVolume = _soundEffectsVolume;
        }

        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                if (!value)
                    StopMusic();
                else
                    PlayMusic();
                SaveSettings();
            }
        }

        public bool SoundEffectsEnabled
        {
            get => _soundEffectsEnabled;
            set
            {
                _soundEffectsEnabled = value;
                SaveSettings();
            }
        }

        public double MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Max(0, Math.Min(1, value));
                _musicPlayer.Volume = _musicVolume;
                SaveSettings();
            }
        }

        public double SoundEffectsVolume
        {
            get => _soundEffectsVolume;
            set
            {
                _soundEffectsVolume = Math.Max(0, Math.Min(1, value));
                SaveSettings();
            }
        }

        public void PlayMusic()
        {
            if (!_musicEnabled) return;

            try
            {
                string musicPath = Path.Combine(_soundsPath, "background.mp3");

                if (!File.Exists(musicPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Файл музыки не найден: {musicPath}");
                    return;
                }

                _musicPlayer.Open(new Uri(musicPath, UriKind.Absolute));
                _musicPlayer.Volume = _musicVolume;
                _musicPlayer.MediaEnded += (s, e) =>
                {
                    _musicPlayer.Position = TimeSpan.Zero;
                    _musicPlayer.Play();
                };
                _musicPlayer.Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка воспроизведения музыки: {ex.Message}");
            }
        }

        public void StopMusic()
        {
            _musicPlayer.Stop();
        }

        public void PlaySound(string soundName)
        {
            if (!_soundEffectsEnabled)
            {
                return;
            }

            try
            {
                string soundPath = Path.Combine(_soundsPath, $"{soundName}.mp3");

                if (!File.Exists(soundPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Файл звука не найден: {soundPath}");
                    return;
                }

                if (!_soundEffects.ContainsKey(soundName))
                {
                    var player = new MediaPlayer();
                    player.Open(new Uri(soundPath, UriKind.Absolute));
                    _soundEffects[soundName] = player;
                }

                _soundEffects[soundName].Stop();
                _soundEffects[soundName].Position = TimeSpan.Zero;
                _soundEffects[soundName].Volume = _soundEffectsVolume;
                _soundEffects[soundName].Play();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка воспроизведения звука {soundName}: {ex.Message}");
            }
        }

        public void PlayAchievementUnlock()
        {
            PlaySound("achievement");
        }

        public void PlayMatchSound()
        {
            PlaySound("match");
        }

        public void PlaySwapSound()
        {
            PlaySound("swap");
        }

        public void PlayGameOver()
        {
            PlaySound("gameover");
        }

        public void PlayPowerUpSound()
        {
            PlaySound("powerup");
        }

        public void PlayClickSound()
        {
            PlaySound("click");
        }

        public void PlayComboSound(int comboLength)
        {
            if (!_soundEffectsEnabled) return;

            string soundName;
            if (comboLength <= 1)
                soundName = "combo1";
            else if (comboLength == 2)
                soundName = "combo2";
            else if (comboLength == 3)
                soundName = "combo3";
            else if (comboLength == 4)
                soundName = "combo4";
            else if (comboLength == 5)
                soundName = "combo5";
            else
                soundName = "combo6";

            PlaySound(soundName);
        }

        // Специальный метод для звука окончания игры с контролем
        public void PlayGameOverAndStop()
        {
            if (!_soundEffectsEnabled) return;

            try
            {
                string soundPath = Path.Combine(_soundsPath, "gameover.mp3");

                if (!File.Exists(soundPath)) return;

                var gameOverPlayer = new MediaPlayer();
                gameOverPlayer.Open(new Uri(soundPath, UriKind.Absolute));
                gameOverPlayer.Volume = _soundEffectsVolume;
                gameOverPlayer.Play();

                // Останавливаем фоновую музыку
                StopMusic();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}