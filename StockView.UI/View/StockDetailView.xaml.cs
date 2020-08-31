using System.Windows.Controls;
using System.Windows.Input;

namespace StockView.UI.View
{
    /// <summary>
    /// Interaction logic for StockDetailView.xaml
    /// </summary>
    public partial class StockDetailView : UserControl
    {
        public StockDetailView()
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
    }
}
