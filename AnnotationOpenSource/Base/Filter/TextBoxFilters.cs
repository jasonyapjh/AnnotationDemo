using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Base.Filter
{
    public static class TextBoxFilters
    {
        private static readonly List<Key> _controlKeys = new List<Key>
                                                            {
                                                                Key.Back,
                                                                Key.CapsLock,
                                                                Key.LeftCtrl,
                                                                Key.RightCtrl,
                                                                Key.Down,
                                                                Key.End,
                                                                Key.Enter,
                                                                Key.Escape,
                                                                Key.Home,
                                                                Key.Insert,
                                                                Key.Left,
                                                                Key.PageDown,
                                                                Key.PageUp,
                                                                Key.Right,
                                                                Key.LeftShift,
                                                                Key.RightShift,
                                                                Key.Tab,
                                                                Key.Up
                                                            };

        private static bool _IsDigit(Key key)
        {
            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            bool retVal;
            if (key >= Key.D0 && key <= Key.D9 && !shiftKey)
            {
                retVal = true;
            }
            else
            {
                retVal = key >= Key.NumPad0 && key <= Key.NumPad9;
            }
            if (key == Key.Decimal)
            {
                retVal = true;
            }
            return retVal;
        }
        public static bool GetIsPositiveNumericFilter(DependencyObject src)
        {
            return (bool)src.GetValue(IsPositiveNumericFilterProperty);
        }

        public static void SetIsPositiveNumericFilter(DependencyObject src, bool value)
        {
            src.SetValue(IsPositiveNumericFilterProperty, value);
        }

        public static DependencyProperty IsPositiveNumericFilterProperty =
            DependencyProperty.RegisterAttached(
            "IsPositiveNumericFilter", typeof(bool), typeof(TextBoxFilters),
            new PropertyMetadata(false, IsPositiveNumericFilterChanged));

        public static void IsPositiveNumericFilterChanged
            (DependencyObject src, DependencyPropertyChangedEventArgs args)
        {
            if (src != null && src is TextBox)
            {
                TextBox textBox = src as TextBox;

                if ((bool)args.NewValue)
                {
                    textBox.KeyDown += _TextBoxPositiveNumericKeyDown;
                }
            }
        }

        static void _TextBoxPositiveNumericKeyDown(object sender, KeyEventArgs e)
        {
            bool handled = true;

            if (_controlKeys.Contains(e.Key) || _IsDigit(e.Key))
            {
                handled = false;
            }

            e.Handled = handled;
        }
    }
}
