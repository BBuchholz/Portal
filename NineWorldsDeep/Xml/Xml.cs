using NineWorldsDeep.Core;
using NineWorldsDeep.Synergy.V5;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NineWorldsDeep.Mnemosyne.V5;
using System.Xml;
using NineWorldsDeep.Tapestry.NodeUI;
using NineWorldsDeep.Hierophant;

namespace NineWorldsDeep.Xml
{
    public class Xml
    {
        #region "contract"

        //tags -
        private static string TAG_NWD = "nwd";

        public static string TAG_MNEMOSYNE_SUBSET = "mnemosyneSubset";
        private static string TAG_MEDIA = "media";
        private static string TAG_TAG = "tag";
        private static string TAG_MEDIA_DEVICE = "mediaDevice";
        private static string TAG_PATH = "path";

        public static string TAG_SYNERGY_SUBSET = "synergySubset";
        private static string TAG_SYNERGY_LIST = "synergyList";
        private static string TAG_SYNERGY_ITEM = "synergyItem";
        private static string TAG_ITEM_VALUE = "itemValue";
        private static string TAG_TO_DO = "toDo";

        //attributes
        private static string ATTRIBUTE_SHA1_HASH = "sha1Hash";
        private static string ATTRIBUTE_TAGS = "tags";
        private static string ATTRIBUTE_LIST_NAME = "listName";
        private static string ATTRIBUTE_ACTIVATED_AT = "activatedAt";
        private static string ATTRIBUTE_SHELVED_AT = "shelvedAt";
        private static string ATTRIBUTE_POSITION = "position";
        private static string ATTRIBUTE_COMPLETED_AT = "completedAt";
        private static string ATTRIBUTE_ARCHIVED_AT = "archivedAt";

        private static string ATTRIBUTE_TAG_VALUE = "tagValue";
        private static string ATTRIBUTE_TAGGED_AT = "taggedAt";
        private static string ATTRIBUTE_UNTAGGED_AT = "untaggedAt";
        private static string ATTRIBUTE_DESCRIPTION = "description";
        private static string ATTRIBUTE_VALUE = "value";
        private static string ATTRIBUTE_VERIFIED_PRESENT = "verifiedPresent";
        private static string ATTRIBUTE_VERIFIED_MISSING = "verifiedMissing";

        private static string ATTRIBUTE_FILE_NAME = "fileName";

        #endregion

        public static XDocument DocumentFromPath(string xmlFilePath)
        {
            return XDocument.Load(xmlFilePath);
        }

        public static void AddSynergySubsetElement(XDocument doc, 
                                                   List<SynergyV5List> lists)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// expects UTC date string
        /// </summary>
        /// <param name="utcDateString">format: YYYY-MM-DD HH:MM:SS, UTC</param>
        /// <returns></returns>
        public static DateTime? ToTime(string utcDateString)
        {
            //DateTime? output = null;

            //try
            //{
            //    output = DateTime.ParseExact(dateString,
            //                          "yyyy-MM-dd HH:mm:ss",
            //                          CultureInfo.InvariantCulture);
            //}
            //catch (Exception)
            //{
            //    //do nothing
            //}

            //return output;

            return TimeStamp.YYYY_MM_DD_HH_MM_SS_UTC_ToDateTime(utcDateString);
        }

