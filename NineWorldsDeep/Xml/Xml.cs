using NineWorldsDeep.Synergy.V5;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NineWorldsDeep.Xml
{
    public class Xml
    {
        #region "contract"

        //tags -
        private static string TAG_NWD = "nwd";

        private static string TAG_MEDIA_SUBSET = "mediaSubset";
        private static string TAG_MEDIA = "media";

        private static string TAG_SYNERGY_SUBSET = "synergySubset";
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
        /// 
        /// </summary>
        /// <param name="dateString">format: YYYY-MM-DD HH:MM:SS</param>
        /// <returns></returns>
        public static DateTime ToTime(string dateString)
        {
            return DateTime.ParseExact(dateString,
                                      "yyyy-MM-dd HH:mm:ss",
                                      CultureInfo.InvariantCulture);
        }

        public static List<SynergyV5List> RetrieveSynergyV5Lists(XDocument doc)
        {
            List<SynergyV5List> allLists = new List<SynergyV5List>();

            foreach(XElement listEl in doc.Descendants(TAG_SYNERGY_LIST))
            {
                string listName = listEl.Attribute(ATTRIBUTE_LIST_NAME).Value;
                string activatedAt = listEl.Attribute(ATTRIBUTE_ACTIVATED_AT).Value;
                string shelvedAt = listEl.Attribute(ATTRIBUTE_SHELVED_AT).Value;

                DateTime activatedAtTime = ToTime(activatedAt);
                DateTime shelvedAtTime = ToTime(shelvedAt);

                SynergyV5List lst = 
                    new SynergyV5List(listName, 
                                      activatedAtTime, 
                                      shelvedAtTime);

                foreach(XElement itemEl in listEl.Descendants(TAG_SYNERGY_ITEM))
                {
                    string position = itemEl.Attribute(ATTRIBUTE_POSITION).Value;

                    string itemValue = 
                        itemEl.Descendants(TAG_ITEM_VALUE).First().Value;

                    var toDos = itemEl.Descendants(TAG_TO_DO);

                    if(toDos.Count() > 0)
                    {
                        string itemActivatedAt = listEl.Attribute(ATTRIBUTE_ACTIVATED_AT).Value;
                        string completedAt = listEl.Attribute(ATTRIBUTE_COMPLETED_AT).Value;
                        string archivedAt = listEl.Attribute(ATTRIBUTE_ARCHIVED_AT).Value;

                        DateTime itemActivatedAtTime = ToTime(itemActivatedAt);
                        DateTime completedAtTime = ToTime(completedAt);
                        DateTime archivedAtTime = ToTime(archivedAt);

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
    }
}
