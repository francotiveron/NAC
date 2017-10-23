using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Linq;
using static NacWpfGlobal;
using static NacWpfUtils;
using static NacUtils;
using Nac.Wpf.Common;
using Nac.Wpf.Common.Control;
using Nac.Common;
using Nac.Wpf.Fuzzy;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nac.Code.Editor;

namespace Nac.Wpf.UI {
    /// <summary>
    /// Interaction logic for Main1.xaml
    /// </summary>
    public partial class Main2 : ContentControl {
        public Main2() {
            InitializeComponent();
        }

        private void ContentControl_Loaded(object sender, RoutedEventArgs e) {
            var context = DataContext as NacWpfContext;
            context.NewEngineMessageEvent += Context_NewEngineMessageEvent;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            NacSecurity.Log("***** New Session Open *****");
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e) {
            NacSecurity.Log("***** Session Closed *****");
        }

        private Task _uiTask;
        private bool _doLog;
        private void Context_NewEngineMessageEvent(string[] messages) {
            if (_doLog)
                Decouple(() => {
                    Dispatcher.Invoke(() => {
                        Statustext.Text += string.Concat(messages);
                    });
                }
                , ref _uiTask
                );
        }

        private void logCheckBox_Click(object sender, RoutedEventArgs e) {
            _doLog = logCheckBox.IsChecked == true;
        }
        private void clearStatusButton_Click(object sender, RoutedEventArgs e) {
            Statustext.Text = null;
        }

        private static TreeViewItem ContainerFromItem(ItemContainerGenerator containerGenerator, object item) {
            TreeViewItem container = (TreeViewItem)containerGenerator.ContainerFromItem(item);
            if (container != null)
                return container;

            foreach (object childItem in containerGenerator.Items) {
                TreeViewItem parent = containerGenerator.ContainerFromItem(childItem) as TreeViewItem;
                if (parent == null)
                    continue;

                container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (container != null)
                    return container;

                container = ContainerFromItem(parent.ItemContainerGenerator, item);
                if (container != null)
                    return container;
            }
            return null;
        }
        private NacWpfEngine CurEngine {
            get {
                FrameworkElement fe = ContainerFromItem(treeView.ItemContainerGenerator, treeView.SelectedItem);
                while (!(fe.DataContext is NacWpfEngine)) {
                    fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
                }

                return fe.DataContext as NacWpfEngine;
            }
        }
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            propertyGrid.SelectedObject = e.NewValue;
        }

        private void CreateChildOf(object sender) {
            var parent = sender.GetType().GetProperty("DataContext").GetValue(sender) as INacWpfChildrenOwner;
            var parentPath = parent.GetType().GetProperty("Path").GetValue(parent) as string;
            var nacObject = NacFactory.Create(parentPath);
            G.GetEngine(nacObject.Path).Add(nacObject);
        }

        private void New_Click(object sender, RoutedEventArgs e) {
            CreateChildOf(sender);
        }
        private void Delete(NacWpfObject wpfObject) {
            List<object> list = new List<object>();
            foreach (var item in tabControl.Items.Cast<NacWpfObject>())
                if (item.IsSuccessorOf(wpfObject)) list.Add(item);

            foreach (var item in list) tabControl.Items.Remove(item);
            (G as NacWpfContext).GetEngine(wpfObject.Path).Delete(wpfObject);
        }
        private void Delete_Click(object sender, RoutedEventArgs e) {
            var wpfObject = sender.GetType().GetProperty("DataContext").GetValue(sender) as NacWpfObject;
            if (wpfObject is NacWpfEngine) (G as NacWpfContext).RemoveEngine(wpfObject as NacWpfEngine);
            else Delete(wpfObject);
            e.Handled = true;
        }

