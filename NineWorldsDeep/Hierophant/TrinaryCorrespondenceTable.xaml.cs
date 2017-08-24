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

namespace NineWorldsDeep.Hierophant
{
    /// <summary>
    /// Interaction logic for CorrespondenceTable.xaml
    /// </summary>
    public partial class TrinaryCorrespondenceTable : UserControl
    {
        public TrinaryCorrespondenceTable()
        {
            InitializeComponent();
            Mockup();
        }

        private void Mockup()
        {
            List<MothersRow> mothers = new List<MothersRow>();
            List<DoublesRow> doubles = new List<DoublesRow>();
            List<ElementalsRow> elementals = new List<ElementalsRow>();

            mothers.Add(new MothersRow()
            {
                Path = "Binah-Chokmah",
                Element = "Fire",
                Letter = "Shin"
            });

            doubles.Add(new DoublesRow()
            {
                Path = "Yesod-Malkuth",
                Planet = "Mercury",
                Crown = "Grace"
            });

            elementals.Add(new ElementalsRow()
            {
                Path = "Chokmah-Geburah",
                Letter = "Heh",
                Sign = "Aries"
            });
            
            elementals.Add(new ElementalsRow()
            {
                Path = "Binah-Chesed",
                Letter = "Cheth",
                Sign = "Cancer"
            });

            dgridMothers.ItemsSource = mothers;
            dgridDoubles.ItemsSource = doubles;
            dgridElementals.ItemsSource = elementals;
        }
    }
}
