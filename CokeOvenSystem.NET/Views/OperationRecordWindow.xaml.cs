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