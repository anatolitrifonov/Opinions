using System.Diagnostics.CodeAnalysis;
using BestFor.Fakes;
using BestFor.Domain;
using BestFor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BestFor.UnitTests.Fakes
{
    [ExcludeFromCodeCoverage]
    public class FakeDbSetTests
    {
        public class MyFakeDbSet : FakeDbSet<Answer>
        {

        }

        [Fact]
        public void FakeDbSetTests_Implementation_Basic()
        {
            var answer = new Answer();
            answer.ObjectState = ObjectState.Unchanged;
            var dbset = new MyFakeDbSet();
            dbset.Attach(answer);
            answer.ObjectState = ObjectState.Added;
            dbset.Attach(answer);
            answer.ObjectState = ObjectState.Deleted;
            dbset.Attach(answer);
            answer.ObjectState = ObjectState.Detached;
            Assert.ThrowsAny<ArgumentOutOfRangeException>(() => dbset.Attach(answer));
            answer.ObjectState = ObjectState.Modified;
            dbset.Attach(answer);

            var theType = dbset.ElementType;
        }
    }
}
