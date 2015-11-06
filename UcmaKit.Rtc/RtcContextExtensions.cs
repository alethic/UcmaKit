using System;
using System.Collections.Generic;

using Microsoft.Rtc.Collaboration;

using UcmaKit.Rtc.Util;

namespace UcmaKit.Rtc
{

    public static class RtcContextExtensions
    {

        /// <summary>
        /// Gets the context object of the given type from the <see cref="Call"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="call"></param>
        /// <returns></returns>
        public static T GetContext<T>(this Call call)
            where T : class, new()
        {
            return GetContext<T>(call, () => new T());
        }

        /// <summary>
        /// Gets the context object of the given type from the <see cref="Call"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="call"></param>
        /// <returns></returns>
        public static T GetContext<T>(this Call call, Func<T> create)
            where T : class
        {
            lock (call)
            {
                var map = call.ApplicationContext as IDictionary<Type, object>;
                if (map == null)
                    call.ApplicationContext = map = new Dictionary<Type, object>();

                var obj = (T)((IDictionary<Type, object>)map).GetOrDefault(typeof(T));
                if (obj == null)
                    map[typeof(T)] = obj = create();

                return obj;
            }
        }

        /// <summary>
        /// Gets the context object of the given type from the <see cref="LocalEndpoint"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static T GetContext<T>(this LocalEndpoint endpoint)
            where T : class, new()
        {
            return GetContext<T>(endpoint, () => new T());
        }

        /// <summary>
        /// Gets the context object of the given type from the <see cref="LocalEndpoint"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static T GetContext<T>(this LocalEndpoint endpoint, Func<T> create)
            where T : class
        {
            lock (endpoint)
            {
                var map = endpoint.InnerEndpoint.ApplicationContext as IDictionary<Type, object>;
                if (map == null)
                    endpoint.InnerEndpoint.ApplicationContext = map = new Dictionary<Type, object>();

                var obj = (T)((IDictionary<Type, object>)map).GetOrDefault(typeof(T));
                if (obj == null)
                    map[typeof(T)] = obj = create();

                return obj;
            }
        }

        /// <summary>
        /// Gets the presence context for the given <see cref="LocalEndpoint"/>.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static RtcPresenceContext GetPresenceContext(this LocalEndpoint endpoint)
        {
            return endpoint.GetContext<RtcPresenceContext>(() => new RtcPresenceContext(endpoint));
        }

    }

}
