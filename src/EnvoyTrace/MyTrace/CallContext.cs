using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyTrace
{
    public static class CallContext
    {
        private static ConcurrentDictionary<string, AsyncLocal<object>> _states = new ConcurrentDictionary<string, AsyncLocal<object>>();

        public static void SetData<T>(string name, T data) =>
            _states.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public static T GetData<T>(string name) =>
            _states.TryGetValue(name, out AsyncLocal<object> data) ? (T)data.Value : default(T);
    }
}
