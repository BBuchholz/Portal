using NineWorldsDeep.FragmentCloud;
using NineWorldsDeep.Tapestry.Fragments;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.UI;
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

namespace NineWorldsDeep.Tapestry
{
    /// <summary>
    /// Interaction logic for TapestryNodeSplitViewControl.xaml
    /// </summary>
    public partial class TapestryNodeSplitViewControl : UserControl, TapestryHistoryHandler
    {
        private Stack<TapestrySplitViewLoadCommand> history;

        public void NavigateRoot()
        {
            history = new Stack<TapestrySplitViewLoadCommand>();
            //load to left, as if from right
            PerformLoad(rightNodeView, new TapestryRootNode());
        }

        public TapestryNodeSplitViewControl()
        {
            InitializeComponent();

            leftNodeView.RegisterHistoryHandler(this);
            rightNodeView.RegisterHistoryHandler(this);
            
            NavigateRoot();
        }
        
        public void PerformLoad(TapestryNodeViewControl originator, TapestryNode nd)
        {
            TapestrySplitViewLoadCommand lc = new TapestrySplitViewLoadCommand();

            if (originator == leftNodeView)
            {
                if (nd.LoadLocal)
                {
                    lc.LeftNode = nd;
                    lc.RightNode = rightNodeView.CurrentNode;
                }
                else
                {
                    lc.LeftNode = leftNodeView.CurrentNode;
                    lc.RightNode = nd;
                }
            }
            else
            {
                if (nd.LoadLocal)
                {
                    lc.LeftNode = leftNodeView.CurrentNode;
                    lc.RightNode = nd;
                }
                else
                {
                    lc.LeftNode = nd;
                    lc.RightNode = rightNodeView.CurrentNode;
                }
            }
            
            ResolveLoadCommand(lc);
        }

        private void ResolveLoadCommand(TapestrySplitViewLoadCommand lc)
        {
            history.Push(lc);
                     
            if(true)//leftNodeView.CurrentNode != lc.LeftNode)
            {
                leftNodeView.LoadNode(lc.LeftNode);
            }

            if(true)//rightNodeView.CurrentNode != lc.RightNode)
            {
                rightNodeView.LoadNode(lc.RightNode);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //check for 2 or more actions (1 for current, 1 for previous)
                if(history.Count > 1)
                {
                    //pop current action to clear it from history
                    history.Pop();

                    //resolve previous action from stack
                    ResolveLoadCommand(history.Pop());
                }
            }
        }
    }
}
