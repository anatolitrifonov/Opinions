using BestFor.Domain.Entities;
using BestFor.Dto.Account;
using BestFor.Fakes;
using BestFor.Services.Cache;
using BestFor.Services.Services;
using BestFor.UnitTests.Testables;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace BestFor.UnitTests.Services.Services
{
    [ExcludeFromCodeCoverage]
    public class UserServiceTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            public UserService UserService;
            public Mock<ICacheManager> CacheMock;
            public TestLoggerFactory TestLoggerFactory;
            public UserManager<ApplicationUser> UserManager;

            public TestSetup()
            {
                CacheMock = new TestCacheManager().CacheMock;
                TestLoggerFactory = new TestLoggerFactory();
                UserManager = MoqHelper.GetTestUserManager();
                //UserService = new UserService(UserManager, TestLoggerFactory, CacheMock.Object, null);
                UserService = new UserService(UserManager, TestLoggerFactory, CacheMock.Object);
            }
        }

        [Fact]
        public void UserServiceTests_AddUserToCache_Adds()
        {
            // Setup
            var setup = new TestSetup();

            var user = new ApplicationUserDto() { UserId = "A" };
            setup.UserService.AddUserToCache(user);

            Assert.Equal(setup.UserService.FindById("A").UserId, "A");

            // this will give us a cached dictionary of users
            var cache = (Dictionary<string, ApplicationUser>)setup.CacheMock.Object.Get(CacheConstants.CACHE_KEY_USERS_DATA);

            // check that cache has the user
            Assert.NotNull(cache["A"]);

            // Check that random user is not there
            Assert.Null(setup.UserService.FindById("B"));
        }

        [Fact]
        public void UserServiceTests_FindAll_ReturnsAllUsers()
        {
            // Setup
            var setup = new TestSetup();

            var all = setup.UserService.FindAll();

            // Need to check that the number of users returned is the same as FakeUsers loads in constractor
            Assert.Equal(all.Count(), FakeApplicationUsers.DEFAUL_NUMBER_USERS);
        }

        [Fact]
        public void UserServiceTests_FindByDisplayName_FindsUser()
        {
            // Setup
            var setup = new TestSetup();

            // this loads from database so no reason to play with cache.
            // this loads from fakes only
            Assert.Equal(setup.UserService.FindDirectByDisplayName("Orsa").DisplayName, "Orsa");

            // Check that random user is not there
            Assert.Null(setup.UserService.FindDirectByDisplayName("B"));

            // Check that null gives null
            Assert.Null(setup.UserService.FindDirectByDisplayName(null));

            // Check that null gives null
            Assert.Null(setup.UserService.FindDirectByDisplayName(""));

            // Check that null gives null
            Assert.Null(setup.UserService.FindDirectByDisplayName("    "));

            // FYI IsNullOrWhiteSpace includes \n\r\t
            // Check that null gives null
            Assert.Null(setup.UserService.FindDirectByDisplayName("  \n  "));

            // Check that null gives null
            Assert.Null(setup.UserService.FindDirectByDisplayName("  \r  "));

            // Check that null gives null
            Assert.Null(setup.UserService.FindDirectByDisplayName("  \t  "));
        }

        [Fact]
        public void UserServiceTests_FindById_FindsUser()
        {
            // Setup
            var setup = new TestSetup();

            // Check that null gives null
            Assert.Null(setup.UserService.FindById(null));

            // Check that null gives null
            Assert.Null(setup.UserService.FindById(""));

            // Check that null gives null
            Assert.Null(setup.UserService.FindById("    "));

            // FYI IsNullOrWhiteSpace includes \n\r\t
            // Check that null gives null
            Assert.Null(setup.UserService.FindById("  \n  "));

            // Check that null gives null
            Assert.Null(setup.UserService.FindById("  \r  "));

            // Check that null gives null
            Assert.Null(setup.UserService.FindById("  \t  "));
        }

        [Fact]
        public void UserServiceTests_FindByIds_FindsUsers()
        {
            // Setup
            var setup = new TestSetup();

            var user = new ApplicationUserDto() { UserId = "A" };
            setup.UserService.AddUserToCache(user);

            var result = setup.UserService.FindByIds(new List<string>() { "A", "1", "222" });

            // Only two users should be returned because "222" is not there.
            Assert.Equal(result.Count, 2);

            Assert.Null(setup.UserService.FindByIds(null));

            result = setup.UserService.FindByIds(new List<string>());

            Assert.Equal(result.Count, 0);
        }
    }
}
