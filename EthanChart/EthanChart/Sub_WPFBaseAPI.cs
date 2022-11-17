using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WPFBaseAPI_LYH.BooleanToVisibilityConverter;
using System.Windows;
using System.Windows.Data;

namespace EthanChart
{
    public class ViewModelBase : WPFBaseAPI_LYH.ViewModelBase { }
    public class DelegateCommand : WPFBaseAPI_LYH.DelegateCommand
    {
        public DelegateCommand(Action execute) : base(execute) { }
        public DelegateCommand(Action<object> _handler) : base(_handler) { }
        public DelegateCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute) { }
        public DelegateCommand(Action<object> _handler, Func<bool> canExecute) : base(_handler, canExecute) { }
    }
    public class BooleanToVisibilityConverter : WPFBaseAPI_LYH.BooleanToVisibilityConverter { }
    public class BindingProxy : WPFBaseAPI_LYH.BindingProxy { }
    public class INIFile : WPFBaseAPI_LYH.Files.INIFile
    {
        public static string INIPath { get; private set; } = Environment.CurrentDirectory + "\\EtherCATSlaveConfigurator.ini";
    }

    public class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
