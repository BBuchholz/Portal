using System;
using System.Collections.Generic;
using System.Linq;

namespace NineWorldsDeep.Synergy
{
    public class ToDoItem
    {
        private List<Status> _statuses =
            new List<Status>();
        private Fragment _fragment = null;
        private bool _completed = false;
        private bool _archived = false;

        //[Obsolete("use ItemValue")]
        public string Description
        {
            get
            {
                return ItemValue;
            }
            set
            {
                ItemValue = value;
            }
        }

        //[Obsolete("use IsComleted")]
        public bool Completed
        {
            get
            {
                return _completed || IsCompleted;
            }

            set
            {
                _completed = value;
            }
        }

        //[Obsolete("use IsArchived")]
        public bool Archived
        {
            get
            {
                return _archived || IsArchived;
            }

            set
            {
                _archived = value;
            }
        }

        public void Add(IEnumerable<Fragment> fragments)
        {
            foreach(Fragment f in fragments)
            {
                Add(f);
            }
        }

        public void Add(Fragment fragment)
        {
            if(_fragment == null)
            {
                _fragment = fragment;
            }
            else
            {
                _fragment.Merge(fragment);
            }
        }

        public int ListId { get; internal set; }
        public int ItemId { get; internal set; }
        public string ItemValue { get; set; }

        public List<Status> Statuses
        {
            get { return _statuses; }
        }

        public bool IsCompleted
        {
            get
            {
                return Statuses.ContainsStatus("Completed");
            }
        }

        public bool IsArchived
        {
            get
            {
                return Statuses.ContainsStatus("Archived");
            }
        }

        public Fragment Fragment { get { return _fragment; } }

        public string FragmentString
        {
            get
            {
                return Fragment.FragmentValue;
            }
        }
    }
}