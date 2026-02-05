using AutoRentalApp.Data;
using AutoRentalApp.Helpers;
using AutoRentalApp.Services;
using AutoRentalApp.Views; // ДОБАВЛЕНО: для доступа к окнам
using System.Windows;
using System.Windows.Controls;

namespace AutoRentalApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _dbContext;

        public LoginWindow()
        {
            InitializeComponent();

            // Инициализация контекста БД и сервиса авторизации
            string connectionString = DbHelper.GetConnectionString();
            _dbContext = new AppDbContext(connectionString);
            _authService = new AuthService(_dbContext);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Очистка ошибок
            LoginErrorText.Visibility = Visibility.Collapsed;
            PasswordErrorText.Visibility = Visibility.Collapsed;

            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Валидация на клиенте
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(login))
            {
                LoginErrorText.Text = "Введите логин";
                LoginErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordErrorText.Text = "Введите пароль";
                PasswordErrorText.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
                return;

            // Авторизация через сервис (БЕЗ кортежей — для .NET Framework 4.7.2)
            var result = _authService.Login(login, password);

            if (result.success)
            {
                MessageBox.Show($"Добро пожаловать, {result.user.FullName}!\nРоль: {result.user.Role?.RoleName}",
                    "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                // Перенаправление в зависимости от роли
                OpenMainWindow(result.user);
                this.Close();
            }
            else
            {
                MessageBox.Show(result.message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordBox.Clear();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            if (registerWindow.ShowDialog() == true)
            {
                // После успешной регистрации очищаем поля для нового входа
                LoginBox.Clear();
                PasswordBox.Clear();
            }
        }

        private void OpenMainWindow(AutoRentalApp.Models.User user)
        {
            Window mainWindow = null;

            switch (user.Role?.RoleName)
            {
                case "администратор":
                    // Создаем реальную панель администратора
                    mainWindow = new AdminWindow(_authService, _dbContext);
                    break;

                case "менеджер":
                    // ВРЕМЕННАЯ ЗАГЛУШКА для менеджера (пока нет окна)
                    mainWindow = CreatePlaceholderWindow(user.FullName, "Панель менеджера");
                    break;

                case "клиент":
                    // ВРЕМЕННАЯ ЗАГЛУШКА для клиента (пока нет окна)
                    mainWindow = CreatePlaceholderWindow(user.FullName, "Личный кабинет клиента");
                    break;

                default:
                    mainWindow = CreatePlaceholderWindow(user.FullName, "Главное окно");
                    break;
            }

            mainWindow.Show();
        }

        // ВРЕМЕННАЯ ЗАГЛУШКА: Создание простого окна для ролей без полноценного интерфейса
        private Window CreatePlaceholderWindow(string userName, string title)
        {
            var window = new Window
            {
                Title = $"Авто в прокат — {title}",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = System.Windows.Media.Brushes.White
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(60) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });

            // Шапка
            var header = new System.Windows.Controls.Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2196F3")),
                Padding = new System.Windows.Thickness(20, 0, 20, 0)
            };
            var headerPanel = new System.Windows.Controls.StackPanel { Orientation = System.Windows.Controls.Orientation.Horizontal };
            headerPanel.Children.Add(new System.Windows.Controls.TextBlock { Text = "🚗 ", FontSize = 24, Foreground = System.Windows.Media.Brushes.White });
            headerPanel.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = $"{title} | {userName}",
                FontSize = 18,
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            });
            header.Child = headerPanel;
            System.Windows.Controls.Grid.SetRow(header, 0);
            grid.Children.Add(header);

            // Контент
            var content = new System.Windows.Controls.TextBlock
            {
                Text = $"Добро пожаловать в систему проката автомобилей!\n\n" +
                       $"✅ Авторизация успешна\n" +
                       $"✅ Роль: {title}\n" +
                       $"✅ Пользователь: {userName}\n\n" +
                       "Полноценный интерфейс для этой роли находится в разработке.",
                FontSize = 16,
                TextAlignment = System.Windows.TextAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new System.Windows.Thickness(20)
            };
            System.Windows.Controls.Grid.SetRow(content, 1);
            grid.Children.Add(content);

            window.Content = grid;
            return window;
        }
    }
}