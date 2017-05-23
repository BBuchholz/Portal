using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NineWorldsDeep.Core
{
    internal class UiUtils
    {
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

        //public static T GetDataContext<T>(Control control)
        //{
        //    ContentPresenter contentPresenter =
        //        FindAncestor<ContentPresenter>(control);

        //    DataTemplate dataTemplate =
        //        contentPresenter.ContentTemplate;

        //    return (T)control.DataContext;
        //}
    }
}