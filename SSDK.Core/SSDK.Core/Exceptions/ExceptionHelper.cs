using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Core.Exceptions
{
    /// <summary>
    /// Contains helper methods for exception defining.
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Compiles a multi-exception message by preprending the base message,
        /// and then adding all inner exceptions as a new line.
        /// </summary>
        /// <param name="baseMessage">the message to prepend before adding the inner exceptions</param>
        /// <param name="innerExceptions">the inner exceptions whose messages will be added to this message</param>
        /// <returns>a string containing the multi-exception message</returns>
        public static string CompileMultiexceptionMessage(this List<Exception> innerExceptions, string baseMessage)
        {
            // Change message accordingly.
            string msg = baseMessage + ":";

            foreach (Exception exc in innerExceptions)
            {
                msg += "\n" + exc.Message;
            }

            return baseMessage;
        }
    }
}
