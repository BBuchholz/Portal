namespace NineWorldsDeep.Tapestry
{
    public enum TapestryNodeType
    {
        //TODO: this has a bad smell, Collection, List, and SingleNodeDefault are NodeDisplayTypes, not NodeTypes
        //maybe rename for clarity (I was lead here looking for types, but as display types these are already quite internal)?
        Collection,
        List,
        Audio,
        Image,
        Cluster,
        SingleNodeDefault,
        DevicePath,
        Device,
        SynergyList,
        SynergyListItem,
        LyricBit,
        NullCluster,
        ChordProgressions,
        Chord,
        SynergyV5MasterList,
        SynergyV5List,
        SynergyV5ListItem,
        MediaMaster,
        NullSynergyV5List,
        HierophantTreeOfLife,
        HierophantVertex,
        NullHierophantVertex,
        ArchivistMaster,
        ArchivistSource,
        MediaTag,
        HiveMain,
        TaggedMediaMain,
        BooksApiMain
    }
}