using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelSerializer
{
    public class SubTaskWaiter : IDisposable
    {
        private int counter;
        private readonly AutoResetEvent resetEvent;

        public SubTaskWaiter(ISerializationTask task)
        {
            resetEvent = new AutoResetEvent(false);
        }

        public void TaskFinished()
        {
            Interlocked.Decrement(ref counter);
            if (counter == 0)
            {
                resetEvent.Set();
            }
        }

        public void WaitAll()
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

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
