﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Autofac;
using BestFor.Controllers;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Dto.Account;
using BestFor.Fakes;
using BestFor.Services.Cache;
using BestFor.Services.DataSources;
using BestFor.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using BestFor.UnitTests.Testables;

namespace BestFor.UnitTests.Controllers
{
    [ExcludeFromCodeCoverage]
    public class VoteControllerTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            //public SuggestionService SuggestionService;
            //public FakeSuggestions FakeSuggestions;
            //public Mock<ICacheManager> CacheMock;
            //public Repository<Suggestion> Repository;
            //public TestLoggerFactory TestLoggerFactory;
            //public TestLogger<SuggestionService> TestLogger;

            public TestSetup()
            {
                //var dataContext = new FakeDataContext();
                //Repository = new Repository<Suggestion>(dataContext);
                //CacheMock = new Mock<ICacheManager>();

                ////LoggerMock = new Mock<ILogger<SuggestionService>>();
                ////LoggerMock.Setup(x => x.LogInformation(It.IsAny<string>()));
                //// LoggerMock.Setup(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()));

                //TestLogger = new TestLogger<SuggestionService>();
                //TestLoggerFactory = new TestLoggerFactory();

                //SuggestionService = new SuggestionService(CacheMock.Object, Repository, TestLoggerFactory);
                //FakeSuggestions = dataContext.EntitySet<Suggestion>() as FakeSuggestions;
            }
        }

        [Fact]
        public void VoteController_VoteAnswer_AddsVote()
        {
            const string USER_NAME = "asd";
            // Arrange
            var dataContext = new FakeDataContext();
            var testLoggerFactory = new TestLoggerFactory();
            var userManager = new UserManager<ApplicationUser>(dataContext, null, null, null, null, null, null, null, null);
            var identity = new GenericIdentity(USER_NAME);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "ASD"));
            var principal = new GenericPrincipal(identity, null);
            var httpContext = new DefaultHttpContext() { User = principal };
            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var userDto = new ApplicationUserDto() { UserName = USER_NAME };

            // Setup test data
            // var vote = new AnswerVoteDto() { AnswerId = 1 };


            // Setup user service
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(x => x.FindByUserName(It.IsAny<string>())).Returns(userDto);

            // Setup vote service
            var voteServiceMock = new Mock<IVoteService>();
            voteServiceMock.Setup(x => x.VoteAnswer(It.IsAny<AnswerVoteDto>())).Returns(
                new DataOperationResult()
                {
                    IsNew = true
                }
            );

            // Setup resource service
            var resourceServiceMock = new Mock<IResourcesService>();
            resourceServiceMock.Setup(x => x.GetString(It.IsAny<string>(), It.IsAny<string>())).Returns("A");

            // Setup statistics service
            var staticticsServiceMock = new Mock<IStatisticsService>();
            staticticsServiceMock.Setup(x => x.LoadUserStatictics(It.IsAny<ApplicationUserDto>()));

            var controller = new VoteController(userServiceMock.Object, voteServiceMock.Object, resourceServiceMock.Object,
                testLoggerFactory, staticticsServiceMock.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            // var result = controller.VoteAnswer(vote.AnswerId);
            var result = controller.VoteAnswer(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            // Controller did call the vote service
            voteServiceMock.Verify(x => x.VoteAnswer(It.IsAny<AnswerVoteDto>()), Times.Once());
            // Controller did call the resource service
            resourceServiceMock.Verify(x => x.GetString(BaseApiController.DEFAULT_CULTURE, Lines.THANK_YOU_FOR_VOTING), Times.Once());
        }
    }
}
