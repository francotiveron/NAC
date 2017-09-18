using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;
using Nac.Wpf.Common.Control;

namespace Nac.Code.Editor {
    public class NacCodeEditorCompletionData : ICompletionData {

        public NacCodeEditorCompletionData(string text) { Text = text; }
        public NacCodeEditorCompletionData(NacWpfTag tag) { Text = tag.Name; Description = tag.Description;  }

        public string Text { get; private set; }

        //// Use this property if you want to show a fancy UIElement in the list.
        public object Content { get { return Text; } }

        public object Description { get; set; }

        public ImageSource Image { get { return null; } }

        public double Priority { get { return 0; } }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs) {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}