# ReportingEngine (.NET 6) with Telerik Reporting & Kendo UI Dashboard

**Author**: Ashik M Hussain  
**Designation**: Technical Solutions Architect  
**Medium Profile**: [@ashikmhussain.a](https://medium.com/@ashikmhussain.a)

---

## Overview

This project is a .NET 6 Web API and dashboard solution for generating and serving reports using Telerik Reporting and Kendo UI in both Windows and Linux runtime environments. It is containerized with Docker and can be deployed to cloud platforms such as Railway. The reporting engine supports exporting to multiple formats (PDF, XLSX, DOCX, etc.), and exposes REST endpoints for flexible integration.

For a comprehensive walkthrough on designing your first Telerik report, see the official Getting Started guide:  
<https://docs.telerik.com/reporting/getting-started/first-steps-designing>

---

## Features

- **Telerik Reporting Integration**  
  - Generate and export reports (e.g., invoices) using the Telerik Reporting engine.  
  - Supports both Windows (GDI) and Linux (Skia + OpenXmlRendering) rendering pipelines.  
  - Expose report parameters (e.g., `InvoiceNumber`, `JsonPayload`) at runtime.  

- **Kendo UI Dashboard**  
  - Interactive analytics dashboard built with Kendo UI, accessible at `/index.html`.  
  - Charts, grids, and real-time data binding from the API.  

- **REST API Endpoints**  
  - `POST /api/reports/RenderInvoice` — Render a TRDX-based report to a chosen export format (PDF, XLSX, DOCX, etc.).  
  - `GET  /api/reports/GetSupportedExportFormats` — Retrieve the list of export formats supported by the current runtime (Windows or Linux).  

- **Dockerized**  
  - Multi-stage Dockerfile for building and running the application in a container.  
  - Environment-agnostic: Linux container with Skia, SkiaSharp, and OpenXmlRendering for headless report generation.  

- **CORS Enabled**  
  - Out-of-the-box CORS policy configured to allow cross-origin requests for easy frontend integration.  

---

## Project Structure

```

├── Controllers/
│   └── ReportsController.cs       # API controller for rendering reports & listing supported formats
├── Services/
│   └── Invoice/InvoiceService.cs  # Provides sample invoice data for reports (ObjectDataSource)
├── Models/
│   └── RequestModels/
│       └── InvoiceRequest.cs      # Model for report rendering requests (invoiceNumber, reportName, etc.)
├── Reports/
│   └── REInvoice.trdx             # Telerik report definition file (XML-based TRDX)
├── wwwroot/
│   └── index.html                 # Kendo UI dashboard frontend
├── LocalNugetPackages/            # Local NuGet packages for Telerik Reporting dependencies
├── Dockerfile                     # Multi-stage Docker build for the application
└── ReportingEngine.csproj         # Project file with dependencies and content configuration

```

---

## Hosting

This application is hosted on [Railway](https://railway.app/):

- **Kendo UI Dashboard**  
  <https://telerikreporting-test-f7bb.up.railway.app/index.html>

- **Reporting Engine API**  
  - **POST** <https://telerikreporting-test-f7bb.up.railway.app/api/reports/RenderInvoice>  
  - **GET**  <https://telerikreporting-test-f7bb.up.railway.app/api/reports/GetSupportedExportFormats>

---

## API Usage

### 1. Render Invoice Report

**Endpoint**:  
```

POST /api/reports/RenderInvoice

````

**Request Payload** (`application/json`):

```json
{
  "invoiceNumber":   "INV001",
  "reportName":      "REInvoice.trdx",
  "reportExportType": "PDF"
}
````

* **invoiceNumber** (string, required)
  The invoice number that drives the ObjectDataSource parameter (`InvoiceNumber`) in the TRDX.
* **reportName** (string, required)
  The file name of the TRDX report (e.g., `"REInvoice.trdx"`) located under the `Reports/` folder.
* **reportExportType** (string, optional)
  The export format (e.g., `"PDF"`, `"XLSX"`, `"DOCX"`, `"RTF"`, `"PPTX"`, `"CSV"`, `"TXT"`, `"HTML5"`, `"IMAGE"`). Defaults to `"PDF"` if omitted or empty.

**Sample cURL**:

```bash
curl -X POST https://<your-host>/api/reports/RenderInvoice \
  -H "Content-Type: application/json" \
  -d '{
        "invoiceNumber": "INV001",
        "reportName": "REInvoice.trdx",
        "reportExportType": "PDF"
      }' --output Invoice-Output.pdf
```

**Response**:
Returns a file stream (`application/pdf`, `application/vnd.openxmlformats-officedocument.wordprocessingml.document`, etc.) with a filename in the pattern `Invoice-MM-dd-yyyy_hh-mm-ss.{ext}`. For example:

```
Invoice-05-31-2025_02-15-30.pdf
```

---

### 2. Get Supported Export Formats

**Endpoint**:

```
GET /api/reports/GetSupportedExportFormats
```

**Description**:
Returns a JSON payload indicating the current OS platform (`Windows`, `Linux`, `OSX`, or `Unknown`) and the list of rendering extensions supported at runtime. On Windows, you’ll see all formats (PDF, XLS, XLSX, DOCX, RTF, PPTX, CSV, TXT, HTML5, IMAGE). On Linux (Skia + OpenXmlRendering), RTF and XLS (legacy) remain unsupported.

**Sample Response**:

```json
{
  "platform": "Linux",
  "supportedFormats": [
    { "name": "PDF",   "description": "Portable Document Format",            "extension": "pdf" },
    { "name": "XLSX",  "description": "Excel Open XML (modern Excel)",         "extension": "xlsx" },
    { "name": "DOCX",  "description": "Microsoft Word Open XML Format",       "extension": "docx" },
    { "name": "PPTX",  "description": "PowerPoint Presentation Format",       "extension": "pptx" },
    { "name": "CSV",   "description": "Comma Separated Values",               "extension": "csv" },
    { "name": "TXT",   "description": "Plain Text (tab-delimited by default)", "extension": "txt" },
    { "name": "HTML5","description": "HTML with CSS styling",                 "extension": "html" },
    { "name": "IMAGE","description": "Multi-page TIFF (default), or image",    "extension": "tiff" }
  ]
}
```

---

## Running Locally with Docker

1. **Build the Docker image**:

   ```bash
   docker build -t reportingengine .
   ```

2. **Run the container**:

   ```bash
   docker run -p 8080:8080 reportingengine
   ```

3. **Access the Kendo UI Dashboard**:
   [http://localhost:8080/index.html](http://localhost:8080/index.html)

4. **Test the API**:

   * Render Invoice: send a `POST http://localhost:8080/api/reports/RenderInvoice` with the JSON payload above.
   * Check Supported Formats: send a `GET http://localhost:8080/api/reports/GetSupportedExportFormats`.

---

## Notes

* The entire `Reports/` folder (including `REInvoice.trdx`) is included in the published output and Docker image.
* Local Telerik NuGet packages (in `LocalNugetPackages/`) are used for licensing and are copied during the Docker build.
* The application supports both Windows (GDI+) and Linux (Skia + OpenXmlRendering) rendering engines transparently.
* If running on Linux without `Telerik.Reporting.OpenXmlRendering`, certain formats such as RTF, XLS (legacy), and XPS will throw a `NotSupportedException`. Installing the OpenXmlRendering package restores DOCX, XLSX, and PPTX support.
* CORS is enabled by default, allowing integration with external frontends or services.

---

## Dependencies

* **.NET 6 SDK**
* **Telerik Reporting** (Trial or Licensed)
* **Kendo UI** (via CDN in `wwwroot/index.html`)
* **Telerik.Drawing.Skia** and **Telerik.Reporting.OpenXmlRendering** (for Linux support)
* **Docker** (for containerization)

---

## Getting Started with Telerik Report Design

If you’re new to Telerik Reporting and need a step-by-step tutorial on designing your first report (TRDX or TRDP), see:
[https://docs.telerik.com/reporting/getting-started/first-steps-designing](https://docs.telerik.com/reporting/getting-started/first-steps-designing)

This guide covers:

1. Installing the Standalone Report Designer.
2. Creating a new TRDX file.
3. Adding `ObjectDataSource` or `JsonDataSource` components.
4. Binding textboxes, tables, and charts to data fields.
5. Previewing and saving your report definition.

After designing your report, simply place the `.trdx` file into the `Reports/` folder and call the `RenderInvoice` endpoint to see it in action.

---

## License

This project uses Telerik Reporting Trial packages. For production use, obtain a commercial license from [Telerik](https://www.telerik.com/).

```
```
