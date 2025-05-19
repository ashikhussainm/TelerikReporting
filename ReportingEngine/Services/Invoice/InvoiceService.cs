using Azure.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ReportingEngine.Services
{
    // Model representing a single invoice record in the report
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ExcursionDate { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CountryName { get; set; }
        public int NoOfPersons { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Voucher { get; set; }
        public decimal BillingFee { get; set; }
        public decimal BillingAmount { get; set; }
        public decimal BillingTotal { get; set; }
    }

    [DataObject]
    public class InvoiceService
    {
        public InvoiceService()
        {
        }


        /// <summary>
        /// Returns report data for the given invoiceNumber.
        /// Tagged so Telerik Reporting can discover it as a data method.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number parameter passed from the report.</param>
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<InvoiceModel> GetReportData(string invoiceNumber)
        {

            // Hard-coded sample data matching the report fields
            var allData = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    InvoiceNumber   = "INV001",
                    Date            = new DateTime(2025,5,16),
                    DueDate         = new DateTime(2025,5,31),
                    ExcursionDate   = new DateTime(2025,6,1),
                    CustomerNumber  = "CUST001",
                    CustomerName    = "ACME Corp",
                    CustomerAddress = "123 Main St, Springfield",
                    CountryName     = "USA",
                    NoOfPersons     = 2,
                    Price           = 150.00m,
                    Amount          = 300.00m,
                    Voucher         = "VCH100",
                    BillingFee      = 5.00m,
                    BillingAmount   = 305.00m,
                    BillingTotal    = 310.00m
                },
                new InvoiceModel
                {
                    InvoiceNumber   = "INV002",
                    Date            = new DateTime(2025,5,17),
                    DueDate         = new DateTime(2025,6,1),
                    ExcursionDate   = new DateTime(2025,6,5),
                    CustomerNumber  = "CUST002",
                    CustomerName    = "Globex Inc",
                    CustomerAddress = "456 Elm St, Shelbyville",
                    CountryName     = "Canada",
                    NoOfPersons     = 4,
                    Price           = 120.00m,
                    Amount          = 480.00m,
                    Voucher         = "VCH200",
                    BillingFee      = 7.50m,
                    BillingAmount   = 487.50m,
                    BillingTotal    = 495.00m
                }
            };

            // Filter by invoiceNumber parameter
            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                return allData.FindAll(i => i.InvoiceNumber.Equals(invoiceNumber, StringComparison.OrdinalIgnoreCase));
            }

            return allData;
        }
    }
}
