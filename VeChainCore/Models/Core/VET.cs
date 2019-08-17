namespace VeChainCore.Models.Core
{
    public class VET : Amount
    {
        public static readonly VET Unit = new VET(1);

        public new const int Decimals = 18;

        public new const string ContractAddress = null;
        public override decimal Value { get; }
        protected override string GetContractAddress() => ContractAddress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="preMultipliedPrecision">Indicate whether the precision is already taken into account</param>
        public VET(decimal value, bool preMultipliedPrecision = true)
            : base(Decimals)
        {
            Value = preMultipliedPrecision
                    ? value / _decimalsMultiplier
                    : value;
        }
    }
}