using NineWorldsDeep.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Synergy
{
    public class SynergyItem
    {
        private Dictionary<string, string> _keyVals;
        private string _fragment;
        private bool _fragmentReadyForUpdating;
        private bool _dictionaryReadyForUpdating;

        public SynergyItem()
        {
            _keyVals = new Dictionary<string, string>();

            //defer processing of dictionary into fragment string
            _fragmentReadyForUpdating = false;

            //will ensure we don't get null values for the attributes
            Item = "";
            CompletedAt = "";
            ArchivedAt = "";

            _fragmentReadyForUpdating = true;

            _dictionaryReadyForUpdating = false;
            UpdateFragment(); //will set to empty string
            _dictionaryReadyForUpdating = true;
        }

        /// <summary>
        /// ensures the two items are brought into agreement
        /// regarding common boolean properties
        /// </summary>
        /// <param name="si"></param>
        public void True(SynergyItem si)
        {
            if(!GauntletUtils.TrimCategory(Item).Equals(
                GauntletUtils.TrimCategory(si.Item), 
                StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException(
                    "SyncItem.True() failed for items: [" + 
                    Item + "] and [" + si.Item + "]");
            }

            if (string.IsNullOrWhiteSpace(ArchivedAt))
            {
                ArchivedAt = si.ArchivedAt;
            }

            if (string.IsNullOrWhiteSpace(CompletedAt))
            {
                CompletedAt = si.CompletedAt;
            }
        }

        public string Fragment
        {
            get
            {
                return _fragment;
            }

            set
            {
                if (_fragment == null ||
                    !_fragment.Equals(value))
                {
                    _fragment = value;
                    UpdateDictionary();
                }
            }
        }
        
        public string Item
        {
            get
            {
                return _keyVals["item"];
            }

            set
            {
                _keyVals["item"] = value;
                UpdateFragment();
            }
        }

        public string CompletedAt
        {
            get
            {
                return _keyVals["completedAt"];
            }

            set
            {
                _keyVals["completedAt"] = value;
                UpdateFragment();
            }
        }

        public string ArchivedAt
        {
            get
            {
                return _keyVals["archivedAt"];
            }

            set
            {
                _keyVals["archivedAt"] = value;
                UpdateFragment();
            }
        }

        public void CompleteNow()
        {
            CompletedAt = Core.NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
        }

        public void ArchiveNow()
        {
            ArchivedAt = Core.NwdUtils.GetTimeStamp_yyyyMMddHHmmss();
        }

        public void UndoCompletion()
        {
            CompletedAt = "";
        }

        public void UndoArchival()
        {
            ArchivedAt = "";
        }

        private void UpdateFragment()
        {
            if (_fragmentReadyForUpdating)
            {
                string newFragmentString = "";

                foreach(KeyValuePair<string, string> kv in _keyVals)
                {
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                    {
                        newFragmentString += kv.Key + "={" + kv.Value + "} ";
                    }
                }

                _dictionaryReadyForUpdating = false;
                Fragment = newFragmentString.Trim(); //remove trailing space char
                _dictionaryReadyForUpdating = true;
            }
        }

        private void UpdateDictionary()
        {
            if (_dictionaryReadyForUpdating)
            {
                _fragmentReadyForUpdating = false;

                Item = NwdParser.Extract("item", _fragment);
                CompletedAt = NwdParser.Extract("completedAt", _fragment);
                ArchivedAt = NwdParser.Extract("archivedAt", _fragment);

                _fragmentReadyForUpdating = true;
            }
        }

        /// <summary>
        /// fragment updates after every property
        /// change. this method will turn off
        /// updating so multiple properties can be set
        /// at one time without triggering the update
        /// over and over. Be sure to use
        /// TurnOnFragmentUpdatingAndUpdate() when
        /// done or fragment will not update in future.
        /// </summary>
        public void TurnOffFragmentUpdating()
        {
            _fragmentReadyForUpdating = false;
        }

        /// <summary>
        /// used in conjunction with TurnOffFragmentUpdating()
        /// will turn updating back on, and perform an
        /// update so all attributes will propagate to
        /// the fragment value
        /// </summary>
        public void TurnOnFragmentUpdatingAndUpdate()
        {
            _fragmentReadyForUpdating = true;
            UpdateFragment();
        }
    }
}
