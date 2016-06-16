using NineWorldsDeep.FragmentCloud;
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

namespace NineWorldsDeep.Tapestry
{
    /// <summary>
    /// Interaction logic for TapestryNodeBrowser.xaml
    /// </summary>
    public partial class TapestryNodeBrowserWindow : Window
    {
        NwdUriResolver ur = NwdUriResolver.GetInstance();

        public TapestryNodeBrowserWindow()
        {
            InitializeComponent();
            fragmentCloud.FragmentClicked += Fragment_Clicked;
            ResolveContentControlForUri(new RootFragment());
        }

        private void Fragment_Clicked(object sender,
            FragmentClickedEventArgs e)
        {
            txtUri.Text = e.Fragment.URI;
            txtUri.Focus();
            ResolveContentControlForUri(e.Fragment);
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

        private void ResolveContentControlForUri(TapestryNode frg)
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
