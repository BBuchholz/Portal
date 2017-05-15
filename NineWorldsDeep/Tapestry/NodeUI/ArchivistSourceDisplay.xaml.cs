using NineWorldsDeep.Archivist;
using NineWorldsDeep.Tapestry.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for ArchivistSourceDisplay.xaml
    /// </summary>
    public partial class ArchivistSourceDisplay : UserControl
    {
        private Db.Sqlite.ArchivistSubsetDb db;
        private ArchivistSourceNode sourceNode;

        public ArchivistSourceDisplay()
        {
            InitializeComponent();
        }

        public void Display(ArchivistSourceNode src)
        {
            this.sourceNode = src;
            Refresh();            
        }

        public void Refresh()
        {
            ccSourceDetails.Content = sourceNode.Source;

            //mimic SynergyV5ListDisplay user control for async load of listview

            //testing
            List<ArchivistSourceExcerpt> testExcerpts =
                new List<ArchivistSourceExcerpt>();

            Random r = new Random();
            int rndInt = r.Next(3, 15);
            int rndTags = r.Next(1, 8);
            int rndParas = r.Next(1, 4);

            for (int i = 1; i < rndInt; i++)
            {
                testExcerpts.Add(new ArchivistSourceExcerpt()
                {
                    ExcerptValue = string.Join(System.Environment.NewLine +
                                               System.Environment.NewLine, 
                                               Faker.Lorem.Paragraphs(rndParas)),
                    TagString = string.Join(", ",Faker.Lorem.Words(rndTags))
                });  
            }

            lvSourceExcerpts.ItemsSource = testExcerpts;
            
        }

        private void Hyperlink_OnClick(object sender, EventArgs e)
        {
            var link = (Hyperlink)sender;

            if (link != null)
            {
                var run = link.Inlines.FirstOrDefault() as Run;
                string text = run == null ? string.Empty : run.Text;

                UI.Display.Message("clicked " + text);
            }
        }

        //private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        //{
        //    if (lvSourceExcerpts.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        //    {
        //        foreach (var item in lvSourceExcerpts.Items)
        //        {
        //            var container = lvSourceExcerpts.ItemContainerGenerator.ContainerFromItem(item);
        //            ListViewItem i = (ListViewItem)container;

        //            if (i != null)
        //            {
        //                //Seek out the ContentPresenter that actually presents our DataTemplate
        //                ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(i);

        //                //int count = VisualTreeHelper.GetChildrenCount(contentPresenter);

        //                //for (int j = 0; j < count; j++)
        //                //{
        //                //    var innerChild = VisualTreeHelper.GetChild(contentPresenter, j);

        //                //    if (innerChild is Hyperlink)
        //                //    {
        //                //        UI.Display.Message("found hyperlink");
        //                //    }
        //                //}

        //                foreach(var x in FindVisualChildren<Hyperlink>(contentPresenter))
        //                {
        //                    if(x is Hyperlink)
        //                    {
        //                        UI.Display.Message("found link");
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        //{
        //    if (depObj != null)
        //    {
        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //        {
        //            DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
        //            if (child != null && child is T)
        //            {
        //                yield return (T)child;
        //            }

        //            foreach (T childOfChild in FindVisualChildren<T>(child))
        //            {
        //                yield return childOfChild;
        //            }
        //        }
        //    }
        //}

        //private T FindVisualChild<T>(DependencyObject obj)
        //where T : DependencyObject
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is T)
        //            return (T)child;
        //    }

        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        T childOfChild = FindVisualChild<T>(child);
        //        if (childOfChild != null)
        //            return childOfChild;
        //    }

        //    return null;
        //}

        private void lvSourceExcerpts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void txtSourceExcerptInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Enter))
            {
                ProcessEntryInput();
                e.Handled = true;
            }
        }

        private void btnAddSourceExcerpt_Click(object sender, RoutedEventArgs e)
        {
            ProcessEntryInput();
        }

        public void Hyperlink_TagClicked(object sender, RoutedEventArgs e)
        {
            UI.Display.Message("clicked");
        }

        private void ProcessEntryInput()
        {
            string itemValue = txtSourceExcerptInput.Text;

            if (!string.IsNullOrWhiteSpace(itemValue))
            {
                //process entry here

                UI.Display.Message("you entered: " + itemValue);
            }
        }

        //private class SourceExcerptUiWrapper
        //{
        //    public ArchivistSourceExcerpt Excerpt { get; set; }

        //    public string ExcerptValue
        //    {
        //        get
        //        {
        //            return Excerpt.ExcerptValue;
        //        }
        //    }
            
        //}
    }
}
