using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NineWorldsDeep.Core
{
    public class UtilsUi
    {
        public static List<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            List<T> children = new List<T>();

            var chidrenFromTreeHelper = LogicalTreeHelper.GetChildren(parent);

            foreach(var child in chidrenFromTreeHelper)
            {
                T childType = child as T;
                if (childType != null)
                {
                    children.Add((T)child);
                }
            }

            return children;
        }


        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            // Need this call to avoid returning current object if it is the 
            // same type as parent we are looking for
            current = VisualTreeHelper.GetParent(current);

            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            };
            return null;
        }

        
        /// <summary>
        /// manages grid rows to share space between multiple expanded expanders
        /// </summary>
        /// <param name="expander"></param>
        public static void ProcessExpanderState(Expander expander)
        {
            Grid parent = FindAncestor<Grid>(expander);
            int rowIndex = Grid.GetRow(expander);

            if (parent.RowDefinitions.Count > rowIndex && rowIndex >= 0)
                parent.RowDefinitions[rowIndex].Height =
                    (expander.IsExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto);
        }

        public static T GetTemplateSibling<T,K>(K uiSibling, string siblingName)
            where T : DependencyObject
            where K : DependencyObject
        {
            ContentPresenter contentPresenter =
                FindAncestor<ContentPresenter>(uiSibling);

            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            
            T sibling = 
                dataTemplate.FindName(siblingName, contentPresenter) as T;

            return sibling;
        }

        public static T ParentOfType<T>(DependencyObject element) where T : DependencyObject
        {
            if (element == null)
                return default(T);
            else
                return Enumerable.FirstOrDefault<T>(Enumerable.OfType<T>((IEnumerable)GetParents(element)));
        }

        public static IEnumerable<DependencyObject> GetParents(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            while ((element = GetParent(element)) != null)
                yield return element;
        }
        
        private static DependencyObject GetParent(DependencyObject element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null)
            {
                FrameworkElement frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                    parent = frameworkElement.Parent;
            }
            return parent;
        }
    }
}