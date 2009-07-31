using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Common
{
    public static class ControlsManager
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null) return;
            foreach (var item in list) action(item);
        }
        public static void ForEach<T>(this IEnumerable<T> list, Predicate<T> condition, Action<T> action)
        {
            if (list == null) return;
            foreach (var item in list) 
                if (condition(item)) action(item);
        }
        public static IEnumerable<T> FindControls<T>(this Control parent)
            where T : Control
        {
            return parent.FindControls<T>(null);
        }
        public static IEnumerable<T> FindControls<T>(this Control parent, Predicate<T> condition)
            where T : Control
        {
            foreach (T child in parent.Controls)
            {
                if (child != null && (condition == null || condition(child)))
                {
                    yield return child;
                }
                foreach (T deepChild in child.FindControls(condition))
                {
                    yield return deepChild;
                }
            }
        }

    }

    namespace WPF
    {
        using System.Windows.Controls;

        public static class WpfControlsManager
        {
            public static IEnumerable<T> FindControls<T>(this Panel parent)
                where T : Control
            {
                return parent.FindControls<T>(null);
            }

            private static Panel TryGetChildPanel(object control)
            {
                var childPanel = control as Panel;
                if (childPanel != null) return childPanel;     
               
                var childContent = control as ContentControl;
                if (childContent == null) return null;

                return TryGetChildPanel(childContent.Content);                 
            }

            public static IEnumerable<T> FindControls<T>(this Panel parent, Predicate<T> condition)
                where T : Control
            {
                foreach (var child in parent.Children)
                {
                    var child_as_T = child as T;
                    if (child_as_T != null && (condition == null || condition(child_as_T)))
                    {
                        yield return (child as T);
                    }

                    var childPanel = TryGetChildPanel(child);
                    if (childPanel != null)
                    {
                        foreach (T deepChild in childPanel.FindControls(condition))
                        {
                            yield return deepChild;
                        }
                    }
                }
            }
        }
    }
}
