namespace ReportingEngine.Models.RequestModels
{
    public class InvoiceRequest
    {
        public string InvoiceNumber { get; set; }

        public string ReportName { get; set; }

        public string ReportExportType { get; set; }
    }
}
