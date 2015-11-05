using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;

namespace ISI.Rtc.Util
{

    /// <summary>
    /// Simple <see cref="SynchronizationContext"/> implementation that simply buffers callbacks to be executed when
    /// Run is invoked manually.
    /// </summary>
    public class RunnableSynchronizationContext : SynchronizationContext
    {

        /// <summary>
        /// Set of actions to be dispatched.
        /// </summary>
        readonly BlockingCollection<Tuple<SendOrPostCallback, object>> queue =
            new BlockingCollection<Tuple<SendOrPostCallback, object>>();

        /// <summary>
        /// Signals completion of Run method.
        /// </summary>
        readonly TaskCompletionSource complete = new TaskCompletionSource();

        /// <summary>
        /// Dispatches an asychronous message to the synchronization context.
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="state"></param>
        public override void Post(SendOrPostCallback cb, object state)
        {
            queue.Add(new Tuple<SendOrPostCallback, object>(cb, state));
        }

        /// <summary>
        /// Dispatches the given action.
        /// </summary>
        /// <param name="action"></param>
        public void Dispatch(Action action)
        {
            Post((a) => action(), null);
        }

        /// <summary>
        /// Invokes the given action.
        /// </summary>
        /// <param name="action"></param>
        public Task InvokeAsync(Action action)
        {
            var tcs = new TaskCompletionSource();
            Post((a) =>
            {
                try
                {
                    action();
                    tcs.SetResult();
                }
                catch (TaskCanceledException)
                {
                    tcs.SetCanceled();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);
            return tcs.Task;
        }

        /// <summary>
        /// Invokes the given action.
        /// </summary>
        /// <param name="action"></param>
        public Task<TResult> InvokeAsync<TResult>(Func<TResult> func)
        {
            var tcs = new TaskCompletionSource<TResult>();
            Post((a) =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (TaskCanceledException)
                {
                    tcs.SetCanceled();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);
            return tcs.Task;
        }

        /// <summary>
        /// Runs the actions posted to the synchronization context until completion.
        /// </summary>
        public void Run()
        {
            // save previous context
            var s = SynchronizationContext.Current;

            try
            {
                // set self as new context
                SynchronizationContext.SetSynchronizationContext(this);

                // continue until we're complete
                foreach (var item in queue.GetConsumingEnumerable())
                    item.Item1(item.Item2);

                // finish any left over items added as a result of being complete
                foreach (var item in queue.GetConsumingEnumerable())
                    item.Item1(item.Item2);

                complete.SetResult();
            }
            finally
            {
                // restore context
                SynchronizationContext.SetSynchronizationContext(s);
            }
        }

        /// <summary>
        /// Completes the synchronization context.
        /// </summary>
        public Task Complete()
        {
            queue.CompleteAdding();
            return complete.Task;
        }

    }

}
