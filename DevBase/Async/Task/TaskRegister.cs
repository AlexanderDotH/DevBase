using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBase.Async.Task
{
    public class TaskRegister
    {
        private GenericTupleList<TaskSuspensionToken, Object> _suspensionList;
        private GenericTupleList<System.Threading.Tasks.Task, Object> _taskList;

        public TaskRegister()
        {
            this._suspensionList = new GenericTupleList<TaskSuspensionToken, object>();
            this._taskList = new GenericTupleList<System.Threading.Tasks.Task, object>();
        }

        public void RegisterTask(out TaskSuspensionToken token, Action action, Object type, bool startAfterCreation = false)
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(action);
            
            if (startAfterCreation)
                task.Start();

            token = GenerateNewToken(type);

            RegisterTask(task, type);
        }

        public void RegisterTask(out TaskSuspensionToken token, System.Threading.Tasks.Task task, Object type, bool startAfterCreation = false)
        {
            if (startAfterCreation)
                task.Start();

            token = GenerateNewToken(type);

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
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            return token;
        }

        public void Suspend(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            token.Suspend();
        }

        public void Resume(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            token.Resume();
        }

        public void Kill(Object type)
        {
            this._taskList.FindEntries(type).ForEach(t => t.Wait(0));
        }
    }
}
