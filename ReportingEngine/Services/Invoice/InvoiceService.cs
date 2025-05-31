using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace ReportingEngine.Services
{
    /// <summary>
    /// Invoice Model class representing the invoice details.
    /// </summary>
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public decimal BillingFee { get; set; }
        public decimal BillingAmount { get; set; }
        public decimal BillingTotal { get; set; }
        public string FooterDate { get; set; }
    }
    /// <summary>
    /// Invoice Items Model class representing the items in an invoice.
    /// </summary>
    public class InvoiceItemsModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime ExcursionDate { get; set; }
        public string CountryName { get; set; }
        public int NoOfPersons { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string Voucher { get; set; }
    }

    /// <summary>
    /// InvoiceService class provides methods to retrieve invoice data for reporting.
    /// </summary>
    [DataObject]
    public class InvoiceService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceService"/> class.
        /// </summary>
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
            var today = DateTime.Today;
            var dueDate = today.AddDays(15);
            var footerDate = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt", CultureInfo.GetCultureInfo("en-US"));

            var allData = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    InvoiceNumber   = "INV001",
                    Date            = today,
                    DueDate         = dueDate,
                    CustomerNumber  = "CUST001",
                    CustomerName    = "ACME Corp",
                    CustomerAddress = "123 Main St, Springfield",
                    BillingFee      = 5.00m,
                    BillingAmount   = 300.00m,
                    BillingTotal    = 305.00m,
                    FooterDate      = footerDate
                },
                new InvoiceModel
                {
                    InvoiceNumber   = "INV002",
                    Date            = today,
                    DueDate         = dueDate,
                    CustomerNumber  = "CUST002",
                    CustomerName    = "Globex Inc",
                    CustomerAddress = "456 Elm St, Shelbyville",
                    BillingFee      = 7.50m,
                    BillingAmount   = 480.00m,
                    BillingTotal    = 487.50m,
                    FooterDate      = footerDate
                },
                new InvoiceModel
                {
                    InvoiceNumber   = "INV003",
                    Date            = today,
                    DueDate         = dueDate,
                    CustomerNumber  = "CUST003",
                    CustomerName    = "Initech",
                    CustomerAddress = "789 Oak St, Capital City",
                    BillingFee      = 10.00m,
                    BillingAmount   = 800.00m,
                    BillingTotal    = 810.00m,
                    FooterDate      = footerDate
                }
            };

            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                return allData.FindAll(i => i.InvoiceNumber.Equals(invoiceNumber, StringComparison.OrdinalIgnoreCase));
            }

            return allData;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<InvoiceItemsModel> GetInvoiceItemList(string invoiceNumber)
        {
            var today = DateTime.Today;

            var allData = new List<InvoiceItemsModel>
            {
                // INV001: 1 item
                new InvoiceItemsModel
                {
                    InvoiceNumber   = "INV001",
                    CountryName     = "USA",
                    NoOfPersons     = 2,
                    Price           = 150.00m,
                    Voucher         = "VCH100",
                    ExcursionDate   = today.AddDays(-7)
                },
                // INV002: 2 items
                new InvoiceItemsModel
                {
                    InvoiceNumber   = "INV002",
                    CountryName     = "USA",
                    NoOfPersons     = 2,
                    Price           = 120.00m,
                    Voucher         = "VCH200",
                    ExcursionDate   = today.AddDays(-10)
                },
                new InvoiceItemsModel
                {
                    InvoiceNumber   = "INV002",
                    CountryName     = "India",
                    NoOfPersons     = 2,
                    Price           = 120.00m,
                    Voucher         = "VCH201",
                    ExcursionDate   = today.AddDays(-2)
                },
                // INV003: 3 items
                new InvoiceItemsModel
                {
                    InvoiceNumber   = "INV003",
                    CountryName     = "USA",
                    NoOfPersons     = 1,
                    Price           = 200.00m,
                    Voucher         = "VCH300",
                    ExcursionDate   = today.AddDays(-5)
                },
                new InvoiceItemsModel
                {
                    InvoiceNumber   = "INV003",
                    CountryName     = "USA",
                    NoOfPersons     = 2,
                    Price           = 200.00m,
                    Voucher         = "VCH301",
                    ExcursionDate   = today.AddDays(-3)
                },
                new InvoiceItemsModel
                {
                    InvoiceNumber   = "INV003",
                    CountryName     = "Brazil",
                    NoOfPersons     = 1,
                    Price           = 200.00m,
                    Voucher         = "VCH302",
                    ExcursionDate   = today.AddDays(-3)
                }
            };

            // Calculate Amount for each item
            foreach (var item in allData)
            {
                item.Amount = item.NoOfPersons * item.Price;
            }

            if (!string.IsNullOrEmpty(invoiceNumber))
            {
                return allData.FindAll(i => i.InvoiceNumber.Equals(invoiceNumber, StringComparison.OrdinalIgnoreCase));
            }

            return allData;
        }
    }
}
