using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBase.Async.Task
{
    public class TaskRegister
    {
        private GenericTupleList<TaskActionEntry, Object> _actionList;
        private GenericTupleList<System.Threading.Tasks.Task, Object> _taskList;
        private GenericTupleList<CancellationTokenSource, Object> _tokenList;

        public TaskRegister()
        {
            this._actionList = new GenericTupleList<TaskActionEntry, Object>();
            this._tokenList = new GenericTupleList<CancellationTokenSource, Object>();
            this._taskList = new GenericTupleList<System.Threading.Tasks.Task, Object>();
        }

        public void RegisterTask(Action action, Object type, bool startTask = true)
        {
            TaskActionEntry taskActionEntry = new TaskActionEntry(action, TaskCreationOptions.LongRunning);
            RegisterTask(taskActionEntry, type, startTask);
        }

        public void RegisterTask(TaskActionEntry action, Object type, bool startTask = true)
        {
            CancellationTokenSource token = FindTokenByType(type);

            if (token == null)
            {
                token = AddCancellationTokenSource(type);
            }

            if (token == null)
                return;

            Tuple<TaskActionEntry, Object> actionEntry = new Tuple<TaskActionEntry, Object>(action, type);

            System.Threading.Tasks.Task t =
                new System.Threading.Tasks.Task(action.Action, token.Token, action.CreationOptions);

            if (startTask)
                t.Start();

            Tuple<System.Threading.Tasks.Task, Object> taskEntry = new Tuple<System.Threading.Tasks.Task, Object>(t, type);

            this._taskList.Add(taskEntry);
            this._tokenList.Add(new Tuple<CancellationTokenSource, object>(token, type));
            this._actionList.Add(actionEntry);
        }

        public void Suspend(Object type)
        {
            CancellationTokenSource tokenSource = FindTokenByType(type);

            if (tokenSource != null)
                tokenSource.Cancel();

            this._taskList.FindEntries(type).ForEach(t =>
            {
                t.Wait(0);
            });
            this._tokenList.SafeRemove(new Tuple<CancellationTokenSource, object>(tokenSource, type));
        }

        public void Resume(Object type)
        {
            CancellationTokenSource token = FindTokenByType(type);

            if (token == null)
            {
                token = AddCancellationTokenSource(type);
            }

            if (token == null)
                return;

            this._actionList.FindFullEntries(type).ForEach(t =>
            {
                System.Threading.Tasks.Task task =
                    new System.Threading.Tasks.Task(t.Item1.Action, token.Token, t.Item1.CreationOptions);

                this._taskList.Add(new Tuple<System.Threading.Tasks.Task, object>(task, type));
                task.Start();
            });

            this._tokenList.Add(new Tuple<CancellationTokenSource, object>(token, type));
        }

        public bool IsCancelationRequested(object type)
        {
            CancellationTokenSource cancellationTokenSource = FindTokenByType(type);

            if (cancellationTokenSource == null)
                return false;

            return cancellationTokenSource.IsCancellationRequested;
        }

        private CancellationTokenSource FindTokenByType(object type)
        {
            for (int i = 0; i < this._tokenList.Length; i++)
            {
                Tuple<CancellationTokenSource, object> t = this._tokenList[i];

                if (t.Item2.Equals(type))
                {
                    return t.Item1;
                }
            }

            return null;
        }

        private bool ContainsToken(CancellationTokenSource token)
        {

            for (int i = 0; i < this._tokenList.Length; i++)
            {
                if (this._tokenList[i].Equals(token))
                    return true;
            }

            return false;
        }

        private CancellationTokenSource AddCancellationTokenSource(Object type)
        {
            Tuple<CancellationTokenSource, Object> entry =
                new Tuple<CancellationTokenSource, Object>(new CancellationTokenSource(), type);

            CancellationTokenSource cancellationToken = FindTokenByType(type);

            if (!ContainsToken(cancellationToken))
                this._tokenList.Add(entry);

            return entry.Item1;
        }
    }
}
