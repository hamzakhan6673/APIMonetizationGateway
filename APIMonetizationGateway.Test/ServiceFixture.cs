using APIMonetizationGateway.Services;
using Moq;

namespace APIMonetizationGateway.Test
{
    public class ServiceFixture
    {
        public ITierService TierService { get; }
        public IUsageService UsageService { get; }

        public ServiceFixture()
        {
            TierService = new Mock<ITierService>().Object;
            UsageService = new Mock<IUsageService>().Object;
        }
    }
}
