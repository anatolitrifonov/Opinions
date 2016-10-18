using BestFor.Domain.Entities;

namespace BestFor.Fakes
{
    /// <summary>
    /// Implements a fake dbset of known answers. Used in unit tests.
    /// </summary>
    public class FakeAnswerFlags : FakeDbSet<AnswerFlag>
    {
        public FakeAnswerFlags() : base()
        {
            Add(new AnswerFlag { Id = 1, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 2, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 3, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 4, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 5, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 6, AnswerId = 1, UserId = "A" });

            Add(new AnswerFlag { Id = 11, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 12, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 13, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 14, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 15, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 16, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 17, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 18, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 19, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 20, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 21, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 22, AnswerId = 1, UserId = "A" });
            Add(new AnswerFlag { Id = 23, AnswerId = 1, UserId = "A" });
        }
    }
}
