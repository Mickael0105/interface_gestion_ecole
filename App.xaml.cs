using System.Windows;

namespace MyApp.Interface
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Une erreur inattendue est survenue : " + e.Exception.Message + "\n\n" + e.Exception.StackTrace, 
                "Erreur Critique", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true; // Prevent crash if possible
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                MyApp.Interface.Services.InMemoryStore.Save();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Erreur lors de la sauvegarde : " + ex.Message);
            }
            base.OnExit(e);
        }
    }
}

