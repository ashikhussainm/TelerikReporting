# ReportingEngine (.NET 6) with Telerik Reporting & Kendo UI Dashboard

**Author**: Ashik M Hussain
**Designation**: Technical Solutions Architect
**Medium Profile**: [@ashikmhussain.a](https://medium.com/@ashikmhussain.a)

---

## Overview

This project is a .NET 6 Web API and dashboard solution for generating and serving reports using Telerik Reporting and Kendo UI in both Windows and Linux Runtime. It is containerized with Docker and can be deployed to cloud platforms such as Railway.

---

## Features

* **Telerik Reporting Integration**: Generate reports (e.g., invoices) using the Telerik Reporting engine and serve them as downloadable files.
* **Kendo UI Dashboard**: Interactive analytics dashboard built with Kendo UI, available at `/index.html`.
* **REST API**: Expose endpoints for report rendering.
* **Dockerized**: Ready for containerized deployment.
* **CORS Enabled**: Allows cross-origin requests for easy integration with frontends.

---

## Project Structure

```
├── Controllers/
│   └── ReportsController.cs       # API controller for rendering reports
├── Services/
│   └── Invoice/InvoiceService.cs  # Provides sample invoice data for reports
├── Models/
│   └── RequestModels/
│       └── InvoiceRequest.cs      # Model for report rendering requests
├── Reports/
│   └── REInvoice.trdx             # Telerik report definition file
├── wwwroot/
│   └── index.html                 # Kendo UI dashboard frontend
├── LocalNugetPackages/            # Local NuGet packages for Telerik dependencies
├── Dockerfile                     # Multi-stage Docker build for the application
└── ReportingEngine.csproj         # Project file with dependencies and content configuration
```

---

## Hosting

This application is hosted on [Railway](https://railway.app/):

* **Kendo UI Dashboard**:
  [https://telerikreporting-test-f7bb.up.railway.app/index.html](https://telerikreporting-test-f7bb.up.railway.app/index.html)

* **Reporting Engine API**:
  **POST** [https://telerikreporting-test-f7bb.up.railway.app/api/reports/RenderInvoice](https://telerikreporting-test-f7bb.up.railway.app/api/reports/RenderInvoice)

---

## API Usage

### Render Invoice Report

**Endpoint**:
`POST /api/reports/RenderInvoice`

**Sample Request Payload**:

```json
{
  "invoiceNumber": "INV001",
  "reportName": "REInvoice.trdx",
  "reportExportType": "PDF"
}
```

**Response**:
Returns the generated report as a PDF file.

---

## Running Locally with Docker

1. **Build the Docker image**:

   ```bash
   ```

docker build -t reportingengine .

````
2. **Run the container**:
```bash
docker run -p 8080:8080 reportingengine
````

3. **Access the dashboard**:
   [http://localhost:8080/index.html](http://localhost:8080/index.html)
4. **Test the API**:
   Send a POST request to `http://localhost:8080/api/reports/RenderInvoice` with the JSON payload above.

---

## Notes

* The `Reports` folder and report definitions are included in the published output and Docker image.
* Local Telerik NuGet packages are used for licensing and are copied during the Docker build.
* The API and dashboard are both accessible from the same base URL.

---

## Dependencies

* .NET 6
* Telerik Reporting (Trial)
* Kendo UI (via CDN)
* Docker

---

## License

This project uses Telerik Reporting Trial packages. For production use, obtain a commercial license from [Telerik](https://www.telerik.com/).
