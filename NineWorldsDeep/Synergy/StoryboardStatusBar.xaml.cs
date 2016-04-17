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

namespace NineWorldsDeep.Synergy
{
    /// <summary>
    /// Interaction logic for StoryboardStatusBar.xaml
    /// </summary>
    public partial class StoryboardStatusBar : UserControl
    {
        //ADAPTED FROM: 
        //http://stackoverflow.com/questions/3970986/wpf-fade-out-status-bar-text-after-x-seconds

        //MODIFICATIONS MADE FROM SUGGESTION HERE: 
        //http://stackoverflow.com/questions/17955833/binding-usercontrol-to-its-own-dependencyproperty-doesnt-work

        public StoryboardStatusBar()
        {
            InitializeComponent();
        }

        public string StatusBarText
        {
            get { return (string)GetValue(StatusBarTextProperty); }
            set { SetValue(StatusBarTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusBarText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusBarTextProperty =
            DependencyProperty.Register("StatusBarText", typeof(string), typeof(StoryboardStatusBar), new UIPropertyMetadata(""));

    }
}