        //Rename
        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount >= 2) {
                TextBlock tb = sender as TextBlock;
                var obj = tb.DataContext as NacWpfNamedObject;
                var eng = CurEngine;
                if (TextInputBox.Show("Change Name", "New Name", obj.Name))
                    CurEngine.Update(obj.Path, "Name", TextInputBox.Value);
                e.Handled = true;
            }
        }

        private bool _updating;
        private void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
            if (G.Refreshing || _updating) return;
            PropertyGrid pg = sender as PropertyGrid;
            var obj = pg.SelectedObject as NacWpfObject;
            string property = (e.OriginalSource as PropertyItem).PropertyName;
            NacWpfEngine engine = G.GetEngine(obj.Path) as NacWpfEngine;
            _updating = true;
            engine.Update(obj.Path, property, e.NewValue);
            _updating = false;
        }
        private void propertyGrid_SelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> e) {

        }
        private void EditDatabase_Click(object sender, RoutedEventArgs e) {
            tabControl.SelectedIndex = tabControl.Items.Add(((sender as MenuItem).DataContext as NacWpfProject).Database);
        }

        private void EditSection_Click(object sender, RoutedEventArgs e) {
            tabControl.SelectedIndex = tabControl.Items.Add((sender as MenuItem).DataContext);
        }

        private void sectionEditor_BlockSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems?.Count > 0 && e.AddedItems[0] is NacWpfBlock)
                propertyGrid.SelectedObject = e.AddedItems?.Count > 0 ? e.AddedItems[0] : null;
        }

        private void databaseEditor_TagSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems?.Count > 0 && e.AddedItems[0] is NacWpfTag)
                propertyGrid.SelectedObject = e.AddedItems[0];
        }

        private void closeButton_Click(object sender, RoutedEventArgs e) {
            var item = ResolveItem(sender);
            var pi = item.GetType().GetProperty("Online");
            pi?.SetValue(item, false);
            tabControl.Items.Remove(item);
        }

        private void editFuzzySetsButton_Click(object sender, RoutedEventArgs e) {
            PropertyGrid pg = NacWpfUtils.FindAncestor<PropertyGrid>(sender as FrameworkElement);
            var wpfTag = pg.SelectedObject as NacWpfTag;
            var fuzzySetEditor = new NacWpfFuzzySetEditor(wpfTag.FuzzySets);
            fuzzySetEditor.Owner = Application.Current.MainWindow;
            if (fuzzySetEditor.ShowDialog() == true) {
                var fuzzySets = fuzzySetEditor.Return;
                if (fuzzySets == null || fuzzySets.Count == 0) wpfTag.FuzzySets = null;
                else wpfTag.FuzzySets = fuzzySets;
            }
        }
        private void editFuzzyRulesButton_Click(object sender, RoutedEventArgs e) {
            PropertyGrid pg = NacWpfUtils.FindAncestor<PropertyGrid>(sender as FrameworkElement);
            var wpfBlock = pg.SelectedObject as NacWpfBlockFuzzy;
            var fuzzyRuleEditor = new NacWpfFuzzyRuleEditor(wpfBlock.FuzzyRules);
            fuzzyRuleEditor.Owner = Application.Current.MainWindow;
            if (fuzzyRuleEditor.ShowDialog() == true) {
                var fuzzyRules = fuzzyRuleEditor.Return;
                if (fuzzyRules == null || fuzzyRules.Count == 0) wpfBlock.FuzzyRules = null;
                else wpfBlock.FuzzyRules = fuzzyRules;
            }
        }

        private void addServerMenuItem_Click(object sender, RoutedEventArgs e) {
            if (!NacSecurity.CanAccessRemoteEngines) return;
            if (TextInputBox.Show("New Engine", "Enter IP", "0.0.0.0")) {
                var context = G as NacWpfContext;
                context.AddEngine(new NacWpfEngine(TextInputBox.Value));
                NacSecurity.Log("Connect to remote engine", TextInputBox.Value);
            }
            e.Handled = true;
        }

        private void valuetextbox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            TextBox t = (TextBox)sender;
            double d;
            e.Handled = !double.TryParse(t.Text + e.Text, out d);
        }

        private void Load_Click(object sender, RoutedEventArgs e) {
            var engine = ResolveItem(sender) as NacWpfEngine;
            engine.LoadProject();
            //treeView.Refresh();
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            NacWpfProject project = ResolveItem(sender) as NacWpfProject;
            project.Save();
        }

        private void Move_Click(object sender, RoutedEventArgs e) {
            var wpfObject = ResolveItem(sender) as NacWpfObject;
            CurEngine.Move(wpfObject, ((string)(sender as MenuItem).Header) == "Move Down");
        }

        private void editCodeButtonButton_Click(object sender, RoutedEventArgs e) {
            PropertyGrid pg = FindAncestor<PropertyGrid>(sender as FrameworkElement);
            var wpfBlock = pg.SelectedObject as NacWpfBlock;
            //var codeEditor = new NacCodeEditorWindow(wpfBlock.FuzzyRules);
            var codeEditor = new NacCodeEditor(wpfBlock);
            codeEditor.Owner = Application.Current.MainWindow;
            if (codeEditor.ShowDialog() == true)
                wpfBlock.Code = codeEditor.Text;
        }



        //private void MoveDown_Click(object sender, RoutedEventArgs e) {
        //    var wpfObject = ResolveItem(sender);
        //    CurEngine.Move(wpfObject, true);
        //}
    }
}
