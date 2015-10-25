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

namespace NineWorldsDeep
{
    /// <summary>
    /// Interaction logic for Workbench.xaml
    /// </summary>
    public partial class Workbench : Window
    {
        private List<ListView> listViews =
            new List<ListView>();

        private MenuController _menu;

        public IEnumerable<Fragment> GetFragments(int index)
        {
            if(index < listViews.Count)
            {
                return (IEnumerable<Fragment>)listViews[index].ItemsSource;
            }

            return null;
        }

        //singleton implementation
        private static Workbench instance;

        //singleton implementation
        public static Workbench Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Workbench();
                }
                
                return instance;

            }
        }

        //singleton implementation
        private Workbench()
        {
            InitializeComponent();
            _menu = new MenuController();
            _menu.Configure(mainMenu);
            this.Closing += Workbench_Closing;
            new WorkbenchWindowController().ConfigureClosingEvent(this);
            new WorkbenchController().Configure(this);
        }
        
        //hides window instead of closing it for singleton implementation    
        private void Workbench_Closing(object sender, 
                                       System.ComponentModel.CancelEventArgs e)
        {        
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;            
        }

        public MenuController Menu {  get { return _menu; } }

        public void Receive(IEnumerable<Fragment> ie)
        {
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            int colsCount = mainGrid.ColumnDefinitions.Count();
            
            ListView lv = new ListView();
            lv.SetValue(Grid.ColumnProperty, colsCount - 1);
            mainGrid.Children.Add(lv);
            lv.ItemsSource = ie;
            listViews.Insert(colsCount - 1, lv);
        }
        
        public void RemoveLast()
        {
            if(listViews.Count > 0)
            {
                ListView last = listViews.Last();
                mainGrid.Children.Remove(last);
                ColumnDefinition lastCol = mainGrid.ColumnDefinitions.Last();
                mainGrid.ColumnDefinitions.Remove(lastCol);
                listViews.Remove(last);
            }
        }
        
        public void RemoveFirst()
        {
            if(listViews.Count > 0)
            {
                ListView first = listViews.First();
                mainGrid.Children.Remove(first);
                ColumnDefinition firstCol = mainGrid.ColumnDefinitions.First();
                mainGrid.ColumnDefinitions.Remove(firstCol);
                listViews.Remove(first);
                foreach(ListView lv in listViews)
                {
                    //shift all cols over one
                    int colProp = (int)lv.GetValue(Grid.ColumnProperty);
                    lv.SetValue(Grid.ColumnProperty, colProp - 1);
                }
            }
        }
    }
}
