using System;
using System.Threading;
using System.Threading.Tasks;

namespace UcmaKit.Rtc.Util
{

    public static class TaskUtil
    {

        /// <summary>
        /// Wraps a task and invokes the action when the cancellation token fires.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken, Func<Task, Task> cancel)
        {
            cancellationToken.Register(() =>
            {
                if (!task.IsCompleted)
                    cancel(task);
            });

            return task;
        }

        /// <summary>
        /// Wraps a task and invokes the action when the cancellation token fires.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken, Func<Task> cancel)
        {
            cancellationToken.Register(() =>
            {
                if (!task.IsCompleted)
                    cancel();
            });

            return task;
        }

    }

}
