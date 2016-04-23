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
using System.Windows.Shapes;

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// Interaction logic for VisualKabbalisticTreeTestHarness.xaml
    /// </summary>
    public partial class VisualKabbalisticTreeTestHarness : Window
    {
        public VisualKabbalisticTreeTestHarness()
        {
            InitializeComponent();
            kabbalisticTree.SephirahClicked += 
                new EventHandler(EventHandler_SephirahClicked);
            kabbalisticTree.PathClicked +=
                new EventHandler(EventHandler_PathClicked);
        }

        private void EventHandler_PathClicked(object sender, EventArgs e)
        {
            KabbalisticPath path = (KabbalisticPath)sender;

            Display.Message("Event Handler triggered for: " + path.Description);
        }

        public void EventHandler_SephirahClicked(object sender, EventArgs e)
        {
            Sephirah seph = (Sephirah)sender;

            Display.Message("Event Handler triggered for: " + seph.Name);
        }
    }
}
