using BestFor.Fakes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Xunit;

namespace BestFor.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    public class FakeDataContextTests
    {
        [Fact]
        public void FakeDataContextTests_IUserStore_Implemented()
        {
            var context = new FakeDataContext();
            Assert.ThrowsAny<NotImplementedException>(() => context.GetUserIdAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.GetUserNameAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.SetUserNameAsync(null, null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.GetNormalizedUserNameAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.SetNormalizedUserNameAsync(null, null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.CreateAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.UpdateAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.DeleteAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.FindByIdAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.FindByNameAsync(null, new CancellationToken()));
            Assert.ThrowsAny<NotImplementedException>(() => context.Dispose());

            var task = context.SaveChangesAsync();
            task.Wait();
            Assert.Equal(task.Result, 0);
        }
    }
}
