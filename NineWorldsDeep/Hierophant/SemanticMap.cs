using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    /*
     * SemanticMatrix Mockup With Underlying Model Definitions
     *
     * _____________________
     * |Z|Z|Z|Z|Z|Z|Z|Z| X |
     * |_|_|_|_|_|_|_|_|___|
     * |Z|Z|Z|Z|Z|Z|Z|Z| X |
     * |_|_|_|_|_|_|_|_|___|
     * |Z|Z|Z|Z|Z|Z|Z|Z| X |
     * |_|_|_|_|_|_|_|_|___|
     * |Z|Z|Z|Z|Z|Z|Z|Z|
     * |_|_|_|_|_|_|_|_|
     * |Z|Z|Z|Z|Z|Z|Z|Z|
     * |_|_|_|_|_|_|_|_|
     * |Z|Z|Z|Z|Z|Z|Z|Z|
     * |_|_|_|_|_|_|_|_|
     * | Y | Y | Y | 
     * |___|___|___|
     *
     * Above is a datagrid (Z) within a tab control (Y) within a tab control (X)
     * 
     * The entire ui control is a SemanticMatrix.
     *
     * Our model associates in the following manner:
     *
     * "X" is a SemanticSet (model item), a SemanticSet has one or more SemanticGroups
     *
     * "Y" is a SemanticGroup (model item), a SemanticGroup is 
     *      a named SemanticMap with an underlying commonality
     *
     * "Z" is a SemanticGrid (UI element), a SemanticGrid displays a SemanticMap
     *
     * A SemanticMap is a Dictionary<SemanticKey, Dictionary<string, string>>(), 
     *      it binds a SemanticKey to a Dictionary of Key/Value pairs (eg. "Planet":"Mercury", 
     *      "Alchemical Element":"Sulphur", "Zodiacal Sign":"Aries", &c.)
     *
     */

    public class SemanticMap
    {
    }
}
