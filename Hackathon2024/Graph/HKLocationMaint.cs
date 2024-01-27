
using PX.Data;

namespace Hackathon2024
{
    public class HKLocationMaint : PXGraph<HKLocationMaint>
    {
        #region Actions

        public PXSave<HKLocation> Save;

        public PXCancel<HKLocation> Cancel;

        #endregion

        #region Views

        [PXImport(typeof(HKLocation))]
        public PXSelect<HKLocation> Locations;

        #endregion
    }
}
