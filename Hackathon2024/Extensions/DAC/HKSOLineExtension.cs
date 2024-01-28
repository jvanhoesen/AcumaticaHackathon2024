using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using static Hackathon2024.Common;

namespace Hackathon2024
{
    public class HKSOLineExtension : PXCacheExtension<SOLine>
    {
        public static bool IsActive() { return true; }

        #region UsrCarbinFootprint
        [PXDBDecimal]
        [PXUIField(DisplayName = "Estimated CO2", Enabled = true)]
        [PXParent(typeof(SOLine.FK.Order))]
        [PXFormula(null, typeof(SumCalc<HKSOOrderExtension.usrTotalCarbonFootprint>))]
        public virtual Decimal? UsrCarbinFootprint { get; set; }
        public abstract class usrCarbinFootprint : PX.Data.BQL.BqlDecimal.Field<usrCarbinFootprint> { }
        #endregion

        #region UsrOptimalRoute
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Optimal CO2 Route", Enabled = false)]
        [PXStringList(
            new string[]
            {
               OptimalRouteOpt.Optimal, OptimalRouteOpt.NotOptimal, OptimalRouteOpt.Undefined
            },
            new string[]
            {
                "Optimal", "Not Optimal", "Undefined"
            })]
        public virtual string UsrOptimalRoute { get; set; }
        public abstract class usrOptimalRoute : PX.Data.BQL.BqlString.Field<usrOptimalRoute> { }
        #endregion

        #region UsrClosestWarehouse
        [PXDBInt]
        [PXUIField(DisplayName = "Closest Warehouse", Enabled = false)]
        [PXSelector(typeof(SelectFrom<INSite>.
                            Where<INSite.active.IsEqual<True>>.
                            SearchFor<INSite.siteID>),
                            SubstituteKey = typeof(INSite.siteCD))]
        public virtual int? UsrClosestWarehouse { get; set; }
        public abstract class usrClosestWarehouse : PX.Data.BQL.BqlInt.Field<usrClosestWarehouse> { }
        #endregion
    }
}
