using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReportingEngine.Models.RequestModels;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using TR = Telerik.Reporting;

namespace TelerikReportEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ReportsControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReportServiceConfiguration _reportServiceConfiguration;
        private readonly IConfiguration _configuration;

        public ReportsController(IReportServiceConfiguration reportServiceConfiguration, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
            : base(reportServiceConfiguration)
        {
            _reportServiceConfiguration = reportServiceConfiguration;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        [HttpPost("RenderInvoice")]
        public IActionResult RenderInvoice([FromBody] InvoiceRequest request)
        {
            if (string.IsNullOrEmpty(request.ReportName) || string.IsNullOrEmpty(request.InvoiceNumber))
            {
                return BadRequest("Report name and invoice number are required.");
            }
            if (string.IsNullOrEmpty(request.ReportExportType))
            {
                request.ReportExportType = "PDF";
            }
            string reportPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", request.ReportName);

            if (!System.IO.File.Exists(reportPath))
            {
                return NotFound($"Report '{request.ReportName}' not found.");
            }
            var reportSource = new UriReportSource
            {
                Uri = reportPath
            };
            reportSource.Parameters.Add("InvoiceNumber", request.InvoiceNumber);

            var deviceInfo = new Hashtable
            {
              { "IncludeNonPrintedPages", true }
            };
            // 2) Render
            var processor = new ReportProcessor(_configuration);
            var result = processor.RenderReport(request.ReportExportType, reportSource, deviceInfo);

            // 3) Return PDF

            return File(result.DocumentBytes, "application/pdf", $"Invoice.{request.ReportExportType.ToLower()}");
        }
    }
}
