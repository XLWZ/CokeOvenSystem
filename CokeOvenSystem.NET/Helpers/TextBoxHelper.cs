using System.Windows;
using System.Windows.Controls;

namespace CokeOvenSystem.Helpers
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty AutoSelectAllProperty =
            DependencyProperty.RegisterAttached(
                "AutoSelectAll",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(false, OnAutoSelectAllChanged));

        public static bool GetAutoSelectAll(TextBox textBox) =>
            (bool)textBox.GetValue(AutoSelectAllProperty);

        public static void SetAutoSelectAll(TextBox textBox, bool value) =>
            textBox.SetValue(AutoSelectAllProperty, value);

        private static void OnAutoSelectAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.GotFocus -= TextBox_GotFocus;

                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += TextBox_GotFocus;
                }
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // 使用Dispatcher确保在焦点设置完成后执行全选
                textBox.Dispatcher.BeginInvoke(() =>
                {
                    textBox.SelectAll();
                }, System.Windows.Threading.DispatcherPriority.Input);
            }
        }
    }
}