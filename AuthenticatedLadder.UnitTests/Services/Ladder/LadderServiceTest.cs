using Moq;
using AuthenticatedLadder.DomainModels;

namespace AuthenticatedLadder.UnitTests.Services.Ladder
{
    public class LadderServiceTest
    {
        private ILadderRepository _repository;

        public LadderServiceTest()
        {
            _repository = Mock.Of<ILadderRepository>();
        }


    }
}
