using AutoRentalApp.Data;
using AutoRentalApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoRentalApp.Views
{
    public partial class EmployeesView : UserControl
    {
        private readonly AppDbContext _dbContext;
        private System.Collections.Generic.List<Employee> _allEmployees;

        public EmployeesView(AppDbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                StatusText.Text = "Загрузка данных...";

                _allEmployees = _dbContext.Employees
                    .Include(e => e.User)
                        .ThenInclude(u => u.Role)
                    .ToList();

                EmployeesDataGrid.ItemsSource = _allEmployees;
                StatusText.Text = $"Всего сотрудников: {_allEmployees.Count}";
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
            FilterEmployees();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterEmployees();
        }

        private void FilterEmployees()
        {
            string query = SearchBox.Text?.ToLower().Trim() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                EmployeesDataGrid.ItemsSource = _allEmployees;
                StatusText.Text = $"Все сотрудники: {_allEmployees.Count}";
                return;
            }

            var filtered = _allEmployees.Where(e =>
                e.User.FirstName.ToLower().Contains(query) ||
                e.User.LastName.ToLower().Contains(query) ||
                e.Position.ToLower().Contains(query) ||
                e.User.Login.ToLower().Contains(query)
            ).ToList();

            EmployeesDataGrid.ItemsSource = filtered;
            StatusText.Text = filtered.Any()
                ? $"Найдено: {filtered.Count} сотрудников"
                : "Сотрудники не найдены";
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditEmployeeWindow(_dbContext, null);
            if (addWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            LoadEmployees();
        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Действия в строке деталей
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.Tag as Employee;

            if (employee != null)
            {
                var editWindow = new AddEditEmployeeWindow(_dbContext, employee);
                if (editWindow.ShowDialog() == true)
                {
                    LoadEmployees();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var employee = button?.Tag as Employee;

            if (employee != null)
            {
                string employeeName = employee.User?.FullName ?? "Неизвестный сотрудник";

                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить сотрудника {employeeName}?\n\n" +
                    $"Внимание: Будет удалена только запись о сотруднике. Учётная запись пользователя останется в системе.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем только запись из Employees, пользователь остаётся
                        _dbContext.Employees.Remove(employee);
                        _dbContext.SaveChanges();
                        LoadEmployees();
                        MessageBox.Show("Сотрудник успешно удалён", "Успех",
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