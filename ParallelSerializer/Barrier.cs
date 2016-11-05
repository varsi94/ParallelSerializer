using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class Barrier : IDisposable
    {
        private volatile int counter = 0;
        private readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

        public void Start()
        {
            Interlocked.Increment(ref counter);
        }

        public void Stop()
        {
            if (Interlocked.Decrement(ref counter) == 0)
            {
                resetEvent.Set();
            }
        }

        public void WaitForAll()
        {
            resetEvent.WaitOne();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    resetEvent.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
