using SSDK.Core;
using SSDK.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core
{
    /// <summary>
    /// A function type for a flag definition, which
    /// returns true if the flag consumes the next argument.
    /// </summary>
    /// <typeparam name="T">the object type to apply the flag to</typeparam>
    /// <param name="arg">
    /// the string argument of the flag, which is automatically set
    /// to the next string argument and not necessarily to be consumed.
    /// </param>
    /// <returns>true if the flag consumes the next argument.</returns>
    public delegate bool FlagDefinition(string arg);

    /// <summary>
    /// The flag helper class which provides a method to import
    /// flags on a specific class.
    /// 
    /// See FlagDefinition for how flags should be defined within a dictionary.
    /// </summary>
    public static class FlagIntepreter
    {
        /// <summary>
        /// Imports the flags on a specific object, using the flag definitions dictionary.
        /// </summary>
        /// <param name="args">the arguments to process</param>
        /// <param name="argMappings">
        /// The mappings from flag names (e.g. -v) to action invocations to process the flag with a given argument.
        /// e.g. -path 'path/' would invoke whatever the program's -path flag definition is, with the argument after it ('path/')
        /// </param>
        /// <param name="endWithUndefinedFlag">
        /// If true, then the function will take an undefined flag and return its indices.
        /// Useful when having a default end function.
        /// </param>
        /// <returns>-1 if all arguments were processed, else the index that the function stopped processing at</returns>
        /// <exception cref="InvalidFlagException">
        /// This exception occurs when endWithUndefinedFlag is false, 
        /// and a flag that did not exist in the mapping was detected.
        /// </exception>
        /// <exception cref="MultipleFlagException">
        /// This exception occurs when accumulateErrors is true,
        /// and multiple errors have occured
        /// </exception>
        public static int ProcessAsArgs(this string[] args, Dictionary<string, FlagDefinition> argMappings, bool endWithUndefinedFlag=true, bool accumulateErrors=false)
        {
            List<Exception> errors = accumulateErrors ? new List<Exception>() : null;

            for (int i = 0; i < args.Length; i++)
            {
                if (!argMappings.ContainsKey(args[i]))
                {
                    if (endWithUndefinedFlag)
                        return i;

                    // Throw exception as flag was not found
                    InvalidFlagException exc = new InvalidFlagException(args[i]);
                    if (accumulateErrors)
                        errors.Add(exc);
                    else throw exc;
                }

                // Invoke function defined by program
                try
                {
                    FlagDefinition def = argMappings[args[i]];
                    if (def(i + 1 < args.Length ? args[i + 1] : null))
                    {
                        // Skip next argument as has been consumed
                        i++;
                    }
                }
                catch (Exception exc)
                {
                    if (accumulateErrors)
                        errors.Add(exc);
                    else throw exc;
                }
            }

            if(accumulateErrors && errors.Count > 0)
            {
                throw new MultipleFlagErrorsException(errors);
            }
            return -1;
        }
    }
}
