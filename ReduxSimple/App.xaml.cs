using ReduxSimple.Redux;
using System.Windows;
using Unity;

namespace ReduxSimple
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var container = new UnityContainer();

            var store = new ReduxAppStore();
            store.InitializeStore(container);
            store.OpenDevTools();
            container.RegisterInstance<IStore>(store);

            var mainWindow = new MainWindow()
            {
                DataContext = container.Resolve<MainWindowViewModel>()
            };
            mainWindow.Show();
        }
    }
}
