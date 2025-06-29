using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CokeOvenSystem.Services;
using CokeOvenSystem.Views;
using Microsoft.Win32;
using System.Windows;

namespace CokeOvenSystem.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isDatabaseLoaded;

        [ObservableProperty]
        private string _databaseName = "未加载数据库";

        [ObservableProperty]
        private string _databasePath = "";

        [RelayCommand]
        private void InitializeSystem()
        {
            var dialog = new OpenFileDialog
            {
                Title = "选择数据库文件",
                Filter = "SQLite数据库|*.db;*.sqlite;*.sqlite3|所有文件|*.*",
                CheckFileExists = false
            };

            if (dialog.ShowDialog() == true)
            {
                int result = NativeInterop.InitSystem(dialog.FileName);
                if (result != 0)
                {
                    MessageBox.Show($"数据库初始化失败，错误代码: {result}", "错误",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateDatabaseStatus(false, "初始化失败", dialog.FileName);
                }
                else
                {
                    UpdateDatabaseStatus(true, System.IO.Path.GetFileName(dialog.FileName), dialog.FileName);
                    MessageBox.Show("数据库初始化成功!", "成功",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void UpdateDatabaseStatus(bool isLoaded, string name, string path)
        {
            IsDatabaseLoaded = isLoaded;
            DatabaseName = name;
            DatabasePath = path;
        }

        [RelayCommand]
        private void OpenTemperatureRecordWindow()
        {
            if (!IsDatabaseLoaded)
            {
                MessageBox.Show("请先加载数据库!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new TemperatureRecordWindow();
            window.ShowDialog();
        }

        [RelayCommand]
        private void OpenOperationRecordWindow()
        {
            if (!IsDatabaseLoaded)
            {
                MessageBox.Show("请先加载数据库！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var window = new OperationRecordWindow();
            window.ShowDialog();
        }

        public void ShutdownSystem()
        {
            NativeInterop.coke_system_shutdown();
            UpdateDatabaseStatus(false, "未加载数据库", "");
        }
    }
}