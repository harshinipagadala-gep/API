using System.Diagnostics.CodeAnalysis;
using GEP.SMART.Configuration;
using System.Collections.Generic;

namespace GEP.SMART.Requisition.API.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ServiceUrlHelper
    {

        public static long buyerpartnercode;

        public static string getPartnerServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.PartnerServiceURL); }
        }
        public static string getCatalogRestURL
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SmartCatalogRestURL); }
        }
        public static string getSearchServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SearchServiceURL); }
        }

        public static string getRestServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.Smart2RestServiceURL); }
        }

        public static string getPortalRestServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.PortalRestServiceURL); }
        }

        public static string getPortalServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.PortalServiceURL); }
        }
        public static string getSearchRestServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SearchRestURL); }
        }

        public static string getCSMServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.CSMServiceURL); }
        }


        public static string getDocmentServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.P2PDocumentServiceURL); }
        }

        public static string getP2PCommonServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.P2PCommonServiceURL); }
        }
        public static string getRequisitionServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.RequisitionServiceURL); }
        }

        public static string getNewRequisitionServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SmartRequisitionServiceURL); }
        }

        public static string getNewOrderServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.OrderServiceURL); }
        }
        public static string getOrderServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SmartOrderServiceURL); }
        }
        public static string getSettingServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SettingServiceURL); }
        }

        public static string getInvoiceServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.InvoiceServiceURL); }
        }
        public static List<string> invoiceServiceUrl(long bpc)
        {
            buyerpartnercode = bpc;
            return getInvoiceV2ServiceUrl;


        }
        public static List<string> getInvoiceV2ServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(MultiRegionConfigTypes.PrimaryRegion, CloudConfig.InvoiceV2ServiceURL, buyerpartnercode); }
        }

        public static string getP2PRestURL
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.RestBaseURL); }
        }

        public static string getCatalogServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.CatalogServiceURL); }
        }

        public static string getDocumentUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.P2PDocumentURL); }
        }

        public static string getNewCatalogServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.NewCatalogServiceURL); }
        }

        public static string getBizTalkServiceEndPoint
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.BizTalkServiceURL); }
        }

        public static string getNewCatalogHomeUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.NewCatalogHomeURL); }
        }

        public static string getFileManagerServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.FileManagerServiceURL); }
        }
        public static string getAsposeLicencePath
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.AsposeLicensePath); }
        }
        public static string getBlobURL
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.BlobURL); }
        }
        public static string getFileDownloadURL
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.FileDownloadURL); }
        }
        public static string getDashboardServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.DashboardServiceURL); }
        }
        public static string getRTCServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SpendRTCServiceURL); }
        }
        public static string getTransactionExportServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.TransactionExportServiceURL); }
        }
        public static string getSpendAlertServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SpendAlertServiceURL); }
        }
        public static string getReportsServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.ReportsServiceURL); }
        }
        public static string getSpendDashboardPrefix
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SpendDashboardPrefix); }
        }
        public static string getPPSTServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.PPSTServiceURL); }
        }
        public static string getDownloadAttachmentServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.FileManagerServiceURL); }
        }
        public static string GetSourcing2APIUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.Sourcing2APIIntegrationURL); }
        }

        public static string getSpendAPIUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SpendAPIURL); }
        }

        public static string getAnalyticsAPIUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.AnalyticsAPIURL); }
        }
        public static string getASNServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.ASNServiceURL); }
        }

        public static string getIRServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.NewIRServiceURL); }
        }

        public static string getExecutionServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.ExecutionServiceURL); }
        }

        public static string getItemMasterRestServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.ItemMasterRestServiceURL); }
        }

        public static string GetImportExportUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.FileDownloadURL); }
        }
        public static string getScannedImageServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.ScannedImageServiceURL); }
        }
        public static string getOrganizaionServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.OrganizationStructureServiceURL); }
        }
        public static string getRestBaseURL
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.RestBaseURL); }
        }
        public static string SignalRUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.SignalRServiceURL); }
        }
        public static string GetAppId
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.appId); }
        }
        public static string GetSecret
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.secret); }
        }
        public static string GetPusherKey
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.pusherKey); }
        }

        public static string GetClusterKey
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.cluster); }
        }

        public static string GetProcurementProfileServiceUrl
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.ProcurementProfileServiceURL); }
        }

        public static string APIMSubscriptionKey
        {
            get { return MultiRegionConfig.GetConfig(CloudConfig.APIMSubscriptionKey); }
        }
        public static string PartnerServiceUrl
        {
            get
            {
                return MultiRegionConfig.GetConfig(CloudConfig.PartnerServiceURL);
            }
        }
    }
}
