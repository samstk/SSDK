using Microsoft.CodeAnalysis;
using SSDK.CSC.ScriptComponents.Trivia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SSDK.CSC.ScriptComponents
{
    public abstract class CSharpComponent
    {
        /// <summary>
        /// Gets the symbol that represents this component.
        /// </summary>
        /// <remarks>
        /// ResolveMembers must be called on the project to be set.
        /// </remarks>
        public CSharpMemberSymbol Symbol { get; internal set; }
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
        
        /// <summary>
        /// Creates a member symbol for this component
        /// </summary>
        internal abstract void CreateMemberSymbols(CSharpProject project, CSharpMemberSymbol parentSymbol);

        /// <summary>
        /// Resolves all member references of this component.
        /// </summary>
        /// <param name="project">the project to resolve under</param>
        internal abstract void ResolveMembers(CSharpProject project);

        /// <summary>
        /// Gets the resulting type of the component's operation.
        /// </summary>
        /// <param name="project">the project to report under</param>
        /// <returns>the c# type resulting from the operatioin</returns>
        internal virtual CSharpType GetComponentType(CSharpProject project)
        {
            return null;
        }
    }
}
