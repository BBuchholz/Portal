using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace NineWorldsDeep.Tapestry.NodeUI
{
    //working from: http://stackoverflow.com/a/18076638/670768
    //very new to the classes used, copy-pasting and 
    //experimenting this may get messy and need cleanup
    public class ArchivistUiTagFormatter
    {
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
        "FormattedText",
        typeof(string),
        typeof(ArchivistUiTagFormatter),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure, FormattedTextPropertyChanged));

        public static void SetFormattedText(DependencyObject textBlock, string value)
        {
            textBlock.SetValue(FormattedTextProperty, value);
        }

        public static string GetFormattedText(DependencyObject textBlock)
        {
            return (string)textBlock.GetValue(FormattedTextProperty);
        }

        private static void FormattedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = d as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            var formattedText = (string)e.NewValue ?? string.Empty;

            //do tag split and hyperlink formatting here?
            var splitTags = formattedText.Split(',');
            var processedHyperlinkTags = new List<string>();
            
            foreach(string tag in splitTags)
            {
                string xamlLink =
                //"<Hyperlink Hyperlink.Click=\"Hyperlink_TagClicked\">" + tag + "</Hyperlink>";
                "<Hyperlink><Run Text=\"" + tag.Trim() + "\"/></Hyperlink>";

                processedHyperlinkTags.Add(xamlLink);
            }

            formattedText = string.Join(", ", processedHyperlinkTags);

            formattedText = string.Format("<Span xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{0}</Span>", formattedText);

            textBlock.Inlines.Clear();
            using (var xmlReader = XmlReader.Create(new StringReader(formattedText)))
            {
                var result = (Span)XamlReader.Load(xmlReader);
                //add handler here?
                textBlock.Inlines.Add(result);
            }
        }

        public static void Hyperlink_TagClicked(object sender, RoutedEventArgs e)
        {
            UI.Display.Message("clicked");
        }
    }
}
