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

namespace Nac.Wpf.UI {
    /// <summary>
    /// Interaction logic for TextInputBox.xaml
    /// </summary>
    public partial class TextInputBox : Window {
        private static string _value;

        public static bool Show(string title = "Title", string question = "Question", string value = default(string)) {
            TextInputBox _instance = new TextInputBox(title, question, value);
            _instance.ShowDialog();
            bool ret = _instance.DialogResult.Value;
            _value = _instance.txtAnswer.Text;
            return ret;
        }
        public static string Value { get { return _value; } }

        private TextInputBox(string title = "Title", string question = "Question", string value = default(string)) {
            InitializeComponent();
            Title = title;
            lblQuestion.Content = question;
            txtAnswer.Text = _value = value;
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            txtAnswer.Focus();
            txtAnswer.SelectAll();
        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
