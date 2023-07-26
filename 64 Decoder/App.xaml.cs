using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace _64_Decoder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
