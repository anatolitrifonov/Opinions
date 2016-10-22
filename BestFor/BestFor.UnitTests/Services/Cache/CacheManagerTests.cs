using BestFor.Domain.Entities;
using BestFor.Services.Cache;
using BestFor.UnitTests.Testables;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BestFor.UnitTests.Services.Cache
{
    /// <summary>
    /// Unit tests for DefaultSuggestions object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CacheManagerTests
    {
        [Fact]
        public void CacheManagerTests_BasicTests_Pass()
        {
            var memoryCache = new Mock<IMemoryCache>();
            memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns((object o) => { return new Mock<ICacheEntry>().Object; });

            var cache = new CacheManager(memoryCache.Object);

            cache.Add("hi", new Answer() { Id = 8 });
            // Internally it is going to call CreateEntry
            // Since .Set it an extension we can not do a setup for it using Moq.
            memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.AtLeastOnce());
        }

        [Fact]
        public void CacheManagerTests_Other_Pass()
        {
            var memoryCache = new Mock<IMemoryCache>();
            memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns((object o) => { return new Mock<ICacheEntry>().Object; });

            var cache = new CacheManager(memoryCache.Object);

            cache.Add("hi", new Answer() { Id = 8 }, 23);
            // Internally it is going to call CreateEntry
            // Since .Set it an extension we can not do a setup for it using Moq.
            memoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.AtLeastOnce());
        }

        [Fact]
        public void CacheManagerTests_Get_Gets()
        {
            var memoryCache = new Mock<IMemoryCache>();
            memoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns((object o) => { return new Mock<ICacheEntry>().Object; });
            Answer AnyAnswer = new Answer() { Id = 5 };
            object AnyObject = AnyAnswer;

            // memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out AnyObject)).Returns(true);
            //memoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out AnyObject)).Returns(
            //    (object o) =>
            //    {
            //        return o.ToString() == "hi" ? true : false;
            //    }
            //    );

            var cache = new CacheManager(memoryCache.Object);

            cache.Add("hi", new Answer() { Id = 8 }, 23);

            var z = cache.Get("hi") as Answer;
            var z1 = cache.Get("not hi") as Answer;

            // Assert.Equal(z.Id, 5);
        }

        [Fact]
        public void CacheManagerTests_GetThroughTestables_Gets()
        {
            var memoryCache = new TestMemoryCache();
            var cache = new CacheManager(memoryCache);
            Assert.Equal("A", cache.Get("hi").ToString());
            Assert.Null(cache.Get("not hi"));
        }
    }
}
