using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Nac.Wpf.Fuzzy {
    /// <summary>
    /// Interaction logic for FuzzyRuleEditor.xaml
    /// </summary>
    public partial class NacWpfFuzzyRuleEditor : Window {
        public NacWpfFuzzyRuleEditor() {
            InitializeComponent();
        }

        public NacWpfFuzzyRuleEditor(HashSet<string> rules) {
            InitializeComponent();
            HashSet<string> edit = rules == null ? new HashSet<string>() : new HashSet<string>(rules);
            DataContext = new NacWpfFuzzyRuleEditorViewModel(edit);
        }

        public HashSet<string> Return { get { return new HashSet<string>((DataContext as NacWpfFuzzyRuleEditorViewModel).Select(fr => fr.Content)); } }

        private void Add_Click(object sender, RoutedEventArgs e) {
            var viewModel = DataContext as NacWpfFuzzyRuleEditorViewModel;
            if (viewModel.Current.IsSyntaxValid) viewModel.Add(new NacWpfFuzzyRule(viewModel.Current));
        }

        private void Remove_Click(object sender, RoutedEventArgs e) {
            var viewModel = DataContext as NacWpfFuzzyRuleEditorViewModel;
            var sel = ruleListBox.SelectedItems.OfType<NacWpfFuzzyRule>().ToArray();
            foreach (var item in sel) viewModel.Remove(item);
            ruleListBox.DataContext = null;
            ruleListBox.DataContext = viewModel;
        }

        private void OK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
