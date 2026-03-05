using System;
using System.Globalization;
using System.Windows.Data;
using MyApp.Interface.Services;

namespace MyApp.Interface.Converters
{
    public class ClassNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            // gère valeurs boxed Guid et chaînes contenant Guid
            if (value is Guid g) return InMemoryStore.ClassNameFor(g);
            if (value is string s && Guid.TryParse(s, out var parsed)) return InMemoryStore.ClassNameFor(parsed);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    public class StudentNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is Guid g) return InMemoryStore.StudentName(g);
            if (value is string s && Guid.TryParse(s, out var parsed)) return InMemoryStore.StudentName(parsed);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    public class SubjectNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is Guid g) return InMemoryStore.SubjectName(g);
            if (value is string s && Guid.TryParse(s, out var parsed)) return InMemoryStore.SubjectName(parsed);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}