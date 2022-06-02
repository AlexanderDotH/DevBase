using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevBase.Async.Task
{
    public class TaskActionEntry
    {
        private Action _action;
        private TaskCreationOptions _creationOptions;

        public TaskActionEntry(Action action, TaskCreationOptions creationOptions)
        {
            _action = action;
            _creationOptions = creationOptions;
        }

        public Action Action
        {
            get => _action;
        }

        public TaskCreationOptions CreationOptions
        {
            get => _creationOptions;
        }
    }
}
