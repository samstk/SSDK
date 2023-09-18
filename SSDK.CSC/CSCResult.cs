using SSDK.CSC.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.CSC
{
    /// <summary>
    /// Represents a conversion result from processing scripts
    /// through the CSCMapping class.
    /// </summary>
    public sealed class CSCResult
    {
        /// <summary>
        /// Gets the mapping that created this result.
        /// </summary>
        public CSCMapping Mapping { get; private set; }

        /// <summary>
        /// Gets the re-sources dictionary, which contains all
        /// output scripts by a key.
        /// </summary>
        public Dictionary<string, string> Resources { get; private set; }
        
        /// <summary>
        /// Gets the ordering that each script should be included in.
        /// </summary>
        public Dictionary<string, int> Ordering { get; private set; }

        /// <summary>
        /// Creates a new CSC result with the given conversions and ordering
        /// </summary>
        internal CSCResult(CSCMapping mapping)
        {
            Mapping = mapping;
        }

        /// <summary>
        /// True if the result is finalised.
        /// </summary>
        public bool Finalised { get; private set; } = false;

        /// <summary>
        /// Finalises the result with the given conversions
        /// </summary>
        /// <param name="resources">the re-sources dictionary containing all output scripts</param>
        /// <param name="ordering">the ordering dictionary which maps a script to a particular index of which it should be included</param>
        /// <exception cref="Exception"></exception>
        public void Finalise(Dictionary<string, StringBuilder> resources, Dictionary<string, int> ordering)
        {
            if (ordering.Count != resources.Count) throw new Exception("No ordering generated for conversion result");
            Resources = new Dictionary<string, string>();
            foreach (string key in resources.Keys)
            {
                Resources.Add(key, resources[key].ToString());
            }
            Ordering = ordering;
        }

        /// <summary>
        /// Joins every script together into a single script in the correct ordering.
        /// Only usable after finalised.
        /// </summary>
        /// <returns>a single source code containing all inputs</returns>
        public string Join()
        {
            if (!Finalised) throw new Exception("Result must be finalised before any conversions can be joined.");

            string output = "";
            
            string[] ordering = new string[Resources.Count];

            // Calculate correct ordering
            foreach(string key in Resources.Keys)
            {
                ordering[Ordering[key]] = key;
            }

            // Convert list to string array
            foreach(string key in ordering)
            {
                // Add reference comment (e.g. /* Console.cs */ )
                output += (Mapping.Comment(this, new CSCComment(new FileInfo(key).Name))) + "\r\n";
                output += Resources[key] + "\r\n";
            }
            return output;
        }
    }
}
