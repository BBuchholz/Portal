﻿using System;
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

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// Interaction logic for SemanticGrid.xaml
    /// </summary>
    public partial class SemanticGrid : UserControl
    {
        #region fields
             
        private Dictionary<string, DataGrid> semanticGroupNamesToDataGrids =
            new Dictionary<string, DataGrid>();

        private Dictionary<string, SemanticGridPane> semanticGroupsToPanes =
            new Dictionary<string, SemanticGridPane>();

        #endregion

        #region properties

        public SemanticMap CurrentSemanticMap { get; private set; }

        #endregion
        
        #region creation

        public SemanticGrid()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        #endregion

        #region public interface

        /// <summary>
        /// creates an empty map with the specified name
        /// </summary>
        /// <param name="semanticMapName"></param>
        public void AssignNewMap(string semanticMapName)
        {
            CurrentSemanticMap = new SemanticMap()
            {
                Name = semanticMapName
            };
        }

        public void DisplaySemanticMap(SemanticMap semanticMap)
        {
            CurrentSemanticMap = semanticMap;

            DisplaySemanticMap(
                ConfigHierophant.ALL_KEYS_GROUP_NAME,
                CurrentSemanticMap);

            foreach (string semanticGroupName in CurrentSemanticMap.SemanticGroupNames)
            {
                DisplaySemanticMap(semanticGroupName, CurrentSemanticMap.SemanticGroup(semanticGroupName));
            }
        }

        public void AddAsGroupToCurrentSemanticMap(SemanticMap semanticMap)
        {
            if(CurrentSemanticMap != null && semanticMap != null)
            {
                if (string.IsNullOrWhiteSpace(semanticMap.Name))
                {
                    semanticMap.Name = AutoGenerateGroupName();
                }

                CurrentSemanticMap.AddAsGroup(semanticMap);
                DisplaySemanticMap(CurrentSemanticMap); //refresh
            }
        }

        //public void DisplaySemanticMapToDataGrid(SemanticMap semanticMap)
        //{
        //    CurrentSemanticMap = semanticMap;

        //    DisplaySemanticMap(
        //        ConfigHierophant.ALL_KEYS_GROUP_NAME,
        //        CurrentSemanticMap);

        //    foreach (string semanticGroupName in CurrentSemanticMap.SemanticGroupNames)
        //    {
        //        DisplaySemanticMap(
        //            semanticGroupName,
        //            CurrentSemanticMap.SemanticGroup(semanticGroupName));
        //    }
        //}

        #endregion

        #region private helper methods

        private void DisplaySemanticMap(string semanticGroupName, SemanticMap semanticMap)
        {
            EnsureSemanticGroupGridPane(semanticGroupName);
            var gridPane = semanticGroupsToPanes[semanticGroupName];

            gridPane.DisplaySemanticMap(semanticMap);            
        }
        
        private string AutoGenerateGroupName()
        {
            int i = 0;
            string autoGenName;

            do
            {
                i++;
                autoGenName = "Semantic Group " + i;
            }
            while (CurrentSemanticMap.HasGroup(autoGenName));
            return autoGenName;
        }
        
        private void EnsureSemanticGroupGrid(string semanticGroupName)
        {
            //prevent overwrite of existing groups
            if (!semanticGroupNamesToDataGrids.ContainsKey(semanticGroupName))
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = semanticGroupName;
                DataGrid dataGrid = new DataGrid();
                dataGrid.AutoGenerateColumns = false;

                semanticGroupNamesToDataGrids[semanticGroupName] = dataGrid;

                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Binding = new Binding("Key");
                dataGrid.Columns.Add(textColumn);

                tabItem.Content = dataGrid;
                tcSemanticGroups.Items.Add(tabItem);
            }
        }
        
        private void EnsureSemanticGroupGridPane(string semanticGroupName)
        {
            //prevent overwrite of existing groups
            if (!semanticGroupsToPanes.ContainsKey(semanticGroupName))
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = semanticGroupName;

                //DataGrid dataGrid = new DataGrid();
                //dataGrid.AutoGenerateColumns = false;

                //DataGridTextColumn textColumn = new DataGridTextColumn();
                //textColumn.Binding = new Binding("Key");
                //dataGrid.Columns.Add(textColumn);

                var gridPane = new SemanticGridPane();

                semanticGroupsToPanes[semanticGroupName] = gridPane;
                gridPane.GroupName = semanticGroupName;

                tabItem.Content = gridPane;
                tcSemanticGroups.Items.Add(tabItem);

                tcSemanticGroups.SelectedItem = tabItem;
            }
        }

        private void RemoveSemanticGroupGridPane(string semanticGroupName)
        {

        }

        private void Refresh()
        {
            DisplaySemanticMap(CurrentSemanticMap);
        }

        #endregion

        #region event handlers

        private void btnAddSemanticGroup_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSemanticMap == null)
            {
                CurrentSemanticMap = new SemanticMap();
            }

            string autoGenName = AutoGenerateGroupName();

            var name = UI.Prompt.Input("enter a group name: ", autoGenName);

            if (!string.IsNullOrWhiteSpace(name))
            {
                //creates it if it doesn't exist
                CurrentSemanticMap.SemanticGroup(name);
                Refresh();
            }
        }

        private void chkHighlightActiveGroup_checkToggled(object sender, RoutedEventArgs e)
        {
            bool? isChecked = chkHighlightActiveGroup.IsChecked;

            if (isChecked.HasValue &&
                isChecked.Value == true) //intentionally redundant for clarity
            {
                UI.Display.Message("do stuff here");
            }
        }

        private void tcSemanticGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            TabItem selectedTab = null;

            if (e.AddedItems.Count > 0)
            {
                selectedTab = e.AddedItems[0] as TabItem;  // Gets selected tab
            }

            if (selectedTab != null)
            {
                SemanticGridPane pane =
                    Core.UtilsUi.FindChildren<SemanticGridPane>(selectedTab).First();

                if (pane != null)
                {                    
                    pane.RefreshFromMap();
                    OnGroupSelected(
                        new SemanticGridGroupSelectedEventArgs(
                            pane.CurrentSemanticMap));
                }
            }
        }

        private void MenuItemChangeName_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                var tabItem =
                    ((ContextMenu)menuItem.Parent).PlacementTarget as TabItem;

                if (tabItem != null)
                {
                    string currentGroupName = tabItem.Header.ToString();
                    string newName = UI.Prompt.Input("Enter new name", currentGroupName);

                    if (!string.IsNullOrWhiteSpace(newName) && !newName.Equals(currentGroupName))
                    {
                        CurrentSemanticMap.RenameGroup(currentGroupName, newName);

                        if (semanticGroupNamesToDataGrids.ContainsKey(currentGroupName))
                        {
                            semanticGroupNamesToDataGrids.Remove(currentGroupName);
                        }

                        tcSemanticGroups.Items.Remove(tabItem);
                        Refresh();
                    }
                }
            }
        }



        #endregion

        #region group selected event

        public event EventHandler<SemanticGridGroupSelectedEventArgs> SemanticGridGroupSelected;

        protected virtual void OnGroupSelected(SemanticGridGroupSelectedEventArgs args)
        {
            SemanticGridGroupSelected?.Invoke(this, args);
        }

        public class SemanticGridGroupSelectedEventArgs
        {
            public SemanticGridGroupSelectedEventArgs(SemanticMap semanticMap)
            {
                GroupSemanticMap = semanticMap;
            }

            public SemanticMap GroupSemanticMap { get; private set; }

        }

        #endregion
    }
}
