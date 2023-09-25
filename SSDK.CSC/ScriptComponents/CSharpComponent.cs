using SSDK.CSC.ScriptComponents.Trivia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC.ScriptComponents
{
    public abstract class CSharpComponent
    {
        /// <summary>
        /// Gets the trivia comment laid before a component.
        /// At this point in time, no trivia syntax reading is supported.
        /// </summary>
        public CSharpComment TriviaComment { get; internal set; }

        /// <summary>
        /// Gets the trivia documentation laid before a component.
        /// At this point in time, no trivia syntax reading is supported.
        /// </summary>
        public CSharpDoc TriviaDoc { get; internal set; }
    }
}
