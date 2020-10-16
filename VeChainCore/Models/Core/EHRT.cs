namespace VeChainCore.Models.Core
{
    public class EHRT : Amount
    {
        public static readonly EHRT Unit = new EHRT(1);

        public new const int Decimals = 18;

        public new const string ContractAddress = "0xf8e1fAa0367298b55F57Ed17F7a2FF3F5F1D1628";

        public static readonly Contract Contract = new Contract
        {
            Address = ContractAddress,
            Name = "Ehrt token contract"
        };
        public override decimal Value { get; }
        
        protected override string GetContractAddress() => ContractAddress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="preMultipliedPrecision">Indicate whether the precision is already taken into account</param>
        public EHRT(decimal value, bool preMultipliedPrecision = true)
            : base(Decimals)
        {
            Value =
                preMultipliedPrecision
                    ? value / _decimalsMultiplier
                    : value;
        }
    }
}
