using System.Configuration;
using System.Data;
using System.Windows;
using Wpf.Ui.Appearance;

namespace SmartLedger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplicationAccentColorManager.Apply(
                System.Windows.Media.Color.FromRgb(16, 137, 62) 
            );
        }
    }

}
