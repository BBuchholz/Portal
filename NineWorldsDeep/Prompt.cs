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
        public static string ForXmlFileSave(string defaultPath)
        {
            return ForFileSave(defaultPath, "Xml files (*.xml)|*.xml");
        }

        public static string ForXmlFileLoad(string defaultPath)
        {
            return ForFileOpen(defaultPath, "Xml files (*.xml)|*.xml");
        }

        public static string ForTextFile(string defaultPath)
        {
            //TODO: deprecate this and refactor to ForTextFileOpen()
            return ForFileOpen(defaultPath, "Text files (*.txt)|*.txt");
        }

        private static string ForFileSave(string defaultPath, string filter)
        {
            var dlg = new System.Windows.Forms.SaveFileDialog();

            dlg.InitialDirectory = defaultPath;
            dlg.Filter = filter;
            dlg.FilterIndex = 1;

            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.FileName;
            }

            return null;
        }
        
        private static string ForFileOpen(string defaultPath, string filter)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();

            dlg.InitialDirectory = defaultPath;
            dlg.Filter = filter;
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
        
        public static bool Confirm(string message)
        {
            DialogResult result =
                MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }
    }
}
