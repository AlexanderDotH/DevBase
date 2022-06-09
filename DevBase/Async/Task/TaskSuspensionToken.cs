using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevBase.Async.Task
{
    public class TaskSuspensionToken
    {
        private CancellationTokenSource _tokenSource;
        private SemaphoreSlim _lock;
        private bool _suspended;

        public TaskSuspensionToken(CancellationTokenSource cancellationToken)
        {
            this._tokenSource = cancellationToken;
            this._suspended = false;
            this._lock = new SemaphoreSlim(1);
        }

        public TaskSuspensionToken() : this(new CancellationTokenSource()) { }

        public async System.Threading.Tasks.Task WaitForRelease()
        {
            if (!this._suspended)
                return;

            await this._lock.WaitAsync(this._tokenSource.Token);
            this._lock.Release();
        }

        public void Suspend()
        {
            this._suspended = true;
            this._lock.Wait();
        }

        public void Resume()
        {
            this._suspended = false;
            this._lock.Release();
        }
    }
}
