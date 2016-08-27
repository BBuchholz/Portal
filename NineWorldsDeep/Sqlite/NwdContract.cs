using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Sqlite
{
    public class NwdContract
    {
        //tables
        public static string TABLE_DISPLAY_NAME = "DisplayName";
        public static string TABLE_PATH = "Path";
        public static string TABLE_HASH = "Hash";
        public static string TABLE_DEVICE = "Device";
        public static string TABLE_FILE = "File";
        public static string TABLE_TAG = "Tag";
        public static string TABLE_FILE_TAG = "FileTag";
        public static string TABLE_AUDIO_TRANSCRIPT = "AudioTranscript";
        public static string TABLE_LOCAL_CONFIG = "LocalConfig";
        public static string TABLE_JUNCTION_FILE_TAG = "junction_File_Tag";
        public static string TABLE_JUNCTION_DEVICE_SYNC_PROFILE = "junction_Device_SyncProfile";
        public static string TABLE_LIST = "List";
        public static string TABLE_ITEM = "Item";
        public static string TABLE_FRAGMENT = "Fragment";
        public static string TABLE_SYNC_ACTION = "SyncAction";
        public static string TABLE_SYNC_DIRECTION = "SyncDirection";
        public static string TABLE_SYNC_PROFILE = "SyncProfile";
        public static string TABLE_SYNC_MAP = "SyncMap";
        public static string TABLE_FILE_HASH = "FileHash";
        public static string TABLE_SYNERGY_LIST = "SynergyList";
        public static string TABLE_SYNERGY_LIST_ITEM = "SynergyListItem";
        public static string TABLE_SYNERGY_ITEM = "SynergyItem";
        public static string TABLE_SYNERGY_TO_DO = "SynergyToDo";

        //public static string TABLE_ = "";
        //public static string TABLE_ = "";

        ////columns
        //public static string COLUMN_ = "";
        //public static string COLUMN_ = "";
        public static string COLUMN_SYNERGY_TO_DO_UPDATED_AT = "SynergyToDoUpdatedAt";
        public static string COLUMN_SYNERGY_LIST_NAME = "SynergyListName";
        public static string COLUMN_SYNERGY_ITEM_VALUE = "SynergyItemValue";
        public static string COLUMN_SYNERGY_TO_DO_COMPLETED_AT = "SynergyToDoCompletedAt";
        public static string COLUMN_SYNERGY_TO_DO_ARCHIVED_AT = "SynergyToDoArchivedAt";
        public static string COLUMN_SYNERGY_LIST_ID = "SynergyListId";
        public static string COLUMN_SYNERGY_ITEM_ID = "SynergyItemId";
        public static string COLUMN_SYNERGY_LIST_ITEM_ID = "SynergyListItemId";
        public static string COLUMN_SYNERGY_LIST_ACTIVATED_AT = "SynergyListActivatedAt";
        public static string COLUMN_SYNERGY_LIST_SHELVED_AT = "SynergyListShelvedAt";
        public static string COLUMN_SYNC_ACTION_ID_DEFAULT = "SyncActionIdDefault";
        public static string COLUMN_PATH_ID_SOURCE = "PathIdSource";
        public static string COLUMN_PATH_ID_DESTINATION = "PathIdDestination";
        public static string COLUMN_DEVICE_ID_EXT = "DeviceIdExt";
        public static string COLUMN_DEVICE_ID_HOST = "DeviceIdHost";
        public static string COLUMN_SYNC_ACTION_ID = "SyncActionId";
        public static string COLUMN_SYNC_PROFILE_ID = "SyncProfileId";
        public static string COLUMN_SYNC_PROFILE_NAME = "SyncProfileName";
        public static string COLUMN_SYNC_DIRECTION_ID = "SyncDirectionId";
        public static string COLUMN_SYNC_DIRECTION_VALUE = "SyncDirectionValue";
        public static string COLUMN_SYNC_ACTION_VALUE = "SyncActionValue";
        public static string COLUMN_UPDATED_AT = "UpdatedAt";
        public static string COLUMN_FRAGMENT_VALUE = "FragmentValue";
        public static string COLUMN_ARCHIVED_AT = "ArchivedAt";
        public static string COLUMN_COMPLETED_AT = "CompletedAt";
        public static string COLUMN_DISPLAY_NAME_ID = "DisplayNameId";
        public static string COLUMN_DISPLAY_NAME_VALUE = "DisplayNameValue";
        public static string COLUMN_PATH_ID = "PathId";
        public static string COLUMN_PATH_VALUE = "PathValue";
        public static string COLUMN_HASH_ID = "HashId";
        public static string COLUMN_HASH_VALUE = "HashValue";
        public static string COLUMN_DEVICE_ID = "DeviceId";
        public static string COLUMN_DEVICE_DESCRIPTION = "DeviceDescription";
        public static string COLUMN_DEVICE_FRIENDLY_NAME = "DeviceFriendlyName";
        public static string COLUMN_DEVICE_MODEL = "DeviceModel";
        public static string COLUMN_DEVICE_TYPE = "DeviceType";
        public static string COLUMN_FILE_ID = "FileId";
        public static string COLUMN_FILE_HASHED_AT = "FileHashedAt";
        public static string COLUMN_TAG_ID = "TagId";
        public static string COLUMN_TAG_VALUE = "TagValue";
        public static string COLUMN_FILE_TAGS_ID = "FileTagsId";
        public static string COLUMN_FILE_DESCRIPTION = "FileDescription";
        public static string COLUMN_AUDIO_TRANSCRIPT_ID = "AudioTranscriptId";
        public static string COLUMN_AUDIO_TRANSCRIPT_VALUE = "AudioTranscriptValue";
        public static string COLUMN_FILE_NAME = "FileName";
        public static string COLUMN_LOCAL_CONFIG_ID = "LocalConfigId";
        public static string COLUMN_LOCAL_CONFIG_KEY = "LocalConfigKey";
        public static string COLUMN_LOCAL_CONFIG_VALUE = "LocalConfigValue";
        public static string COLUMN_LIST_ID = "ListId";
        public static string COLUMN_LIST_NAME = "ListName";
        public static string COLUMN_ITEM_ID = "ItemId";
        public static string COLUMN_ITEM_VALUE = "ItemValue";
        public static string COLUMN_LIST_ACTIVE = "ListActive";
    }
}
