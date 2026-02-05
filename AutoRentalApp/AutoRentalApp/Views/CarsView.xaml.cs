using AutoRentalApp.Data;
using AutoRentalApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoRentalApp.Views
{
    public partial class CarsView : UserControl
    {
        private readonly AppDbContext _dbContext;
        private System.Collections.Generic.List<Car> _allCars;

        public CarsView(AppDbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            LoadCars();
        }

        private void LoadCars()
        {
            try
            {
                StatusText.Text = "Загрузка данных...";

                _allCars = _dbContext.Cars
                    .Include(c => c.CarStatus)
                    .ToList();

                CarsDataGrid.ItemsSource = _allCars;
                StatusText.Text = $"Всего автомобилей: {_allCars.Count}";
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
            FilterCars();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCars();
        }

        private void FilterCars()
        {
            string query = SearchBox.Text?.ToLower().Trim() ?? "";

            if (string.IsNullOrWhiteSpace(query))
            {
                CarsDataGrid.ItemsSource = _allCars;
                StatusText.Text = $"Все автомобили: {_allCars.Count}";
                return;
            }

            var filtered = _allCars.Where(c =>
                c.Brand.ToLower().Contains(query) ||
                c.Model.ToLower().Contains(query) ||
                c.PlateNumber.ToLower().Contains(query) ||
                c.Color.ToLower().Contains(query)
            ).ToList();

            CarsDataGrid.ItemsSource = filtered;
            StatusText.Text = filtered.Any()
                ? $"Найдено: {filtered.Count} автомобилей"
                : "Автомобили не найдены";
        }

        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditCarWindow(_dbContext, null);
            if (addWindow.ShowDialog() == true)
            {
                LoadCars();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            LoadCars();
        }

        private void CarsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Действия в строке деталей
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var car = button?.Tag as Car;

            if (car != null)
            {
                var editWindow = new AddEditCarWindow(_dbContext, car);
                if (editWindow.ShowDialog() == true)
                {
                    LoadCars();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var car = button?.Tag as Car;

            if (car != null)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить автомобиль {car.Brand} {car.Model} ({car.PlateNumber})?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbContext.Cars.Remove(car);
                        _dbContext.SaveChanges();
                        LoadCars();
                        MessageBox.Show("Автомобиль успешно удалён", "Успех",
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