using Microsoft.Extensions.Caching.Memory;

namespace Cogworks.Essentials.EventArgs
{
    public class CacheEvictionArgs : System.EventArgs
    {
        public object Key { get; }

        public object Value { get; }

        public EvictionReason EvictionReason { get; set; }

        public CacheEvictionArgs(object key, object value, EvictionReason evictionReason)
        {
            Key = key;
            Value = value;
            EvictionReason = evictionReason;
        }
    }
}