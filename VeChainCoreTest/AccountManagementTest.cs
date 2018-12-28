using Xunit;
using VeChainCore.Logic.Account;


namespace VeChainCoreTest
{
    public class AccountManagementTest
    {
        [Fact]
        public void GetNewVeChainAddress()
        {
            var key = AccountManagement.CreateNewPrivateKey();
            ;
        }
    }
}
