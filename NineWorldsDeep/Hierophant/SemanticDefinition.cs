using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class SemanticDefinition
    {
        private Dictionary<string, string> ColumnValues { get; set; }

        public SemanticKey SemanticKey { get; private set; }
        public IEnumerable<string> ColumnNames
        {
            get
            {
                return ColumnValues.Keys;
            }
        }

        public SemanticDefinition(SemanticKey semanticKey)
        {
            SemanticKey = semanticKey;
            ColumnValues = new Dictionary<string, string>();
        }

        public void Add(string columnName, string columnValue)
        {
            this[columnName] = columnValue;           
        }

        public string this[string columnName]
        {
            get
            {
                return ColumnValues[columnName];
            }
            set
            {
                ColumnValues[columnName] = value;
            }
        }

        
    }
}
