using System.Diagnostics.CodeAnalysis;
using GEP.SMART.Requisition.BusinessEntities;

namespace GEP.SMART.Requisition.API.Models
{
    [ExcludeFromCodeCoverage]
    public class BasicDetailsWithSettings  
    {    
        public BasicDetailsWithSettings(BasicDetails basicDetails)
        {
            this.BasicDetails = basicDetails;
        }
        public GEP.SMART.Settings.Entities.Settings Settings { get; set; }

        public BasicDetails BasicDetails { get; set; }
    }
}
