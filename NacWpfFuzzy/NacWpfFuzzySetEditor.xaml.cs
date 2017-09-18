using Nac.Common.Fuzzy;
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
using System.Windows.Shapes;
using System.Globalization;
using Accord.Fuzzy;
using Accord;
using Nac.Fuzzy.Common;
using Nac.Common.Control;

namespace Nac.Wpf.Fuzzy {
    /// <summary>
    /// Interaction logic for FuzzySetEditor.xaml
    /// </summary>
    public partial class NacWpfFuzzySetEditor : Window {
        private const int cChartPoints = 100;

        public NacWpfFuzzySetEditor() {
            InitializeComponent();
        }

        public NacWpfFuzzySetEditor(HashSet<Common.Fuzzy.NacFuzzySet> sets) {
            InitializeComponent();
            HashSet<Common.Fuzzy.NacFuzzySet> edit = sets == null ? new HashSet<Common.Fuzzy.NacFuzzySet>() : new HashSet<Common.Fuzzy.NacFuzzySet>(sets);
            DataContext = new NacWpfFuzzySetEditorViewModel(edit);
        }

        public HashSet<Common.Fuzzy.NacFuzzySet> Return { get { return new HashSet<Common.Fuzzy.NacFuzzySet>((DataContext as NacWpfFuzzySetEditorViewModel).Select(fs => fs.Base)); } }

        private void addButton_Click(object sender, RoutedEventArgs e) {
            var newSet = new NacWpfFuzzySet();
            var viewModel = DataContext as NacWpfFuzzySetEditorViewModel;
            viewModel.Add(newSet);
            fsListbox.SelectedItem = newSet;
            nameTextBox.Focus();
            nameTextBox.SelectAll();
        }

        private void remButton_Click(object sender, RoutedEventArgs e) {
            if (fsListbox.SelectedIndex < 0) return;
            var viewModel = DataContext as NacWpfFuzzySetEditorViewModel;
            viewModel.RemoveAt(fsListbox.SelectedIndex);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) RefreshListBox();
        }
        private void edgeRadio_Click(object sender, RoutedEventArgs e) {
            RefreshListBox();
        }
        private void RefreshListBox() {
            fsListbox.Items.Refresh();
            Keyboard.Focus(fsListbox);
        }

        private void RefreshChart() {
            float minX = float.MaxValue, maxX = float.MinValue;
            fsChart.RemoveAllDataSeries();
            var viewModel = DataContext as NacWpfFuzzySetEditorViewModel;

            var fuzzySets = viewModel.Select(fs => fs.Base.GetFuzzySet());
            minX = fuzzySets.Min(fs => fs.LeftLimit);
            maxX = fuzzySets.Max(fs => fs.RightLimit);

            foreach (var fuzzySet in fuzzySets) {
                fsChart.AddDataSeries(fuzzySet.Name, System.Drawing.Color.Red, Accord.Controls.Chart.SeriesType.Line, fuzzySet.Name == SelectedSet ? 5 : 2);
                fsChart.UpdateDataSeries(fuzzySet.Name, CalcSeries(fuzzySet, minX, maxX));
            }

            fsChart.RangeX = new Range(minX, maxX);
            fsChart.RangeY = new Range(0f, 1f);
        }

        private double[,] CalcSeries(FuzzySet fuzzySet, float minX, float maxX) {
            double[,] ret = new double[cChartPoints + 1, 2];
            double xGap = (maxX - minX) / cChartPoints;

            for (int i = 0; i <= cChartPoints; i++) {
                double x = ret[i, 0] = minX + xGap * i;
                ret[i, 1] = fuzzySet.GetMembership((float)x);
            }
            return ret;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var viewModel = DataContext as NacWpfFuzzySetEditorViewModel;
            viewModel.CollectionChanged += ViewModel_CollectionChanged;
        }

        private void ViewModel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            bool refresh = false;

            if (e.NewItems?.Count > 0) {
                foreach(NacWpfFuzzySet fuzzySet in e.NewItems) fuzzySet.PropertyChanged += FuzzySet_PropertyChanged;
                refresh = true;
            }
            if (e.OldItems?.Count > 0) {
                foreach (NacWpfFuzzySet fuzzySet in e.OldItems) fuzzySet.PropertyChanged -= FuzzySet_PropertyChanged;
                refresh = true;
            }
            if (refresh) RefreshChart();
        }

        private void FuzzySet_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            RefreshChart();
        }
        private string SelectedSet {
            get {
                if (fsListbox.SelectedItems.Count < 1) return null;
                var fuzzySet = fsListbox.SelectedItems[0] as NacWpfFuzzySet;
                return fuzzySet.Name;
            }
        }
        private void fsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            RefreshChart();
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

    [ValueConversion(typeof(int), typeof(bool))]
    public class FuzzySetEdgeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var ival = (int)value;
            var ipar = int.Parse(parameter as string);
            return ival == ipar;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return int.Parse(parameter as string); ;
        }
    }
}
