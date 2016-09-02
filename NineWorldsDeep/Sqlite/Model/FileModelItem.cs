using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Sqlite.Model
{
    [Obsolete("migrating to new package NineWorldsDeep.Model")]
    public class FileModelItem
    {
        private String mId;
        private String mDevice, mDisplayName, mPath,
                mAudioTranscript, mDescription, mName;
        private List<string> mTags = new List<string>();
        private List<HashModelItem> mHashes = new List<HashModelItem>();

        public FileModelItem(Dictionary<String, String> record)
        {

            if (record.ContainsKey(NwdContract.COLUMN_FILE_ID))
            {

                SetId(record[NwdContract.COLUMN_FILE_ID]);
            }

            if (record.ContainsKey(NwdContract.COLUMN_DEVICE_DESCRIPTION))
            {

                SetDevice(record[NwdContract.COLUMN_DEVICE_DESCRIPTION]);
            }

            if (record.ContainsKey(NwdContract.COLUMN_PATH_VALUE))
            {

                SetPath(record[NwdContract.COLUMN_PATH_VALUE]);
            }

            if (record.ContainsKey(NwdContract.COLUMN_DISPLAY_NAME_VALUE))
            {

                SetDisplayName(record[NwdContract.COLUMN_DISPLAY_NAME_VALUE]);
            }

            String hash = null;
            String hashedAt = null;

            if (record.ContainsKey(NwdContract.COLUMN_HASH_VALUE))
            {

                hash = record[NwdContract.COLUMN_HASH_VALUE];
            }

            if (record.ContainsKey(NwdContract.COLUMN_FILE_HASHED_AT))
            {

                hashedAt = record[NwdContract.COLUMN_FILE_HASHED_AT];
            }

            if (GetId() != null && hash != null)
            {

                GetHashes().Add(new HashModelItem(GetId(), hash, hashedAt));
            }

            if (record.ContainsKey(NwdContract.COLUMN_FILE_DESCRIPTION))
            {

                SetDescription(record[NwdContract.COLUMN_FILE_DESCRIPTION]);
            }

            if (record.ContainsKey(NwdContract.COLUMN_FILE_NAME))
            {

                SetName(record[NwdContract.COLUMN_FILE_NAME]);
            }

            if (record.ContainsKey(NwdContract.COLUMN_AUDIO_TRANSCRIPT_VALUE))
            {

                SetAudioTranscript(record[NwdContract.COLUMN_AUDIO_TRANSCRIPT_VALUE]);
            }
        }

        public FileModelItem(String device, String path)
        {

            SetDevice(device);
            SetPath(path);
        }

        public String GetDescription()
        {
            return mDescription;
        }

        public void SetDescription(String mDescription)
        {
            this.mDescription = mDescription;
        }

        public String GetName()
        {
            return mName;
        }

        public void SetName(String mName)
        {
            this.mName = mName;
        }

        public String GetId()
        {
            return mId;
        }

        public void SetId(String mId)
        {
            this.mId = mId;
        }

        public String GetDevice()
        {
            return mDevice;
        }

        public void SetDevice(String mDevice)
        {
            this.mDevice = mDevice;
        }

        public String GetDisplayName()
        {
            return mDisplayName;
        }

        public void SetDisplayName(String mDisplayName)
        {
            this.mDisplayName = mDisplayName;
        }

        public String GetPath()
        {
            return mPath;
        }

        public void SetPath(String mPath)
        {
            this.mPath = mPath;
        }

        public String GetAudioTranscript()
        {
            return mAudioTranscript;
        }

        public void SetAudioTranscript(String mAudioTranscript)
        {
            this.mAudioTranscript = mAudioTranscript;
        }

        public List<String> GetTags()
        {
            return mTags;
        }

        public List<HashModelItem> GetHashes()
        {
            return mHashes;
        }
    }
}
