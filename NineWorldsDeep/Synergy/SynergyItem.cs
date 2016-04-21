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
    }
}
