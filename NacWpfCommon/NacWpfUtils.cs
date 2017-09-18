using Nac.Common;
using Nac.Wpf.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

public static class NacWpfUtils  {
    public static T FindAncestor<T>(DependencyObject dependencyObject)
    where T : class {
        DependencyObject target = dependencyObject;
        do {
            target = VisualTreeHelper.GetParent(target);
        }
        while (target != null && !(target is T));
        return target as T;
    }

    public static T TryFindParent<T>(DependencyObject child) where T : DependencyObject {
        DependencyObject parentObject = LogicalTreeHelper.GetParent(child);
        if (parentObject == null) return null;

        T parent = parentObject as T;
        if (parent != null)
            return parent;
        else
            return TryFindParent<T>(parentObject);
    }

    public static object ResolveItem(object obj) {
        FrameworkElement fe = obj as FrameworkElement;
        return fe?.DataContext;
    }

    public static void Refresh(this FrameworkElement fe) {
        //FrameworkElement fe = obj as FrameworkElement;
        var dataContext = fe.DataContext;
        if (dataContext != null) {
            fe.DataContext = null;
            fe.DataContext = dataContext;
        }
    }

    public static IEnumerable Controls(Visual control, Predicate<object> pred) {
        int ChildNumber = VisualTreeHelper.GetChildrenCount(control);

        for (int i = 0; i <= ChildNumber - 1; i++) {
            Visual v = (Visual)VisualTreeHelper.GetChild(control, i);
            if (pred(v)) yield return v;
            if (VisualTreeHelper.GetChildrenCount(v) > 0) {
                foreach (var child in Controls(v, pred))
                    yield return child;
            }
        }
    }

    public class NacWpfWaitCursor : IDisposable {
        private Cursor _previousCursor;

        public NacWpfWaitCursor() {
            _previousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose() {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
