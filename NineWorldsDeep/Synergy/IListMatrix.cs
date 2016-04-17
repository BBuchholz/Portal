using System.Collections.Generic;

namespace NineWorldsDeep.Synergy
{
    public interface IListMatrix
    {
        IEnumerable<ToDoList> Lists { get; }
        ToDoList EnsureList(string listName);
        void ClearAll();
    }
}