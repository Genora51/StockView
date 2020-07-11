using StockView.UI.Wrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StockView.UI.View.Converters
{
    public class ComparisonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                var snapshot = (StockSnapshotWrapper)values[1];
                IEnumerable<StockSnapshotWrapper> snapshots;
                if (values[0] is IEnumerable<StockSnapshotWrapper> enumerable)
                    snapshots = enumerable;
                else snapshots = default;
                var prevSnap = snapshots.FindPrevious(snapshot);
                return CompareSnapshots(prevSnap, snapshot);
            } else if (values.Length >= 3)
            {
                // TODO: Implement
                StockSnapshotWrapper snapshot;
                if (values[0] is StockSnapshotWrapper snap)
                    snapshot = snap;
                else return 0;
                var columnName = snapshot.Model.Stock.Symbol; // values[1].ToString();
                DataView table;
                if (values[2] is DataView view)
                    return 0;
                else if (values[2] is DataRowView row)
                    table = row.DataView;
                else return 0;
                var prevSnap = table.Cast<DataRowView>().Select(
                    r => r[columnName]
                ).OfType<StockSnapshotWrapper>().FindPrevious(snapshot);
                return CompareSnapshots(prevSnap, snapshot);
            }
            return 0;

        }

        private int CompareSnapshots(StockSnapshotWrapper prevSnap, StockSnapshotWrapper snapshot)
        {
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
