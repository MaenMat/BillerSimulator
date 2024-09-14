using System;

namespace BillerSimulator.Invoices;

[Serializable]
public class InvoiceExcelDownloadTokenCacheItem
{
    public string Token { get; set; }
}