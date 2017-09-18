using Nac.Common.Control;
using Nac.Wpf.Common;
using Nac.Wpf.Common.Control;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Nac.Common;
using System;
using System.Windows.Input;
using System.Collections.Specialized;
using static NacWpfGlobal;

namespace Nac.Wpf.UI {
    /// <summary>
    /// Interaction logic for RTDBViewer1.xaml
    /// </summary>
    public partial class NacWpfDatabaseEditor1 : UserControl {
        public NacWpfDatabaseEditor1() {
            InitializeComponent();

        }
        public void NacWpfRefresh() {
            if (Database.Online) Refresh();
        }
        private NacWpfDatabase Database { get { return DataContext as NacWpfDatabase; } }
        private string Path { get { return Database.Path; } }
        private IEnumerable<NacWpfTag> Tags { get { return Database.Tags; } }
        private NacWpfEngine Engine { get { return G.GetEngine(Path) as NacWpfEngine; } }

        //private void newTagButton_Click(object sender, RoutedEventArgs e) {
        //    var obj = Engine.Create(Path);
        //    RTDB?.OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction.Add, new List<NacWpfObject>() { obj });
        //}
        private void newTagButton_Click(object sender, RoutedEventArgs e) {
            var nacObject = NacFactory.Create(Path);
            Engine.Add(nacObject);
        }

        public event SelectionChangedEventHandler TagSelectionChanged;
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            TagSelectionChanged?.Invoke(this, e);
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e) {
            Refresh();
        }

        private void Refresh() {
            Dictionary<string, NacTagValue> values = Engine.GetRTDBValues(Path);
            foreach (NacWpfTag tag in Tags) tag.Tag = values[tag.Path];
        }

        private void browseButton_Click(object sender, RoutedEventArgs e) {
            NacWpfFieldBrowseDialog d = new NacWpfFieldBrowseDialog(Database.Engine.Host);
            d.Owner = Application.Current.MainWindow;
            d.AddTagsEvent += browseDialog_AddTagsEvent;
            d.Show();
        }

        private void browseDialog_AddTagsEvent(object sender, NacWpfFieldBrowseDialog.AddTagsEventArgs e) {
            var newTags =
                    e.Tags.Select(x => new NacTag() {
                        Name = x.Name
                        , Description = x.Description
                        , Address = $"{e.FieldEndpoint}/{x.Name}"
                        , Path = $"{Path}{Guid.NewGuid()}"
                        , Scope = e.Scope
                    })
                    .Cast<NacObject>()
                    .ToList();

            Engine.Add(newTags);
        }

        private void dataGrid_KeyUp(object sender, KeyEventArgs e) {
            if(e.Key == Key.Delete) {
                Engine.Delete(
                    dataGrid.SelectedItems.Cast<NacWpfObject>()
                    .Select(tag => tag.Path)
                    .ToArray()
                );
            }
        }

        private void CopySelectedTags(object sender, RoutedEventArgs e) {
            var selected = dataGrid.SelectedItems;

            NacClipboard<NacTag[]>.Clear();
            NacClipboard<NacTag[]>.Put(selected.OfType<NacWpfTag>().Select(tag => tag.Base).ToArray());
        }

        private void PasteTags(object sender, RoutedEventArgs e) {
            var tags = NacClipboard<NacTag[]>.Get();

            foreach (var tag in tags) {
                tag.Path = NacPath.Parse(Path).Child(Guid.NewGuid().ToString());
                tag.Name = tag.Name + "1";
            }
            Engine.Add(tags);
        }
    }
}
