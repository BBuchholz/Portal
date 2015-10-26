using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NineWorldsDeep
{
    public class WorkbenchController
    {
        private Grid mainGrid;

        private List<ListView> listViews =
            new List<ListView>();

        private Dictionary<ListView, ComboBox> mapListViewComboBox =
            new Dictionary<ListView, ComboBox>();
        private Dictionary<ComboBox, ListView> mapComboBoxListView =
            new Dictionary<ComboBox, ListView>();

        public void Configure(Grid g)
        {
            this.mainGrid = g;
        }

        public IEnumerable<Fragment> GetFragments(int index)
        {
            if (index < listViews.Count)
            {
                return GetFragments(listViews[index]);
            }

            return null;
        }

        private IEnumerable<Fragment> GetFragments(ListView lv)
        {
            return (IEnumerable<Fragment>)lv.ItemsSource;
        }

        private void Associate(ListView lv, ComboBox cmb)
        {
            mapListViewComboBox[lv] = cmb;
            mapComboBoxListView[cmb] = lv;
        }

        public void RefreshMetaKeys(ComboBox cmb, ListView lv)
        {
            cmb.ItemsSource = GetFragments(lv).GetMetaKeys();
        }

        public void AddListView(IEnumerable<Fragment> ie)
        {
            if(mainGrid != null) { 

                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                int colsCount = mainGrid.ColumnDefinitions.Count();

                ListView lv = new ListView();
                lv.SetValue(Grid.ColumnProperty, colsCount - 1);
                lv.SetValue(Grid.RowProperty, 1);

                ComboBox cmb = new ComboBox();
                cmb.SetValue(Grid.ColumnProperty, colsCount - 1);
                cmb.SetValue(Grid.RowProperty, 0);
                cmb.SelectionChanged += Cmb_SelectionChanged;
                Associate(lv, cmb);

                mainGrid.Children.Add(cmb);
                mainGrid.Children.Add(lv);

                lv.ItemsSource = ie;
                listViews.Insert(colsCount - 1, lv);
                RefreshMetaKeys(cmb, lv);
            }
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            string selected = (string)cmb.SelectedItem;

            if(selected != null)
            {
                ListView lv = mapComboBoxListView[cmb];
                foreach (Fragment f in lv.Items)
                {
                    f.DisplayKey = selected;
                }
                RefreshFragmentList(lv);
            }
        }

        public void RefreshFragmentList(ListView lv)
        {
            IEnumerable<Fragment> frgs = GetFragments(lv);
            lv.ItemsSource = null;
            lv.ItemsSource = frgs.OrderBy(s => s);
        }

        public void RemoveLast()
        {
            if(mainGrid != null)
            {
                if (listViews.Count > 0)
                {
                    ListView last = listViews.Last();
                    mainGrid.Children.Remove(last);
                    mainGrid.Children.Remove(mapListViewComboBox[last]);
                    ColumnDefinition lastCol = mainGrid.ColumnDefinitions.Last();
                    mainGrid.ColumnDefinitions.Remove(lastCol);
                    listViews.Remove(last);
                }
            }            
        }

        public void RemoveFirst()
        {
            if (mainGrid != null)
            {
                if (listViews.Count > 0)
                {
                    ListView first = listViews.First();
                    mainGrid.Children.Remove(first);
                    mainGrid.Children.Remove(mapListViewComboBox[first]);
                    ColumnDefinition firstCol = mainGrid.ColumnDefinitions.First();
                    mainGrid.ColumnDefinitions.Remove(firstCol);
                    listViews.Remove(first);
                    foreach (ListView lv in listViews)
                    {
                        //shift all cols over one
                        int newColProp = 
                            (int)lv.GetValue(Grid.ColumnProperty) - 1;
                        lv.SetValue(Grid.ColumnProperty, newColProp);
                        mapListViewComboBox[lv].SetValue(Grid.ColumnProperty,
                                                         newColProp);
                    }
                }
            }            
        }
    }
}
