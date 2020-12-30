using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Controllers
{
    public class ControllerHelper 
    {
        private IRequisitionManager requisitionManager;

        public ControllerHelper (IRequisitionManager requisitionManager)
        {
            this.requisitionManager = requisitionManager;
        }     

        public async Task<BusinessEntities.MethodResult<System.Data.DataTable>> GetMappedTaxItems(BusinessEntities.BasicDetails basicDetails, Settings.Entities.Settings settings)
        {

            if (settings.DocumentSettings.ContainsKey("ENTITYTYPE_TAX"))
            {
                var entityTypeTax = Convert.ToInt32(settings.DocumentSettings["ENTITYTYPE_TAX"] != null ? settings.DocumentSettings["ENTITYTYPE_TAX"] : 0);
                var entity = basicDetails.headerSplitAccountingFields.Find(c => c.EntityTypeId == entityTypeTax);
                long entityDetailCode = entity == null ? 0 : entity.EntityDetailCode;
                var mappedTaxItemsTask = await this.requisitionManager.GetTaxItemsByMappedEntity(entityDetailCode, entityTypeTax);
                return mappedTaxItemsTask;
            }
            else
            {
                return new SuccessMethodResult<DataTable>(new DataTable());
            }
        }
    }
}
