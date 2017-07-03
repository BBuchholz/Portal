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
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Core;
using NineWorldsDeep.Tapestry.NodeUI;
using NineWorldsDeep.Hierophant;

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
            clusterNodeDisplay.ClusterDisplayRequested += ClusterDisplay_Requested;
            clusterNodeDisplay.NodeDisplayRequested += NodeDisplay_Requested;
            chordProgressionsNodeDisplay.ChordClicked += ChordDisplay_Requested;
            synergyV5MasterListDisplay.SynergyV5ListClicked += SynergyV5ListDisplay_Requested;
            mediaMasterDisplay.PathSelected += MediaMasterDisplay_PathSelected;
            hierophantTreeOfLifeDisplay.VertexClicked += HierophantTreeOfLifeDisplay_VertexClicked;
            archivistMasterDisplay.SourceSelected += ArchivistMasterDisplay_SourceSelected;
            archivistSourceDisplay.HyperlinkClicked += ArchivistSourceDisplay_HyperlinkClicked;
        }

        private void ArchivistSourceDisplay_HyperlinkClicked(object sender, ArchivistSourceDisplay.HyperlinkClickedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.MediaTagNode);
            }
        }

        private void ArchivistMasterDisplay_SourceSelected(object sender, ArchivistMasterDisplay.SourceSelectedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.SourceNode);
            }
        }

        private void HierophantTreeOfLifeDisplay_VertexClicked(object sender, HierophantVertexClickedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.VertexNode);
            }
        }

        private void MediaMasterDisplay_PathSelected(object sender, MediaMasterDisplay.PathSelectedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.FileSystemNode);
            }
        }

        private void SynergyV5ListDisplay_Requested(object sender, SynergyV5MasterListDisplay.SynergyV5ListClickedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.ListNode);
            }
        }

        private void ChordDisplay_Requested(object sender, ChordProgressionsNodeDisplay.ChordClickedEventArgs e)
        {
            if(historyHandler != null)
            {
                historyHandler.PerformLoad(this, e.ChordNode);
            }
        }

        private void NodeDisplay_Requested(object sender, NodeDisplayRequestedEventArgs e)
        {
            UI.Display.Message("this is where node load will occur, code is written but commented out, need to verify its triggered by the action we want first, hence this msg, in TapestryNodeViewControl");
            ////the projected code
            //if (historyHandler != null)
            //{
            //    historyHandler.PerformLoad(this, e.TapestryNode);
            //}
        }

        private void ClusterDisplay_Requested(object sender, ClusterDisplayRequestedEventArgs e)
        {
            if(historyHandler != null)
            {   
                historyHandler.PerformLoad(this, e.ClusterNode);
            }
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
            contentControls.Add(ccClusterNode);
            contentControls.Add(ccChordProgressionsNode);
            contentControls.Add(ccChordNode);
            contentControls.Add(ccSynergyV5MasterListNode);
            contentControls.Add(ccSynergyV5ListNode);
            contentControls.Add(ccMediaMasterNode);
            contentControls.Add(ccHierophantTreeOfLifeNode);
            contentControls.Add(ccHierophantVertexNode);
            contentControls.Add(ccArchivistMasterNode);
            contentControls.Add(ccArchivistSourceNode);
            contentControls.Add(ccMediaTagNode);
            contentControls.Add(ccHiveMainNode);
            contentControls.Add(ccTaggedMediaNode);
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

                        if(node.Children().Count() < Configuration.NodeCollectionThreshHold())
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

                    case TapestryNodeType.ChordProgressions:

                        SetVisible(ccChordProgressionsNode);
                        break;

                    case TapestryNodeType.SynergyV5MasterList:

                        SetVisible(ccSynergyV5MasterListNode);
                        break;

                    case TapestryNodeType.HierophantTreeOfLife:

                        SetVisible(ccHierophantTreeOfLifeNode);
                        break;

                    case TapestryNodeType.HierophantVertex:

                        SetVisible(ccHierophantVertexNode);
                        HierophantVertexNode vNode = (HierophantVertexNode)node;
                        hierophantVertexDisplay.Display(vNode);
                        break;

                    case TapestryNodeType.NullHierophantVertex:

                        SetVisible(ccHierophantVertexNode);
                        hierophantVertexDisplay.Display((HierophantVertexNode)node);
                        break;

                    case TapestryNodeType.MediaMaster:

                        SetVisible(ccMediaMasterNode);
                        break;

                    case TapestryNodeType.ArchivistMaster:

                        SetVisible(ccArchivistMasterNode);
                        break;

                    case TapestryNodeType.ArchivistSource:

                        SetVisible(ccArchivistSourceNode);
                        ArchivistSourceNode srcNode = (ArchivistSourceNode)node;
                        archivistSourceDisplay.Display(srcNode);
                        break;

                    case TapestryNodeType.MediaTag:
                        SetVisible(ccMediaTagNode);
                        MediaTagNode tagNode = (MediaTagNode)node;
                        mediaTagDisplay.Display(tagNode);
                        break;

                    case TapestryNodeType.HiveMain:
                        SetVisible(ccHiveMainNode);                        
                        break;

                    case TapestryNodeType.TaggedMediaMain:
                        SetVisible(ccTaggedMediaNode);
                        break;

                    case TapestryNodeType.SynergyV5List:
                        
                        SetVisible(ccSynergyV5ListNode);
                        SynergyV5ListNode listNode = (SynergyV5ListNode)node;
                        synergyV5ListDisplay.Display(listNode);
                        break;

                    case TapestryNodeType.NullSynergyV5List:
                        
                        SetVisible(ccSynergyV5ListNode);
                        synergyV5ListDisplay.Display((SynergyV5ListNode)node);
                        break;

                    case TapestryNodeType.Chord:

                        SetVisible(ccChordNode);
                        ChordNode chordNode = (ChordNode)node;
                        chordNodeDisplay.Display(chordNode);
                        break;

                    case TapestryNodeType.Cluster:

                        SetVisible(ccClusterNode);
                        ClusterNode clusterNode = (ClusterNode)node;
                        clusterNodeDisplay.Display(clusterNode);
                        break;

                    case TapestryNodeType.NullCluster:

                        SetVisible(ccClusterNode);
                        clusterNodeDisplay.Display(null);
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
