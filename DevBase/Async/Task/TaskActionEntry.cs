namespace DevBase.Async.Task
{
    public class TaskActionEntry
    {
        private readonly Action _action;
        private readonly TaskCreationOptions _creationOptions;

        public TaskActionEntry(Action action, TaskCreationOptions creationOptions)
        {
            _action = action;
            _creationOptions = creationOptions;
        }

        public Action Action
        {
            get => this._action;
        }

        public TaskCreationOptions CreationOptions
        {
            get => this._creationOptions;
        }
    }
}
