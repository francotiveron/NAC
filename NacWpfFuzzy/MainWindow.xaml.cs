using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nac.Wpf.Fuzzy {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void fsButton_Click(object sender, RoutedEventArgs e) {
            var fsEditor = new NacWpfFuzzySetEditor();
            fsEditor.Owner = this;
            //fsEditor.DataContext = new NacWpfFuzzySetEditorViewModel();
            fsEditor.ShowDialog();
            if (fsEditor.DialogResult.Value) {
                var sets = 
                (fsEditor.DataContext as NacWpfFuzzySetEditorViewModel)
                .Select(item => item.ToString())
                .ToArray();
                
                MessageBox.Show(string.Join(",", sets));
            }
        }

        private void frButton_Click(object sender, RoutedEventArgs e) {
            var frEditor = new NacWpfFuzzyRuleEditor();
            frEditor.Owner = this;
            //frEditor.DataContext = new NacWpfFuzzySetEditorViewModel();
            frEditor.ShowDialog();
            if (frEditor.DialogResult.Value) {
                var rules =
                (frEditor.DataContext as NacWpfFuzzyRuleEditorViewModel)
                .Select(item => item.ToString())
                .ToArray();

                MessageBox.Show(string.Join(",", rules));
            }
        }
    }
}
