using BestFor.Domain.Entities;

namespace BestFor.Fakes
{
    public class FakeSearchEntries : FakeDbSet<SearchEntry>
    {
        public FakeSearchEntries() : base()
        {
            Add(new SearchEntry { Id = 1, SearchPhrase = "test2" });
        }
    }
}
