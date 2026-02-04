using AutoRentalApp.Data;
using AutoRentalApp.Helpers;
using AutoRentalApp.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            // Авторизация через сервис
            var (success, message, user) = _authService.Login(login, password);

            if (success)
            {
                MessageBox.Show($"Добро пожаловать, {user.FullName}!\nРоль: {user.Role?.RoleName}",
                    "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                // Перенаправление в зависимости от роли
                OpenMainWindow(user);
                this.Close();
            }
            else
            {
                MessageBox.Show(message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
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
            // Создание главного окна в зависимости от роли
            Window mainWindow = null;

            switch (user.Role?.RoleName)
            {
                case "администратор":
                    // Здесь будет окно администратора
                    mainWindow = CreateTestWindow(user.FullName, "Панель администратора");
                    break;
                case "менеджер":
                    // Здесь будет окно менеджера
                    mainWindow = CreateTestWindow(user.FullName, "Панель менеджера");
                    break;
                case "клиент":
                    // Здесь будет окно клиента
                    mainWindow = CreateTestWindow(user.FullName, "Личный кабинет клиента");
                    break;
                default:
                    mainWindow = CreateTestWindow(user.FullName, "Главное окно");
                    break;
            }

            mainWindow.Show();
        }

        private Window CreateTestWindow(string userName, string title)
        {
            var window = new Window
            {
                Title = $"Авто в прокат — {title}",
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = Brushes.White
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Шапка
            var header = new Border
            {
                Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString("#2196F3")),
                Padding = new Thickness(20, 0, 20, 0)
            };
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            headerPanel.Children.Add(new TextBlock { Text = "🚗 ", FontSize = 24, Foreground = Brushes.White });
            headerPanel.Children.Add(new TextBlock
            {
                Text = $"{title} | {userName}",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center
            });
            header.Child = headerPanel;
            Grid.SetRow(header, 0);
            grid.Children.Add(header);

            // Контент
            var content = new TextBlock
            {
                Text = $"Добро пожаловать в систему проката автомобилей!\n\n" +
                       $"✅ Авторизация успешна\n" +
                       $"✅ Роль: {title}\n" +
                       $"✅ Пользователь: {userName}\n\n" +
                       "Дальнейшая разработка интерфейса в процессе...",
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20)
            };
            Grid.SetRow(content, 1);
            grid.Children.Add(content);

            window.Content = grid;
            return window;
        }
    }
}