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

            pathEl.Add(new XAttribute(ATTRIBUTE_VALUE, path.Path));

            pathEl.Add(
                new XAttribute(ATTRIBUTE_VERIFIED_PRESENT, verifiedPresent));
            
            pathEl.Add(
                new XAttribute(ATTRIBUTE_VERIFIED_MISSING, verifiedMissing));
            
            return pathEl;
        }
    }
}
