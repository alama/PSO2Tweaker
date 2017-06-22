using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ArksLayer.Tweaker.Abstractions
{
    /// <summary>
    /// Extension methods for interacting with ITrigger.
    /// </summary>
    public static class TriggerExtensions
    {
        /// <summary>
        /// Logs a process benchmark with a given name. Restarts the stopwatch after logging.
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="benchmark"></param>
        /// <param name="name"></param>
        public static void Benchmark(this ITrigger trigger, Stopwatch benchmark, string name)
        {
            trigger.AppendLog($"STOPWATCH {name}: {benchmark.ElapsedMilliseconds}ms");
            benchmark.Restart();
        }
    }
}
