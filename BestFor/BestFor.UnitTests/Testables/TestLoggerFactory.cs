using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace BestFor.UnitTests.Testables
{
    [ExcludeFromCodeCoverage]
    public class TestLoggerFactory: ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger<object>();
        }

        public void Dispose()
        {
            // intentionally does nothing
        }
    }
}
