// Adapted from https://github.com/PimDeWitte/UnityMainThreadDispatcher
// See https://github.com/PimDeWitte/UnityMainThreadDispatcher/blob/master/LICENSE for license

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RevitToVR
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance = null;
        public static MainThreadDispatcher Instance => _instance;

        private static readonly Queue<Action> _executionQueue = new Queue<Action>();
        private static SemaphoreSlim _executionQueueLock = new SemaphoreSlim(1, 1);

        private void Awake()
        {
            // Singleton behaviour
            if (_instance != null && _instance != this) { Destroy(this); } else { _instance = this; }
        }

        private void Update()
        {
            _executionQueueLock.Wait();
            try
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
            finally
            {
                _executionQueueLock.Release();
            }
        }

        /// <summary>
        /// Locks the queue and adds the IEnumerator to the queue
        /// </summary>
        /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
        public void Enqueue(IEnumerator action)
        {
            _executionQueueLock.Wait();
            try
            {
                _executionQueue.Enqueue(() =>
                {
                    StartCoroutine(action);
                });
            }
            finally
            {
                _executionQueueLock.Release();
            }
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        public void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        /// <returns>A Task that can be awaited until the action completes</returns>
        public Task EnqueueAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        /// <returns>A Task that can be awaited until the action completes</returns>
        public Task EnqueueAsync<T>(Action<T> action, T arg)
        {
            var tcs = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    action(arg);
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }

        IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }

        IEnumerator ActionWrapper<T>(Action<T> action, T arg)
        {
            action(arg);
            yield return null;
        }
    }
}
