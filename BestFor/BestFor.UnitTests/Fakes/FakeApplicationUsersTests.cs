using BestFor.Domain.Entities;
using BestFor.Fakes;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BestFor.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    public class FakeApplicationUsersTests
    {
        [Fact]
        public void FakeApplicationUsers_Update_Updates()
        {
            var fake = new FakeApplicationUsers();
            var user = new ApplicationUser();

            EntityEntry<ApplicationUser> result = fake.Update(user);

            var result1 = fake.ElementType;
            var result2 = fake.Expression;
            var result3 = fake.Provider;
        }
    }
}
