using Microsoft.Extensions.Caching.Memory;
using Moq;
using BestFor.Fakes;
using Autofac;
using BestFor.Data;
using BestFor.Domain;
using BestFor.Domain.Entities;
using BestFor.Services.Cache;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics.CodeAnalysis;
using BestFor.UnitTests.Testables;

namespace BestFor.UnitTests.Services.Datasources
{
    /// <summary>
    /// Unit tests for DefaultSuggestions object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CacheManagerTests
    {
        public class TestSetup
        {
            public TestMemoryCache MemoryCache;

            public TestSetup()
            {
                MemoryCache = new TestMemoryCache();
            }
        }

        [Fact]
        public void CacheManagerTests_BasicTests_Pass()
        {
            // var setup = new TestSetup();
            var memoryCache = new Mock<IMemoryCache>();
            memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns((object o) => { return new Mock<ICacheEntry>().Object; });

            var cache = new CacheManager(memoryCache.Object);

            cache.Add("hi", new Answer() { Id = 8 });
            // Internally it is going to call CreateEntry
            // Since .Set it an extension we can not do a setup for it using Moq.
            memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.AtLeastOnce());
        }
    }
}
