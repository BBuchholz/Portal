using NineWorldsDeep.FragmentCloud;
using System;
using System.Windows.Controls;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for NodeList.xaml
    /// </summary>
    public partial class NodeList : UserControl
    {
        public NodeList()
        {
            InitializeComponent();
        }

        public void Display(TapestryNode nd)
        {
            nodeListView.ItemsSource = nd.Children();
        }

        private void NodeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TapestryNode nd = (TapestryNode)nodeListView.SelectedItem;

            if(nd != null)
            {
                FragmentClickedEventArgs args =
                    new FragmentClickedEventArgs(nd);
                OnFragmentClicked(args);
            }
        }


        protected virtual void OnFragmentClicked(FragmentClickedEventArgs e)
        {
            //EventHandler<FragmentClickedEventArgs> handler = FragmentClicked;
            //if (handler != null)
            //{
            //    handler(this, e);
            //}

            //ide suggested rewrite of the above
            FragmentClicked?.Invoke(this, e);
        }

        public event EventHandler<FragmentClickedEventArgs> FragmentClicked;
    }

}
