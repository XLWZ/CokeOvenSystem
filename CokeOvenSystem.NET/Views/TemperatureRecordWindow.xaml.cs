using CokeOvenSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CokeOvenSystem.Views
{
    public partial class TemperatureRecordWindow : Window
    {
        public TemperatureRecordWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TemperatureRecordViewModel viewModel)
            {
                viewModel.SaveSuccess += () =>
                {
                    // 保存成功后设置焦点到第一个温度输入框
                    Dispatcher.BeginInvoke(() =>
                    {
                        Oven1MachineTempTextBox.Focus();
                    }, System.Windows.Threading.DispatcherPriority.Input);
                };
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Oven3CokeTempTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // 改为触发命令而不是直接调用方法
                if (DataContext is TemperatureRecordViewModel viewModel)
                {
                    viewModel.SaveRecordCommand.Execute(null);
                }
            }
        }
    }
}