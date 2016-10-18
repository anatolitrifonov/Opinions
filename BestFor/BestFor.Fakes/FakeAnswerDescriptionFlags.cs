using BestFor.Domain.Entities;

namespace BestFor.Fakes
{
    /// <summary>
    /// Implements a fake dbset of known answers. Used in unit tests.
    /// </summary>
    public class FakeAnswerDescriptionFlags : FakeDbSet<AnswerDescriptionFlag>
    {
        public FakeAnswerDescriptionFlags() : base()
        {
            Add(new AnswerDescriptionFlag { Id = 1, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 2, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 3, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 4, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 5, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 6, AnswerDescriptionId = 1 });

            Add(new AnswerDescriptionFlag { Id = 11, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 12, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 13, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 14, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 15, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 16, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 17, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 18, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 19, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 20, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 21, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 22, AnswerDescriptionId = 1 });
            Add(new AnswerDescriptionFlag { Id = 23, AnswerDescriptionId = 1 });
        }
    }
}
