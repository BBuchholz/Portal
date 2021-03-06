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
using NineWorldsDeep.Hierophant;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Core;
using System.IO;
using NineWorldsDeep.Hive;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for HierophantTreeOfLifeDisplay.xaml
    /// </summary>
    public partial class HierophantTreeOfLifeDisplay : UserControl
    {
        #region creation

        public HierophantTreeOfLifeDisplay()
        {
            InitializeComponent();
            hierophantTreeOfLifeInstance.VertexClicked += TreeOfLife_VertexClicked;
            lurianicTreeOfLifeInstance.VertexClicked += TreeOfLife_VertexClicked;
            treeOfLifePreFallInstance.VertexClicked += TreeOfLife_VertexClicked;
            semanticMatrix.SemanticMatrixGroupSelected += SemanticMatrix_GroupSelected;
        }

        #endregion

        #region public interface

        public void Display(HierophantVertexNode vNode)
        {
            string details = "";

            if (vNode.Coupling != null)
            {
                HierophantVertex vertex = vNode.Coupling.Vertex;
                details += vertex.Details();
            }

            tbTestText.Text = details;
        }

        #endregion

        #region private helper methods

        /// <summary>
        /// gets the tree for the currently selected tab item
        /// </summary>
        /// <returns></returns>
        private ISemanticallyAddressable GetActiveTree()
        {
            //TabItem tabItem = tcTrees.SelectedItem as TabItem;

            //if(tabItem == tiHierophantTreeOfLife)
            //{
            //    return hierophantTreeOfLifeInstance;
            //}

            //if(tabItem == tiLurianicTreeOfLife)
            //{
            //    return lurianicTreeOfLifeInstance;
            //}

            //return null;

            return tcTrees.SelectedContent as ISemanticallyAddressable;
        }
        
        #endregion

        #region VertexClicked event

        public event EventHandler<HierophantVertexClickedEventArgs> VertexClicked;
        
        protected virtual void OnVertexClicked(HierophantVertexClickedEventArgs args)
        {
            VertexClicked?.Invoke(this, args);
        }

        #endregion

        #region event handlers

        private void TreeOfLife_VertexClicked(object sender, HierophantVertexClickedEventArgs args)
        {
            args.VertexNode.LoadLocal = chkLoadLocal.IsChecked.Value;
            OnVertexClicked(args);
        }

        private void SemanticMatrix_GroupSelected(object sender, SemanticGrid.SemanticGridGroupSelectedEventArgs e)
        {
            var semMap = e.GroupSemanticMap;
            var renderMap = new SemanticRenderMap();
            renderMap.SelectAllForRender(semMap);
            GetActiveTree().Display(renderMap);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            UtilsUi.ProcessExpanderState((Expander)sender);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            UtilsUi.ProcessExpanderState((Expander)sender);
        }
        
        private void btnExample_Click(object sender, RoutedEventArgs e)
        {
            SemanticRenderMap map = new SemanticRenderMap();


            //map.Select(new SemanticKey("Binah-Chokmah")); //should display on both
            //map.Select(new SemanticKey("Chesed")); //should display on both
            //map.Select(new SemanticKey("Hod-Malkuth")); //should only display on GD tree
            //map.Select(new SemanticKey("Chokmah-Geburah")); //should only display on Lurianic tree
            
            map.Select(new SemanticKey("Binah::Chokmah")); //should display on both
            map.Select(new SemanticKey("Chesed")); //should display on both
            map.Select(new SemanticKey("Hod::Malkuth")); //should only display on GD tree
            map.Select(new SemanticKey("Chokmah::Geburah")); //should only display on Lurianic tree

            hierophantTreeOfLifeInstance.Display(map);
            lurianicTreeOfLifeInstance.Display(map);
        }

        private void btnSendAllKeys_Click(object sender, RoutedEventArgs e)
        {            
            ISemanticallyAddressable tree = GetActiveTree();
            SemanticMap semanticMap = new SemanticMap();

            foreach (SemanticKey semKey in tree.AllSemanticKeys)
            {
                semanticMap.Add(new SemanticDefinition(semKey));
            }

            semanticMatrix.DisplaySemanticMap(semanticMap);
        }

        private void btnSendHighlightedKeyGroup_Click(object sender, RoutedEventArgs e)
        {
            ISemanticallyAddressable tree = GetActiveTree();
            SemanticMap semanticMap = new SemanticMap();

            foreach (SemanticKey semKey in tree.HighlightedSemanticKeys)
            {
                semanticMap.Add(new SemanticDefinition(semKey));
            }

            semanticMatrix.AddAsGroupToSelectedSemanticSet(semanticMap);
        }

        #endregion

    }
}
