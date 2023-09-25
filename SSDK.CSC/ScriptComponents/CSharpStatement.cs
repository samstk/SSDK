using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    /// <summary>
    /// A c# statement, which may execute or declare a number of different things.
    /// </summary>
    public abstract class CSharpStatement : CSharpComponent
    {
        /// <summary>
        /// Processes the map in the correct corresponding function for this
        /// statement.
        /// </summary>
        /// <param name="map">the conversion map</param>
        /// <param name="result">the string builder result</param>
        public abstract void ProcessMap(CSharpConversionMap map, StringBuilder result);
    }
}
