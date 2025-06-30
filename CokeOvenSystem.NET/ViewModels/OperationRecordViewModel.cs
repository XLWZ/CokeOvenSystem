using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CokeOvenSystem.Models;
using CokeOvenSystem.Services;
using System.Windows;
using System.Windows.Threading;

namespace CokeOvenSystem.ViewModels
{
    public partial class OperationRecordViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ChamberOptions))]
        private int _ovenNumber = 1;

        [ObservableProperty]
        private string _chamber = string.Empty;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _isStatusVisible;

        [ObservableProperty]
        private string _previousPushTimeStr;

        [ObservableProperty]
        private string _newLoadTimeStr;

        [ObservableProperty]
        private DateTime _previousPushDate = DateTime.Now;

        [ObservableProperty]
        private DateTime _newLoadDate = DateTime.Now;

        public string[] ChamberOptions => AppModel.GetChambersForOven(OvenNumber);

        private readonly DispatcherTimer _statusTimer;

        public OperationRecordViewModel()
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

            // 初始化时间字符串为当前时间
            PreviousPushTimeStr = DateTime.Now.ToString("HH:mm");
            NewLoadTimeStr = DateTime.Now.ToString("HH:mm");
        }

        public event Action<string>? RequestFocus;

        [RelayCommand]
        public void SaveRecord()
        {
            if (string.IsNullOrEmpty(Chamber))
            {
                MessageBox.Show("请选择炭化室", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime previousPushTime;
            DateTime newLoadTime;

            if(!TryParseDateTime(PreviousPushDate, PreviousPushTimeStr, out previousPushTime))
            {
                MessageBox.Show("推焦时间格式错误，请使用HH:mm格式", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!TryParseDateTime(NewLoadDate, NewLoadTimeStr, out newLoadTime))
            {
                MessageBox.Show("装煤时间时间格式错误，请使用HH:mm格式", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 记录前一炉推焦操作
            int result = NativeInterop.record_operation(
                OvenNumber,
                Chamber,
                "PUSH",
                previousPushTime.ToString("yyyy-MM-dd HH:mm")
            );

            if (result != 0)
            {
                MessageBox.Show($"推焦记录失败，错误代码: {result}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 记录新一炉装煤操作
            result = NativeInterop.record_operation(
                OvenNumber,
                Chamber,
                "LOAD",
                newLoadTime.ToString("yyyy-MM-dd HH:mm")
            );

            if (result != 0)
            {
                MessageBox.Show($"装煤记录失败，错误代码: {result}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 保存成功，显示状态信息
            ShowStatus("操作记录保存成功！", true);

            AdvanceToNextChamber();

            // 触发焦点切换事件
            RequestFocus?.Invoke("PreviousPushTime");
        }

        private void AdvanceToNextChamber()
        {
            // 获取当前焦炉的顺序表
            string[] sequence = AppModel.GetSequenceForOven(OvenNumber);
            if (sequence.Length == 0) return;

            // 获取当前炭化室在顺序表中的位置
            int currentIndex = -1;
            for (int i = 0; i < sequence.Length; i++)
            {
                if (sequence[i] == Chamber)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex >= 0)
            {
                // 计算下一个炭化室的位置（循环）
                int nextIndex = (currentIndex + 1) % sequence.Length;
                Chamber = sequence[nextIndex];
            }
            else
            {
                // 当前炭化室不在循环表中（理论上不可能），则取第一个
                Chamber = sequence.FirstOrDefault() ?? string.Empty;
            }
        }

        private bool TryParseDateTime(DateTime date, string timeStr, out DateTime result)
        {
            result = date;

            if (string.IsNullOrWhiteSpace(timeStr))
                return false;

            // 尝试解析 HH:mm 时间格式
            string[] parts = timeStr.Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out int hours) || hours < 0 || hours > 23)
                return false;

            if (!int.TryParse(parts[1], out int minutes) || minutes < 0 || minutes > 59)
                return false;

            result = new DateTime(date.Year, date.Month, date.Day, hours, minutes, 0);
            return true;
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