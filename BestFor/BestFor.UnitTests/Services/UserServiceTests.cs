using Microsoft.AspNetCore.Identity;
using Autofac;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Fakes;
using BestFor.Services;
using BestFor.Services.Cache;
using BestFor.Services.DataSources;
using BestFor.Services.Services;
using BestFor.UnitTests.Testables;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace BestFor.UnitTests.Services
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
                UserService = new UserService(CacheMock.Object, UserManager, TestLoggerFactory);
            }
        }

        [Fact]
        public void UserServiceTests_AddUserToCache_Adds()
        {
            // Setup
            var setup = new TestSetup();

            var user = new ApplicationUser() { Id = "A" };
            setup.UserService.AddUserToCache(user);

            Assert.Equal(setup.UserService.FindById("A").Id, "A");

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
        public void UserServiceTests_UpdateUserFromAnswer_UpdatesUser()
        {
            // Setup
            var setup = new TestSetup();

            var answer = new Answer();

            var result = setup.UserService.UpdateUserFromAnswer(answer);

            // result should be zero since there is no user id
            Assert.Equal(result, 0);

            // Set some random userid. And we should get zero again because random user is not in cache.
            answer.UserId = "asdasdasd";
            result = setup.UserService.UpdateUserFromAnswer(answer);
            // result should be zero since random user is not in cache.
            Assert.Equal(result, 0);

            // Fakes loads user with id DEFAUL_USER_ID
            answer.UserId = FakeApplicationUsers.DEFAUL_USER_ID;
            var user = setup.UserService.FindById(FakeApplicationUsers.DEFAUL_USER_ID);
            var currentCount = user.NumberOfAnswers;
            currentCount++;
            result = setup.UserService.UpdateUserFromAnswer(answer);
            Assert.Equal(result, 1);
            // We are verifying two things.
            // The fact that user object is the same in cache and that current is incremented.
            Assert.Equal(user.NumberOfAnswers, currentCount);
        }

        [Fact]
        public void UserServiceTests_FindByDisplayName_FindsUser()
        {
            // Setup
            var setup = new TestSetup();

            // this loads from database so no reason to play with cache.
            // this loads from fakes only
            Assert.Equal(setup.UserService.FindByDisplayName("Orsa").DisplayName, "Orsa");

            // Check that random user is not there
            Assert.Null(setup.UserService.FindByDisplayName("B"));

            // Check that null gives null
            Assert.Null(setup.UserService.FindByDisplayName(null));

            // Check that null gives null
            Assert.Null(setup.UserService.FindByDisplayName(""));

            // Check that null gives null
            Assert.Null(setup.UserService.FindByDisplayName("    "));

            // FYI IsNullOrWhiteSpace includes \n\r\t
            // Check that null gives null
            Assert.Null(setup.UserService.FindByDisplayName("  \n  "));

            // Check that null gives null
            Assert.Null(setup.UserService.FindByDisplayName("  \r  "));

            // Check that null gives null
            Assert.Null(setup.UserService.FindByDisplayName("  \t  "));
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

            var user = new ApplicationUser() { Id = "A" };
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
