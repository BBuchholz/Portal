namespace NineWorldsDeep.Synergy
{
    public class StringListItem
    {
        private string s;

        public StringListItem(string s)
        {
            this.s = s;
        }

        public string Value { get { return s; } set { s = value; } }
    }
}