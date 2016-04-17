using System.Collections.Generic;

namespace NineWorldsDeep.Synergy
{
    public interface IGauntletDbAdapter
    {
        List<ToDoList> GetActiveListItems();
        void SetActive(string listName, bool active);
        List<ToDoList> GetLists(bool active);
        string Save(IListMatrix ilm);
        string Load(IListMatrix ilm);
        string Load(IListMatrix ilm, bool loadAllStatuses);
        void UpdateActiveInactive(IEnumerable<ToDoList> setToActive, IEnumerable<ToDoList> setToInactive);
    }
}