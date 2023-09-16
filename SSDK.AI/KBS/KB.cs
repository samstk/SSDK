using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.AI.KBS
{
    /// <summary>
    /// Represents the knowledge base
    /// </summary>
    public sealed class KB
    {

        /// <summary>
        /// The next symbol id that will be returned on GetNextSymbolID
        /// </summary>
        private int _NextSymbolID = 0;

        /// <summary>
        /// Gets the next symbol ID, and increments 1 each time this function is called.
        /// </summary>
        /// <returns>the next symbol id</returns>
        public int GetNextSymbolID()
        {
            return _NextSymbolID++;
        }

        public KBSymbol Is { get; private set; }

        public KB()
        {
            Is = new KBSymbol(this, "is");
        }

        /// <summary>
        /// A list of assertions that should definitely hold true regardless of the world state.
        /// </summary>
        public List<KBFactor> Assertions { get; private set; } = new List<KBFactor>();

        /// <summary>
        /// If true, the KB has been solved at least once.
        /// </summary>
        public bool Solved { get; private set; } = false;

        /// <summary>
        /// Returns the first conflict within the assertions found.
        /// Only applies to solved assertions.
        /// </summary>
        /// <returns></returns>
        public (KBFactor, KBFactor, Exception) HasConflict()
        {
            if(!Solved)
            {
                Solve();
            }
            for (int i = 0; i < Assertions.Count; i++)
            {
                HashSet<KBSymbol> syms = Assertions[i].GetSymbols();
                bool allSolved = syms.All((sym) => sym.Solved);
                List<KBFactor> children = Assertions[i].GetChildren();
                foreach(KBFactor factor in children)
                {
                   
                    
                    if (factor.Solved && factor.HasConflict() || allSolved && !factor.Solved)
                    {
                        string symbols = "";
                        foreach(KBSymbol sym in Assertions[i].GetSymbols())
                        {
                            if (symbols.Length > 0)
                                symbols += ", ";
                            if (!sym.Solved)
                                symbols += sym.ToString(false, false) + " is unsolvable";
                            else
                                symbols += sym.ToStringWithProperties(false);
                        }
                        return (Assertions[i], factor, new Exception($"{factor} in assertion {Assertions[i]} does not hold true where {symbols}, indicating a conflict with the assertions."));
                    }
                }

                if (Assertions[i].HasConflict())
                {
                    string symbols = "";
                    foreach (KBSymbol sym in Assertions[i].GetSymbols())
                    {
                        if (symbols.Length > 0)
                            symbols += ", ";
                        if (!sym.Solved)
                            symbols += sym.ToString(false, false) + " is unsolvable";
                        else
                        symbols += sym.ToStringWithProperties(false);
                    }
                    return (Assertions[i], Assertions[i], new Exception($"Assertion {Assertions[i]} does not hold true where {symbols}, indicating a conflict with the assertions."));
                }
            }
            return (null, null, null);
        }

        /// <summary>
        /// The dictionary of existing symbols, that contains all symbols ever created.
        /// </summary>
        public Dictionary<string, KBSymbol> ExistingSymbols { get; internal set; } = new Dictionary<string, KBSymbol>();

        /// <summary>
        /// Gets the symbol from the existing symbols dictionary using the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public KBSymbol GetSymbol(string id)
        {
            if (ExistingSymbols.ContainsKey(id))
                return ExistingSymbols[id];
            return null;
        }
        /// <summary>
        /// Solves the current knowledge base's assertions.
        /// You will need to assert the necessary information for it to solve.
        /// e.g. an undefined symbol (symbol that appears but is not asserted to be either true or false),
        ///      cannot be used to solve any other symbol.
        /// </summary>
        public void Solve()
        {
            Solved = false;
            // Unset assertions except direct assertions
            Assertions.ForEach((sentence) =>
            {
                sentence.ResetSolution();
            });
            foreach(KBSymbol symbol in ExistingSymbols.Values)
            {
                symbol.ResetSolution();
            }

            // Simplify all assertions (only removes invalid and/ors)
            for (int i = 0; i<Assertions.Count; i++)
            {
                Assertions[i] = Assertions[i].Simplify();
            }
            

            int changes = -1;
            while (changes != 0)
            {
                changes = 0;
                Assertions.ForEach((sentence) =>
                {
                    changes += sentence.SolveAssertion(this, null);
                });
            }

            // Simplify all assertions (removes unnecessary relations)
            for (int i = 0; i < Assertions.Count; i++)
            {
                Assertions[i] = Assertions[i].Simplify();
            }
            Solved = true;
        }

        /// <summary>
        /// Gets a list of symbols mentioned in the assertions.
        /// </summary>
        /// <returns></returns>
        public HashSet<KBSymbol> GetSymbols()
        {
            HashSet<KBSymbol> symbols = new HashSet<KBSymbol>();
            foreach(HashSet<KBSymbol> symbolList in Assertions.Select((sentence) => sentence.GetSymbols()))
            {
                foreach(KBSymbol symbol in symbolList)
                {
                    symbols.Add(symbol);
                }
            }
            return symbols;
        }

        public override string ToString()
        {
            return ToString(true, true);
        }

        public string ToString(bool includeSolution=true, bool showSolvedOnly=true)
        {
            StringBuilder builder = new StringBuilder();
            HashSet<KBSymbol> symbols = GetSymbols();

            // Create symbol list
            builder.Append("[SYMBOLS: ");
            if (symbols.Count > 0)
            {
                bool firstSymbol = true;
                foreach (KBSymbol symbol in symbols)
                {
                    if (symbol.IsRelationalSymbol || showSolvedOnly && !symbol.Solved) continue;

                    if (firstSymbol)
                    {
                        builder.Append(!includeSolution ? symbol.ToString() : symbol.ToStringWithProperties());
                        firstSymbol = false;
                    }
                    else
                    {
                        builder.Append(", " + (!includeSolution ? symbol.ToString() : symbol.ToStringWithProperties()));
                    }
                    if (includeSolution && symbol.Relations.Count > 0)
                    {
                        builder.Append(" [");
                        bool added = false;
                        foreach (KBWrappedSymbol cl in symbol.Relations)
                        {
                            if (added)
                            {
                                builder.Append(", ");
                            }
                            builder.Append(cl);
                            added = true;
                        }
                        builder.Append("]");
                    }
                }

            }
            builder.Append("]\n");
            builder.Append("[ASSERTIONS]\n");
            Assertions.ForEach((sentence) =>
            {
                builder.AppendLine(sentence.ToString());
            });
            return builder.ToString();
        }
    }
}
