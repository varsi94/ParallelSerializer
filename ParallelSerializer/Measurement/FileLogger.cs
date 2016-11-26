using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelSerializer.Measurement
{
    public class FileLogger : ILogger, IDisposable
    {
        private readonly FileStream fs;
        private readonly TextWriter writer;

        public static FileLogger GetInstance(string fileName)
        {
            return new FileLogger(fileName);
        }

        protected FileLogger(string fileName)
        {
            fs = new FileStream(fileName, FileMode.Create);
            writer = StreamWriter.Synchronized(new StreamWriter(fs));
        }

        public void WriteLine(string s)
        {
            writer.WriteLine(s);
        }

        public void Dispose()
        {
            writer.Dispose();
            fs.Dispose();
        }
    }
}
