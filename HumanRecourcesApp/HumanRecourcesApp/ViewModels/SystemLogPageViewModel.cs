using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HumanResourcesApp.DBClasses;
using HumanResourcesApp.Models;
using HumanResourcesApp.Views;

namespace HumanResourcesApp.ViewModels
{
    public partial class SystemLogViewModel : ObservableObject
    {
        private readonly HumanResourcesDB _context;

        [ObservableProperty]
        private ObservableCollection<SystemLog> systemLogs = new();

        [ObservableProperty]
        private SystemLog? selectedSystemLog;

        [ObservableProperty]
        private bool isLoading;

        public SystemLogViewModel()
        {
            _context = new HumanResourcesDB();
            LoadSystemLogs();
        }

        private void LoadSystemLogs()
        {
            try
            {
                IsLoading = true;
                var logs = _context.GetAllSystemLogs()
                    .OrderByDescending(l => l.LogDate)
                    .ToList();

                SystemLogs = new ObservableCollection<SystemLog>(logs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading system logs: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ShowOldValues(SystemLog log)
        {
            if (string.IsNullOrEmpty(log.OldValues))
            {
                MessageBox.Show("No old values available.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var formattedJson = FormatJson(log.OldValues);
                ShowJsonDialog("Old Values", formattedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying old values: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ShowNewValues(SystemLog log)
        {
            if (string.IsNullOrEmpty(log.NewValues))
            {
                MessageBox.Show("No new values available.", "Information",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var formattedJson = FormatJson(log.NewValues);
                ShowJsonDialog("New Values", formattedJson);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying new values: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ShowDetails(SystemLog log)
        {
            if (log == null) return;

            var details = new StringBuilder()
                .AppendLine($"Log ID: {log.LogId}")
                .AppendLine($"Date/Time: {log.LogDate:yyyy-MM-dd HH:mm:ss}")
                .AppendLine($"Level: {log.LogLevel}")
                .AppendLine($"Source: {log.LogSource}")
                .AppendLine($"Action: {log.Action}")
                .AppendLine($"Entity Type: {log.EntityType ?? "N/A"}")
                .AppendLine($"Entity ID: {log.EntityId?.ToString() ?? "N/A"}")
                .AppendLine($"User: {log.User?.Username ?? "N/A"}")
                .AppendLine($"IP Address: {log.IpAddress ?? "N/A"}")
                .AppendLine($"User Agent: {log.UserAgent ?? "N/A"}")
                .AppendLine($"Additional Info: {log.AdditionalInfo ?? "N/A"}");

            MessageBox.Show(details.ToString(), "Log Details", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Refresh()
        {
            LoadSystemLogs();
        }

        private string FormatJson(string json)
        {
            try
            {
                // Parse and format the JSON for display
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch
            {
                // If parsing fails, return the original string
                return json;
            }
        }

        private void ShowJsonDialog(string title, string content)
        {
            MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}