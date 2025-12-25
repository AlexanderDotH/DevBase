using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevBase.Async.Task
{
    /// <summary>
    /// A token that allows for suspending and resuming tasks.
    /// </summary>
    public class TaskSuspensionToken
    {
        private readonly SemaphoreSlim _lock;
        private bool _suspended;
        private TaskCompletionSource<bool> _resumeRequestTcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSuspensionToken"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token source (not currently used in constructor logic but kept for signature).</param>
        public TaskSuspensionToken(CancellationTokenSource cancellationToken)
        {
            this._suspended = false;
            this._lock = new SemaphoreSlim(1);
            this._resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskSuspensionToken"/> class with a default cancellation token source.
        /// </summary>
        public TaskSuspensionToken() : this(new CancellationTokenSource()) { }

        /// <summary>
        /// Waits for the suspension to be released if currently suspended.
        /// </summary>
        /// <param name="delay">Optional delay before checking.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A task representing the wait operation.</returns>
        public async System.Threading.Tasks.Task WaitForRelease(int delay = 0, CancellationToken token = default(CancellationToken))
        {
            if (delay != 0)
                await System.Threading.Tasks.Task.Delay(delay);
            
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

        /// <summary>
        /// Suspends the task associated with this token.
        /// </summary>
        public void Suspend()
        {
            this._suspended = true;
            this._resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        }

        /// <summary>
        /// Resumes the task associated with this token.
        /// </summary>
        public void Resume()
        {
            this._suspended = false;

            var resumeRequestTcs = _resumeRequestTcs;
            _resumeRequestTcs = null;
            resumeRequestTcs.TrySetResult(true);
        }
    }
}
