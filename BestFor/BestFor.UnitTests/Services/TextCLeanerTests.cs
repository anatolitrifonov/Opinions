using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestFor.Services;
using System.Text;

namespace BestFor.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    public class TextCleanerTests
    {
        [Fact]
        public void TextCleanerTests_IndexKey_Indexes()
        {
            Assert.Null(TextCleaner.Clean(null));
            Assert.Equal(TextCleaner.Clean("    "), "    ");

            var test = ((char)147).ToString() + ((char)148).ToString() +
                ((char)8220).ToString() + ((char)8221).ToString() + " " +
                ((char)133).ToString() + ((char)8230).ToString() + " " +
                ((char)146).ToString() + ((char)8217).ToString() +
                ((char)145).ToString() + ((char)8216).ToString() + " " +
                ((char)8211).ToString();
            var result = TextCleaner.Clean(test);
            Assert.Equal(result, "\"\"\"\" ...... '''' -");
        }
    }
}
