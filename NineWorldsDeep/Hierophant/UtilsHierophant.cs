using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineWorldsDeep.Hierophant
{
    public class UtilsHierophant
    {
        public static SemanticDefinition MockDefPlanet(SemanticKey semanticKey, 
            string planet, string alchemy, string astro, string color)
        {
            var def = new SemanticDefinition(semanticKey);

            def.Add("Planet", planet);
            def.Add("Alchemical Element", alchemy);
            def.Add("Astrological Sign", astro);
            def.Add("Color", color);

            return def;
        }

        public static SemanticDefinition MockDefHerb(SemanticKey semanticKey,
            string herb, string stone, string color)
        {
            var def = new SemanticDefinition(semanticKey);

            def.Add("Herb", herb);
            def.Add("Stone", stone);
            def.Add("Color", color);

            return def;
        }

        public static SemanticMap MockMap2()
        {
            var semanticMap = new SemanticMap();
            
            semanticMap.Add(MockDefPlanet(new SemanticKey("testKey2"), "Venus", "Salt", "Libra", "Yellow"));
            semanticMap.Add(MockDefHerb(new SemanticKey("testKey3"), "Aconite", "Obsidian", "Black"));

            return semanticMap;
        }
        
        public static SemanticMap MockMap1()
        {
            var semanticMap = new SemanticMap();

            semanticMap.Add(MockDefPlanet(new SemanticKey("testKey1"), "Mercury", "Sulphur", "Aries", "Silver"));
            semanticMap.Add(MockDefHerb(new SemanticKey("testKey4"), "Cannabis", "Emerald", "Green"));

            return semanticMap;
        }
    }
}
