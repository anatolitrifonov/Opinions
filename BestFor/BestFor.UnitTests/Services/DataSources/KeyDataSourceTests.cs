using BestFor.Fakes;
using Autofac;
using BestFor.Data;
using BestFor.Domain;
using BestFor.Domain.Entities;
using BestFor.Services.DataSources;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics.CodeAnalysis;

namespace BestFor.UnitTests.Services.Datasources
{
    /// <summary>
    /// Unit tests for DefaultSuggestions object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class KeyDataSourceTests
    {
        public class TestSetup
        {
            public Repository<Answer> AnswerRepository;

            public TestSetup()
            {
                var dataContext = new FakeDataContext();
                AnswerRepository = new Repository<Answer>(dataContext);
            }
        }

        [Fact]
        public void KeyDataSourceTests_BasicTests_Pass()
        {
            var indexDataSource = new KeyDataSource<Answer>();

            Assert.False(indexDataSource.IsInitialized);
            Assert.ThrowsAny<NullReferenceException>(() => indexDataSource.Size);

            var setup = new TestSetup();

            // Size is number of distinct keys
            var count = setup.AnswerRepository.Active().Select(x => x.IndexKey).Distinct().Count();

            var result = indexDataSource.Initialize(setup.AnswerRepository, 0);
            Assert.True(indexDataSource.IsInitialized);
            Assert.Equal(count, result);

            // Can not initialize the same instance twice.
            Assert.ThrowsAny<Exception>(() => indexDataSource.Initialize(setup.AnswerRepository, 0));

            Assert.Equal(count, indexDataSource.Size);

            var items = indexDataSource.Items;
            Assert.Equal(count, items.Count());

        }

        [Fact]
        public void KeyDataSourceTests_LimitedInit_Initializes()
        {
            var setup = new TestSetup();

            var indexDataSource = new KeyDataSource<Answer>();
            var result = indexDataSource.Initialize(setup.AnswerRepository, 1);
            Assert.True(indexDataSource.IsInitialized);
            Assert.Equal(1, result);
        }

        [Fact]
        public void KeyDataSourceTests_OverInit_Initializes()
        {
            var setup = new TestSetup();

            var indexDataSource = new KeyDataSource<Answer>();
            var result = indexDataSource.Initialize(setup.AnswerRepository, 1000);
            Assert.True(indexDataSource.IsInitialized);
            // Size is number of distinct keys
            var count = setup.AnswerRepository.Active().Select(x => x.IndexKey).Distinct().Count();
            Assert.Equal(count, result);
        }

        [Fact]
        public void KeyDataSourceTests_Find_Finds()
        {
            var setup = new TestSetup();

            var indexDataSource = new KeyDataSource<Answer>();
            indexDataSource.Initialize(setup.AnswerRepository, 0);

            // Pick the first item
            var key = setup.AnswerRepository.Active().First().IndexKey;
            // make sure that index key from this item is only in the index once
            // Although generally this might not be the case if index has something with key + something.
            // But for now let's leave it like this. Will be interesting to see if tests break because
            // of that at some point.
            var result = indexDataSource.Find(key).Count();
            Assert.Equal(result, 1);

            // Search on null
            Assert.Null(indexDataSource.Find(null));
        }

        [Fact]
        public void KeyDataSourceTests_FindTopItems_Finds()
        {
            var setup = new TestSetup();

            var indexDataSource = new KeyDataSource<Answer>();
            indexDataSource.Initialize(setup.AnswerRepository, 0);

            // Search on null
            Assert.Null(indexDataSource.FindTopItems(null));

            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev1", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev2", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev3", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev4", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev5", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev6", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev7", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev8", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev9", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev10", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev11", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev12", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev13", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev14", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev15", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev16", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev17", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev18", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev19", Phrase = "A" });
            indexDataSource.Insert(new Answer() { LeftWord = "Andy", RightWord = "Goosev20", Phrase = "A" });

            var key = "Andy Goosev";
            var count = indexDataSource.FindTopItems(key).Count();
            Assert.Equal(count, KeyDataSource<Answer>.DEFAULT_TOP_COUNT);
        }

        [Fact]
        public void KeyDataSourceTests_FindExact_FindsOne()
        {
            var setup = new TestSetup();

            var indexDataSource = new KeyDataSource<Answer>();
            indexDataSource.Initialize(setup.AnswerRepository, 0);

            var answer = new Answer() { LeftWord = "Andy", RightWord = "Goosev1", Phrase = "A" };

            // Insert twice intentionally
            indexDataSource.Insert(answer);
            indexDataSource.Insert(answer);

            var result = indexDataSource.FindExact(answer.IndexKey);
            Assert.Equal(result.IndexKey, answer.IndexKey);

            result = indexDataSource.FindExact(answer.IndexKey.ToLower());
            Assert.Equal(result.IndexKey, answer.IndexKey);

            Assert.Null(indexDataSource.FindExact("junk"));
        }

        [Fact]
        public void KeyDataSourceTests_InsertNull_OK()
        {
            var dataSource = new KeyDataSource<Answer>();
            Assert.Null(dataSource.Insert(null));
            Assert.Null(dataSource.Find(null));
            Assert.Null(dataSource.FindTopItems(null));
            Assert.Null(dataSource.FindExact(null));
        }
    }
}
