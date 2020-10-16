namespace VeChainCore.Models.Core
{
    public class VTHO : Amount
    {
        public static readonly VTHO Unit = new VTHO(1);

        public new const int Decimals = 18;

        public static Contract Contract = new Contract
        {
            Address = "0x0000000000000000000000000000456e65726779"
        };

        public override decimal Value { get; }
        protected override string GetContractAddress() => ContractAddress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="preMultipliedPrecision">Indicate whether the precision is already taken into account</param>
        public VTHO(decimal value, bool preMultipliedPrecision = true)
            : base(Decimals)
        {
            Value =
                preMultipliedPrecision
                    ? value / _decimalsMultiplier
                    : value;
        }
    }
}