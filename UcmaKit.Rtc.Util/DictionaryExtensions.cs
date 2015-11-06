﻿using System.Collections.Generic;

namespace UcmaKit.Rtc.Util
{

    public static class DictionaryExtensions
    {

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        {
            TValue value;
            return self.TryGetValue(key, out value) ? value : default(TValue);
        }

    }

}
