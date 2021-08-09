using System;

namespace Abyss.Attributes
{
    public class EconomicImpactAttribute : Attribute
    {
        public EconomicImpactType ImpactType { get; }
        
        public EconomicImpactAttribute(EconomicImpactType type)
        {
            ImpactType = type;
        }
    }

    public enum EconomicImpactType
    {
        /// <summary>
        ///     Users can gain coins.
        /// </summary>
        UserGainCoins,
        
        /// <summary>
        ///     Users can spend coins.
        /// </summary>
        UserSpendCoins,
        
        /// <summary>
        ///     Users send coins to each other, or trade items of value (neutral effect on economy)
        /// </summary>
        UserCoinNeutral
    }
}