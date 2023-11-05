using ReScanVisualizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace ReScanVisualizer.Converters
{
    public class CommandKeyToInputGestureTextConverter : IValueConverter
    {
        private static readonly ModifierKeysConverter keysConverter = new ModifierKeysConverter();
        private static readonly KeyConverter keyConverter = new KeyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string modifiers = string.Empty;
            string keys = string.Empty;

            if (value is KeyBinding keyBinding)
            {
                modifiers = keysConverter.ConvertToString(keyBinding.Modifiers);
                keys = keyConverter.ConvertToString(keyBinding.Key);
            }
            else if (value is CommandKey commandGestureKey)
            {
                modifiers = keysConverter.ConvertToString(commandGestureKey.Modifiers);
                keys = keyConverter.ConvertToString(commandGestureKey.Key);
            }
            string[] tmp = string.IsNullOrEmpty(modifiers) ? keys.Split('+') : $"{modifiers}+{keys}".Split('+');

            for (int i = 0; i < tmp.Length; i++)
            {
                switch (tmp[i])
                {
                    //case "Escape":
                    //case "Esc":
                    //    tmp[i] = "Ecs";
                    //    break;
                    //case "Return":
                    //    tmp[i] = "Enter";
                    //    break;
                    //case "Back":
                    //case "Backspace":
                    //    tmp[i] = "Back";
                    //    break;
                    //case "Delete":
                    //    tmp[i] = "Delete";
                    //    break;
                    case "OemComma":
                        tmp[i] = "?";
                        break;
                }
            }

            return parameter is null ? string.Join("+", tmp) : $"{parameter.ToString().Replace('_', ' ')} [{string.Join("+", tmp)}]";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
