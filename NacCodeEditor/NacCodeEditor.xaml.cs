using ICSharpCode.AvalonEdit.Highlighting;
using Nac.Common.Control;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.Generic;
using Nac.Wpf.Common.Control;
using static NacWpfGlobal;
using Nac.Common;

namespace Nac.Code.Editor {
    /// <summary>
    /// Interaction logic for NacCodeEditor.xaml
    /// </summary>
    public partial class NacCodeEditor : Window {
        private NacWpfBlock Block { get; set; }

        public NacCodeEditor(NacWpfBlock wpfBlock = null) {
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(NacCodeEditor).Assembly.GetManifestResourceStream("Nac.Code.Editor.NAC-Mode.xshd")) {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s)) {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("NAC", new string[] { ".nacs" }, customHighlighting);

            InitializeComponent();

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("NAC");
            //textEditor.SyntaxHighlighting = customHighlighting;

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            Block = wpfBlock;
            Text = wpfBlock == null ? @"//Write your code here" : wpfBlock.Code;
        }

        private void foldingUpdateTimer_Tick(object sender, EventArgs e) {
            
        }

        CompletionWindow completionWindow;
        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e) {
            //if (e.Text == " " && (e.Device is KeyboardDevice) && (e.Device as KeyboardDevice).IsKeyDown(Key.LeftCtrl)) {
            //    // Open code completion after the user has pressed dot:
            //    completionWindow = new CompletionWindow(textEditor.TextArea);
            //    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            //    data.Add(new NacCodeEditorCompletionData("Item1"));
            //    data.Add(new NacCodeEditorCompletionData("Item2"));
            //    data.Add(new NacCodeEditorCompletionData("Item3"));
            //    completionWindow.Show();
            //    completionWindow.Closed += delegate {
            //        completionWindow = null;
            //    };
            //}
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e) {
            //if (e.Text.Length > 0 && completionWindow != null) {
            //    if (!char.IsLetterOrDigit(e.Text[0])) {
            //        // Whenever a non-letter is typed while the completion window is open,
            //        // insert the currently selected element.
            //        completionWindow.CompletionList.RequestInsertion(e);
            //    }
            //}

            if (e.Text == " " && (e.Device is KeyboardDevice) && (e.Device as KeyboardDevice).IsKeyDown(Key.LeftCtrl)) {
                // Open code completion after the user has pressed dot:
                if (Block != null) {
                    completionWindow = new CompletionWindow(textEditor.TextArea);
                    completionWindow.Width = 600;
                    IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                    var path = NacPath.Parse(Block.Path);
                    var database = G.GetEngine(path)[path.DatabasePath] as NacWpfDatabase;
                    foreach (var tag in database.Tags) data.Add(new NacCodeEditorCompletionData(tag));
                    //data.Add(new NacCodeEditorCompletionData("Item1"));
                    //data.Add(new NacCodeEditorCompletionData("Item2"));
                    //data.Add(new NacCodeEditorCompletionData("Item3"));
                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    e.Handled = true;
                }
            }
        }

        public string Text { get { return textEditor.Text; } private set { textEditor.Text = value; } }

        private void openFileClick(object sender, RoutedEventArgs e) {

        }

        private void saveFileClick(object sender, RoutedEventArgs e) {

        }

        private void confirmClick(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var sel = ((ComboBoxItem)(sender as ComboBox)?.SelectedItem)?.Content;
            if (sel is string) textEditor.FontSize = double.Parse((string)sel);
        }
    }
}
