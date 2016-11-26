using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Measurement
{
    public class MethodStopwatch : IDisposable
    {
        private readonly Stopwatch stopWatch;
        private readonly ILogger logger;
        private string name;

        protected MethodStopwatch(string name, ILogger logger)
        {
            stopWatch = Stopwatch.StartNew();
            this.logger = logger;
            this.name = name;
        }

        public static void MeasureMethodCall(string name, ILogger logger, Action a)
        {
            using (var sw = new MethodStopwatch(name, logger))
            {
                a();
            }
        }

        public void Dispose()
        {
            stopWatch.Stop();
            logger.WriteLine(name + "\t" + stopWatch.ElapsedTicks);
        }
    }
}
