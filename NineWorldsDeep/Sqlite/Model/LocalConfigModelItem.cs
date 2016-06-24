using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Sqlite.Model
{
    public class LocalConfigModelItem
    {
        private String mKey, mValue;

        public LocalConfigModelItem(Dictionary<String, String> record)
        {

            if (record.ContainsKey(NwdContract.COLUMN_LOCAL_CONFIG_KEY))
            {

                SetKey(record[NwdContract.COLUMN_LOCAL_CONFIG_KEY]);
            }

            if (record.ContainsKey(NwdContract.COLUMN_LOCAL_CONFIG_VALUE))
            {

                SetValue(record[NwdContract.COLUMN_LOCAL_CONFIG_VALUE]);
            }
        }

        public LocalConfigModelItem(String key, String value)
        {

            SetKey(key);
            SetValue(value);
        }

        public String GetKey()
        {
            return mKey;
        }

        public void SetKey(String mKey)
        {
            this.mKey = mKey;
        }

        public String GetValue()
        {
            return mValue;
        }

        public void SetValue(String mValue)
        {
            this.mValue = mValue;
        }
    }
}
