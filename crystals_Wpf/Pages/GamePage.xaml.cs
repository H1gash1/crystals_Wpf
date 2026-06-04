using crystals_Wpf.GameLogic;
using crystals_Wpf.Models;
using crystals_Wpf.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System;
using System.Windows.Media.Effects;

namespace crystals_Wpf.Pages
{
    public partial class GamePage : Page
    {
        private Random random = new Random();
        private MatchFinder matchFinder = new MatchFinder();
        private int score = 0;
        private int totalMatches = 0;
        private int maxComboLength = 0;
        private int _bombCount = 2;
        private int _rocketCount = 2;
        private int _rainbowCount = 2;
        private int movesLeft;
        private Border[,] cellBorders = new Border[8, 8];
        private Image[,] crystalImages = new Image[8, 8];
        private BoardManager boardManager = new BoardManager();
        private RecordService recordService = new RecordService();
        private StatisticsService statisticsService = new StatisticsService();
        private AchievementService achievementService = new AchievementService();
        private Crystal[,] board;
        private bool isProcessing = false;
        private Crystal selectedCrystal = null;
        private Border selectedBorder = null;
        private bool _achievement100Unlocked = false;
        private bool _achievement500Unlocked = false;
        private bool _achievement1000Unlocked = false;
        private bool _achievementComboUnlocked = false;

        private string[] crystalTextures = new string[]
        {
            "pack://application:,,,/crystals_Wpf;component/assets/11.png",
            "pack://application:,,,/crystals_Wpf;component/assets/13.png",
            "pack://application:,,,/crystals_Wpf;component/assets/21.png",
            "pack://application:,,,/crystals_Wpf;component/assets/3.png",
            "pack://application:,,,/crystals_Wpf;component/assets/14.png"
        };

        public GamePage()
        {
            InitializeComponent();

            movesLeft = AppSettings.DefaultMoves; // Используем настройки

            PlayerText.Text = CurrentUser.Login;
            UpdateScoreUI();
            UpdateMovesUI();

            GenerateBoard();
            CreateBoardUI();
            UpdateBoardUI();

            InitializePowerUpButtons();
        }

        private void GenerateBoard()
        {
            board = boardManager.GenerateBoard();
        }

