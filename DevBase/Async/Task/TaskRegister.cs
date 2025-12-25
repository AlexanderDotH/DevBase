using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generics;

namespace DevBase.Async.Task
{
    /// <summary>
    /// Registers and manages tasks, allowing for suspension, resumption, and termination by type.
    /// </summary>
    public class TaskRegister
    {
        private readonly ATupleList<TaskSuspensionToken, Object> _suspensionList;
        private readonly ATupleList<System.Threading.Tasks.Task, Object> _taskList;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRegister"/> class.
        /// </summary>
        public TaskRegister()
        {
            this._suspensionList = new ATupleList<TaskSuspensionToken, object>();
            this._taskList = new ATupleList<System.Threading.Tasks.Task, object>();
        }

        /// <summary>
        /// Registers a task created from an action with a specific type.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="type">The type identifier for the task.</param>
        /// <param name="startAfterCreation">Whether to start the task immediately.</param>
        public void RegisterTask(Action action, Object type, bool startAfterCreation = true)
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(action);
            RegisterTask(task, type, startAfterCreation);
        }

        /// <summary>
        /// Registers an existing task with a specific type.
        /// </summary>
        /// <param name="task">The task to register.</param>
        /// <param name="type">The type identifier for the task.</param>
        /// <param name="startAfterCreation">Whether to start the task immediately if not already started.</param>
        public void RegisterTask(System.Threading.Tasks.Task task, Object type, bool startAfterCreation = true)
        {
            if (startAfterCreation)
                task.Start();

            GenerateNewToken(type);

            RegisterTask(task, type);
        }

        /// <summary>
        /// Registers a task created from an action and returns a suspension token.
        /// </summary>
        /// <param name="token">The returned suspension token.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="type">The type identifier for the task.</param>
        /// <param name="startAfterCreation">Whether to start the task immediately.</param>
        public void RegisterTask(out TaskSuspensionToken token, Action action, Object type, bool startAfterCreation = true)
        {
            System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(action);
            RegisterTask(out token, task, startAfterCreation);
        }

        /// <summary>
        /// Registers an existing task and returns a suspension token.
        /// </summary>
        /// <param name="token">The returned suspension token.</param>
        /// <param name="task">The task to register.</param>
        /// <param name="type">The type identifier for the task.</param>
        /// <param name="startAfterCreation">Whether to start the task immediately.</param>
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

        /// <summary>
        /// Generates or retrieves a suspension token for a specific type.
        /// </summary>
        /// <param name="type">The type identifier.</param>
        /// <returns>The suspension token.</returns>
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

        /// <summary>
        /// Gets the suspension token associated with a specific type.
        /// </summary>
        /// <param name="type">The type identifier.</param>
        /// <returns>The suspension token.</returns>
        public TaskSuspensionToken GetTokenByType(Object type)
        {
            return this._suspensionList.FindEntry(type);
        }

        /// <summary>
        /// Gets the suspension token associated with a specific task.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>The suspension token.</returns>
        public TaskSuspensionToken GetTokenByTask(System.Threading.Tasks.Task task)
        {
            Object type = this._taskList.FindEntry(task);
            TaskSuspensionToken token = GetTokenByType(type);
            return token;
        }

        /// <summary>
        /// Suspends tasks associated with an array of types.
        /// </summary>
        /// <param name="types">The array of types to suspend.</param>
        public void SuspendByArray(Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Suspend(types[i]);
            }
        }

        /// <summary>
        /// Suspends tasks associated with the specified types.
        /// </summary>
        /// <param name="types">The types to suspend.</param>
        public void Suspend(params Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Suspend(types[i]);
            }
        }

        /// <summary>
        /// Suspends tasks associated with a specific type.
        /// </summary>
        /// <param name="type">The type to suspend.</param>
        public void Suspend(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            token.Suspend();
        }

        /// <summary>
        /// Resumes tasks associated with an array of types.
        /// </summary>
        /// <param name="types">The array of types to resume.</param>
        public void ResumeByArray(Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Resume(types[i]);
            }
        }

        /// <summary>
        /// Resumes tasks associated with the specified types.
        /// </summary>
        /// <param name="types">The types to resume.</param>
        public void Resume(params Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Resume(types[i]);
            }
        }

        /// <summary>
        /// Resumes tasks associated with a specific type.
        /// </summary>
        /// <param name="type">The type to resume.</param>
        public void Resume(Object type)
        {
            TaskSuspensionToken token = this._suspensionList.FindEntry(type);
            token.Resume();
        }

        /// <summary>
        /// Kills (waits for) tasks associated with the specified types.
        /// </summary>
        /// <param name="types">The types to kill.</param>
        public void Kill(params Object[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                Kill(types[i]);
            }
        }

        /// <summary>
        /// Kills (waits for) tasks associated with a specific type.
        /// </summary>
        /// <param name="type">The type to kill.</param>
        public void Kill(Object type)
        {
            this._taskList.FindEntries(type).ForEach(t => t.Wait(0));
        }
    }
}