        public static XElement Export(SynergyV5List lst)
        {
            XElement synergyListEl =
                    new XElement(TAG_SYNERGY_LIST);

            synergyListEl.Add(new XAttribute(ATTRIBUTE_LIST_NAME, lst.ListName));

            string activatedAt =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(lst.ActivatedAt);
            string shelvedAt =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(lst.ShelvedAt);

            synergyListEl.Add(
                new XAttribute(ATTRIBUTE_ACTIVATED_AT, activatedAt));
            synergyListEl.Add(
                new XAttribute(ATTRIBUTE_SHELVED_AT, shelvedAt));

            for (int i = 0; i < lst.ListItems.Count; i++)
            {
                SynergyV5ListItem item = lst.ListItems[i];

                XElement synergyItemEl =
                    new XElement(TAG_SYNERGY_ITEM);

                synergyItemEl.Add(new XAttribute(ATTRIBUTE_POSITION, i));

                XElement itemValueEl =
                    new XElement(TAG_ITEM_VALUE);

                itemValueEl.SetValue(item.ItemValue);

                synergyItemEl.Add(itemValueEl);

                SynergyV5ToDo toDo = item.ToDo;

                if (toDo != null)
                {
                    string toDoActivatedAt =
                        TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.ActivatedAt);
                    string toDoCompletedAt =
                        TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.CompletedAt);
                    string toDoArchivedAt =
                        TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.ArchivedAt);

                    XElement toDoEl =
                        new XElement(TAG_TO_DO,
                            new XAttribute(ATTRIBUTE_ACTIVATED_AT, toDoActivatedAt),
                            new XAttribute(ATTRIBUTE_COMPLETED_AT, toDoCompletedAt),
                            new XAttribute(ATTRIBUTE_ARCHIVED_AT, toDoArchivedAt));

                    synergyItemEl.Add(toDoEl);
                }

