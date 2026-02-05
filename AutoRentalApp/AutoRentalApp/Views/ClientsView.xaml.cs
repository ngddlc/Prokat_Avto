using AutoRentalApp.Data;
using AutoRentalApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoRentalApp.Views
{
    public partial class ClientsView : UserControl
    {
        private readonly AppDbContext _dbContext;
        private System.Collections.Generic.List<Client> _allClients;

        public ClientsView(AppDbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                StatusText.Text = "Загрузка данных...";

                _allClients = _dbContext.Clients
                    .Include(c => c.User)
                    .ToList();

                ClientsDataGrid.ItemsSource = _allClients;
                StatusText.Text = $"Всего клиентов: {_allClients.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Ошибка загрузки";
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterClients();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterClients();
        }

        private void FilterClients()
        {
            string query = SearchBox.Text?.ToLower().Trim() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                ClientsDataGrid.ItemsSource = _allClients;
                StatusText.Text = $"Все клиенты: {_allClients.Count}";
                return;
            }

            var filtered = _allClients.Where(c =>
                c.User.FirstName.ToLower().Contains(query) ||
                c.User.LastName.ToLower().Contains(query) ||
                c.Phone.ToLower().Contains(query) ||
                (c.Email != null && c.Email.ToLower().Contains(query))
            ).ToList();

            ClientsDataGrid.ItemsSource = filtered;
            StatusText.Text = filtered.Any()
                ? $"Найдено: {filtered.Count} клиентов"
                : "Клиенты не найдены";
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditClientWindow(_dbContext, null);
            if (addWindow.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            LoadClients();
        }

        private void ClientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Действия в строке деталей (кнопки Редактировать/Удалить)
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button?.Tag as Client;

            if (client != null)
            {
                var editWindow = new AddEditClientWindow(_dbContext, client);
                if (editWindow.ShowDialog() == true)
                {
                    LoadClients();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button?.Tag as Client;

            if (client != null)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить клиента {client.User.FullName}?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbContext.Clients.Remove(client);
                        _dbContext.SaveChanges();
                        LoadClients();
                        MessageBox.Show("Клиент успешно удалён", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}