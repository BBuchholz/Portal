using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NineWorldsDeep
{
    public class WorkbenchListViewsController
    {
        private Grid mainGrid;

        private List<ListView> listViews =
            new List<ListView>();

        private Dictionary<ListView, ComboBox> mapListViewComboBox =
            new Dictionary<ListView, ComboBox>();
        private Dictionary<ComboBox, ListView> mapComboBoxListView =
            new Dictionary<ComboBox, ListView>();

        public int ColumnCount
        {
            get
            {
                return listViews.Count;
            }
        }

        public void Configure(Grid g)
        {
            this.mainGrid = g;
        }

        public IEnumerable<Core.Fragment> GetFragments(int index)
        {
            if (index < listViews.Count)
            {
                return GetFragments(listViews[index]);
            }

            return null;
        }

        private IEnumerable<Core.Fragment> GetFragments(ListView lv)
        {
            return (IEnumerable<Core.Fragment>)lv.ItemsSource;
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

        public void AddListView(IEnumerable<Core.Fragment> ie)
        {
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            AddListView(ie, mainGrid.ColumnDefinitions.Count() - 1);
        }

        public void AddListView(IEnumerable<Core.Fragment> ie, int colIndex)
        {
            if (mainGrid != null)
            {                
                ListView lv = new ListView();
                lv.SetValue(Grid.ColumnProperty, colIndex);
                lv.SetValue(Grid.RowProperty, 1);

                ComboBox cmb = new ComboBox();
                cmb.SetValue(Grid.ColumnProperty, colIndex);
                cmb.SetValue(Grid.RowProperty, 0);
                cmb.SelectionChanged += Cmb_SelectionChanged;
                Associate(lv, cmb);

                mainGrid.Children.Add(cmb);
                mainGrid.Children.Add(lv);

                lv.ItemsSource = ie;
                listViews.Insert(colIndex, lv);
                RefreshMetaKeys(cmb, lv);
                lv.UpdateLayout();
            }
        }

        public void AddListViewFirst(IEnumerable<Fragment> ie)
        {
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ShiftListViews(1);
            AddListView(ie, 0);            
        }

        public Fragment GetSelectedFragment(int index)
        {
            if (index < listViews.Count)
            {
                return (Fragment)listViews[index].SelectedItem;
            }

            return null;
        }

        public string GetDisplayValue(int index)
        {
            Fragment f = GetSelectedFragment(index);
            if(f != null)
            {
                return f.DisplayValue;
            }
            return null;
        }

        public string GetDisplayKey(int index)
        {
            Fragment f = GetSelectedFragment(index);
            if (f != null)
            {
                return f.DisplayKey;
            }
            return null;
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
            IEnumerable<Core.Fragment> frgs = GetFragments(lv);
            lv.ItemsSource = null;
            lv.ItemsSource = frgs.OrderBy(s => s);
        }

        public void SendFirst()
        {
            Send(listViews.First());
        }

        private void Send(ListView lv)
        {
            if (mainGrid != null)
            {
                if (listViews.Count > 0)
                {
                    FragmentMetaWindow fmw = new FragmentMetaWindow();
                    fmw.Receive(GetFragments(lv).DeepCopy());
                    fmw.Show();
                }
            }
        }

        public void SendLast()
        {
            Send(listViews.Last());
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
                    ShiftListViews(-1);       
                }
            }            
        }

        public void ShiftListViews(int indexShift)
        {
            foreach (ListView lv in listViews)
            {
                //shift all cols over one
                int newColProp =
                    (int)lv.GetValue(Grid.ColumnProperty) + indexShift;
                lv.SetValue(Grid.ColumnProperty, newColProp);
                mapListViewComboBox[lv].SetValue(Grid.ColumnProperty,
                                                 newColProp);
            }
        }

        public void RefreshMetaKeys(int index)
        {
            if(index < listViews.Count)
            {
                ListView lv = listViews[index];
                RefreshMetaKeys(mapListViewComboBox[lv], lv);
            }
        }
    }
}
