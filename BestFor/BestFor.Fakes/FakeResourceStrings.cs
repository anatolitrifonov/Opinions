using BestFor.Domain.Entities;

namespace BestFor.Fakes
{
    /// <summary>
    /// Implements a fake dbset of known answers. Used in unit tests.
    /// </summary>
    public class FakeResourceStrings : FakeDbSet<ResourceString>
    {
        public const string UsCulture = "en-US";
        public const string RuCulture = "ru-RU";
        public const string RandomStringKey = "randomString";
        public const string RandomStringValueUs = "Random String";
        public const string RandomStringValueRu = "Случайная строка";

        public const string RandomPatternStringKey = "randomString2";
        public const string RandomPatternStringValueUs = "Test {{best_start_capital}}";
        public const string RandomPatternStringTestValueUs = "Test Best";


        public FakeResourceStrings() : base()
        {
            Add(new ResourceString { CultureName = UsCulture, Key = "a", Value = "A" });
            Add(new ResourceString { CultureName = UsCulture, Key = "best_start_capital", Value = "Best" });
            Add(new ResourceString { CultureName = UsCulture, Key = RandomStringKey, Value = RandomStringValueUs });
            Add(new ResourceString { CultureName = UsCulture, Key = RandomPatternStringKey, Value = RandomPatternStringValueUs });

            Add(new ResourceString { CultureName = RuCulture, Key = RandomStringKey, Value = RandomStringValueRu });
        }
    }
}
