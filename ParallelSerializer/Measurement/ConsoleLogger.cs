using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Measurement
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }
    }
}
