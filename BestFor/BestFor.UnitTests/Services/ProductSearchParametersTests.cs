using BestFor.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BestFor.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    public class ProductSearchParametersTests
    {
        [Fact]
        public void ProductSearchParametersTests_IndexKey_Indexes()
        {
            // Setup
            var setup = new ProductSearchParameters();
            Assert.ThrowsAny<Exception>(() => setup.IndexKey);
            setup.Keyword = "     ";
            Assert.ThrowsAny<Exception>(() => setup.IndexKey);
            setup.Keyword = "a";
            Assert.Equal(setup.IndexKey, setup.Keyword);
            setup.Category = "z";
            Assert.Equal(setup.IndexKey, setup.Keyword + "_" + setup.Category);
        }
    }
}