                synergyListEl.Add(synergyItemEl);
            }

            return synergyListEl;
        }

        public static XElement Export(List<SynergyV5List> synergyV5Lists)
        {
            XElement synergySubsetEl = new XElement(TAG_SYNERGY_SUBSET);

            foreach (SynergyV5List lst in synergyV5Lists)
            {
                synergySubsetEl.Add(Export(lst));
            }

            return synergySubsetEl;
        }


        //public static XElement Export(List<SynergyV5List> synergyV5Lists)
        //{
        //    XElement synergySubsetEl = new XElement(TAG_SYNERGY_SUBSET);

        //    foreach (SynergyV5List lst in synergyV5Lists)
        //    {
        //        XElement synergyListEl = 
        //            new XElement(TAG_SYNERGY_LIST);

        //        synergyListEl.Add(new XAttribute(ATTRIBUTE_LIST_NAME, lst.ListName));

        //        string activatedAt = 
        //            TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(lst.ActivatedAt);
        //        string shelvedAt =
        //            TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(lst.ShelvedAt);

        //        synergyListEl.Add(
        //            new XAttribute(ATTRIBUTE_ACTIVATED_AT, activatedAt));
        //        synergyListEl.Add(
        //            new XAttribute(ATTRIBUTE_SHELVED_AT, shelvedAt));

        //        for(int i = 0; i < lst.ListItems.Count; i++)
        //        {
        //            SynergyV5ListItem item = lst.ListItems[i];

        //            XElement synergyItemEl =
        //                new XElement(TAG_SYNERGY_ITEM);

        //            synergyItemEl.Add(new XAttribute(ATTRIBUTE_POSITION, i));

        //            XElement itemValueEl =
        //                new XElement(TAG_ITEM_VALUE);

        //            itemValueEl.SetValue(item.ItemValue);

        //            synergyItemEl.Add(itemValueEl);

        //            SynergyV5ToDo toDo = item.ToDo;

        //            if(toDo != null)
        //            {
        //                string toDoActivatedAt =
        //                    TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.ActivatedAt);
        //                string toDoCompletedAt =
        //                    TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.CompletedAt);
        //                string toDoArchivedAt =
        //                    TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(toDo.ArchivedAt); 

        //                XElement toDoEl =
        //                    new XElement(TAG_TO_DO,
        //                        new XAttribute(ATTRIBUTE_ACTIVATED_AT, toDoActivatedAt),
        //                        new XAttribute(ATTRIBUTE_COMPLETED_AT, toDoCompletedAt),
        //                        new XAttribute(ATTRIBUTE_ARCHIVED_AT, toDoArchivedAt));

        //                synergyItemEl.Add(toDoEl);
        //            }

        //            synergyListEl.Add(synergyItemEl);
        //        }

        //        synergySubsetEl.Add(synergyListEl);
        //    }

        //    return synergySubsetEl;
        //}

        public static List<SynergyV5List> RetrieveSynergyV5Lists(XDocument doc)
        {
            List<SynergyV5List> allLists = new List<SynergyV5List>();

            foreach(XElement listEl in doc.Descendants(TAG_SYNERGY_LIST))
            {
                string listName = listEl.Attribute(ATTRIBUTE_LIST_NAME).Value;
                string activatedAt = listEl.Attribute(ATTRIBUTE_ACTIVATED_AT).Value;
                string shelvedAt = listEl.Attribute(ATTRIBUTE_SHELVED_AT).Value;

                DateTime? activatedAtTime = ToTime(activatedAt);
                DateTime? shelvedAtTime = ToTime(shelvedAt);

                SynergyV5List lst = 
                    new SynergyV5List(listName);

                lst.SetTimeStamps(activatedAtTime, shelvedAtTime);

                foreach(XElement itemEl in listEl.Descendants(TAG_SYNERGY_ITEM))
                {
                    string position = itemEl.Attribute(ATTRIBUTE_POSITION).Value;

                    string itemValue = 
                        itemEl.Descendants(TAG_ITEM_VALUE).First().Value;

                    var toDos = itemEl.Descendants(TAG_TO_DO);

                    if(toDos.Count() > 0)
                    {
                        XElement toDoEl = toDos.First();

                        string itemActivatedAt = toDoEl.Attribute(ATTRIBUTE_ACTIVATED_AT).Value;
                        string completedAt = toDoEl.Attribute(ATTRIBUTE_COMPLETED_AT).Value;
                        string archivedAt = toDoEl.Attribute(ATTRIBUTE_ARCHIVED_AT).Value;

                        DateTime? itemActivatedAtTime = ToTime(itemActivatedAt);
                        DateTime? completedAtTime = ToTime(completedAt);
                        DateTime? archivedAtTime = ToTime(archivedAt);

                        SynergyV5ListItem item =
                            new SynergyV5ListItem(itemValue,
                                                  itemActivatedAtTime,
                                                  completedAtTime,
                                                  archivedAtTime);

                        lst.ListItems.Add(item);
                    }
                    else
                    {
                        lst.ListItems.Add(new SynergyV5ListItem(itemValue));
                    }
                }

                allLists.Add(lst);
            }

            return allLists;
        }

        /// <summary>
        /// returns the value of an attribute if found, and an 
        /// empty string if the attribute does not exist
        /// </summary>
        /// <param name="el"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        private static string GetAttributeValueIfExists(
            XElement el, string attributeName)
        {
            var attr = el.Attribute(attributeName);

            if(attr != null)
            {
                return attr.Value;
            }
            else
            {
                return "";
            }
        }
        
        public static List<Media> RetrieveMediaWithReaderAsync(
            string documentPath, IAsyncStatusResponsive ui)
        {
            List<Media> allMedia = new List<Media>();

            //read once first to count elements

            int totalElements = 0;
            int count = 0;
            
            using (XmlReader reader = XmlReader.Create(documentPath))
            {
                while (reader.ReadToFollowing(TAG_MEDIA))
                {
                    totalElements++;
                }
            }

            using (XmlReader reader = XmlReader.Create(documentPath))
            {
                //reader.ReadStartElement(TAG_NWD);
                //reader.ReadStartElement(TAG_MNEMOSYNE_SUBSET);

                while (reader.Read())
                {
                    //for testing
                    //if(count > 200)
                    //{
                    //    break;
                    //}

                    if (reader.Name == TAG_MEDIA)
                    {
                        count++;
                        
                        XElement mediaEl = (XElement)XNode.ReadFrom(reader);

                        string sha1Hash =
                            GetAttributeValueIfExists(mediaEl, ATTRIBUTE_SHA1_HASH);

                        //for testing
                        //if(sha1Hash.Equals("83559ec05eecef4625acde9e76bc1f06f118fecd", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    bool testing = true;
                        //}

                        string fileName =
                            GetAttributeValueIfExists(mediaEl, ATTRIBUTE_FILE_NAME);

                        string description =
                            GetAttributeValueIfExists(mediaEl, ATTRIBUTE_DESCRIPTION);

                        ui.StatusDetailUpdate(
                            "processing " + count + " of " + 
                            totalElements + " xml media elements with hash: " + 
                            sha1Hash);

                        Media media = new Media()
                        {
                            MediaHash = sha1Hash,
                            MediaDescription = description,
                            MediaFileName = fileName
                        };

                        //get tags
                        foreach (XElement tagEl in mediaEl.Elements(TAG_TAG))
                        {

                            //tagValue
                            //taggedAt
                            //untaggedAt
                            //mediaHash
                            string tagValue =
                                GetAttributeValueIfExists(tagEl, ATTRIBUTE_TAG_VALUE);

                            string taggedAt =
                                GetAttributeValueIfExists(tagEl, ATTRIBUTE_TAGGED_AT);

                            string untaggedAt =
                                GetAttributeValueIfExists(tagEl, ATTRIBUTE_UNTAGGED_AT);

                            DateTime? taggedAtTime = ToTime(taggedAt);
                            DateTime? untaggedAtTime = ToTime(untaggedAt);

                            MediaTagging tagging = new MediaTagging()
                            {
                                MediaHash = media.MediaHash,
                                MediaTagValue = tagValue
                            };

                            tagging.SetTimeStamps(taggedAtTime, untaggedAtTime);

                            media.Add(tagging);
                        }

                        //get device paths
                        foreach (XElement mediaDeviceEl in mediaEl.Elements(TAG_MEDIA_DEVICE))
                        {
                            string deviceName =
                                GetAttributeValueIfExists(mediaDeviceEl, ATTRIBUTE_DESCRIPTION);

                            foreach (XElement pathEl in mediaDeviceEl.Descendants(TAG_PATH))
                            {
                                string pathValue =
                                    GetAttributeValueIfExists(pathEl, ATTRIBUTE_VALUE);

                                string verifiedPresent =
                                    GetAttributeValueIfExists(pathEl, ATTRIBUTE_VERIFIED_PRESENT);

                                string verifiedMissing =
                                    GetAttributeValueIfExists(pathEl, ATTRIBUTE_VERIFIED_MISSING);

                                DateTime? verifiedPresentTime = ToTime(verifiedPresent);
                                DateTime? verifiedMissingTime = ToTime(verifiedMissing);

                                DevicePath dp = new DevicePath()
                                {
                                    DeviceName = deviceName,
                                    DevicePathValue = pathValue
                                };

                                dp.SetTimeStamps(verifiedPresentTime, verifiedMissingTime);

                                media.Add(dp);
                            }
                        }

                        allMedia.Add(media);
                    }
                }

                //reader.ReadEndElement();
                //reader.ReadEndElement();
            }

            return allMedia;
        }


        public static List<Media> RetrieveMedia(XDocument doc)
        {
            List<Media> allMedia = new List<Media>();

            foreach(XElement mediaEl in doc.Elements(TAG_MEDIA))
            {
                string sha1Hash = 
                    GetAttributeValueIfExists(mediaEl, ATTRIBUTE_SHA1_HASH);

                string fileName =
                    GetAttributeValueIfExists(mediaEl, ATTRIBUTE_FILE_NAME);

                string description =
                    GetAttributeValueIfExists(mediaEl, ATTRIBUTE_DESCRIPTION);

                Media media = new Media()
                {
                    MediaHash = sha1Hash,
                    MediaDescription = description,
                    MediaFileName = fileName
                };

                //get tags
                foreach(XElement tagEl in mediaEl.Elements(TAG_TAG)){

                    //tagValue
                    //taggedAt
                    //untaggedAt
                    //mediaHash
                    string tagValue = 
                        GetAttributeValueIfExists(tagEl, ATTRIBUTE_TAG_VALUE);

                    string taggedAt =
                        GetAttributeValueIfExists(tagEl, ATTRIBUTE_TAGGED_AT);

                    string untaggedAt =
                        GetAttributeValueIfExists(tagEl, ATTRIBUTE_UNTAGGED_AT);

                    DateTime? taggedAtTime = ToTime(taggedAt);
                    DateTime? untaggedAtTime = ToTime(untaggedAt);

                    MediaTagging tagging = new MediaTagging()
                    {
                        MediaHash = media.MediaHash,
                        MediaTagValue = tagValue
                    };

                    tagging.SetTimeStamps(taggedAtTime, untaggedAtTime);

                    media.Add(tagging);
                }

                //get device paths
                foreach(XElement mediaDeviceEl in mediaEl.Elements(TAG_MEDIA_DEVICE))
                {
                    string deviceName =
                        GetAttributeValueIfExists(mediaDeviceEl, ATTRIBUTE_DESCRIPTION);

                    foreach(XElement pathEl in mediaDeviceEl.Descendants(TAG_PATH))
                    {
                        string pathValue =
                            GetAttributeValueIfExists(pathEl, ATTRIBUTE_VALUE);

                        string verifiedPresent =
                            GetAttributeValueIfExists(pathEl, ATTRIBUTE_VERIFIED_PRESENT);

                        string verifiedMissing =
                            GetAttributeValueIfExists(pathEl, ATTRIBUTE_VERIFIED_MISSING);

                        DateTime? verifiedPresentTime = ToTime(verifiedPresent);
                        DateTime? verifiedMissingTime = ToTime(verifiedMissing);

                        DevicePath dp = new DevicePath()
                        {
                            DeviceName = deviceName,
                            DevicePathValue = pathValue
                        };

                        dp.SetTimeStamps(verifiedPresentTime, verifiedMissingTime);

                        media.Add(dp);
                    }
                }

                allMedia.Add(media);
            }

            return allMedia;
        }

        public static XElement CreateMediaElement(string hash)
        {
            XElement mediaEl = new XElement(TAG_MEDIA);

            mediaEl.Add(new XAttribute(ATTRIBUTE_SHA1_HASH, hash));

            return mediaEl;
        }

        public static XElement CreateTagElement(MediaTagging tag)
        {
            XElement tagEl = new XElement(TAG_TAG);

            //tagValue
            //taggedAt
            //untaggedAt

            string taggedAt =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(tag.TaggedAt);

            string untaggedAt =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(tag.UntaggedAt);

            tagEl.Add(new XAttribute(ATTRIBUTE_TAG_VALUE, tag.MediaTagValue));
            tagEl.Add(new XAttribute(ATTRIBUTE_TAGGED_AT, taggedAt));
            tagEl.Add(new XAttribute(ATTRIBUTE_UNTAGGED_AT, untaggedAt));

            return tagEl;
        }

        public static XElement CreateDeviceElement(string deviceName)
        {
            XElement mediaDeviceEl = new XElement(TAG_MEDIA_DEVICE);

            mediaDeviceEl.Add(new XAttribute(ATTRIBUTE_DESCRIPTION, deviceName));

            return mediaDeviceEl;
        }

        public static XElement CreatePathElement(DevicePath path)
        {
            XElement pathEl = new XElement(TAG_PATH);

            string verifiedPresent =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(path.VerifiedPresent);

            string verifiedMissing =
                TimeStamp.To_UTC_YYYY_MM_DD_HH_MM_SS(path.VerifiedMissing);

            pathEl.Add(new XAttribute(ATTRIBUTE_VALUE, path.DevicePathValue));

            pathEl.Add(
                new XAttribute(ATTRIBUTE_VERIFIED_PRESENT, verifiedPresent));
            
            pathEl.Add(
                new XAttribute(ATTRIBUTE_VERIFIED_MISSING, verifiedMissing));
            
            return pathEl;
        }

        public static XDocument Export(SemanticMap semMap)
        {
            var lst = new List<SemanticMap>();
            lst.Add(semMap);
            return Export(lst);
        }
        public static XDocument Export(IEnumerable<SemanticMap> semanticMaps)
        {
            /*
             * 
             * desired output:
             * 
             * <nwd>
             *   <hierophantSubset>
             *     <semanticMap>
             *       <semanticDefinitions>
             *         <semanticDefinition>
             *           <semanticKey>key as string</semanticKey>
             *           <columns>
             *             <column>
             *               <columnName>column name</columnName>
             *               <columnValue>column value</columnValue>
             *             </column>
             *           </columns>
             *         </semanticDefinition>
             *         <semanticDefinition/>
             *         <semanticDefinition/>
             *       </semanticDefinitions>
             *       <semanticGroups>
             *         <semanticGroup>
             *           <semanticGroupName>name</semanticGroupName>
             *           <semanticKeys>
             *             <semanticKey>just the key as string here</semanticKey>
             *             <semanticKey/>
             *             <semanticKey/>
             *           </semanticKeys>
             *         </semanticGroup>
             *         <semanticGroup/>
             *         <semanticGroup/>
             *       </semanticGroups>
             *     </semanticMap>
             *     <semanticMap/>
             *     <semanticMap/>
             *   </hierophantSubset>
             * </nwd>
             * 
            */
            
            XElement hierophantSubsetEl = new XElement("hierophantSubset");
            
            foreach (var semanticMap in semanticMaps)
            {
                XElement semanticMapEl = new XElement("semanticMap");
                hierophantSubsetEl.Add(semanticMapEl);

                XElement semanticDefinitionsEl =
                    new XElement("semanticDefinitions");

                semanticMapEl.Add(semanticDefinitionsEl);

                foreach (var def in semanticMap.SemanticDefinitions)
                {
                    semanticDefinitionsEl.Add(CreateSemanticDefinitionElement(def));
                }

                XElement semanticGroupsEl = new XElement("semanticGroups");
                semanticMapEl.Add(semanticGroupsEl);

                foreach (var semGroupName in semanticMap.SemanticGroupNames)
                {
                    semanticGroupsEl.Add(
                        CreateSemanticGroupElement(
                            semanticMap.SemanticGroup(semGroupName),
                            semGroupName));
                }
            }

            return new XDocument(new XElement("nwd", hierophantSubsetEl));
        }

        private static XElement CreateSemanticGroupElement(
            SemanticMap semanticMapForGroup, string semanticGroupName)
        {
            XElement semanticKeysEl = new XElement("semanticKeys");

            XElement semanticGroupEl = 
                new XElement("semanticGroup",
                    new XElement("semanticGroupName", semanticGroupName),
                    semanticKeysEl);

            foreach(var semKey in semanticMapForGroup.SemanticKeys)
            {
                semanticKeysEl.Add( 
                    new XElement("semanticKey", semKey.ToString()));
            }

            return semanticGroupEl;
        }

        private static XElement CreateSemanticDefinitionElement(SemanticDefinition def)
        {
            XElement semanticDefinitionEl =
                new XElement("semanticDefinition");
            
            semanticDefinitionEl.Add(
                new XElement("semanticKey", def.SemanticKey.ToString()));
            
            XElement columnsEl = new XElement("columns");
            semanticDefinitionEl.Add(columnsEl);

            foreach(var colName in def.ColumnNames)
            {
                columnsEl.Add(
                    new XElement("column", 
                        new XElement("columnName", colName),
                        new XElement("columnValue", def[colName].ToString())));
            }

            return semanticDefinitionEl;
        }
    }
}
