using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NineWorldsDeep.Synergy
{
    public class ToDoList
    {
        private ObservableCollection<ToDoItem> _items =
            new ObservableCollection<ToDoItem>();

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public List<ToDoItem> Items
        {
            get
            {
                return _items.OrderBy(x => x.Completed).ToList();
            }
        }

        public void UpdateStatus(string item, bool completed, bool archived)
        {
            Parser.Parser p = new Parser.Parser();
            string itemValue = p.Extract("item", item);
            if (string.IsNullOrWhiteSpace(itemValue))
            {
                itemValue = item;
            }

            ToDoItem tdi = new ToDoItem()
            {
                Description = itemValue,
                Completed = completed,
                Archived = archived
            };

            tdi.Fragments.Add(new Fragment(item));

            AddWithMerge(tdi);
        }

        public void AddWithMerge(IEnumerable<ToDoItem> ie)
        {
            AddWithMerge(ie, false);
        }

        public void AddWithMerge(IEnumerable<ToDoItem> ie,
            bool allItems)
        {
            foreach (ToDoItem tdi in ie)
            {
                if (allItems ||
                    !tdi.Statuses.ContainsNonActiveStatuses())
                {
                    AddWithMerge(tdi);
                }

            }
        }

        public void AddWithMerge(ToDoItem item)
        {
            bool found = false;

            //check any existing and just transfer status
            foreach (ToDoItem tdi in _items)
            {
                if (tdi.Description.Equals(item.Description))
                {
                    //archived and completed status should be 
                    //merged if true for either

                    if (item.Archived || item.IsArchived)
                    {
                        tdi.Archived = true;
                    }

                    if (item.Completed || item.IsCompleted)
                    {
                        tdi.Completed = true;
                    }

                    tdi.Statuses.AddWithMerge(item.Statuses);
                    tdi.Fragments.AddWithMerge(item.Fragments);

                    found = true;
                }
            }

            if (!found)
            {
                _items.Add(item);
            }
        }

    }
}