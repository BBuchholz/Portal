using NineWorldsDeep.Hive;
using NineWorldsDeep.Mnemosyne.V5;
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

namespace NineWorldsDeep.Tapestry.NodeUI
{
    /// <summary>
    /// Interaction logic for HiveMain.xaml
    /// </summary>
    public partial class HiveMain : UserControl
    {
        public HiveMain()
        {
            InitializeComponent();
            PopulateRoots(GetAllRoots());
        }

        //TODO: modify this to support lazy-loading
        //http://www.wpf-tutorial.com/treeview-control/lazy-loading-treeview-items/
        private List<HiveRoot> GetAllRoots()
        {
            List<HiveRoot> lst = new List<HiveRoot>();

            //mockup
            for(int i = 0; i < 4; i++)
            {
                HiveRoot hr = new HiveRoot() { Name = "device" + i };

                //HiveFileGrouping mocks itself currently

                //foreach(HiveFileGrouping hfg in hr.Files.AllValues())
                //{
                //    for(int j = 0; j < 5; j++)
                //    {
                //        hfg.Add(new DevicePath() {
                //            DeviceName = "laptop",
                //            DevicePathValue = @"C:\NWD-SNDBX\dummyFile" + j + ".mock"
                //        });
                //    }
                //}

                lst.Add(hr);
            }

            return lst;
        }



        //TODO: modify this to support lazy-loading of device paths
        //http://www.wpf-tutorial.com/treeview-control/lazy-loading-treeview-items/
        private void PopulateRoots(List<HiveRoot> allRoots)
        {
            foreach(HiveRoot hr in allRoots)
            {
                TreeViewItem hrItem = new TreeViewItem() { Header = hr.Name };
                tvHive.Items.Add(hrItem);

                TreeViewItem filesItem = 
                    new TreeViewItem() { Header = "files" };
                hrItem.Items.Add(filesItem);

                foreach(HiveFileGrouping hfg in hr.Files.AllValues())
                {
                    TreeViewItem fileGroupingItem =
                        new TreeViewItem() { Header = hfg.Name };                    
                    filesItem.Items.Add(fileGroupingItem);

                    foreach(DevicePath dp in hfg.DevicePaths)
                    {
                        TreeViewItem fileItem =
                            new TreeViewItem() { Header = dp.DevicePathValue };
                        fileGroupingItem.Items.Add(fileItem);
                    }
                }

            }
        }

    }
}
