using Nac.Wpf.Common;
using Nac.Wpf.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static NacWpfGlobal;

namespace NacXbapHost {
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            MessageBox.Show(e.ExceptionObject.ToString(), "CurrentDomain_UnhandledException");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            NacSecurity.Off = true;
            NacWpfContext.Create();
            G.AddEngine(new NacWpfEngine("offline"));

            //Parser.Run(Environment.GetCommandLineArgs().Skip(1).ToArray(), this);

            //context.AddEngine(new NacWpfEngine("127.0.0.1")); //temp
            //context.AddEngine(new NacWpfEngine("203.15.179.7")); //temp
            DataContext = G;

            Main2 main = new Main2();
            Content = main;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            NacWpfContext.Destroy();
        }

    }
}
