using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CokeOvenSystem.Models;
using CokeOvenSystem.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace CokeOvenSystem.ViewModels
{
    public partial class TemperatureRecordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _statusMessage;

        [ObservableProperty]
        private bool _isStatusVisible;

        [ObservableProperty]
        private TemperatureRecord _record = new(); 

        private readonly DispatcherTimer _statusTimer;

        public event Action SaveSuccess;

        public TemperatureRecordViewModel()
        {
            _statusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _statusTimer.Tick += (s, e) =>
            {
                IsStatusVisible = false;
                _statusTimer.Stop();
            };
        }

        [RelayCommand]
        private void SaveRecord()
        {
            if (string.IsNullOrEmpty(Record.TimePoint))
            {
                MessageBox.Show("请选择时间点", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime recordDate;
            if (!DateTime.TryParseExact(Record.Date, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out recordDate))
            {
                MessageBox.Show("日期格式不正确，请使用YYYY-MM-DD格式", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fullTime = $"{recordDate:yyyy-MM-dd} {Record.TimePoint}:00";

            // 记录1号焦炉温度
            int result = NativeInterop.record_temperature(1, fullTime,
                Record.Oven1MachineTemp, Record.Oven1CokeTemp);
            if (result != 0)
            {
                MessageBox.Show($"1号焦炉温度记录失败，错误代码: {result}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 记录2号焦炉温度
            result = NativeInterop.record_temperature(2, fullTime,
                Record.Oven2MachineTemp, Record.Oven2CokeTemp);
            if (result != 0)
            {
                MessageBox.Show($"2号焦炉温度记录失败，错误代码: {result}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 记录3号焦炉温度
            result = NativeInterop.record_temperature(3, fullTime,
                Record.Oven3MachineTemp, Record.Oven3CokeTemp);
            if (result != 0)
            {
                MessageBox.Show($"3号焦炉温度记录失败，错误代码: {result}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ShowStatus("温度记录保存成功！", true);
            SaveSuccess?.Invoke();

            // 保存成功后推进到下一个时间点
            MoveToNextTimePoint();
        }

        /// <summary>
        /// 推进到下一测温时间点
        /// </summary>
        private void MoveToNextTimePoint()
        {
            // 确保时间点数组不为空
            if (AppModel.TimePoints == null || AppModel.TimePoints.Length == 0)
            {
                return;
            }

            // 获取当前时间点在数组中的索引
            int currentIndex = Array.IndexOf(AppModel.TimePoints, Record.TimePoint);

            if (currentIndex == -1)
            {
                // 如果未找到当前时间点，设为第一个时间点
                Record.TimePoint = AppModel.TimePoints[0];
                return;
            }

            // 计算下一个索引
            int nextIndex = currentIndex + 1;

            if(nextIndex>=AppModel.TimePoints.Length)
            {
                // 如果是最后一个时间点，日期加一天，时间点重置为第一个
                DateTime currentDate;
                if(DateTime.TryParseExact(Record.Date, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out currentDate))
                {
                    Record.Date = currentDate.AddDays(1).ToString("yyyy-MM-dd");
                }
                else
                {
                    // 时间解析失败，则使用今天加一天
                    Record.Date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                }
                Record.TimePoint = AppModel.TimePoints[0];
            }
            else
            {
                // 否则推进到下一个时间点
                Record.TimePoint = AppModel.TimePoints[nextIndex];
            }
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            StatusMessage = message;
            IsStatusVisible = true;
            _statusTimer.Stop();
            _statusTimer.Start();
        }
    }
}