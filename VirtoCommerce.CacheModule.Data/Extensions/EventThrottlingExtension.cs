using System;
using System.Threading.Tasks;

namespace VirtoCommerce.CacheModule.Data.Extensions
{
    public static class EventThrottlingExtension
    {
        public static EventHandler<T> Throttle<T>(this EventHandler<T> handler, TimeSpan throttle)
        {
            var throttling = false;
            return (s, e) =>
            {
                if (throttling)
                    return;

                throttling = true;
                handler(s, e);
                Task.Delay(throttle).ContinueWith(x => throttling = false);
            };
        }

    }
}
