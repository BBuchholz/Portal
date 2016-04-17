namespace NineWorldsDeep.Synergy
{
    public class GauntletUtils
    {
        public static bool IsCategorizedItem(string item)
        {
            return item.StartsWith("::") &&
                item.Contains(":: - ");
        }

        public static string ParseCategory(string item)
        {
            if (IsCategorizedItem(item))
            {
                int endIdx = item.IndexOf(":: - ");
                int length = endIdx - 2;
                return item.Substring(2, length);
            }
            else
            {
                return null;
            }
        }

        public static string TrimCategory(string item)
        {
            if (IsCategorizedItem(item))
            {
                int startIdx = item.IndexOf(":: - ") + 5;
                return item.Substring(startIdx);
            }
            else
            {
                return item;
            }
        }
    }
}