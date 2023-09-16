using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS.Symbols
{
    public static class SymbolExt
    {
        /// <summary>
        /// Selects a single symbol given a string id from symbols.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static KBSymbol SelectOne(this IEnumerable<KBSymbol> symbols, string id)
        {
            foreach(KBSymbol symbol in symbols)
            {
                if(symbol.ID == id)
                {
                    return symbol;
                }
            }
            return null;
        }
    }
}
