using CokeOvenSystem.ViewModels;
using System.Windows;

namespace CokeOvenSystem.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ShutdownSystem();
            }
        }
    }
}