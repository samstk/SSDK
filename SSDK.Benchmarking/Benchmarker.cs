using SSDK.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSDK.Benchmarking
{
    /// <summary>
    /// Contains static methods that help with benchmarking.
    /// </summary>
    public static class Benchmarker
    {
        /// <summary>
        /// Is true if the benchmark is currently running.
        /// Controlled by Start() and End/Report()
        /// </summary>
        private static bool _IsBenchmarkStarted = false;

        /// <summary>
        /// Gets the time that the test was started.
        /// </summary>
        public static DateTime BenchmarkStarted { get; private set; }

        /// <summary>
        /// Gets the time that the test ran for.
        /// </summary>
        public static TimeSpan BenchmarkTime { get; private set; }

        /// <summary>
        /// Starts the benchmark
        /// </summary>
        public static void Start()
        {
            BenchmarkStarted = DateTime.Now;
            _IsBenchmarkStarted = true;
        }

        /// <summary>
        /// Ends the benchmark test
        /// </summary>
        /// <exception cref="OperationNotStartedException">
        /// occurs when Start was never called (or end was called two times in a row)
        /// </exception>
        public static void End()
        {
            if (!_IsBenchmarkStarted) throw new OperationNotStartedException("benchmark test");
            _IsBenchmarkStarted = false;
            BenchmarkTime = DateTime.Now - BenchmarkStarted;
        }

        /// <summary>
        /// Runs the given action as a benchmark test, so that the time taken is captured.
        /// If report is set to a prefix, then it is reported to console.
        /// </summary>
        /// <param name="action">the action to complete</param>
        /// <param name="report">the report prefix, or null if not reporting</param>
        public static void Do(Action action, string report=null)
        {
            Start();

            // Complete action
            action();

            if (report == null) End();
            else Report(report);
        }

        /// <summary>
        /// Ends the benchmark test and reports the time to console.
        /// </summary>
        /// <param name="prefix">the caption reported behind the e.g. Time: </param>
        /// <exception cref="OperationNotStartedException">
        /// occurs when Start was never called (or end was called two times in a row)
        /// </exception>
        public static void Report(string prefix="Time: ")
        {
            End();
            Console.WriteLine($"{prefix}{BenchmarkTime}");
        }
    }
}
