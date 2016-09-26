using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NineWorldsDeep.Core
{
    class WordUtils
    {
        internal static string CapitalizeFully(string textToCapitalize)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(textToCapitalize);
        }
    }
}
