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
            nodeList.FragmentClicked += Fragment_Clicked;
        }

        public TapestryNode CurrentNode { get { return currentNode; } }

        public void RegisterHistoryHandler(TapestryHistoryHandler tvc)
        {
            this.historyHandler = tvc;
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

        private void IndexContentControls()
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // IF YOU ADD A CASE HERE, 
            // ADD IT TO ResolveContentControl() ALSO!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            contentControls = new List<ContentControl>();
            contentControls.Add(ccFragCloud);
            contentControls.Add(ccFragment);
            contentControls.Add(ccAudioNode);
            contentControls.Add(ccNodeList);
            contentControls.Add(ccImageNode);
        }

        private void ResolveContentControl(TapestryNode node)
        {
            if(node != null)
            {
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // IF YOU ADD A CASE HERE, 
                // ADD IT TO IndexContentControls() ALSO!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                switch (node.NodeType)
                {
                    case TapestryNodeType.Collection:

                        if(node.Children.Count() < 10)
                        {
                            SetVisible(ccFragCloud);
                            fragmentCloud.Display(node);
                        }
                        else
                        {
                            SetVisible(ccNodeList);
                            nodeList.Display(node);
                        }

                        break;

                    case TapestryNodeType.Audio:

                        SetVisible(ccAudioNode);
                        FileSystemNode nd = (FileSystemNode)node;
                        audioNodeDisplay.Display(nd);
                        break;

                    case TapestryNodeType.Image:

                        SetVisible(ccImageNode);
                        FileSystemNode ind = (FileSystemNode)node;
                        imageNodeDisplay.Display(ind);
                        break;

                    default:

                        SetVisible(ccFragment);
                        nodeDisplayDefault.Display(node);
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
