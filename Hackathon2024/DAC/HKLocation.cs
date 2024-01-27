using PX.Data;
using PX.Data.BQL;
using System;

namespace Hackathon2024
{
    [Serializable]
    [PXPrimaryGraph(typeof(HKLocationMaint))]
    [PXCacheName("Locations")]
    public class HKLocation : IBqlTable
    {
        #region ZIP
        public abstract class zIP : BqlString.Field<zIP>
        {
        }
        [PXDBString(5, IsUnicode = true, IsKey = true)]
        [PXUIField(DisplayName = "ZIP")]
        public virtual string ZIP { get; set; }
        #endregion

        #region Latitude
        public abstract class latitude : PX.Data.BQL.BqlDecimal.Field<latitude> { }

        /// <summary>
        /// The latitude coordinates that are entered for a location if the location does not contain the postal address or the postal address cannot be validated.
        /// The field value is filled using the Google or Bing lookup.
        /// </summary>
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "Latitude")]
        public virtual decimal? Latitude { get; set; }
        #endregion
        #region Longitude
        public abstract class longitude : PX.Data.BQL.BqlDecimal.Field<longitude> { }

        /// <summary>
        /// The longitude coordinates that are entered for a location if the location does not contain the postal address or the postal address cannot be validated.
        /// The field value is filled using the Google or Bing lookup.
        /// </summary>
        [PXDBDecimal(6)]
        [PXUIField(DisplayName = "Longitude")]
        public virtual decimal? Longitude { get; set; }
        #endregion

        #region NoteID
        public abstract class noteID : BqlGuid.Field<noteID>
        {
        }
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        #endregion

        #region CreatedByID
        public abstract class createdByID : BqlGuid.Field<createdByID>
        {
        }
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        #endregion

        #region CreatedByScreenID
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID>
        {
        }
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        #endregion

        #region CreatedDateTime
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime>
        {
        }
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastModifiedByID
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID>
        {
        }
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        #endregion

        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID>
        {
        }
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        #endregion

        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime>
        {
        }
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        #endregion

        #region tstamp
        public abstract class Tstamp : BqlByteArray.Field<Tstamp>
        {
        }
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        #endregion
    }
}
