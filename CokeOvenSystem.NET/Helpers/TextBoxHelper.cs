using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        #region 时间输入行为附加属性
        public static readonly DependencyProperty TimeInputBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "TimeInputBehavior",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(false, OnTimeInputBehaviorChanged));

        public static bool GetTimeInputBehavior(TextBox textBox) =>
            (bool)textBox.GetValue(TimeInputBehaviorProperty);

        public static void SetTimeInputBehavior(TextBlock textBox, bool value) =>
            textBox.SetValue(TimeInputBehaviorProperty, value);

        private static void OnTimeInputBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotFocus -= TimeTextBox_GotFocus;
                textBox.PreviewMouseDown -= TimeTextBox_PreviewMouseDown;

                if ((bool)e.NewValue)
                {
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.GotFocus += TimeTextBox_GotFocus;
                    textBox.PreviewMouseDown += TimeTextBox_PreviewMouseDown;
                }
            }
        }

        private static void TimeTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (sender is TextBox textBox)
                {
                    textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private static void TimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // 延迟执行以确保焦点设置完成
                textBox.Dispatcher.BeginInvoke(() =>
                {
                    // 尝试选中分钟部分
                    if (textBox.Text.Length == 5 && textBox.Text[2] == ':')
                    {
                        textBox.Select(3, 2);
                    }
                    else if (textBox.Text.Length == 4)
                    {
                        // 没有冒号时，尝试格式化为 HH:mm
                        FormatTimeText(textBox);
                        if (textBox.Text.Length == 5)
                        {
                            textBox.Select(3, 2);
                        }
                        else
                        {
                            textBox.SelectAll();
                        }
                    }
                    else
                    {
                        textBox.SelectAll();
                    }
                }, System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // 只允许输入数字和冒号与退格键
                if(!char.IsDigit(e.Text, 0) && e.Text != ":")
                {
                    e.Handled = true;
                    return;
                }

                // 获取当前文本与光标位置
                string currentText = textBox.Text;
                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;

                // 处理冒号自动输入
                if (char.IsDigit(e.Text, 0))
                {
                    // 如果输入数字时文本长度达到2且没有冒号，自动输入冒号
                    if (selectionStart == 2 && !currentText.Contains(":"))
                    {
                        textBox.Text = currentText.Substring(0, 2) + ":" + e.Text;
                        textBox.CaretIndex = 4; // 光标定位到分钟
                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormatTimeText(textBox);
            }
        }

        private static void FormatTimeText(TextBox textBox)
        {
            string text = textBox.Text;
            int caretIndex = textBox.CaretIndex;

            // 移除所有非数字字符（除 ':' 保留）
            string cleanText = Regex.Replace(text, @"[^\d:]", "");

            // 处理 HHmm 格式 （四位数字）
            if (cleanText.Length == 4 && !cleanText.Contains(":"))
            {
                cleanText = $"{cleanText.Substring(0, 2)}:{cleanText.Substring(2,2)}";
                caretIndex = caretIndex < 2 ? caretIndex :
                    caretIndex == 2 ? 3 :
                    caretIndex + 1;
            }

            // 确保格式为 HH:mm
            if (cleanText.Length >= 2 && !cleanText.Contains(":"))
            {
                cleanText = cleanText.Insert(2, ":");
                if (caretIndex >= 2) caretIndex++;
            }

            // 限制长度
            if (cleanText.Length > 5)
            {
                cleanText = cleanText.Substring(0, 5);
            }

            // 验证时间有效性
            if (cleanText.Length == 5 && cleanText[2] == ':')
            {
                string[] parts = cleanText.Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int hours) && (hours < 0 || hours > 23))
                    {
                        cleanText = textBox.Text; // 恢复原文本
                    }
                    else if (int.TryParse(parts[1], out int minutes) && (minutes < 0 || minutes > 59))
                    {
                        cleanText += textBox.Text;
                    }
                }
            }

            // 更新文本框内容
            if (text != cleanText)
            {
                textBox.Text = cleanText;
                textBox.CaretIndex = Math.Min(caretIndex, cleanText.Length);
            }
        }
        #endregion
    }
}