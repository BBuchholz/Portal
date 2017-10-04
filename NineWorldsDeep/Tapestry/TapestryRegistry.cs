namespace NineWorldsDeep.Tapestry
{
    public class TapestryRegistry
    {
        static TapestryRegistry()
        {
            GlobalLoadLocal = false;
        }

        /// <summary>
        /// if true, all nodes will be loaded into the pane they are selected from
        /// as opposed to the default behavior of loading to the opposite pane
        /// </summary>
        public static bool GlobalLoadLocal { get; internal set; }
    }
}