using System;
using System.Windows.Data;

//http://stackoverflow.com/questions/8701117/whats-the-wpf-way-to-add-support-for-this-feature-to-an-existing-color-editor

namespace KanbanizeToolUi
{
    public class SystemDrawingColorConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)value;
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
