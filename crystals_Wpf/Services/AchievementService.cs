using crystals_Wpf.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace crystals_Wpf.Services
{
    public class AchievementService
    {
        private string connectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;
              Initial Catalog=AuthorizationDB;
              Integrated Security=True";

        public void UnlockAchievement(int userId, int achievementId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = @"SELECT COUNT(*) FROM UserAchievements WHERE UserId = @UserId AND AchievementId = @AchievementId";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@UserId", userId);
                checkCommand.Parameters.AddWithValue("@AchievementId", achievementId);

                int count = (int)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    string insertQuery = @"INSERT INTO UserAchievements (UserId, AchievementId, DateReceived) VALUES (@UserId, @AchievementId, GETDATE())";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@UserId", userId);
                    insertCommand.Parameters.AddWithValue("@AchievementId", achievementId);
                    insertCommand.ExecuteNonQuery();

                    SoundService.Instance.PlayAchievementUnlock();
                    ShowAchievementNotification(achievementId);
                }
            }
        }

        public int GetTotalGamesPlayed(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT GamesPlayed FROM UserStatistics WHERE UserId = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public int GetTotalScore(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TotalScore FROM UserStatistics WHERE UserId = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public void SaveRecordIfBetter(int userId, int score)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT BestScore FROM Records WHERE UserId = @UserId";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@UserId", userId);

                object result = checkCommand.ExecuteScalar();

                if (result == null)
                {
                    string insertQuery = @"INSERT INTO Records (UserId, BestScore, RecordDate) VALUES (@UserId, @Score, GETDATE())";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@UserId", userId);
                    insertCommand.Parameters.AddWithValue("@Score", score);
                    insertCommand.ExecuteNonQuery();
                }
                else
                {
                    int currentBest = Convert.ToInt32(result);
                    if (score > currentBest)
                    {
                        string updateQuery = "UPDATE Records SET BestScore = @Score, RecordDate = GETDATE() WHERE UserId = @UserId";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@UserId", userId);
                        updateCommand.Parameters.AddWithValue("@Score", score);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateStatistics(int userId, int score, int totalMatches, int maxComboLength)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM UserStatistics WHERE UserId = @UserId";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@UserId", userId);
                int exists = (int)checkCommand.ExecuteScalar();

                if (exists > 0)
                {
                    string updateQuery = @"UPDATE UserStatistics 
                                          SET GamesPlayed = GamesPlayed + 1,
                                              TotalScore = TotalScore + @Score,
                                              TotalMatches = TotalMatches + @TotalMatches,
                                              MaxComboLength = CASE 
                                                  WHEN @MaxComboLength > MaxComboLength THEN @MaxComboLength 
                                                  ELSE MaxComboLength 
                                              END
                                          WHERE UserId = @UserId";

                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@UserId", userId);
                    updateCommand.Parameters.AddWithValue("@Score", score);
                    updateCommand.Parameters.AddWithValue("@TotalMatches", totalMatches);
                    updateCommand.Parameters.AddWithValue("@MaxComboLength", maxComboLength);
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    string insertQuery = @"INSERT INTO UserStatistics (UserId, GamesPlayed, TotalScore, TotalMatches, MaxComboLength) 
                                          VALUES (@UserId, 1, @Score, @TotalMatches, @MaxComboLength)";

                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@UserId", userId);
                    insertCommand.Parameters.AddWithValue("@Score", score);
                    insertCommand.Parameters.AddWithValue("@TotalMatches", totalMatches);
                    insertCommand.Parameters.AddWithValue("@MaxComboLength", maxComboLength);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void CheckAndUnlockAchievements(int userId, int currentGameScore, int currentGameCombo)
        {
            int totalGames = GetTotalGamesPlayed(userId);

            if (currentGameScore >= 100) UnlockAchievement(userId, 1);
            if (totalGames >= 1) UnlockAchievement(userId, 2);
            if (currentGameScore >= 500) UnlockAchievement(userId, 3);
            if (currentGameCombo >= 5) UnlockAchievement(userId, 4);
            if (totalGames >= 10) UnlockAchievement(userId, 5);
            if (currentGameScore >= 1000) UnlockAchievement(userId, 6);
        }

        private void ShowAchievementNotification(int achievementId)
        {
            var achievement = GetAchievementInfo(achievementId);
            if (achievement.Name == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new Window
                {
                    Width = 380,
                    Height = 130,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Topmost = true,
                    WindowStyle = WindowStyle.None,  // Оставляем None для прозрачности
                    AllowsTransparency = true,
                    Background = Brushes.Transparent,
                    ResizeMode = ResizeMode.NoResize,  // Запрещаем изменение размера
                    ShowInTaskbar = false  // Не показываем в панели задач
                };

                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(230, 255, 107, 107)),
                    CornerRadius = new CornerRadius(12),
                    Margin = new Thickness(5),
                    Effect = new DropShadowEffect  // Добавляем тень вместо рамки
                    {
                        BlurRadius = 10,
                        ShadowDepth = 2,
                        Opacity = 0.3
                    }
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var iconBorder = new Border
                {
                    Width = 55,
                    Height = 55,
                    CornerRadius = new CornerRadius(27.5),
                    Margin = new Thickness(12),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new LinearGradientBrush(Colors.Orange, Colors.Red, 45)
                };

                var iconText = new TextBlock
                {
                    Text = GetAchievementIcon(achievementId),
                    FontSize = 30,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                iconBorder.Child = iconText;

                var infoPanel = new StackPanel { Margin = new Thickness(0, 10, 12, 10) };
                infoPanel.Children.Add(new TextBlock
                {
                    Text = "🏆 НОВОЕ ДОСТИЖЕНИЕ!",
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    Foreground = Brushes.White
                });
                infoPanel.Children.Add(new TextBlock
                {
                    Text = achievement.Name,
                    FontWeight = FontWeights.Bold,
                    FontSize = 16,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 5, 0, 0)
                });
                infoPanel.Children.Add(new TextBlock
                {
                    Text = achievement.Description,
                    FontSize = 11,
                    Foreground = Brushes.White,
                    Opacity = 0.8
                });

                Grid.SetColumn(iconBorder, 0);
                Grid.SetColumn(infoPanel, 1);
                grid.Children.Add(iconBorder);
                grid.Children.Add(infoPanel);

                border.Child = grid;
                notification.Content = border;

                notification.Opacity = 0;
                notification.Show();

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                notification.BeginAnimation(Window.OpacityProperty, fadeIn);

                var timer = new System.Timers.Timer(3000);
                timer.Elapsed += (s, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
                        fadeOut.Completed += (a, b) => notification.Close();
                        notification.BeginAnimation(Window.OpacityProperty, fadeOut);
                    });
                    timer.Dispose();
                };
                timer.Start();
            });
        }

        private (string Name, string Description) GetAchievementInfo(int id)
        {
            switch (id)
            {
                case 1: return ("100 очков", "Наберите 100 очков в одной игре");
                case 2: return ("Первая игра", "Сыграйте первую игру");
                case 3: return ("500 очков", "Наберите 500 очков в одной игре");
                case 4: return ("Комбо мастер", "Соберите комбо из 5 кристаллов");
                case 5: return ("Игрок", "Сыграйте 10 игр");
                case 6: return ("1000 очков", "Наберите 1000 очков в одной игре");
                default: return (null, null);
            }
        }

        private string GetAchievementIcon(int id)
        {
            switch (id)
            {
                case 1: return "⭐";
                case 2: return "🎮";
                case 3: return "🌟🌟";
                case 4: return "⚡";
                case 5: return "👑";
                case 6: return "🌟🌟🌟";
                default: return "🏆";
            }
        }

        public List<UserAchievement> GetUserAchievements(int userId)
        {
            List<UserAchievement> achievements = new List<UserAchievement>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT a.Id, a.Name, a.Description, ua.DateReceived 
                                FROM Achievements a
                                LEFT JOIN UserAchievements ua ON a.Id = ua.AchievementId AND ua.UserId = @UserId
                                ORDER BY a.Id";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    achievements.Add(new UserAchievement
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Icon = GetAchievementIcon(Convert.ToInt32(reader["Id"])),
                        IsUnlocked = reader["DateReceived"] != DBNull.Value,
                        UnlockDate = reader["DateReceived"] != DBNull.Value ?
                            Convert.ToDateTime(reader["DateReceived"]).ToString("dd.MM.yyyy HH:mm") : ""
                    });
                }
            }

            return achievements;
        }

        public bool IsAchievementUnlocked(int userId, int achievementId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM UserAchievements WHERE UserId = @UserId AND AchievementId = @AchievementId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@AchievementId", achievementId);
                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}