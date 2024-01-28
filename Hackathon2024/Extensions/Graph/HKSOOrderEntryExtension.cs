using System;
using System.Collections;
using PX.Data;
using PX.Objects.IN;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;
using static Hackathon2024.Common;
using System.Device.Location;
using PX.Data.Licensing;
using System.Collections.Generic;
using System.Linq;

namespace Hackathon2024
{
    public class HKSOOrderEntryExt : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public static bool IsActive() { return true; }

        //ROW SELECTED - SOOrder
        public virtual void _(Events.RowSelected<SOOrder> e)
        {
            if (e.Row == null) return;

            //Set Fields Enabled
            PXUIFieldAttribute.SetEnabled<HKSOOrderExtension.usrTotalCarbonFootprint>(e.Cache, null, false);
        }

        //ROW SELECTED - SOLine
        public virtual void _(Events.RowSelected<SOLine> e)
        {
            if (e.Row == null) return;

            HKSOLineExtension soLineExt = ((SOLine)e.Row).GetExtension<HKSOLineExtension>();

            if (soLineExt != null)
            {
                switch (soLineExt.UsrOptimalRoute)
                {
                    case OptimalRouteOpt.Undefined:
                        e.Cache.RaiseExceptionHandling<SOLine.siteID>(e.Row, e.Row.SiteID,
                            new PXSetPropertyException(HKMessages.RouteOptimizationNotDefined, PXErrorLevel.Warning));
                        break;
                    case OptimalRouteOpt.NotOptimal:
                        e.Cache.RaiseExceptionHandling<SOLine.siteID>(e.Row, e.Row.SiteID,
                            new PXSetPropertyException(HKMessages.RouteNotOptimal, PXErrorLevel.Warning));
                        break;
                    default:
                        break;
                }
            }

            //Set Fields Enabled
            PXUIFieldAttribute.SetEnabled<HKSOLineExtension.usrCarbinFootprint>(e.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<HKSOLineExtension.usrClosestWarehouse>(e.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<HKSOLineExtension.usrOptimalRoute>(e.Cache, null, false);
        }

        //FIELD UPDATED - SOLine.inventoryID
        public virtual void _(Events.FieldUpdated<SOLine.inventoryID> e)
        {
            if (e.Row == null) return;

            SOOrder order = Base.Document.Current;
            SOLine currentLine = (SOLine)e.Row;
            SOShippingAddress customerAddress = Base.Shipping_Address.Current;

            //Set Closest Warehouse
            int? closestWarehouse = GetClosestWarehouse(currentLine.InventoryID, currentLine.Qty ?? 0, customerAddress);
            e.Cache.SetValueExt<HKSOLineExtension.usrClosestWarehouse>(e.Row, closestWarehouse);

            //Set Optimal Route Status
            if (closestWarehouse == null)
            {
                e.Cache.SetValueExt<HKSOLineExtension.usrOptimalRoute>(e.Row, OptimalRouteOpt.Undefined);
            }
            else
            {
                if (currentLine.SiteID == closestWarehouse)
                {
                    e.Cache.SetValueExt<HKSOLineExtension.usrOptimalRoute>(e.Row, OptimalRouteOpt.Optimal);
                }
                else
                {
                    e.Cache.SetValueExt<HKSOLineExtension.usrOptimalRoute>(e.Row, OptimalRouteOpt.NotOptimal);
                }
            }
        }

        public virtual void _(Events.FieldUpdated<SOLine.orderQty> e)
        {
            if (e.Row == null) return;

            SOLine currentLine = (SOLine)e.Row;

            //Set Closest Warehouse
            SOShippingAddress customerAddress = Base.Shipping_Address.Current;
            int? closestWarehouse = GetClosestWarehouse(currentLine.InventoryID, currentLine.Qty ?? 0, customerAddress);
            e.Cache.SetValueExt<HKSOLineExtension.usrClosestWarehouse>(e.Row, closestWarehouse);
        }

        //FIELD UPDATED - SOLine.siteID
        public virtual void _(Events.FieldUpdated<SOLine.siteID> e)
        {
            if (e.Row == null) return;

            SOOrder order = Base.Document.Current;
            SOLine currentLine = (SOLine)e.Row;
            HKSOLineExtension soLineExt = ((SOLine)e.Row).GetExtension<HKSOLineExtension>();

            //Set Closest Warehouse
            int? closestWarehouse = soLineExt.UsrClosestWarehouse;

            //Set Optimal Route Status
            if (closestWarehouse == null)
            {
                e.Cache.SetValueExt<HKSOLineExtension.usrOptimalRoute>(e.Row, OptimalRouteOpt.Undefined);
            }
            else
            {
                if (currentLine.SiteID == closestWarehouse)
                {
                    e.Cache.SetValueExt<HKSOLineExtension.usrOptimalRoute>(e.Row, OptimalRouteOpt.Optimal);
                }
                else
                {
                    e.Cache.SetValueExt<HKSOLineExtension.usrOptimalRoute>(e.Row, OptimalRouteOpt.NotOptimal);
                }
            }
        }

        //Action -  Optimize All order lines
        public PXAction<SOOrder> optimizeCarbonFootprint;
        [PXUIField(DisplayName = "GO GREEN", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable OptimizeCarbonFootprint(PXAdapter adapter)
        {
            Dictionary<int, List<SOLine>> closestSites = new Dictionary<int, List<SOLine>>();
            Dictionary<int, List<SOLine>> remainingSites = new Dictionary<int, List<SOLine>>();
            List<int> items = new List<int>();

            SOShippingAddress customerAddress = Base.Shipping_Address.Current;

            foreach(SOLine line in Base.Transactions.Select())
            {
                //Make a list of inventory items being utilized
                if(!items.Contains(line.InventoryID.Value)) items.Add(line.InventoryID.Value);

                //Find the closest warehouse to fulfill
                int warehouseID = GetClosestWarehouse(line.InventoryID, line.Qty ?? 0, customerAddress) ?? 0;
                if(!closestSites.ContainsKey(warehouseID))
                {
                    closestSites.Add(warehouseID, new List<SOLine>());
                }
                closestSites[warehouseID].Add(line);
            }

            bool calculating = true;

            while (calculating)
            {
                //Find warehouse that has most types of inventory items available from shipment
                int itemCount = 0;
                int bestRouteID = 0;
                foreach (int warehouse in closestSites.Keys)
                {
                    if (closestSites[warehouse].Count > itemCount)
                    {
                        bestRouteID = warehouse;
                        itemCount = closestSites[warehouse].Count;
                    }
                }

                SOLine updatedLine;
                //Update each line to have best warehouse
                foreach (SOLine item in closestSites[bestRouteID])
                {
                    item.GetExtension<HKSOLineExtension>().UsrClosestWarehouse = bestRouteID;
                    item.GetExtension<HKSOLineExtension>().UsrOptimalRoute = OptimalRouteOpt.Optimal;
                    item.SiteID = bestRouteID;

                    updatedLine = Base.Transactions.Update(item);
                }

                remainingSites = new Dictionary<int, List<SOLine>>(closestSites);//closestSites;
                foreach(int warehouse in remainingSites.Keys)
                {
                    if(closestSites.ContainsKey(bestRouteID))
                    {
                        closestSites[warehouse] = closestSites[warehouse].Except(closestSites[bestRouteID]).ToList();
                    }
                    if (closestSites[warehouse].Count == 0) closestSites.Remove(warehouse);
                }
                if (closestSites.Count == 0) calculating = false;
            }
            return adapter.Get();
        }

        #region Internal Methods

        //Get Route Carbon Footprint value, based on the distance
        public virtual decimal GetRouteCarbonFootprint()
        {
            decimal travelDistance = 0M;

            var sCoord = new GeoCoordinate(0, 0);
            var eCoord = new GeoCoordinate(0, 0);

            travelDistance = Convert.ToDecimal(sCoord.GetDistanceTo(eCoord));

            return travelDistance;
        }

        //Get the closest Warehouse for the Customer
        public virtual int? GetClosestWarehouse(int? inventoryItem, decimal Qty, SOShippingAddress customerAddress)
        {
            if (inventoryItem == null) return null;

            int? closestWarehouse = null;
            decimal closestDistance = -1;

            //Get List of Warehouses for InventoryItem
            //Loop thorugh warehouse and get closest warehouse, set closestWarehouse
            foreach (INSiteStatus siteStatus in SelectFrom<INSiteStatus>.
                                                   Where<INSiteStatus.qtyAvail.IsGreaterEqual<P.AsInt>.
                                                    And<INSiteStatus.inventoryID.IsEqual<P.AsInt>>.
                                                    And<INSiteStatus.siteID.IsNotEqual<P.AsInt>>>.
                                                   View.Select(Base, Qty, inventoryItem, 209))
            {
                decimal customerDistance = GetCustomerDistanceFromWarehouse(siteStatus.SiteID, customerAddress);

                if (closestDistance < 0) closestDistance = customerDistance + 1;

                if (customerDistance < closestDistance)
                {
                    closestDistance = customerDistance;
                    closestWarehouse = siteStatus.SiteID;
                }
            }
            return closestWarehouse;
        }

        //Get distance from the warehouse to the customer
        public virtual decimal GetCustomerDistanceFromWarehouse(int? siteID, SOShippingAddress customerAddress)
        {
            decimal travelDistance = 0M;
            GeoCoordinate customerCoor = null;
            GeoCoordinate warehouseCoor = null;

            INSite site = PXSelectReadonly<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.Select(Base, siteID);
            if(site != null)
            {
                Address warehouseAddress = PXSelectReadonly<Address, Where<Address.bAccountID, Equal<Required<Address.bAccountID>>, And<Address.addressID, Equal<Required<Address.addressID>>>>>.Select(Base, site.BAccountID, site.AddressID);
                if(warehouseAddress != null)
                {
                    HKLocation warehouseLocation = PXSelectReadonly<HKLocation, Where<HKLocation.zIP, Equal<Required<HKLocation.zIP>>>>.Select(Base, warehouseAddress.PostalCode);
                    if(warehouseLocation != null)
                    {
                        warehouseCoor = new GeoCoordinate((double)warehouseLocation.Latitude.Value, (double)warehouseLocation.Longitude.Value);
                    }
                }
            }

            HKLocation customerLocation = PXSelectReadonly<HKLocation, Where<HKLocation.zIP, Equal<Required<HKLocation.zIP>>>>.Select(Base, customerAddress.PostalCode);
            if (customerLocation != null)
            {
                customerCoor = new GeoCoordinate((double)customerLocation.Latitude.Value, (double)customerLocation.Longitude.Value);
            }

            travelDistance = Convert.ToDecimal(warehouseCoor.GetDistanceTo(customerCoor));

            return travelDistance;
        }
        #endregion
    }
}
