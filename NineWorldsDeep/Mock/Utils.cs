using NineWorldsDeep.Archivist;
using NineWorldsDeep.Core;
using NineWorldsDeep.FragmentCloud;
using NineWorldsDeep.Mnemosyne.V5;
using NineWorldsDeep.Tapestry.Nodes;
using NineWorldsDeep.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Mock
{
    class Utils
    {
        private static Random rnd =
            new Random();

        private static List<String> mAdjectives =
            new List<String>();
        private static List<String> mNouns =
                new List<String>();

        private static List<String> mVerbs =
                new List<String>();


        //NEED LISTS FOR EACH MODEL ITEM TO HOLD A FEW MOCKED COMMON ITEMS
        //eg. A couple FileModelItems to add into multiple nodes to test commonality
        private static List<DevicePathNode> mDevicePaths = new List<DevicePathNode>();


        private static List<HashNode> mHashes = new List<HashNode>();
        private static List<TagNode> mTags = new List<TagNode>();
        private static List<LocalConfigNode> mLocalConfig = new List<LocalConfigNode>();
        private static List<SynergyListNode> mSynergyLists = new List<SynergyListNode>();
        private static List<SynergyItemNode> mSynergyListItems = new List<SynergyItemNode>();

        static Utils()
        {

            //NB: all words were randomly generated with an online app
            populateNouns();
            populateAdjectives();
            populateVerbs();

            PopulateFiles();
            PopulateLocalConfig();
            PopulateSynergyLists();
        }

        public static IEnumerable<DevicePathNode> GetAllDevicePathNodes()
        {
            return mDevicePaths;
        }

        internal static IEnumerable<SynergyListNode> GetAllSynergyListNodes()
        {
            return mSynergyLists;
        }

        private static void PopulateSynergyListItems(SynergyListNode lst)
        {

            //add a random number of existing items
            int numberOfExistingToAdd = GetRandomInteger(6);

            for (int i = 0; i < numberOfExistingToAdd; i++)
            {

                if (mSynergyListItems.Count > 0)
                {

                    int idx = GetRandomInteger(mSynergyListItems.Count);

                    lst.Add(mSynergyListItems[idx]);
                }
            }

            //generate a random number of new items to add to shared list
            int numberOfNewSharedToAdd = GetRandomInteger(4);

            for (int i = 0; i < numberOfNewSharedToAdd; i++)
            {

                String testText = getRandomVerb() + " " +
                        getRandomAdjective() + " " +
                        GetRandomNoun();

                SynergyItemNode sli =
                        new SynergyItemNode(lst, testText);

                lst.Add(sli);
                mSynergyListItems.Add(sli);
            }

            //generate a random number of new items to keep seperate
            int numberOfNewPrivateToAdd = GetRandomInteger(4);

            for (int i = 0; i < numberOfNewPrivateToAdd; i++)
            {

                String testText = getRandomVerb() + " " +
                        getRandomAdjective() + " " +
                        GetRandomNoun();

                SynergyItemNode sli =
                        new SynergyItemNode(lst, testText);

                lst.Add(sli);
            }
        }

        private static void PopulateTags(DevicePathNode fmi)
        {

            //add a random number of existing tags
            int numberOfExistingToAdd = GetRandomInteger(3);

            for (int i = 0; i < numberOfExistingToAdd; i++)
            {

                if (mTags.Count > 0)
                {

                    int idx = GetRandomInteger(mTags.Count);

                    fmi.Add(mTags[idx]);
                }
            }

            //generate a random number of new tags to add to shared list
            int numberOfNewSharedToAdd = GetRandomInteger(4);

            for (int i = 0; i < numberOfNewSharedToAdd; i++)
            {

                String testTag;

                if (i % 2 == 0)
                {

                    testTag =
                            getRandomAdjective() + " " +
                                    GetRandomNoun();
                }
                else
                {

                    testTag =
                            GetRandomNoun();
                }

                TagNode tmi = new TagNode(fmi, testTag);
                fmi.Add(tmi);
                mTags.Add(tmi);
            }

            //generate a random number of new tags to keep seperate
            int numberOfNewPrivateToAdd = GetRandomInteger(3);

            for (int i = 0; i < numberOfNewPrivateToAdd; i++)
            {

                String testTag;

                if (i % 2 == 0)
                {

                    testTag =
                            getRandomAdjective() + " " +
                                    GetRandomNoun();
                }
                else
                {

                    testTag =
                            GetRandomNoun();
                }

                TagNode tmi = new TagNode(fmi, testTag);
                fmi.Add(tmi);
            }
        }

        private static void PopulateHashes(DevicePathNode fmi)
        {

            int numberOfExistingToAdd = GetRandomInteger(2);

            for (int i = 0; i < numberOfExistingToAdd; i++)
            {

                if (mHashes.Count > 0)
                {

                    int idx = GetRandomInteger(mHashes.Count);

                    fmi.Add(mHashes[idx]);
                }
            }

            //generate a random number of new hashes to add to shared list
            int numberOfNewSharedToAdd = GetRandomInteger(2);

            for (int i = 0; i < numberOfNewSharedToAdd; i++)
            {

                HashNode hmi = GetRandomHashModelItem(fmi);

                fmi.Add(hmi);
                mHashes.Add(hmi);
            }

            //generate a random number of new hashes to keep seperate
            int numberOfNewPrivateToAdd = GetRandomInteger(2);

            for (int i = 0; i < numberOfNewPrivateToAdd; i++)
            {

                HashNode hmi = GetRandomHashModelItem(fmi);

                fmi.Add(hmi);
            }
        }

        private static HashNode GetRandomHashModelItem(DevicePathNode fmi)
        {

            return new HashNode(fmi,
                    GetRandomSha1HashString(),
                    GetRandomTimeStamp());
        }

        private static String GetRandomTimeStamp()
        {
            DateTime from = new DateTime(2015, 1, 1, 0, 0, 0, 0);
            DateTime to = DateTime.Now;
            return NwdUtils.GetTimeStamp_yyyyMMddHHmmss(GetRandomDate(from, to));

            //long offset = TimeStamp.valueOf("2015-01-01 00:00:00").getTime();
            //long end = TimeStamp.valueOf("2016-09-08 00:00:00").getTime();
            //long diff = end - offset + 1;
            //TimeStamp rand = new TimeStamp(offset + (long)(rnd * diff));

            //Date dt = new Date(rand.getTime());

            //return SynergyUtils.getTimeStamp_yyyyMMddHHmmss(dt);
        }

        public static DateTime GetRandomDate(DateTime from, DateTime to)
        {
            var range = to - from;

            var randTimeSpan = new TimeSpan((long)(rnd.NextDouble() * range.Ticks));

            return from + randTimeSpan;
        }

        private static String GetRandomSha1HashString()
        {

            //just a very random string, all concatenated
            String randomString = getRandomAdjective() + GetRandomNoun() +
                    getRandomVerb() + GetRandomNoun() + getRandomVerb();

            String output;

            try
            {

                output = Hashes.Sha1ForStringValue(randomString);

            }
            catch (Exception)
            {
                //create empty hash
                output = "da39a3ee5e6b4b0d3255bfef95601890afd80709";
            }

            return output;
        }

        private static void PopulateSynergyLists()
        {

            int numberToAdd = GetRandomInteger(5, 21);

            for (int i = 0; i < numberToAdd; i++)
            {

                SynergyListNode lst =
                        new SynergyListNode(
                                WordUtils.CapitalizeFully(getRandomAdjective()) +
                                        WordUtils.CapitalizeFully(GetRandomNoun())
                        );

                PopulateSynergyListItems(lst);

                mSynergyLists.Add(lst);
            }
        }

        private static List<DevicePath> MockDevicePaths(int maxCount)
        {
            List<DevicePath> lst = new List<DevicePath>();

            int numberToAdd = GetRandomInteger(51);

            for (int i = 0; i < numberToAdd; i++)
            {

                String deviceDesc =
                        WordUtils.CapitalizeFully(getRandomAdjective()) + " " +
                                WordUtils.CapitalizeFully(GetRandomNoun());
                
                string path;

                if (i % 3 == 0)
                {
                    path = MockAudioUri();
                }
                else if (i % 2 == 0)
                {
                    path = MockImageUri();
                }
                else
                {
                    path = MockTextUri();
                }

                lst.Add(new DevicePath()
                {
                    DeviceName = deviceDesc,
                    DevicePathValue = path
                });              
            }

            return lst;
        }

        private static void PopulateFiles()
        {

            int numberToAdd = GetRandomInteger(51);

            for (int i = 0; i < numberToAdd; i++)
            {

                String deviceDesc =
                        WordUtils.CapitalizeFully(getRandomAdjective()) + " " +
                                WordUtils.CapitalizeFully(GetRandomNoun());

                //String path =
                //        "/NWD/" + getRandomNoun() + "/" + getRandomNoun() + ".test";

                string path;

                if(i % 3 == 0)
                {
                    path = MockAudioUri();
                }
                else if(i % 2 == 0)
                {
                    path = MockImageUri();
                }
                else
                {
                    path = MockTextUri();
                }

                DevicePathNode fmi = new DevicePathNode(deviceDesc, path);

                PopulateTags(fmi);
                PopulateHashes(fmi);

                mDevicePaths.Add(fmi);
            }
        }

        private static void PopulateLocalConfig()
        {

            int numberToAdd = GetRandomInteger(9);

            for (int i = 0; i < numberToAdd; i++)
            {

                String key = GetRandomNoun();
                String value = getRandomAdjective() + " " + GetRandomNoun();

                mLocalConfig.Add(new LocalConfigNode(key, value));
            }
        }
        
        public static Media MockMedia()
        {            
            Media m = new Media()
            {
                MediaHash = GetRandomSha1HashString()
            };

            foreach(DevicePath dp in MockDevicePaths(15))
            {
                m.Add(dp);
            }

            return m;
        }

        public static void PopulateTestMedia(MediaTag mediaTag)
        {
            int limit = GetRandomInteger(15);

            for (int i = 0; i < limit; i++)
            {
                mediaTag.Add(MockMedia());
            }
        }

        //public static void PopulateTestExcerpts(MediaTag mediaTag)
        //{
        //    foreach(ArchivistSourceExcerpt ase in MockArchivistSourceExcerpts())
        //    {
        //        mediaTag.Add(ase);
        //    }
        //}

        //public static List<ArchivistSourceExcerpt> MockArchivistSourceExcerpts()
        //{
        //    List<ArchivistSourceExcerpt> testExcerpts =
        //        new List<ArchivistSourceExcerpt>();

        //    Random r = new Random();
        //    int rndInt = r.Next(3, 15);
        //    int rndTags = r.Next(1, 8);
        //    int rndParas = r.Next(1, 4);

        //    for (int i = 1; i < rndInt; i++)
        //    {
        //        testExcerpts.Add(new ArchivistSourceExcerpt()
        //        {
        //            ExcerptValue = string.Join(System.Environment.NewLine +
        //                                       System.Environment.NewLine,
        //                                       Faker.Lorem.Paragraphs(rndParas)),
        //            TagString = string.Join(", ", Faker.Lorem.Words(rndTags))
        //        });
        //    }

        //    return testExcerpts;
        //}

        private static void populateVerbs()
        {

            mVerbs.Add("bless");
            mVerbs.Add("phone");
            mVerbs.Add("complete");
            mVerbs.Add("encourage");
            mVerbs.Add("end");
            mVerbs.Add("paint");
            mVerbs.Add("twist");
            mVerbs.Add("tumble");
            mVerbs.Add("slip");
            mVerbs.Add("apologise");
            mVerbs.Add("wobble");
            mVerbs.Add("queue");
            mVerbs.Add("fire");
            mVerbs.Add("supply");
            mVerbs.Add("communicate");
            mVerbs.Add("annoy");
            mVerbs.Add("attract");
            mVerbs.Add("fetch");
            mVerbs.Add("matter");
            mVerbs.Add("crack");
            mVerbs.Add("shade");
            mVerbs.Add("hover");
            mVerbs.Add("warm");
            mVerbs.Add("change");
            mVerbs.Add("rot");
            mVerbs.Add("head");
            mVerbs.Add("attack");
            mVerbs.Add("wash");
            mVerbs.Add("x-ray");
            mVerbs.Add("grab");
            mVerbs.Add("suspend");
            mVerbs.Add("crush");
            mVerbs.Add("shiver");
            mVerbs.Add("intend");
            mVerbs.Add("check");
            mVerbs.Add("approve");
            mVerbs.Add("clap");
            mVerbs.Add("trap");
            mVerbs.Add("excite");
            mVerbs.Add("park");
            mVerbs.Add("punch");
            mVerbs.Add("judge");
            mVerbs.Add("add");
            mVerbs.Add("flood");
            mVerbs.Add("sin");
            mVerbs.Add("explain");
            mVerbs.Add("mug");
            mVerbs.Add("scorch");
            mVerbs.Add("dare");
            mVerbs.Add("jog");
            mVerbs.Add("deceive");
            mVerbs.Add("juggle");
            mVerbs.Add("avoid");
            mVerbs.Add("behave");
            mVerbs.Add("kick");
            mVerbs.Add("whine");
            mVerbs.Add("number");
            mVerbs.Add("escape");
            mVerbs.Add("dress");
            mVerbs.Add("hammer");
            mVerbs.Add("mark");
            mVerbs.Add("present");
            mVerbs.Add("skip");
            mVerbs.Add("drop");
            mVerbs.Add("tremble");
            mVerbs.Add("play");
            mVerbs.Add("command");
            mVerbs.Add("employ");
            mVerbs.Add("pour");
            mVerbs.Add("claim");
            mVerbs.Add("dislike");
            mVerbs.Add("blush");
            mVerbs.Add("delight");
            mVerbs.Add("welcome");
            mVerbs.Add("ski");
            mVerbs.Add("serve");
            mVerbs.Add("thaw");
            mVerbs.Add("appear");
            mVerbs.Add("pretend");
            mVerbs.Add("remove");
            mVerbs.Add("chop");
            mVerbs.Add("want");
            mVerbs.Add("form");
            mVerbs.Add("coil");
            mVerbs.Add("untidy");
            mVerbs.Add("signal");
            mVerbs.Add("spill");
            mVerbs.Add("permit");
            mVerbs.Add("clip");
            mVerbs.Add("bolt");
            mVerbs.Add("hug");
            mVerbs.Add("shave");
            mVerbs.Add("release");
            mVerbs.Add("brush");
            mVerbs.Add("fax");
            mVerbs.Add("train");
            mVerbs.Add("cure");
            mVerbs.Add("learn");
            mVerbs.Add("arrive");
            mVerbs.Add("concern");
        }

        private static void populateAdjectives()
        {

            mAdjectives.Add("spiffy");
            mAdjectives.Add("bloody");
            mAdjectives.Add("grateful");
            mAdjectives.Add("tacit");
            mAdjectives.Add("alcoholic");
            mAdjectives.Add("outstanding");
            mAdjectives.Add("bright");
            mAdjectives.Add("defiant");
            mAdjectives.Add("silly");
            mAdjectives.Add("successful");
            mAdjectives.Add("nappy");
            mAdjectives.Add("luxuriant");
            mAdjectives.Add("condemned");
            mAdjectives.Add("ceaseless");
            mAdjectives.Add("few");
            mAdjectives.Add("arrogant");
            mAdjectives.Add("vacuous");
            mAdjectives.Add("elfin");
            mAdjectives.Add("imported");
            mAdjectives.Add("faint");
            mAdjectives.Add("open");
            mAdjectives.Add("wiry");
            mAdjectives.Add("kaput");
            mAdjectives.Add("thoughtful");
            mAdjectives.Add("reflective");
            mAdjectives.Add("cruel");
            mAdjectives.Add("economic");
            mAdjectives.Add("adaptable");
            mAdjectives.Add("frantic");
            mAdjectives.Add("plain");
            mAdjectives.Add("daily");
            mAdjectives.Add("statuesque");
            mAdjectives.Add("enthusiastic");
            mAdjectives.Add("gusty");
            mAdjectives.Add("wasteful");
            mAdjectives.Add("remarkable");
            mAdjectives.Add("damaging");
            mAdjectives.Add("oafish");
            mAdjectives.Add("lumpy");
            mAdjectives.Add("tart");
            mAdjectives.Add("distinct");
            mAdjectives.Add("bent");
            mAdjectives.Add("sedate");
            mAdjectives.Add("various");
            mAdjectives.Add("burly");
            mAdjectives.Add("flimsy");
            mAdjectives.Add("cloudy");
            mAdjectives.Add("incandescent");
            mAdjectives.Add("scintillating");
            mAdjectives.Add("crooked");
            mAdjectives.Add("beneficial");
            mAdjectives.Add("woebegone");
            mAdjectives.Add("rigid");
            mAdjectives.Add("feigned");
            mAdjectives.Add("synonymous");
            mAdjectives.Add("utter");
            mAdjectives.Add("adjoining");
            mAdjectives.Add("naughty");
            mAdjectives.Add("mighty");
            mAdjectives.Add("tacky");
            mAdjectives.Add("skillful");
            mAdjectives.Add("slow");
            mAdjectives.Add("chilly");
            mAdjectives.Add("dangerous");
            mAdjectives.Add("chemical");
            mAdjectives.Add("useful");
            mAdjectives.Add("absent");
            mAdjectives.Add("thirsty");
            mAdjectives.Add("aquatic");
            mAdjectives.Add("tense");
            mAdjectives.Add("horrible");
            mAdjectives.Add("scandalous");
            mAdjectives.Add("piquant");
            mAdjectives.Add("cynical");
            mAdjectives.Add("gamy");
            mAdjectives.Add("physical");
            mAdjectives.Add("workable");
            mAdjectives.Add("willing");
            mAdjectives.Add("comfortable");
            mAdjectives.Add("guarded");
            mAdjectives.Add("quickest");
            mAdjectives.Add("incompetent");
            mAdjectives.Add("valuable");
            mAdjectives.Add("gaping");
            mAdjectives.Add("trite");
            mAdjectives.Add("vast");
            mAdjectives.Add("wistful");
            mAdjectives.Add("lively");
            mAdjectives.Add("illegal");
            mAdjectives.Add("devilish");
            mAdjectives.Add("gentle");
            mAdjectives.Add("finicky");
            mAdjectives.Add("graceful");
            mAdjectives.Add("wide-eyed");
            mAdjectives.Add("super");
            mAdjectives.Add("actually");
            mAdjectives.Add("entertaining");
            mAdjectives.Add("public");
            mAdjectives.Add("mundane");
            mAdjectives.Add("apathetic");
        }

        private static void populateNouns()
        {
            mNouns.Add("silver");
            mNouns.Add("key");
            mNouns.Add("club");
            mNouns.Add("mouth");
            mNouns.Add("weight");
            mNouns.Add("zinc");
            mNouns.Add("bells");
            mNouns.Add("skirt");
            mNouns.Add("loaf");
            mNouns.Add("dolls");
            mNouns.Add("bike");
            mNouns.Add("pizzas");
            mNouns.Add("unit");
            mNouns.Add("vacation");
            mNouns.Add("song");
            mNouns.Add("tomatoes");
            mNouns.Add("dress");
            mNouns.Add("pear");
            mNouns.Add("scarecrow");
            mNouns.Add("middle");
            mNouns.Add("end");
            mNouns.Add("argument");
            mNouns.Add("tiger");
            mNouns.Add("minister");
            mNouns.Add("detail");
            mNouns.Add("sink");
            mNouns.Add("need");
            mNouns.Add("health");
            mNouns.Add("nut");
            mNouns.Add("effect");
            mNouns.Add("potato");
            mNouns.Add("note");
            mNouns.Add("town");
            mNouns.Add("expansion");
            mNouns.Add("boy");
            mNouns.Add("mice");
            mNouns.Add("rake");
            mNouns.Add("change");
            mNouns.Add("credit");
            mNouns.Add("coast");
            mNouns.Add("rest");
            mNouns.Add("river");
            mNouns.Add("roll");
            mNouns.Add("twig");
            mNouns.Add("smile");
            mNouns.Add("judge");
            mNouns.Add("floor");
            mNouns.Add("furniture");
            mNouns.Add("cent");
            mNouns.Add("tendency");
            mNouns.Add("notebook");
            mNouns.Add("observation");
            mNouns.Add("birds");
            mNouns.Add("church");
            mNouns.Add("cream");
            mNouns.Add("monkey");
            mNouns.Add("degree");
            mNouns.Add("verse");
            mNouns.Add("star");
            mNouns.Add("government");
            mNouns.Add("play");
            mNouns.Add("respect");
            mNouns.Add("approval");
            mNouns.Add("car");
            mNouns.Add("spark");
            mNouns.Add("zephyr");
            mNouns.Add("throne");
            mNouns.Add("geese");
            mNouns.Add("ground");
            mNouns.Add("pollution");
            mNouns.Add("discussion");
            mNouns.Add("destruction");
            mNouns.Add("place");
            mNouns.Add("hammer");
            mNouns.Add("decision");
            mNouns.Add("transport");
            mNouns.Add("view");
            mNouns.Add("fly");
            mNouns.Add("crime");
            mNouns.Add("railway");
            mNouns.Add("fruit");
            mNouns.Add("orange");
            mNouns.Add("match");
            mNouns.Add("hydrant");
            mNouns.Add("coal");
            mNouns.Add("neck");
            mNouns.Add("kittens");
            mNouns.Add("peace");
            mNouns.Add("pleasure");
            mNouns.Add("ghost");
            mNouns.Add("window");
            mNouns.Add("group");
            mNouns.Add("cover");
            mNouns.Add("babies");
            mNouns.Add("lunch");
            mNouns.Add("run");
            mNouns.Add("blade");
            mNouns.Add("sponge");
            mNouns.Add("turkey");
            mNouns.Add("clam");
        }

        public static String GetRandomNoun()
        {

            int idx = GetRandomInteger(mNouns.Count);
            return mNouns[idx];
        }

        public static String getRandomAdjective()
        {

            int idx = GetRandomInteger(mAdjectives.Count);
            return mAdjectives[idx];
        }

        public static String getRandomVerb()
        {

            int idx = GetRandomInteger(mVerbs.Count);
            return mVerbs[idx];
        }

        private static int GetRandomInteger(int upperBoundExclusive)
        {
            return rnd.Next(upperBoundExclusive);
        }

        private static int GetRandomInteger(int lowerBoundInclusive, int upperBoundExclusive)
        {
            return rnd.Next(lowerBoundInclusive, upperBoundExclusive);
        }

        private static string MockFileName(string extension)
        {
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            return WordUtils.CapitalizeFully(getRandomVerb()) + 
                WordUtils.CapitalizeFully(getRandomAdjective()) + 
                WordUtils.CapitalizeFully(GetRandomNoun()) + 
                extension;
        }

        internal static string MockTextUri()
        {
            return MockSandboxFolderURI() + MockFileName("txt");
        }
        
        internal static string MockAudioUri()
        {
            return MockSandboxFolderURI() + MockFileName("wav");
        }

        internal static string MockImageUri()
        {
            return MockSandboxFolderURI() + MockFileName("png");
        }

        internal static string MockSubFolderPathAndName()
        {
            return WordUtils.CapitalizeFully(getRandomAdjective()) +
                WordUtils.CapitalizeFully(GetRandomNoun()) + "/" +
                WordUtils.CapitalizeFully(getRandomAdjective()) +
                WordUtils.CapitalizeFully(GetRandomNoun()) + "/";
        }

        internal static string MockSandboxFolderURI()
        {
            return "NWD-SNDBX/mock/" + MockSubFolderPathAndName();
        }
    }
}
