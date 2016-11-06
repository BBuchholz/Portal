namespace NineWorldsDeep.Synergy.V5
{
    public class SynergyV5ListItem
    {
        public SynergyV5ListItem(string itemValue)
        {
            ItemValue = itemValue;
        }

        //should this become a SynergyItem object? to mirror db?
        public string ItemValue { get; private set; }
    }
}