        private void CreateBoardUI()
        {
            GameBoard.Children.Clear();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Border border = new Border
                    {
                        Background = GetCellBackground(),
                        CornerRadius = new CornerRadius(5),
                        Margin = new Thickness(2),
                        Tag = board[row, col]
                    };

                    Image image = new Image
                    {
                        Stretch = Stretch.Uniform,
                        Margin = new Thickness(5),
                        RenderTransform = new TranslateTransform()
                    };

                    border.Child = image;
                    border.MouseLeftButtonDown += Cell_Click;

                    cellBorders[row, col] = border;
                    crystalImages[row, col] = image;
                    GameBoard.Children.Add(border);
                }
            }
        }

        private Brush GetCellBackground()
        {
            try
            {
                return new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/crystals_Wpf;component/assets/1.png")),
                    Stretch = Stretch.Fill
                };
            }
            catch
            {
                return new SolidColorBrush(Color.FromArgb(100, 200, 200, 200));
            }
        }

        private void UpdateBoardUI()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Crystal crystal = board[row, col];
                    Image image = crystalImages[row, col];

                    // НЕ сбрасываем анимации, если они идут
                    if (crystal.Type < 0)
                    {
                        image.Source = null;
                        image.Opacity = 0;
                        continue;
                    }

                    // Обновляем только если изменился тип кристалла
                    try
                    {
                        var currentSource = image.Source as BitmapImage;
                        var needUpdate = true;

                        if (currentSource != null)
                        {
                            var currentPath = currentSource.UriSource?.OriginalString ?? "";
                            needUpdate = !currentPath.Contains(crystalTextures[crystal.Type]);
                        }

                        if (needUpdate)
                        {
                            var uri = new Uri(crystalTextures[crystal.Type], UriKind.Absolute);
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = uri;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();

                            image.Source = bitmap;
                            image.Opacity = 1;
                        }
                    }
                    catch
                    {
                        image.Source = null;
                        image.Opacity = 0;
                    }
                }
            }
        }

        private async void Cell_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isProcessing) return;

            Border clickedBorder = sender as Border;
            Crystal clickedCrystal = clickedBorder.Tag as Crystal;

            if (clickedCrystal.Type == -1) return;

            if (selectedCrystal == null)
            {
                selectedCrystal = clickedCrystal;
                selectedBorder = clickedBorder;
                AnimateSelect(selectedBorder);
                return;
            }

            if (selectedCrystal == clickedCrystal)
            {
                ClearSelection();
                return;
            }

            if (AreNeighbors(selectedCrystal, clickedCrystal))
            {
                await TrySwapWithAnimation(selectedCrystal, clickedCrystal);
            }

            ClearSelection();
        }

        private void AnimateSelect(Border border)
        {
            var scaleAnim = new DoubleAnimation(1.05, TimeSpan.FromMilliseconds(100))
            {
                AutoReverse = true
            };

            var scaleTransform = new ScaleTransform(1, 1);
            border.RenderTransform = scaleTransform;
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);

            border.BorderBrush = Brushes.Yellow;
            border.BorderThickness = new Thickness(3);
        }

        private void ClearSelection()
        {
            if (selectedBorder != null)
            {
                selectedBorder.BorderBrush = null;
                selectedBorder.BorderThickness = new Thickness(0);
                selectedBorder.RenderTransform = null;
            }
            selectedCrystal = null;
            selectedBorder = null;
        }

        private bool AreNeighbors(Crystal first, Crystal second)
        {
            return (Math.Abs(first.Row - second.Row) == 1 && first.Column == second.Column) ||
                   (Math.Abs(first.Column - second.Column) == 1 && first.Row == second.Row);
        }

        private async Task TrySwapWithAnimation(Crystal first, Crystal second)
        {
            isProcessing = true;

            // Анимация обмена
            await AnimateSwap(first, second);

            // Меняем местами в данных
            SwapCrystals(first, second);
            UpdateBoardUI();

            // Проверяем совпадения
            var matches = matchFinder.FindMatches(board);

            if (matches.Count == 0)
            {
                // Возвращаем обратно с анимацией
                await AnimateSwap(second, first);
                SwapCrystals(second, first);
                UpdateBoardUI();

                // Анимация ошибки
                await AnimateError(first, second);
                isProcessing = false;
                return;
            }

            SoundService.Instance.PlaySwapSound();

            // Уменьшаем ходы
            movesLeft--;
            UpdateMovesUI();
            AnimateText(MovesText);

            // Обрабатываем совпадения с анимациями
            await ProcessMatchesWithAnimation(matches);

            isProcessing = false;

            if (movesLeft <= 0)
            {
                EndGame();
            }
        }

        private async Task AnimateSwap(Crystal first, Crystal second)
        {
            var image1 = crystalImages[first.Row, first.Column];
            var image2 = crystalImages[second.Row, second.Column];

            // Сохраняем исходные позиции
            var transform1 = new TranslateTransform();
            var transform2 = new TranslateTransform();
            image1.RenderTransform = transform1;
            image2.RenderTransform = transform2;

            // Вычисляем смещение
            double offsetX = (second.Column - first.Column) * 65;
            double offsetY = (second.Row - first.Row) * 65;

            // Создаем анимации
            var anim1X = new DoubleAnimation(offsetX, TimeSpan.FromMilliseconds(150))
            {
                EasingFunction = new QuadraticEase()
            };
            var anim1Y = new DoubleAnimation(offsetY, TimeSpan.FromMilliseconds(150))
            {
                EasingFunction = new QuadraticEase()
            };
            var anim2X = new DoubleAnimation(-offsetX, TimeSpan.FromMilliseconds(150))
            {
                EasingFunction = new QuadraticEase()
            };
            var anim2Y = new DoubleAnimation(-offsetY, TimeSpan.FromMilliseconds(150))
            {
                EasingFunction = new QuadraticEase()
            };

            // Запускаем анимации
            transform1.BeginAnimation(TranslateTransform.XProperty, anim1X);
            transform1.BeginAnimation(TranslateTransform.YProperty, anim1Y);
            transform2.BeginAnimation(TranslateTransform.XProperty, anim2X);
            transform2.BeginAnimation(TranslateTransform.YProperty, anim2Y);

            await Task.Delay(150);

            // Сбрасываем трансформации
            image1.RenderTransform = new TranslateTransform();
            image2.RenderTransform = new TranslateTransform();
        }

        private async Task AnimateError(Crystal first, Crystal second)
        {
            var image1 = crystalImages[first.Row, first.Column];
            var image2 = crystalImages[second.Row, second.Column];

            var transform1 = new TranslateTransform();
            var transform2 = new TranslateTransform();
            image1.RenderTransform = transform1;
            image2.RenderTransform = transform2;

            // Создаем анимацию встряски
            for (int i = 0; i < 3; i++)
            {
                var shakeAnim = new DoubleAnimation(-5, 5, TimeSpan.FromMilliseconds(50))
                {
                    AutoReverse = true
                };
                transform1.BeginAnimation(TranslateTransform.XProperty, shakeAnim);
                transform2.BeginAnimation(TranslateTransform.XProperty, shakeAnim);
                await Task.Delay(100);
            }

            image1.RenderTransform = new TranslateTransform();
            image2.RenderTransform = new TranslateTransform();
        }

        private async Task ProcessMatchesWithAnimation(List<Crystal> matches)
        {
            int matchesCount = matches.Count;

            // Анимация исчезновения
            await AnimateMatchesRemoval(matches);

            SoundService.Instance.PlayComboSound(matchesCount);

            // Удаляем совпадения
            foreach (var crystal in matches)
            {
                crystal.Type = -1;
            }
            UpdateBoardUI();

            // Добавляем очки с анимацией
            AddScore(matchesCount);
            AnimateText(ScoreText);
            await ShowFloatingScore(matchesCount);

            // Обновляем статистику
            totalMatches++;
            if (matchesCount > maxComboLength)
            {
                maxComboLength = matchesCount;
                if (!_achievementComboUnlocked && maxComboLength >= 5)
                {
                    _achievementComboUnlocked = true;
                    achievementService.CheckAndUnlockAchievements(CurrentUser.Id, score, maxComboLength);
                }
            }

            // Анимация падения и заполнения
            await AnimateRefillBoard();

            // Проверяем новые совпадения
            var newMatches = matchFinder.FindMatches(board);
            if (newMatches.Count > 0)
            {
                await ProcessMatchesWithAnimation(newMatches);
            }
        }

        private async Task AnimateMatchesRemoval(List<Crystal> matches)
        {
            foreach (var crystal in matches)
            {
                var image = crystalImages[crystal.Row, crystal.Column];

                var scaleAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));
                var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));

                image.BeginAnimation(UIElement.OpacityProperty, null);

                image.RenderTransform =
                    new ScaleTransform(1, 1);

                image.Opacity = 1;

                var scaleTransform = new ScaleTransform(1, 1);
                image.RenderTransform = scaleTransform;
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
                image.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }

            await Task.Delay(150);

            foreach (var crystal in matches)
            {
                var image =
                    crystalImages[
                        crystal.Row,
                        crystal.Column];

                image.BeginAnimation(
                    UIElement.OpacityProperty,
                    null);

                image.Opacity = 1;

                image.RenderTransform =
                    new TranslateTransform();
            }
        }

        private async Task AnimateRefillBoard()
        {
            // Шаг 1: Запоминаем все позиции кристаллов и куда они должны упасть
            var fallAnimations = new List<(Image image, double fromY, double toY, int targetRow, int targetCol, Crystal crystal)>();

            for (int col = 0; col < 8; col++)
            {
                // Собираем все непустые кристаллы в столбце снизу вверх
                List<Crystal> columnCrystals = new List<Crystal>();
                for (int row = 7; row >= 0; row--)
                {
                    if (board[row, col].Type != -1)
                    {
                        columnCrystals.Add(board[row, col]);
                    }
                }

                // Распределяем их снизу вверх
                int targetRow = 7;
                foreach (var crystal in columnCrystals)
                {
                    if (crystal.Row != targetRow)
                    {
                        // Кристалл должен упасть
                        var image = crystalImages[crystal.Row, col];
                        double distance = (targetRow - crystal.Row) * 65;

                        fallAnimations.Add((image, 0, distance, targetRow, col, crystal));
                    }
                    targetRow--;
                }
            }

            // Шаг 2: Запускаем все анимации падения одновременно
            foreach (var anim in fallAnimations)
            {
                var transform = new TranslateTransform();
                anim.image.RenderTransform = transform;

                var fallAnim = new DoubleAnimation(anim.fromY, anim.toY, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new QuadraticEase()
                };
                transform.BeginAnimation(TranslateTransform.YProperty, fallAnim);
            }

            await Task.Delay(300);

            // Шаг 3: Обновляем данные в массиве board
            for (int col = 0; col < 8; col++)
            {
                // Собираем все непустые кристаллы в столбце снизу вверх
                List<Crystal> columnCrystals = new List<Crystal>();
                for (int row = 7; row >= 0; row--)
                {
                    if (board[row, col].Type != -1)
                    {
                        columnCrystals.Add(board[row, col]);
                    }
                }

                // Заполняем столбец снизу вверх
                int targetRow = 7;
                foreach (var crystal in columnCrystals)
                {
                    if (crystal.Row != targetRow)
                    {
                        // Перемещаем кристалл в новую позицию
                        board[targetRow, col].Type = crystal.Type;
                        if (targetRow != crystal.Row)
                        {
                            board[crystal.Row, col].Type = -1;
                        }
                    }
                    targetRow--;
                }
            }

            // Шаг 4: Обновляем изображения на новых позициях
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var crystal = board[row, col];
                    var image = crystalImages[row, col];

                    if (crystal.Type != -1)
                    {
                        try
                        {
                            var uri = new Uri(crystalTextures[crystal.Type], UriKind.Absolute);
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = uri;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();

                            // Сохраняем текущую трансформацию для плавного перехода
                            var currentTransform = image.RenderTransform as TranslateTransform;
                            double currentY = currentTransform?.Y ?? 0;

                            image.Source = bitmap;
                            image.Opacity = 1;

                            // Сбрасываем трансформацию
                            image.RenderTransform = new TranslateTransform();
                        }
                        catch
                        {
                            image.Source = null;
                        }
                    }

                    // Сбрасываем трансформацию
                    image.RenderTransform = new TranslateTransform();
                }
            }

            await Task.Delay(50);

            // Шаг 5: Создаем новые кристаллы на пустых местах с анимацией появления
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col].Type == -1)
                    {
                        int newType = random.Next(0, 5);
                        board[row, col].Type = newType;

                        var image = crystalImages[row, col];

                        // Загружаем изображение
                        try
                        {
                            var uri = new Uri(crystalTextures[newType], UriKind.Absolute);
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = uri;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            image.Source = bitmap;
                        }
                        catch
                        {
                            image.Source = null;
                        }

                        // Анимация появления
                        image.Opacity = 0;
                        image.RenderTransform = new ScaleTransform(0.3, 0.3);

                        var fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                        var scaleAnim = new DoubleAnimation(0.3, 1, TimeSpan.FromMilliseconds(200))
                        {
                            EasingFunction = new ElasticEase { Oscillations = 1, Springiness = 3 }
                        };

                        var scaleTransform = (ScaleTransform)image.RenderTransform;
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
                        image.BeginAnimation(UIElement.OpacityProperty, fadeAnim);

                        await Task.Delay(30);
                    }
                }
            }

            await Task.Delay(250);

            // Шаг 6: Финальный сброс всех трансформаций
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    crystalImages[row, col].RenderTransform = new TranslateTransform();
                    crystalImages[row, col].Opacity = 1;
                }
            }
        }

        private void AnimateText(TextBlock textBlock)
        {
            var scaleAnim = new DoubleAnimation(1.2, 1, TimeSpan.FromMilliseconds(200))
            {
                AutoReverse = true
            };

            var scaleTransform = new ScaleTransform(1, 1);
            textBlock.RenderTransform = scaleTransform;
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
        }

        private async Task ShowFloatingScore(int points)
        {
            int pointValue;
            switch (points)
            {
                case 3: pointValue = 30; break;
                case 4: pointValue = 60; break;
                case 5: pointValue = 100; break;
                default: pointValue = points * 20; break;
            }

            var floatingText = new TextBlock
            {
                Text = $"+{pointValue}",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Gold,
                Opacity = 1,
                RenderTransform = new TranslateTransform()
            };

            GameBoard.Children.Add(floatingText);

            var moveAnim = new DoubleAnimation(0, -50, TimeSpan.FromMilliseconds(800));
            var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(800));

            var transform = (TranslateTransform)floatingText.RenderTransform;
            transform.BeginAnimation(TranslateTransform.YProperty, moveAnim);
            floatingText.BeginAnimation(UIElement.OpacityProperty, fadeAnim);

            await Task.Delay(800);

            if (GameBoard.Children.Contains(floatingText))
                GameBoard.Children.Remove(floatingText);
        }

        private void SwapCrystals(Crystal first, Crystal second)
        {
            int tempType = first.Type;
            first.Type = second.Type;
            second.Type = tempType;
        }

        private void AddScore(int removedCount)
        {
            int points;
            switch (removedCount)
            {
                case 3: points = 30; break;
                case 4: points = 60; break;
                case 5: points = 100; break;
                default: points = removedCount * 20; break;
            }

            int oldScore = score;
            score += points;
            UpdateScoreUI();

            if (!_achievement100Unlocked && score >= 100)
            {
                _achievement100Unlocked = true;
                achievementService.CheckAndUnlockAchievements(CurrentUser.Id, score, maxComboLength);
            }

            if (!_achievement500Unlocked && score >= 500)
            {
                _achievement500Unlocked = true;
                achievementService.CheckAndUnlockAchievements(CurrentUser.Id, score, maxComboLength);
            }

            if (!_achievement1000Unlocked && score >= 1000)
            {
                _achievement1000Unlocked = true;
                achievementService.CheckAndUnlockAchievements(CurrentUser.Id, score, maxComboLength);
            }
        }

        private void UpdateScoreUI()
        {
            ScoreText.Text = score.ToString();
        }

        private void UpdateMovesUI()
        {
            MovesText.Text = movesLeft.ToString();
        }

        private void EndGame()
        {
            // Сохраняем рекорд (если лучше)
            achievementService.SaveRecordIfBetter(CurrentUser.Id, score);

            // Обновляем статистику
            achievementService.UpdateStatistics(CurrentUser.Id, score, totalMatches, maxComboLength);

            // Проверяем достижения (игрок - 10 игр)
            achievementService.CheckAndUnlockAchievements(CurrentUser.Id, score, maxComboLength);

            SoundService.Instance.PlayGameOverAndStop();

            var result = MessageBox.Show($"Игра окончена!\nВаш результат: {score}\n" +
                           $"Всего комбинаций: {totalMatches}\n" +
                           $"Максимальная комбинация: {maxComboLength}",
                           "Конец игры",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);

            if (result == MessageBoxResult.OK)
            {
                MainWindow.Instance.GetFrame().Navigate(new MainMenuPage());
            }
        }



        private void InitializePowerUpButtons()
        {
            // Убираем стили, просто назначаем обработчики
            PowerUpBomb.Click += (s, e) => UseBombPowerUp();
            PowerUpRocket.Click += (s, e) => UseRocketPowerUp();
            PowerUpRainbow.Click += (s, e) => UseRainbowPowerUp();
            UpdatePowerUpCounters();
        }

        private void UpdatePowerUpCounters()
        {
            BombCount.Text = _bombCount.ToString();
            RocketCount.Text = _rocketCount.ToString();
            RainbowCount.Text = _rainbowCount.ToString();
        }

        private async void UseBombPowerUp()
        {
            if (_bombCount <= 0 || isProcessing) return;
            _bombCount--;
            UpdatePowerUpCounters();
            SoundService.Instance.PlayPowerUpSound();

            await ShowPowerUpEffect("💣 БОМБА! 💣", Brushes.Orange);
            Random rand = new Random();
            int centerRow = rand.Next(0, 8);
            int centerCol = rand.Next(0, 8);

            for (int row = centerRow - 1; row <= centerRow + 1; row++)
            {
                for (int col = centerCol - 1; col <= centerCol + 1; col++)
                {
                    if (row >= 0 && row < 8 && col >= 0 && col < 8)
                    {
                        board[row, col].Type = -1;
                    }
                }
            }

            UpdateBoardUI();
            await ProcessMatchesWithAnimation(matchFinder.FindMatches(board));
        }

        private async void UseRocketPowerUp()
        {
            if (_rocketCount <= 0 || isProcessing) return;
            _rocketCount--;
            UpdatePowerUpCounters();
            SoundService.Instance.PlayPowerUpSound();

            await ShowPowerUpEffect("🚀 РАКЕТА! 🚀", Brushes.Cyan);

            // Эффект ракеты - очищаем целый ряд
            Random rand = new Random();
            int row = rand.Next(0, 8);

            for (int col = 0; col < 8; col++)
            {
                board[row, col].Type = -1;
            }

            UpdateBoardUI();
            await ProcessMatchesWithAnimation(matchFinder.FindMatches(board));
        }

        private async void UseRainbowPowerUp()
        {
            if (_rainbowCount <= 0 || isProcessing) return;
            _rainbowCount--;
            UpdatePowerUpCounters();
            SoundService.Instance.PlayPowerUpSound();

            await ShowPowerUpEffect("🌈 РАДУГА! 🌈", Brushes.Magenta);

            // Эффект радуги - превращаем все кристаллы в один цвет
            Random rand = new Random();
            int newType = rand.Next(0, 5);

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col].Type != -1)
                    {
                        board[row, col].Type = newType;
                    }
                }
            }

            UpdateBoardUI();
            await ProcessMatchesWithAnimation(matchFinder.FindMatches(board));
        }

        private async Task ShowPowerUpEffect(string message, Brush color)
        {
            var effectText = new TextBlock
            {
                Text = message,
                FontSize = 28,                      // Увеличил размер
                FontWeight = FontWeights.Bold,
                Foreground = color,
                Opacity = 1,
                RenderTransform = new TranslateTransform(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 400,                        // Фиксированная ширина
                TextAlignment = TextAlignment.Center,
                Padding = new Thickness(10),
                Effect = new DropShadowEffect       // Добавил тень для читаемости
                {
                    BlurRadius = 8,
                    ShadowDepth = 2,
                    Opacity = 0.5
                }
            };

            // Находим родительский Grid и добавляем эффект
            var parentGrid = GameBoard.Parent as Grid;
            if (parentGrid != null)
            {
                // Позиционируем по центру
                effectText.RenderTransform = new TranslateTransform(0, -100);
                parentGrid.Children.Add(effectText);

                var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(1500));
                var moveAnim = new DoubleAnimation(-100, -200, TimeSpan.FromMilliseconds(1500));
                var scaleAnim = new DoubleAnimation(1, 1.3, TimeSpan.FromMilliseconds(500));

                var transform = (TranslateTransform)effectText.RenderTransform;
                transform.BeginAnimation(TranslateTransform.YProperty, moveAnim);

                var scaleTransform = new ScaleTransform(1, 1);
                effectText.RenderTransform = scaleTransform;
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);

                effectText.BeginAnimation(UIElement.OpacityProperty, fadeAnim);

                await Task.Delay(1500);

                if (parentGrid.Children.Contains(effectText))
                    parentGrid.Children.Remove(effectText);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppSettings.ConfirmExit)
            {
                var result = MessageBox.Show("Выйти из игры?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;
            }

            // Возвращаемся в меню
            MainWindow.Instance.GetFrame().Navigate(new MainMenuPage());
        }
    }
}