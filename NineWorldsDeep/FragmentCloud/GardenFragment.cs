using System;

namespace NineWorldsDeep.FragmentCloud
{
    public class GardenFragment : Fragment
    {
        public GardenFragment()
            : base("")
        {
            AddChild(new FileSystemFragment("NWD", true));
            AddChild(new FileSystemFragment("NWD-AUX", true));
            AddChild(new FileSystemFragment("NWD-MEDIA", true));
            AddChild(new FileSystemFragment("NWD-SNDBX", true));
            AddChild(new FileSystemFragment("NWD-SYNC", true));
        }

        public override string GetShortName()
        {
            return "Garden";
        }

        public override void PerformSelectionAction()
        {
            //do nothing
        }
    }
}