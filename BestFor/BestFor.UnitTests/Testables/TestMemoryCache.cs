using Moq;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.UnitTests.Testables
{
    /// <summary>
    /// Extensions are not quite testable using Moq.
    /// Have to create solid test classes some time.
    /// </summary>
    public class TestMemoryCache : IMemoryCache
    {
        public Dictionary<string, object> Cache;

        public TestMemoryCache()
        {
            Cache = new Dictionary<string, object>();
        }

        public ICacheEntry CreateEntry(object key)
        {
            return new Mock<ICacheEntry>().Object;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object value)
        {
            throw new NotImplementedException();
        }
    }
}
