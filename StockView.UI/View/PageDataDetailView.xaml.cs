using StockView.UI.Wrapper;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
                var column = new MyDataGridTemplateColumn
                {
                    CellTemplate = (DataTemplate)dataGrid1.FindResource("SnapshotTemplate"),
                    CellEditingTemplate = (DataTemplate)dataGrid1.FindResource("SnapshotEditingTemplate"),
                    ColumnName = tc.Header.ToString(),
                    MinWidth = 75,
                    Header = tc.Header
                };
                e.Column = column;
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

    public class MyDataGridTemplateColumn : DataGridTemplateColumn
    {
        public string ColumnName
        {
            get;
            set;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            // The DataGridTemplateColumn uses ContentPresenter with your DataTemplate.
            ContentPresenter cp = (ContentPresenter)base.GenerateElement(cell, dataItem);
            // Reset the Binding to the specific column. The default binding is to the DataRowView.
            BindingOperations.SetBinding(cp, ContentPresenter.ContentProperty, new Binding(this.ColumnName));
            return cp;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            ContentPresenter cp = (ContentPresenter)base.GenerateEditingElement(cell, dataItem);
            // Reset the Binding to the specific column. The default binding is to the DataRowView.
            BindingOperations.SetBinding(cp, ContentPresenter.ContentProperty, new Binding(this.ColumnName));
            return cp;
        }
    }
}