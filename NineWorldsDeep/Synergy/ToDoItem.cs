using System.Collections.Generic;

namespace NineWorldsDeep.Synergy
{
    public class ToDoItem
    {
        private List<Status> _statuses =
            new List<Status>();
        private List<Fragment> _fragments =
            new List<Fragment>();
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

        public List<Fragment> Fragments { get { return _fragments; } }
    }
}