using PX.Common;

namespace Hackathon2024
{
    public static class Common
    {
        public static class OptimalRouteOpt
        {
            public const string Optimal = "Y";
            public const string NotOptimal = "N";
            public const string Undefined = "Z";

            public class optimal : PX.Data.BQL.BqlString.Constant<optimal>
            {
                public optimal() : base(Optimal) {; }
            }

            public class notOptimal : PX.Data.BQL.BqlString.Constant<notOptimal>
            {
                public notOptimal() : base(NotOptimal) {; }
            }

            public class undefined : PX.Data.BQL.BqlString.Constant<undefined>
            {
                public undefined() : base(Undefined) {; }
            }
        }

        [PXLocalizable]
        public class HKMessages
        {
            public const string RouteNotOptimal = "Route is not optimal for Carbon Footprint";
            public const string RouteOptimizationNotDefined = "Route CO2 optimization not defined";
        }
    }
}