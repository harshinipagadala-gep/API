using GEP.Cumulus.Logging;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionCommonController : ControllerBase
    {
        public IRequisitionManager requisitionManager;
        public static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public RequisitionCommonController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders)
        {
            this.requisitionManager = requisitionManager;
            this.requisitionManager.UserContext = requestHeaders.Context;
        }

        [HttpGet("/api/RequisitionCommon/GetListofShipToLocDetails/")]
        public async Task<IActionResult> GetListofShipToLocDetails
            (long entityDetailCode, bool getByID, long lOBEntityDetailCode, int pageIndex, int pageSize, string searchText, int shipToLocID, long contactCode = 0)
        {
            if (string.IsNullOrEmpty(searchText))
                searchText = string.Empty;

            ShipToSearchQuery shipToSearchQuery = new ShipToSearchQuery()
            {
                entityDetailCode = entityDetailCode,
                getByID = getByID,
                lOBEntityDetailCode = lOBEntityDetailCode,
                pageIndex = pageIndex,
                pageSize = pageSize,
                searchText = searchText,
                shipToLocID = shipToLocID
            };
            return await requisitionManager
                 .GetListofShipToLocDetails(shipToSearchQuery.searchText, shipToSearchQuery.pageIndex, shipToSearchQuery.pageSize,
                 shipToSearchQuery.getByID, shipToSearchQuery.shipToLocID, shipToSearchQuery.lOBEntityDetailCode, shipToSearchQuery.entityDetailCode, contactCode)
                 .ToActionResultAsync();
        }

        [HttpGet("/api/RequisitionCommon/GetListofBillToLocDetails/")]

        public async Task<IActionResult> GetListofBillToLocDetails
            (long entityDetailCode, bool getDefault, long lOBEntityDetailCode, int pageIndex = 0, int pageSize = 100, string searchText = "", bool needJSON = false)
        {
            if (string.IsNullOrEmpty(searchText))
                searchText = string.Empty;

            BillToSearchQuery searchQuery = new BillToSearchQuery()
            {
                entityDetailCode = entityDetailCode,
                getDefault = getDefault,
                lOBEntityDetailCode = lOBEntityDetailCode,
                pageIndex = pageIndex,
                pageSize = pageSize,
                searchText = searchText
            };

            if (needJSON)
            {
                return await requisitionManager
                  .GetListofBillToLocDetails(searchQuery.searchText, searchQuery.pageIndex, searchQuery.pageSize, searchQuery.entityDetailCode, searchQuery.getDefault, searchQuery.lOBEntityDetailCode)
                  .ToActionResultAsync();
            }
            else
            {
                return await requisitionManager
                     .GetListofBillToLocDetailsWithDataSet(searchQuery.searchText, searchQuery.pageIndex, searchQuery.pageSize, searchQuery.entityDetailCode, searchQuery.getDefault, searchQuery.lOBEntityDetailCode)
                     .ToActionResultAsync();
            }
        }

        [HttpGet("/api/RequisitionCommon/GetDeliverToLocationById/{id}")]

        public async Task<IActionResult> GetDeliverToLocationById(int id)
        {
            return await requisitionManager
                .GetDeliverToLocationById(id)
                .ToActionResultAsync();
        }

        [HttpGet("/api/RequisitionCommon/GetTaxItemsByEntityID/")]

        public async Task<IActionResult> GetTaxItemsByEntityID(int shipToID, long headerEntityID, int headerEntityTypeID)
        {

            return await requisitionManager
                .GetTaxItemsByEntityID(shipToID, headerEntityID, headerEntityTypeID)
                .ToActionResultAsync();
        }


        [HttpGet("/api/RequisitionCommon/GetAllDeliverToLocations/")]

        public async Task<IActionResult> GetAllDeliverToLocations(string term, int shiptoLocId, int pageIndex=0, int pageSize=10)
        {
            string searchText = string.IsNullOrEmpty(term) ? string.Empty : term;

            DeliverToSearchQuery searchQuery = new DeliverToSearchQuery()
            {
                shiptoId = shiptoLocId,
                pageIndex = pageIndex,
                pageSize = pageSize,
                searchText = searchText
            };
            return await requisitionManager
                .GetAllDeliverToLocations(searchQuery.searchText, searchQuery.shiptoId, searchQuery.pageIndex, searchQuery.pageSize)
                .ToActionResultAsync();
        }

        [HttpGet("/api/RequisitionCommon/Test")]
        public string Test(long throwException = 0)
        {
            string result = string.Empty;
            try
            {
                result = DateTime.Now.ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'");
                if (throwException > 0)
                {
                    throw new Exception("Custom exception is thrown for testing with code - " + throwException.ToString());
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in Test Method in RequisitionCommonController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }

        [HttpGet("/api/RequisitionCommon/GetAllCurrency/")]
        public async Task<IActionResult> GetAllCurrency(string term, long partnerCode = 0)
        {
            return await requisitionManager
               .GetAllCurrency(term, partnerCode)
               .ToActionResultAsync();
        }

        /// <summary>
        /// Gets currency by currency code.
        /// </summary>
        /// <param name="request">Request object with required parameters.</param>
        /// <remarks>Gets currency by currency code.</remarks>
        [HttpGet("/api/RequisitionCommon/GetCurrencyByCurrencyCode")]
        public async Task<ActionResult<GetCurrencyByCodeResponse>> GetCurrencyByCurrencyCode(string CultureCode, string CurrencyCode)
        {
            GetCurrencyByCodeRequest request = new GetCurrencyByCodeRequest()
            {
                CultureCode = CultureCode,
                CurrencyCode = CurrencyCode
            };

            var response = await requisitionManager.GetCurrencyByCurrencyCode(request);
            return Ok(response);
        }

        [HttpGet("/api/RequisitionCommon/GetAllCurrencyData")]
        public async Task<IActionResult> GetAllCurrencyData(string term, long partnerCode = 0, int pageIndex = 1, int pageSize = 10)
        {
            return await requisitionManager
               .GetAllCurrencyData(partnerCode, term, pageIndex, pageSize)
               .ToActionResultAsync();
        }

        [HttpPost("/api/RequisitionCommon/GetRequisitionDetailsByP2PItemId")]
        public async Task<IActionResult> GetRequisitionDetailsByP2PItemId([FromBody] JArray data)
        {
            List<long> p2PLineItemIds = data.ToObject<List<long>>();
            return await requisitionManager
               .GetRequisitionDetailsByP2PItemId(p2PLineItemIds)
               .ToActionResultAsync();
        }

    }
}