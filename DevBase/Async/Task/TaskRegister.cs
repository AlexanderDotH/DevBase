using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Async.Task
{
    public class TaskRegister
    {
        private readonly ATupleList<TaskSuspensionToken, Object> _suspensionList;
        private readonly ATupleList<System.Threading.Tasks.Task, Object> _taskList;

        public TaskRegister()
        {
            this._suspensionList = new ATupleList<TaskSuspensionToken, object>();
            this._taskList = new ATupleList<System.Threading.Tasks.Task, object>();
        }

        public void RegisterTask(Action action, Object type, bool startAfterCreation = true)
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(action);
            RegisterTask(task, type, startAfterCreation);
        }

        public void RegisterTask(System.Threading.Tasks.Task task, Object type, bool startAfterCreation = true)
        {
            if (startAfterCreation)
                task.Start();

            GenerateNewToken(type);

            RegisterTask(task, type);
        }

        public void RegisterTask(out TaskSuspensionToken token, Action action, Object type, bool startAfterCreation = true)
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(action);
            RegisterTask(out token, task, startAfterCreation);
        }

        public void RegisterTask(out TaskSuspensionToken token, System.Threading.Tasks.Task task, Object type, bool startAfterCreation = true)
        {
            token = GenerateNewToken(type);

            if (startAfterCreation)
                task.Start();

            RegisterTask(task, type);
        }

        private void RegisterTask(System.Threading.Tasks.Task task, Object type)
        {
            this._taskList.Add(new Tuple<System.Threading.Tasks.Task, object>(task, type));
        }

        public TaskSuspensionToken GenerateNewToken(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);

            if (token == null)
            {
                TaskSuspensionToken taskSuspensionToken = new TaskSuspensionToken();
                this._suspensionList.Add(new Tuple<TaskSuspensionToken, object>(taskSuspensionToken, type));
                return taskSuspensionToken;
            }

            return token;
        }

        public TaskSuspensionToken GetTokenByType(Object type)
        {
            return this._suspensionList.FindEntry(type);
        }

        public TaskSuspensionToken GetTokenByTask(System.Threading.Tasks.Task task)
        {
            Object type = this._taskList.FindEntry(task);
            TaskSuspensionToken token = GetTokenByType(type);
            return token;
        }

        public void SuspendByArray(Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Suspend(types[i]);
            }
        }

        public void Suspend(params Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Suspend(types[i]);
            }
        }

        public void Suspend(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            token.Suspend();
        }

        public void ResumeByArray(Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Resume(types[i]);
            }
        }

        public void Resume(params Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Resume(types[i]);
            }
        }

        public void Resume(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            token.Resume();
        }

        public void Kill(params Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Kill(types[i]);
            }
        }

        public void Kill(Object type)
        {
            this._taskList.FindEntries(type).ForEach(t => t.Wait(0));
        }
    }
}
