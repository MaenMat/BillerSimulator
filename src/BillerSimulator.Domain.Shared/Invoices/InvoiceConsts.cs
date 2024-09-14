namespace BillerSimulator.Invoices
{
    public static class InvoiceConsts
    {
        private const string DefaultSorting = "{0}InvoiceNumber asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Invoice." : string.Empty);
        }

    }
}