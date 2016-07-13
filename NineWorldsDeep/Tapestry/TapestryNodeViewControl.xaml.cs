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
using NineWorldsDeep.FragmentCloud;

namespace NineWorldsDeep.Tapestry
{
    /// <summary>
    /// Interaction logic for TapestryNodeViewControl.xaml
    /// </summary>
    public partial class TapestryNodeViewControl : UserControl
    {
        private List<ContentControl> contentControls;
        private TapestryHistoryHandler historyHandler;
        private TapestryNode currentNode;

        public TapestryNodeViewControl()
        {
            InitializeComponent();
            IndexContentControls();
            fragmentCloud.FragmentClicked += Fragment_Clicked;
        }

        public TapestryNode CurrentNode { get { return currentNode; } }

        public void RegisterHistoryHandler(TapestryHistoryHandler tvc)
        {
            this.historyHandler = tvc;
        }

        private void IndexContentControls()
        {
            contentControls = new List<ContentControl>();
            contentControls.Add(ccFragCloud);
            contentControls.Add(ccFragment);
        }

        private void Fragment_Clicked(object sender, FragmentClickedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.Node);
            }
        }

        private void SetVisible(ContentControl cc)
        {
            foreach(ContentControl anotherCc in contentControls)
            {
                if(anotherCc == cc)
                {
                    anotherCc.Visibility = Visibility.Visible;

                }else
                {
                    anotherCc.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void LoadNode(TapestryNode nd)
        {
            ResolveContentControl(nd);
            currentNode = nd;
        }

        private void ResolveContentControl(TapestryNode frg)
        {
            if(frg != null)
            {
                switch (frg.NodeType)
                {
                    case TapestryNodeType.Collection:

                        SetVisible(ccFragCloud);
                        fragmentCloud.Display(frg);
                        break;

                    default:

                        SetVisible(ccFragment);
                        nodeDisplayDefault.Display(frg);
                        break;

                }
            }
            else
            {
                SetVisible(null);
            }


            //if (frg.Children.Count() > 0)
            //{
            //    ccBranch.Visibility = Visibility.Visible;
            //    ccLeaf.Visibility = Visibility.Collapsed;
            //    fragmentCloud.Display(frg);
            //}
            //else
            //{
            //    ccBranch.Visibility = Visibility.Collapsed;
            //    ccLeaf.Visibility = Visibility.Visible;
            //    nodeDisplay.Display(frg);
            //}
        }
    }
}
