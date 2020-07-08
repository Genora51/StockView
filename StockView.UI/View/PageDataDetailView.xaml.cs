using StockView.UI.Wrapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockView.UI.View
{
    /// <summary>
    /// Interaction logic for PageDataDetailView.xaml
    /// </summary>
    public partial class PageDataDetailView : UserControl
    {
        public PageDataDetailView()
        {
            InitializeComponent();
        }

        private void DatePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dataGrid1.CommitEdit();
            }
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var tc = e.Column as DataGridTextColumn;
            if (e.PropertyType == typeof(DateTime)) {
                var column = new DataGridTemplateColumn
                {
                    CellTemplate = (DataTemplate)dataGrid1.FindResource("DateTemplate"),
                    CellEditingTemplate = (DataTemplate)dataGrid1.FindResource("DateEditingTemplate"),
                    Header = tc.Header,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                };
                e.Column = column;
            } else if (e.PropertyType == typeof(StockSnapshotWrapper)) {
                var binding = tc.Binding as Binding;
                binding.Path = new PropertyPath(binding.Path.Path + ".Value");
                binding.FallbackValue = "-";
                binding.StringFormat = "F2";
                tc.ElementStyle = (Style)dataGrid1.FindResource("PDColumnStyle");
                tc.EditingElementStyle = (Style)FindResource("DataGridEditingColumnStyle");
            }
            
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (((DataRowView)e.Row.Item)[e.Column.Header.ToString()] is DBNull)
            {
                e.Cancel = true;
            }
        }
    }
}