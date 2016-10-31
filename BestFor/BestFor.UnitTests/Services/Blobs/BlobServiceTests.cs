using BestFor.Services.Blobs;
using BestFor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BestFor.UnitTests.Services.Blobs
{
    public class BlobServiceTests
    {
        [Fact]
        public void BlobServiceTests_GetFileExtension_Nulls()
        {
            // Setup
            var settings = Common.AppSettings.ReadSettings();

            var service = new BlobService(settings, null);

            Assert.Null(service.GetFileExtension(null));
            Assert.Null(service.GetFileExtension("     "));
            Assert.Null(service.GetFileExtension("   \r  "));
            Assert.Null(service.GetFileExtension("null"));
            Assert.Null(service.GetFileExtension("nu.ll"));
            Assert.Equal(service.GetFileExtension("nu.jpg"), "jpg");
            Assert.Equal(service.GetFileExtension("nu.Jpg"), "jpg");
            Assert.Null(service.GetFileExtension("nu."));
            Assert.Equal(service.GetFileExtension(".Jpg"), "jpg");
        }
    }
}
