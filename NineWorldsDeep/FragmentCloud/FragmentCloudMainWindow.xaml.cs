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
using System.Windows.Shapes;

namespace NineWorldsDeep.FragmentCloud
{
    /// <summary>
    /// Interaction logic for FragmentCloudMainWindow.xaml
    /// </summary>
    public partial class FragmentCloudMainWindow : Window
    {
        NwdUriResolver ur = NwdUriResolver.GetInstance();

        public FragmentCloudMainWindow()
        {
            InitializeComponent();
            fragmentCloud.FragmentClicked += Fragment_Clicked;
        }

        private void Fragment_Clicked(object sender,
            FragmentClickedEventArgs e)
        {
            txtUri.Text = e.Node.URI;
            ResolveContentControlForUri(e.Node);
        }

        private void txtUri_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ResolveCurrentUri();
            }
        }

        private void ResolveCurrentUri()
        {
            ResolveContentControlForUri(ur.Resolve(txtUri.Text));
        }

        private void ResolveContentControlForUri(Tapestry.TapestryNode frg)
        {
            if (frg.Children.Count() > 0)
            {
                ccBranch.Visibility = Visibility.Visible;
                ccLeaf.Visibility = Visibility.Collapsed;
                fragmentCloud.Display(frg);
            }
            else
            {
                ccBranch.Visibility = Visibility.Collapsed;
                ccLeaf.Visibility = Visibility.Visible;
                nodeDisplay.Display(frg);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                txtUri.Text = Converter.TrimNwdUriNodeName(txtUri.Text);
                ResolveCurrentUri();
            }
        }
    }
}
