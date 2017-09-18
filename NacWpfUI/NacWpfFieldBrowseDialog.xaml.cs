using Nac.Common.Control;
using Nac.Wcf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static NacUtils;
using static NacWpfUtils;

namespace Nac.Wpf.UI {
    /// <summary>
    /// Interaction logic for NacWpfFieldBrowseDialog.xaml
    /// </summary>
    public partial class NacWpfFieldBrowseDialog : Window {
        private const string cLoopback = "127.0.0.1";

        public NacWpfFieldBrowseDialog(string ip = cLoopback) {
            InitializeComponent();
            DataContext = Model = new ViewModel() { Servers = new string[] { ip } };
        }
        public class Row : INotifyPropertyChanged {
            public int N { get; set; }
            bool _selected;
            public bool Selected { get { return _selected; } set { _selected = value; OnNotifyPropertyChanged("Selected"); } }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnNotifyPropertyChanged(string property) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
        public class ViewModel : INotifyPropertyChanged {
            public string[] Servers { get; set; }
            public string Filter { get; set; } = "*";
            public ObservableCollection<Row> Tags { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnNotifyPropertyChanged(string property) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
        public ViewModel Model { get; set; }

        private void queryButton_Click(object sender, RoutedEventArgs e) {
            queryButton.Focus();
            using(new NacWpfWaitCursor()) {                
                var client = new NacWcfFieldClient(_lastQueryUri = $@"net.tcp://{serversCombo.Text}:65457");
                var tags = Decompress(client.Browse(Model.Filter), @"#");
                int i = 1;
                Model.Tags = new ObservableCollection<Row>(tags.Select(s => {
                    var name_type_desc = s.Split(',');
                    if (name_type_desc.Length == 3)
                        return new Row() { N = i++, Selected = false, Name = name_type_desc[0], Type = name_type_desc[1], Description = name_type_desc[2] };
                    else return new Row() { N = i++, Selected = false, Name = "???", Type = "???" };
                }));
                Model.OnNotifyPropertyChanged("Tags");
            }
        }

        public class AddTagsEventArgs : EventArgs {
            public string FieldEndpoint;
            public NacTagScope Scope;
            public Row[] Tags;
        }
        public delegate void AddTagsEventHandler(object sender, AddTagsEventArgs e);
        public event AddTagsEventHandler AddTagsEvent;
        private void addInputButton_Click(object sender, RoutedEventArgs e) { AddTags(NacTagScope.Input); }
       
        private void addOutputButton_Click(object sender, RoutedEventArgs e) { AddTags(NacTagScope.Output); }

        string _lastQueryUri;
        private void AddTags(NacTagScope scope) {
                AddTagsEvent?.
                Invoke(this,
                        new AddTagsEventArgs() {
                            FieldEndpoint = _lastQueryUri,
                            Scope = scope,
                            Tags = Model.Tags.Where(t => t.Selected).ToArray()
                        }
                );
        }

        private void dataGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e) {
            if (e.PropertyName == "Selected") {
                DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();
                templateColumn.CellTemplate = (DataTemplate)Resources["SelectedColumnTemplate"];
                templateColumn.HeaderTemplate = (DataTemplate)Resources["ToggleHeaderTemplate"];
                e.Column = templateColumn;
            }
        }

        private void selectedCheckbox_Click(object sender, RoutedEventArgs e) {
            var value = (sender as CheckBox).IsChecked == true;
            foreach (Row r in dataGrid.SelectedItems) r.Selected = value;
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e) {
            foreach (Row r in dataGrid.SelectedItems) r.Selected ^= true;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
