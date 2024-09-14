using System;

namespace BillerSimulator.Customers;

[Serializable]
public class CustomerExcelDownloadTokenCacheItem
{
    public string Token { get; set; }
}