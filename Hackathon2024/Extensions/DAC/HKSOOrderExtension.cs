

using PX.Data;
using PX.Objects.SO;
using System;

namespace Hackathon2024
{
    public class HKSOOrderExtension : PXCacheExtension<SOOrder>
    {
        public static bool IsActive() { return true; }

        #region UsrTotalCarbonFootprint
        [PXDBDecimal]
        [PXUIField(DisplayName = "Total Carbon Footprint", Enabled = false)]
        public virtual Decimal? UsrTotalCarbonFootprint { get; set; }
        public abstract class usrTotalCarbonFootprint : PX.Data.BQL.BqlDecimal.Field<usrTotalCarbonFootprint> { }
        #endregion
    }
}
