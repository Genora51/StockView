using StockView.UI.Wrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StockView.UI.View.Converters
{
    public class ComparisonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return 0;

            var snapshot = (StockSnapshotWrapper) values[1];
            IEnumerable<StockSnapshotWrapper> snapshots;
            if (values[0] is IEnumerable<StockSnapshotWrapper> enumerable)
                snapshots = enumerable;
            else snapshots = default;

            var prevSnap = snapshots.FindPrevious(snapshot);
            if (prevSnap == null) return 0;
            var value = snapshot.Value;
            var prevValue = prevSnap.Value;
            if (value > prevValue) return 1;
            if (value < prevValue) return -1;
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class FindPreviousExtension
    {
        public static T FindPrevious<T>
           (this IEnumerable<T> source,
            T target)
        {
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    return default;
                }
                T previous = iterator.Current;
                while (iterator.MoveNext())
                {
                    if (iterator.Current.Equals(target))
                        return previous;
                    previous = iterator.Current;
                }
                return default;
            }
        }
    }
}
