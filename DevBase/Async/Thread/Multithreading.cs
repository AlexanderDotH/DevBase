﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevBase.Async
{
    public class Multithreading
    {
        private List<AThread> _threads;
        private ConcurrentQueue<AThread> _queueThreads;

        private int _capacity;

        /// <summary>
        /// Constructs the base of the multithreading system
        /// </summary>
        /// <param name="capacity">Specifies a limit for active working threads</param>
        public Multithreading(int capacity = Int32.MaxValue)
        {
            this._threads = new List<AThread>();
            this._queueThreads = new ConcurrentQueue<AThread>();

            this._capacity = capacity;

            ManageQueuedThreads();
        }

        /// <summary>
        /// Manages the thread list by removing/adding items from the queue
        /// </summary>
        private void ManageQueuedThreads()
        {
            Thread manageThreads = new Thread(() =>
            {
                while (true)
                {
                    foreach (AThread u in GetUnactiveThreads())
                    {
                        this._threads.Remove(u);
                    }

                    if (this._threads.Count < this._capacity && this._queueThreads.Count != 0)
                    {
                        AThread aThreadDequeued = null;

                        this._queueThreads.TryDequeue(out aThreadDequeued);

                        if (aThreadDequeued != null)
                        {
                            this._threads.Add(aThreadDequeued);

                            if (aThreadDequeued.StartAfterCreation)
                                aThreadDequeued.Thread.Start();
                        }
                    }
                }
            });
            manageThreads.Start();
        }

        /// <summary>
        /// Gets all active threads from thread list
        /// </summary>
        /// <returns>All active Threads from thread list</returns>
        private List<AThread> GetActiveThreads()
        {
            List<AThread> tList = new List<AThread>();

            foreach (AThread t in this._threads)
            {
                if (t.Thread.IsAlive)
                    tList.Add(t);
            }

            return tList;
        }

        /// <summary>
        /// Gets all unactive threads from thread list
        /// </summary>
        /// <returns>All unactive Threads from thread list</returns>
        private List<AThread> GetUnactiveThreads()
        {
            List<AThread> tList = new List<AThread>();

            foreach (AThread t in this._threads)
            {
                if (!t.Thread.IsAlive)
                    tList.Add(t);
            }

            return tList;
        }

        /// <summary>
        /// Adds a thread to the ThreadQueue
        /// </summary>
        /// <param name="t">A delivered thread which will be added to the multithreading queue</param>
        /// <param name="startAfterCreation">Specifies if the thread will be started after dequeueing</param>
        /// <returns>The given thread</returns>
        /// <example>
        ///
        /// <code>Thread at = CreateThread(new Thread(null), false);</code>
        ///
        /// Or just use
        ///
        /// <code>CreateThread(new Thread(null), false);</code>
        ///  
        /// </example>
        public AThread CreateThread(Thread t, bool startAfterCreation)
        {
            AThread aThread = new AThread(t);
            aThread.StartAfterCreation = startAfterCreation;

            lock (this._queueThreads)
            {
                this._queueThreads.Enqueue(aThread);
            }

            return aThread;
        }

        /// <summary>
        /// Adds a thread from object AThread to the ThreadQueue
        /// </summary>
        /// <param name="t">A delivered thread which will be added to the multithreading queue</param>
        /// <param name="startAfterCreation">Specifies if the thread will be started after dequeueing</param>
        /// <returns>The given thread</returns>
        /// <example>
        ///
        /// <code>AThread at = CreateThread(new Thread(null), false);</code>
        ///
        /// Or just use
        ///
        /// <code>CreateThread(new Thread(null), false);</code>
        ///  
        /// </example>
        public AThread CreateThread(AThread t, bool startAfterCreation)
        {
            t.StartAfterCreation = startAfterCreation;

            lock (this._queueThreads)
            {
                this._queueThreads.Enqueue(t);
            }

            return t;
        }

        /// <summary>
        /// Abort all active running threads
        /// </summary>
        public void AbortAll()
        {
            foreach (AThread t in _threads)
            {
                t.Thread.Abort();
            }
        }

        /// <summary>
        /// Dequeues all active queue members
        /// </summary>
        public void DequeueAll()
        {
            for (int i = 0; i < this._queueThreads.Count; i++)
            {
                AThread aThreadDequeued = null;
                this._queueThreads.TryDequeue(out aThreadDequeued);
            }
        }

        /// <returns>
        /// Returns the capacity
        /// </returns>
        public int Capacity
        {
            get { return this._capacity; }
        }

        /// <returns>
        /// Returns all active threads
        /// </returns>
        public List<AThread> Threads
        {
            get { return _threads; }
        }
    }
}
