using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineWorldsDeep.UI
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
            return ForFolder(defaultPath, "Select Folder");
        }

        public static string ForFolder(string defaultPath, string description)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = defaultPath;
            dlg.Description = description;
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dlg.SelectedPath;
            }

            return null;
        }

        /// <summary>
        /// if defaultNo is set to true, the default response
        /// will be 'no', if false, default is 'yes'
        /// </summary>
        /// <param name="message"></param>
        /// <param name="defaultNo"></param>
        /// <returns></returns>
        public static bool Confirm(string message, bool defaultNo = false)
        {
            MessageBoxDefaultButton defaultButton;

            if (defaultNo)
            {
                defaultButton = MessageBoxDefaultButton.Button2;
            }
            else
            {
                defaultButton = MessageBoxDefaultButton.Button1;
            }

            DialogResult result =
                MessageBox.Show(message,
                                "Confirm",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question, defaultButton);

            return result == DialogResult.Yes;
        }

        public static string Input(string promptMsg)
        {
            return Interaction.InputBox(promptMsg);
        }

        public static string Input(string promptMsg, string defaultValue)
        {
            return Interaction.InputBox(promptMsg, "Input", defaultValue);
        }

        public static string ForNwdUri()
        {
            return ForNwdUri("Enter Nwd Uri: ");
        }

        public static string ForNwdUri(string message)
        {
            string uri = Input(message);

            Parser.Parser p = new Parser.Parser();

            while (!p.validateNestedKey(uri))
            {
                string newMsg = "Invalid NWD URI. " +
                    message;

                uri = Input(newMsg);
            }

            return uri;
        }

        public static int ForInteger(string message)
        {
            string input = Input(message);
            int output;

            while(!Int32.TryParse(input, out output))
            {
                input = Input("Invalid integer. " + message);
            }

            return output;
        }
    }
}
