using NineWorldsDeep.Archivist;
using NineWorldsDeep.Tapestry.Nodes;
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

        private void ProcessEntryInput()
        {
            string itemValue = txtSourceExcerptInput.Text;

            if (!string.IsNullOrWhiteSpace(itemValue))
            {
                //process entry here

                UI.Display.Message("you entered: " + itemValue);
            }
        }
    }
}
