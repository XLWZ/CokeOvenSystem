using CokeOvenSystem.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CokeOvenSystem.Views
{
    public partial class OperationRecordWindow : Window
    {
        public OperationRecordWindow()
        {
            InitializeComponent();

            if (DataContext is OperationRecordViewModel viewModel)
            {
                viewModel.RequestFocus += (target) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        switch (target)
                        {
                            case "PreviousPushTime":
                                PreviousPushTimeTextBox.Focus();
                                break;
                            case "NewLoadTime":
                                NewLoadTimeTextBox.Focus();
                                break;
                            default:
                                break;
                        }
                    }, System.Windows.Threading.DispatcherPriority.Input);
                };
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                // 在推焦时间框按 Tab 时，直接跳转到装煤时间框
                if (Keyboard.FocusedElement == PreviousPushTimeTextBox)
                {
                    NewLoadTimeTextBox.Focus();
                    e.Handled= true;
                    return;
                }
            }
            base.OnPreviewKeyDown(e);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewLoadTimeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(DataContext is OperationRecordViewModel viewModel)
                {
                    viewModel.SaveRecordCommand.Execute(null);
                }
            }
        }
    }
}