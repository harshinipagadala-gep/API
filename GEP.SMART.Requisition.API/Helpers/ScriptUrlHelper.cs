using System.Diagnostics.CodeAnalysis;
using GEP.SMART.Configuration;
using System;

namespace GEP.SMART.Requisition.API.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ScriptUrlHelper
    {
        private static string _gepCommonContentUrl { get { return MultiRegionConfig.GetConfig(CloudConfig.GEPContentURL); } }

        private static string _gepportalContentUrl { get { return MultiRegionConfig.GetConfig(CloudConfig.GEPContentURL); } }

        public static string PortalVersion { get { return MultiRegionConfig.GetConfig(CloudConfig.ScriptVersion); } }
        public static string GetP2PDocumentSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.P2PDocumentServiceURL);
        }

        public static string GetProcurementProfileSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.ProcurementProfileServiceURL);
        }

        public static string GetRequisitionSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.RequisitionServiceURL);
        }

        public static string GetNewRequisitionSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.SmartRequisitionServiceURL);
        }

        public static string GetQuestionBankServiceURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.QuestionBankServiceURL);
        }

        public static string PartnerServiceUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.PartnerServiceURL);
            }
        }

        public static Uri P2PDocumentServiceUrl
        {
            get
            {
                return new Uri(MultiRegionConfig.GetConfig(CloudConfig.P2PDocumentServiceURL));
            }
        }

        public static Uri InvoiceServiceUrl
        {
            get
            {
                return new Uri(MultiRegionConfig.GetConfig(CloudConfig.InvoiceServiceURL));
            }
        }

        public static string ScannedImageServiceStringUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.ScannedImageServiceURL);
            }
        }


        public static Uri OperationalBudgetServiceUrl
        {
            get
            {
                return new Uri(MultiRegionConfig.GetConfig(CloudConfig.OperationalBudgetServiceURL));
            }
        }

        public static Uri OrderServiceUrl
        {
            get
            {
                return new Uri(MultiRegionConfig.GetConfig(CloudConfig.OrderServiceURL));
            }
        }

        public static string GetWorkflowServiceUrl()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.WorkflowServiceURL);
        }

        public static string GetWorkflowRestServiceUrl()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.WorkFlowRestURL);
        }
        public static string GetPartnerSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.PartnerServiceURL).TrimEnd('/');
        }

        public static string GetInvoiceV2SvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.InvoiceV2ServiceURL).TrimEnd('/');
        }

        public static string GetInvoiceSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.InvoiceServiceURL);
        }

        public static string GetNewIRSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.NewIRServiceURL);
        }

        public static string GetIRSvcURL()
        {
            return MultiRegionConfig.GetConfig(CloudConfig.IRServiceURL);
        }

        public static string P2PCommonServiceStringUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.P2PCommonServiceURL);
            }
        }

        public static string PortalServiceUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.PortalServiceURL);
            }
        }

        public static string OrganizationServiceStringUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.OrganizationStructureServiceURL);
            }
        }

        public static string P2PDocumentServiceStringUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.P2PDocumentServiceURL);
            }
        }

        public static string OrderServiceStringUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.OrderServiceURL);
            }
        }

        public static string ASNServiceStringUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.ASNServiceURL);
            }
        }
        public static string cluster
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.cluster);
            }
        }

        public static string pusherKey
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.pusherKey);
            }
        }

    }
}
