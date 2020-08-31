using Autofac;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using StockView.UI.Startup;
using System.IO;
using System.Windows;
using System.Xml;

namespace StockView.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            using (var stream = new MemoryStream(UI.Properties.Resources.Lua))
            {
                using (var xmlReader = new XmlTextReader(stream))
                {
                    var highlight = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);
                    HighlightingManager.Instance.RegisterHighlighting(
                        "Lua", new string[0], highlight
                    );
                }
            }
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if !DEBUG
            MessageBox.Show("Unexpected error occurred. Please inform the administrator."
                + Environment.NewLine + e.Exception.Message, "Unexpected error");

            e.Handled = true;
#endif
        }
    }
}
