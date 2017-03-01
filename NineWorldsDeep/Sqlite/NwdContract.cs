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
        public static string TABLE_SYNC_PROFILE_DEVICE = "SyncProfileDevice";
        public static string TABLE_FILE_HASH = "FileHash";
        public static string TABLE_SYNERGY_LIST = "SynergyList";
        public static string TABLE_SYNERGY_LIST_ITEM = "SynergyListItem";
        public static string TABLE_SYNERGY_ITEM = "SynergyItem";
        public static string TABLE_SYNERGY_TO_DO = "SynergyToDo";

        public static string TABLE_CHORD_PROGRESSION = "ChordProgression";
                
        public static string TABLE_MEDIA_DEVICE = "MediaDevice";
        public static string TABLE_MEDIA_DEVICE_PATH = "MediaDevicePath";
        public static string TABLE_MEDIA_ROOT = "MediaRoot";
        public static string TABLE_MEDIA_PATH = "MediaPath";
        public static string TABLE_MEDIA = "Media";
        public static string TABLE_MEDIA_TAG = "MediaTag";
        public static string TABLE_MEDIA_TAGGING = "MediaTagging";

        //public static string TABLE_ = "";
        //public static string TABLE_ = "";

        ////columns
        //public static string COLUMN_ = "";
        //public static string COLUMN_ = "";

        #region "Synergy V5"

        public static string COLUMN_SYNERGY_LIST_ID = "SynergyListId";
        public static string COLUMN_SYNERGY_LIST_NAME = "SynergyListName";
        public static string COLUMN_SYNERGY_LIST_ACTIVATED_AT = "SynergyListActivatedAt";
        public static string COLUMN_SYNERGY_LIST_SHELVED_AT = "SynergyListShelvedAt";
        public static string COLUMN_SYNERGY_LIST_CREATED_AT = "SynergyListCreatedAt";
        public static string COLUMN_SYNERGY_LIST_UPDATED_AT = "SynergyListUpdatedAt";

        public static string COLUMN_SYNERGY_LIST_ITEM_ID = "SynergyListItemId";
        public static string COLUMN_SYNERGY_LIST_ITEM_CREATED_AT = "SynergyListItemCreatedAt";
        public static string COLUMN_SYNERGY_LIST_ITEM_UPDATED_AT = "SynergyListItemUpdatedAt";
        public static string COLUMN_SYNERGY_LIST_ITEM_POSITION = "SynergyListItemPosition";

        public static string COLUMN_SYNERGY_ITEM_ID = "SynergyItemId";
        public static string COLUMN_SYNERGY_ITEM_VALUE = "SynergyItemValue";
        public static string COLUMN_SYNERGY_ITEM_CREATED_AT = "SynergyItemCreatedAt";
        public static string COLUMN_SYNERGY_ITEM_UPDATED_AT = "SynergyItemUpdatedAt";

        public static string COLUMN_SYNERGY_TO_DO_ID = "SynergyToDoId";
        public static string COLUMN_SYNERGY_TO_DO_ACTIVATED_AT = "SynergyToDoActivatedAt";
        public static string COLUMN_SYNERGY_TO_DO_COMPLETED_AT = "SynergyToDoCompletedAt";
        public static string COLUMN_SYNERGY_TO_DO_ARCHIVED_AT = "SynergyToDoArchivedAt";
        public static string COLUMN_SYNERGY_TO_DO_CREATED_AT = "SynergyToDoCreatedAt";
        public static string COLUMN_SYNERGY_TO_DO_UPDATED_AT = "SynergyToDoUpdatedAt";

        #endregion

        public static string COLUMN_CHORD_PROGRESSION_ID = "ChordProgressionId";
        public static string COLUMN_CHORD_PROGRESSION_SIGNATURE = "ChordProgressionSignature";
        public static string COLUMN_CHORD_PROGRESSION_NOTES = "ChordProgressionNotes";

        public static string COLUMN_MEDIA_ROOT_ID = "MediaRootId";
        public static string COLUMN_MEDIA_ROOT_PATH = "MediaRootPath";
        public static string COLUMN_MEDIA_DEVICE_ID = "MediaDeviceId";
        public static string COLUMN_MEDIA_DEVICE_DESCRIPTION = "MediaDeviceDescription";
        public static string COLUMN_MEDIA_PATH_VALUE = "MediaPathValue";
        public static string COLUMN_MEDIA_FILE_NAME = "MediaFileName";
        public static string COLUMN_MEDIA_DEVICE_PATH_ID = "MediaDevicePathId";
        public static string COLUMN_MEDIA_ID = "MediaId";
        public static string COLUMN_MEDIA_PATH_ID = "MediaPathId";
        public static string COLUMN_MEDIA_HASH = "MediaHash";
        public static string COLUMN_MEDIA_TAG_ID = "MediaTagId";
        public static string COLUMN_MEDIA_TAG_VALUE = "MediaTagValue";
        public static string COLUMN_MEDIA_DESCRIPTION = "MediaDescription";
        public static string COLUMN_MEDIA_TAGGING_ID = "MediaTaggingId";
        public static string COLUMN_MEDIA_TAGGING_TAGGED_AT = "MediaTaggingTaggedAt";
        public static string COLUMN_MEDIA_TAGGING_UNTAGGED_AT = "MediaTaggingUntaggedAt";
        public static string COLUMN_MEDIA_DEVICE_PATH_VERIFIED_PRESENT = "MediaDevicePathVerifiedPresent";
        public static string COLUMN_MEDIA_DEVICE_PATH_VERIFIED_MISSING = "MediaDevicePathVerifiedMissing";

        //public static string COLUMN_SYNERGY_TO_DO_UPDATED_AT = "SynergyToDoUpdatedAt";
        //public static string COLUMN_SYNERGY_LIST_NAME = "SynergyListName";
        //public static string COLUMN_SYNERGY_ITEM_VALUE = "SynergyItemValue";
        //public static string COLUMN_SYNERGY_TO_DO_COMPLETED_AT = "SynergyToDoCompletedAt";
        //public static string COLUMN_SYNERGY_TO_DO_ARCHIVED_AT = "SynergyToDoArchivedAt";
        //public static string COLUMN_SYNERGY_LIST_ID = "SynergyListId";
        //public static string COLUMN_SYNERGY_ITEM_ID = "SynergyItemId";
        //public static string COLUMN_SYNERGY_LIST_ITEM_ID = "SynergyListItemId";
        //public static string COLUMN_SYNERGY_LIST_ACTIVATED_AT = "SynergyListActivatedAt";
        //public static string COLUMN_SYNERGY_LIST_SHELVED_AT = "SynergyListShelvedAt";
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


        #region "Synergy V5"

        public static string SYNERGY_V5_SELECT_ACTIVE_LISTS =

            "SELECT " + COLUMN_SYNERGY_LIST_NAME + " "
            + "FROM " + TABLE_SYNERGY_LIST + " "
            + "WHERE " + COLUMN_SYNERGY_LIST_SHELVED_AT + " IS NULL "
            + "   OR " + COLUMN_SYNERGY_LIST_ACTIVATED_AT + " >= " + COLUMN_SYNERGY_LIST_SHELVED_AT + " "
            + "ORDER BY " + COLUMN_SYNERGY_LIST_NAME + "; ";

        public static string SYNERGY_V5_SELECT_SHELVED_LISTS =

            "SELECT " + COLUMN_SYNERGY_LIST_NAME + " "
            + "FROM " + TABLE_SYNERGY_LIST + " "
            + "WHERE " + COLUMN_SYNERGY_LIST_ACTIVATED_AT + " < " + COLUMN_SYNERGY_LIST_SHELVED_AT + " "
            + "ORDER BY " + COLUMN_SYNERGY_LIST_NAME + "; ";

        internal static readonly string INSERT_MEDIA_HASH_X =

            "INSERT OR IGNORE INTO " + TABLE_MEDIA + " " +
            "	(" + COLUMN_MEDIA_HASH + ") " +
            "VALUES " +
            "	(?); ";

        internal static readonly string SELECT_MEDIA_FOR_HASH_X =

            "SELECT " + COLUMN_MEDIA_ID + ",  " +
            "	    " + COLUMN_MEDIA_FILE_NAME + ",  " +
            "	    " + COLUMN_MEDIA_DESCRIPTION + ",  " +
            "	    " + COLUMN_MEDIA_HASH + " " +
            "FROM " + TABLE_MEDIA + " " +
            "WHERE " + COLUMN_MEDIA_HASH + " = ? ";

        internal static readonly string SELECT_MEDIA_ID_FOR_HASH_X =

            "SELECT " + COLUMN_MEDIA_ID + " " +
            "FROM " + TABLE_MEDIA + " " +
            "WHERE " + COLUMN_MEDIA_HASH + " = ?;";

        internal static readonly string UPDATE_HASH_FOR_MEDIA_ID_X_Y =

            "UPDATE " + TABLE_MEDIA + " " +
            "SET " + COLUMN_MEDIA_HASH + " = ? " +
            "WHERE " + COLUMN_MEDIA_ID + " = ? ";

        internal static readonly string UPDATE_MEDIA_FILE_DESC_FOR_HASH_X_Y_Z =

            "UPDATE " + TABLE_MEDIA + "  " +
            "SET " + COLUMN_MEDIA_FILE_NAME + " = ?, " +
            "	 " + COLUMN_MEDIA_DESCRIPTION + " = ? " +
            "WHERE " + COLUMN_MEDIA_HASH + " = ?  ";

        internal static readonly string INSERT_MEDIA_DEVICE_PATH_MID_DID_PID =

            "INSERT OR IGNORE INTO " + TABLE_MEDIA_DEVICE_PATH + " " +
            "	(" + COLUMN_MEDIA_ID + ", " + COLUMN_MEDIA_DEVICE_ID + ", " + COLUMN_MEDIA_PATH_ID + ") " +
            "VALUES " +
            "	(?, ?, ?); ";
        
        internal static string INSERT_MEDIA_PATH_X =

            "INSERT OR IGNORE INTO " + NwdContract.TABLE_MEDIA_PATH + " " +
            "	(" + NwdContract.COLUMN_MEDIA_PATH_VALUE + ") " +
            "VALUES " +
            "	(?); ";

        internal static string SELECT_MEDIA_PATH_ID_FOR_PATH_X =

            "SELECT " + COLUMN_MEDIA_PATH_ID + " " +
            "FROM " + TABLE_MEDIA_PATH + " " +
            "WHERE " + COLUMN_MEDIA_PATH_VALUE + " = ?;";

        internal static readonly string SELECT_MEDIA_TAG_ID_VALUE =

            "SELECT " + COLUMN_MEDIA_TAG_ID + ", " + COLUMN_MEDIA_TAG_VALUE + " " +
            "FROM " + TABLE_MEDIA_TAG + ";";

        internal static readonly string INSERT_MEDIA_TAG_X =

            "INSERT OR IGNORE INTO " + TABLE_MEDIA_TAG + " " +
            "	(" + COLUMN_MEDIA_TAG_VALUE + ") " +
            "VALUES " +
            "	(?); ";

        internal static readonly string SELECT_MEDIA_TAG_ID_FOR_VALUE_X =

            "SELECT " + COLUMN_MEDIA_TAG_ID + "  " +
            "FROM " + TABLE_MEDIA_TAG + " " +
            "WHERE " + COLUMN_MEDIA_TAG_VALUE + " = ?; ";

        internal static readonly string 
            SELECT_MEDIA_WHERE_HASH_NOT_NULL_OR_WHITESPACE =

            "SELECT " + COLUMN_MEDIA_ID + ",  " +
            "	   " + COLUMN_MEDIA_FILE_NAME + ",  " +
            "	   " + COLUMN_MEDIA_DESCRIPTION + ",  " +
            "	   " + COLUMN_MEDIA_HASH + " " +
            "FROM " + TABLE_MEDIA + " " +
            "WHERE " + COLUMN_MEDIA_HASH + " IS NOT NULL AND trim(" + COLUMN_MEDIA_HASH + ", ' ') != ''; ";

        internal static readonly string INSERT_OR_IGNORE_MEDIA_TAGGING_X_Y =

            "INSERT OR IGNORE INTO " + TABLE_MEDIA_TAGGING + " " +
            "	(" + COLUMN_MEDIA_ID + ", " + COLUMN_MEDIA_TAG_ID + ") " +
            "VALUES " +
            "	(?, ?); ";

        internal static readonly string SELECT_TAGS_FOR_HASH_X =

            "SELECT mt." + COLUMN_MEDIA_TAG_ID + ", " +
            "	   mt." + COLUMN_MEDIA_TAG_VALUE + ", " +
            "	   mtg." + COLUMN_MEDIA_TAGGING_ID + ", " +
            "	   mtg." + COLUMN_MEDIA_ID + ", " +
            "	   mtg." + COLUMN_MEDIA_TAGGING_TAGGED_AT + ", " +
            "	   mtg." + COLUMN_MEDIA_TAGGING_UNTAGGED_AT + ", " +
            "	   m." + COLUMN_MEDIA_HASH + " " +
            "FROM " + TABLE_MEDIA_TAG + " mt " +
            "JOIN " + TABLE_MEDIA_TAGGING + " mtg " +
            "ON mt." + COLUMN_MEDIA_TAG_ID + " = mtg." + COLUMN_MEDIA_TAG_ID + " " +
            "JOIN " + TABLE_MEDIA + " m " +
            "ON m." + COLUMN_MEDIA_ID + " = mtg." + COLUMN_MEDIA_ID + " " +
            "WHERE m." + COLUMN_MEDIA_HASH + " = ? " +
            "AND (mtg." + COLUMN_MEDIA_TAGGING_UNTAGGED_AT + " IS NULL " +
            "   OR mtg." + COLUMN_MEDIA_TAGGING_UNTAGGED_AT + " <= mtg." + COLUMN_MEDIA_TAGGING_TAGGED_AT + "); ";

        internal static readonly string UPDATE_MEDIA_TAGGING_TAGGED_UNTAGGED_WHERE_MEDIA_ID_AND_TAG_ID_W_X_Y_Z =

            "UPDATE " + TABLE_MEDIA_TAGGING + " " +
            "SET " + COLUMN_MEDIA_TAGGING_TAGGED_AT + " = MAX(IFNULL(" + COLUMN_MEDIA_TAGGING_TAGGED_AT + ", ''), ?), " +
            "	" + COLUMN_MEDIA_TAGGING_UNTAGGED_AT + " = MAX(IFNULL(" + COLUMN_MEDIA_TAGGING_UNTAGGED_AT + ", ''), ?) " +
            "WHERE " + COLUMN_MEDIA_ID + " = ? AND " + COLUMN_MEDIA_TAG_ID + " = ?; ";

        internal static readonly string SELECT_DEVICE_PATHS_FOR_HASH_X =

            "SELECT mp." + COLUMN_MEDIA_PATH_VALUE + ", " +
            "       md." + COLUMN_MEDIA_DEVICE_DESCRIPTION + ", " +
            "       mdp." + COLUMN_MEDIA_DEVICE_PATH_ID + ", " +
            "       mdp." + COLUMN_MEDIA_ID + ", " +
            "       mdp." + COLUMN_MEDIA_DEVICE_ID + ", " +
            "       mdp." + COLUMN_MEDIA_PATH_ID + ", " +
            "       mdp." + COLUMN_MEDIA_DEVICE_PATH_VERIFIED_PRESENT + ", " +
            "       mdp." + COLUMN_MEDIA_DEVICE_PATH_VERIFIED_MISSING + " " +
            "FROM " + TABLE_MEDIA + " m " +
            "JOIN " + TABLE_MEDIA_DEVICE_PATH + " mdp " +
            "ON m." + COLUMN_MEDIA_ID + " = mdp." + COLUMN_MEDIA_ID + " " +
            "JOIN " + TABLE_MEDIA_DEVICE + " md " +
            "ON mdp." + COLUMN_MEDIA_DEVICE_ID + " = md." + COLUMN_MEDIA_DEVICE_ID + " " +
            "JOIN " + TABLE_MEDIA_PATH + " mp " +
            "ON mp." + COLUMN_MEDIA_PATH_ID + " = mdp." + COLUMN_MEDIA_PATH_ID + " " +
            "WHERE m." + COLUMN_MEDIA_HASH + " = ?; ";

        #endregion
    }
}
