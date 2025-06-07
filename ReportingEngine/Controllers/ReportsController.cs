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

            // 1. Determine requested type and whether to wrap
            var requestedType = string.IsNullOrEmpty(request.ReportExportType)
                ? "PDF"
                : request.ReportExportType.Trim().ToUpperInvariant();

            bool wrapPdfInHtml = requestedType == "HTML" || requestedType == "HTML5";

            // 2. Always render PDF if wrapping, otherwise render the requested format
            var exportType = wrapPdfInHtml ? "PDF" : requestedType;

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

            // For HTML/HTML5, wrap the PDF bytes in an iframe page
            if (wrapPdfInHtml)
            {
                var htmlPage = GeneratePdfIframePage(result.DocumentBytes, $"Invoice {request.InvoiceNumber}");
                // Convert to UTF‐8 bytes
                var htmlBytes = System.Text.Encoding.UTF8.GetBytes(htmlPage);

                // Create a timestamped filename
                var timestamp = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss");
                var fileName = $"Invoice-{timestamp}.html";

                // Return as downloadable HTML file
                return File(
                    htmlBytes,
                    "text/html",
                    fileName
                );
            }

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
        /// <summary>
        ///  Generates a simple HTML page that embeds the PDF in an iframe.
        /// </summary>
        /// <param name="pdfBytes"></param>
        /// <param name="title"></param>
        /// <returns></returns>
            private string GeneratePdfIframePage(byte[] pdfBytes, string title)
            {
                // 1) Convert PDF bytes to Base64
                string base64Pdf = Convert.ToBase64String(pdfBytes);

                // 2) HTML‐encode title for safety
                string safeTitle = WebUtility.HtmlEncode(title);

                // 3) Build the HTML with JS blob logic
                return $@"<!DOCTYPE html>
    <html lang=""en"">
    <head>
      <meta charset=""utf-8"" />
      <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
      <title>PDF Viewer – {safeTitle}</title>
      <style>
        html, body {{
          margin: 0; padding: 0;
          width: 100%; height: 100%;
        }}
        #pdf-viewer {{
          width: 100%; height: 100%;
          border: none;
        }}
      </style>
    </head>
    <body>
      <!-- Placeholder iframe; src set by JS -->
      <iframe id=""pdf-viewer"" title=""{safeTitle}""></iframe>

      <script>
        // Base64 PDF string
        const b64 = ""{base64Pdf}"";

        // Decode Base64 to binary
        const binary = atob(b64);
        const len = binary.length;
        const bytes = new Uint8Array(len);
        for (let i = 0; i < len; i++) {{
          bytes[i] = binary.charCodeAt(i);
        }}

        // Create a Blob and object URL
        const blob = new Blob([bytes], {{ type: 'application/pdf' }});
        const url  = URL.createObjectURL(blob);

        // Point the iframe at the blob URL
        document.getElementById('pdf-viewer').src = url;
      </script>
    </body>
    </html>";
            }

        [HttpGet("GetSupportedExportFormats")]
        public IActionResult GetSupportedExportFormats()
        {
            // Determine the runtime platform
            string platform =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "OSX" :
                "Unknown";

            bool isWindows = platform == "Windows";

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

            var response = new
            {
                Platform = platform,
                SupportedFormats = supportedFormats
            };

            return Ok(response);
        }
    }
}