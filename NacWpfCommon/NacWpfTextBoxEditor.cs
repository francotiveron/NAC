using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace Nac.Wpf.Common {
    public class NacWpfTextBoxEditor : TextBox, ITypeEditor {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem) {
            Background = new SolidColorBrush(Colors.Red);

            //create the binding from the bound property item to the editor
            //var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            //_binding.Source = propertyItem;
            //_binding.ValidatesOnExceptions = true;
            //_binding.ValidatesOnDataErrors = true;
            //_binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            //_binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            //BindingOperations.SetBinding(this, TextProperty, _binding);
            return this;
        }
        protected override void OnGotFocus(RoutedEventArgs e) {
            base.OnGotFocus(e);
            BindingExpression be = BindingOperations.GetBindingExpression(this, TextBox.TextProperty);
        }
        protected override void OnTextChanged(TextChangedEventArgs e) {
            e.Handled = true;
            //base.OnTextChanged(e);
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
        }
        protected override void OnPreviewKeyUp(KeyEventArgs e) {
            base.OnPreviewKeyUp(e);
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e) {
            base.OnPreviewTextInput(e);
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
        }
        protected override void OnTextInput(TextCompositionEventArgs e) {
            base.OnTextInput(e);
        }
    }
}
