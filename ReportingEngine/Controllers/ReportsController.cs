using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReportingEngine.Models.RequestModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
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

        private static readonly Dictionary<string, string> ExportFormatContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { "PDF",   "application/pdf" },
            { "XLS",   "application/vnd.ms-excel" },
            { "XLSX",  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { "DOCX",  "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { "RTF",   "application/rtf" },
            { "PPTX",  "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { "CSV",   "text/csv" },
            { "TXT",   "text/plain" },
            { "HTML5", "text/html" },
            { "IMAGE", "image/tiff" } // Default to TIFF; adjust if needed
        };

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

            var exportType = string.IsNullOrEmpty(request.ReportExportType) ? "PDF" : request.ReportExportType.ToUpperInvariant();

            string reportPath = Path.Combine("Reports", request.ReportName);

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

            var processor = new ReportProcessor(_configuration);
            var result = processor.RenderReport(exportType, reportSource, deviceInfo);

            var contentType = ExportFormatContentTypes.TryGetValue(exportType, out var type)
                ? type
                : "application/octet-stream";

            var fileExtension = exportType.ToLowerInvariant() switch
            {
                "pdf" => "pdf",
                "xls" => "xls",
                "xlsx" => "xlsx",
                "docx" => "docx",
                "rtf" => "rtf",
                "pptx" => "pptx",
                "csv" => "csv",
                "txt" => "txt",
                "html5" => "html",
                "image" => "tiff", // Default; could be parameterized
                _ => exportType.ToLowerInvariant()
            };
            DateTime now = DateTime.Now;
            string usDateTimeFileName = now.ToString("MM-dd-yyyy_hh-mm-ss");
            return File(result.DocumentBytes, contentType, $"Invoice-{usDateTimeFileName}.{fileExtension}");
        }

        [HttpGet("SupportedExportFormats")]
        public IActionResult GetSupportedExportFormats()
        {
            // Determine if RTF is supported (only on Windows)
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var supportedFormats = new List<object>
    {
        new { Name = "PDF",   Description = "Portable Document Format", Extension = "pdf" },
        new { Name = "XLS",   Description = "Excel 97-2003 (legacy)", Extension = "xls" },
        new { Name = "XLSX",  Description = "Excel Open XML (modern Excel)", Extension = "xlsx" },
        new { Name = "DOCX",  Description = "Microsoft Word Open XML Format", Extension = "docx" },
        new { Name = "PPTX",  Description = "PowerPoint Presentation Format", Extension = "pptx" },
        new { Name = "CSV",   Description = "Comma Separated Values", Extension = "csv" },
        new { Name = "TXT",   Description = "Plain Text (tab-delimited by default)", Extension = "txt" },
        new { Name = "HTML5", Description = "HTML with CSS styling", Extension = "html" },
        new { Name = "IMAGE", Description = "Multi-page TIFF (default), or other image", Extension = "tiff" }
    };

            if (isWindows)
            {
                supportedFormats.Insert(4, new { Name = "RTF", Description = "Rich Text Format", Extension = "rtf" });
            }

            return Ok(supportedFormats);
        }
    }
}