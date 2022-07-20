namespace DevBase.Async.Thread
{
    public class AThread
    {
        private readonly System.Threading.Thread _thread;
        private bool _startAfterCreation;

        /// <summary>
        /// Constructs a editable thread
        /// </summary>
        /// <param name="t">Delivers a thread object</param>
        public AThread(System.Threading.Thread t)
        {
            this._thread = t;
            this._startAfterCreation = false;
        }

        /// <summary>
        /// Starts a thread with a given condition
        /// </summary>
        /// <param name="condition">A given condition needs to get delivered which is essential to let this method work</param>
        public void StartIf(bool condition)
        {
            if (condition && _thread != null && !_thread.IsAlive)
                _thread.Start();
        }

        /// <summary>
        /// Starts a thread with a given condition
        /// </summary>
        /// <param name="condition">A given condition needs to get delivered which is essential to let this method work</param>
        /// <param name="parameters">A parameter can be used to give a thread some start parameters</param>
        public void StartIf(bool condition, object parameters)
        {
            if (condition && _thread != null && !_thread.IsAlive)
                _thread.Start(parameters);
        }

        /// <returns>
        /// Returns the given Thread
        /// </returns>
        public System.Threading.Thread Thread
        {
            get { return this._thread; }
        }

        /// <summary>
        /// Changes the StartAfterCreation status of the thread
        /// </summary>
        public bool StartAfterCreation
        {
            get { return this._startAfterCreation; }
            set { this._startAfterCreation = value; }
        }
    }
}
