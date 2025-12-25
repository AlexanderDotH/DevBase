namespace DevBase.Async.Task
{
    /// <summary>
    /// Represents an entry for a task action with creation options.
    /// </summary>
    public class TaskActionEntry
    {
        private readonly Action _action;
        private readonly TaskCreationOptions _creationOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskActionEntry"/> class.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <param name="creationOptions">The task creation options.</param>
        public TaskActionEntry(Action action, TaskCreationOptions creationOptions)
        {
            _action = action;
            _creationOptions = creationOptions;
        }

        /// <summary>
        /// Gets the action associated with this entry.
        /// </summary>
        public Action Action
        {
            get => this._action;
        }

        /// <summary>
        /// Gets the task creation options associated with this entry.
        /// </summary>
        public TaskCreationOptions CreationOptions
        {
            get => this._creationOptions;
        }
    }
}
