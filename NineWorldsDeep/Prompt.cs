using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineWorldsDeep
{
    public class Prompt
    {
        public static string ForTextFile(string defaultPath)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();

            dlg.InitialDirectory = defaultPath;
            dlg.Filter = "Text files (*.txt)|*.txt";
            dlg.FilterIndex = 1;

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.FileName;
            }

            return null;
        }

        public static string ForFolder(string defaultPath)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = defaultPath;
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.SelectedPath;
            }

            return null;
        }
    }
}
