using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Warehouse
{//TODO: LICENSE NOTES
    //inheriting this way allows xml data binding
    //see: https://msdn.microsoft.com/en-us/library/ff407126(v=vs.100).aspx
    public class SyncItemCollection : ObservableCollectionWithItemNotify<SyncItem>
    {

    }
}
