using Nac.Common;
using Nac.Common.Control;
using System.Windows;

namespace Nac.Code.Editor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
        }

        private void OpenDialogButton_Click(object sender, RoutedEventArgs e) {
            var codeEditor = new NacCodeEditor();
            codeEditor.Owner = Application.Current.MainWindow;
            if (codeEditor.ShowDialog() == true) {
                MessageBox.Show(codeEditor.Text);
            }
        }
    }
}
