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
        private readonly SemaphoreSlim _lock;
        private bool _suspended;
        private TaskCompletionSource<bool> _resumeRequestTcs;

        public TaskSuspensionToken(CancellationTokenSource cancellationToken)
        {
            this._suspended = false;
            this._lock = new SemaphoreSlim(1);
            this._resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public TaskSuspensionToken() : this(new CancellationTokenSource()) { }

        public async System.Threading.Tasks.Task WaitForRelease(CancellationToken token = default(CancellationToken))
        {
            System.Threading.Tasks.Task resumeRequestTask = null;

            await _lock.WaitAsync(token);
            try
            {
                if (!_suspended)
                    return;

                resumeRequestTask = WaitForResumeRequestAsync(token);
            }
            finally
            {
                _lock.Release();
            }

            await resumeRequestTask;
        }

        private async System.Threading.Tasks.Task WaitForResumeRequestAsync(CancellationToken token)
        {
            using (token.Register(() => _resumeRequestTcs.TrySetCanceled(), useSynchronizationContext: false))
            {

                if (_resumeRequestTcs != null)
                {
                    if (_resumeRequestTcs.Task != null)
                    {
                        await _resumeRequestTcs.Task;
                    }
                }
            }
        }

        public void Suspend()
        {
            this._suspended = true;
            this._resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        }

        public void Resume()
        {
            this._suspended = false;

            var resumeRequestTcs = _resumeRequestTcs;
            _resumeRequestTcs = null;
            resumeRequestTcs.TrySetResult(true);
        }
    }
}
