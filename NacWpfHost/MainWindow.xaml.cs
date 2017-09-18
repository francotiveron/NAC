using Nac.Wpf.Common;
using Nac.Wpf.UI;
using System;
using System.Windows;
using System.Linq;
using CLAP;
using static NacWpfGlobal;

namespace Nac.Wpf.Host {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            NacWpfContext.Create();
            G.AddEngine(new NacWpfEngine("offline"));

            Parser.Run(Environment.GetCommandLineArgs().Skip(1).ToArray(), this);

            //context.AddEngine(new NacWpfEngine("127.0.0.1")); //temp
            //context.AddEngine(new NacWpfEngine("203.15.179.7")); //temp
            DataContext = G;

            Main2 main = new Main2();
            Content = main;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            MessageBox.Show(e.ExceptionObject.ToString(), "CurrentDomain_UnhandledException");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            NacWpfContext.Destroy();
        }

        #region CLAP verbs
        [Verb(IsDefault = true)]
        void Servers(string[] ip) {
            foreach (var i in ip) G.AddEngine(new NacWpfEngine(i));
        }

        [Verb]
        void Options(string security) {
            NacSecurity.Off = security.Equals("off", StringComparison.OrdinalIgnoreCase);
        }

        [Empty]
        void NoInput() {}

        #endregion
    }
}
