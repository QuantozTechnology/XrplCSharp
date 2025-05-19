using System;

namespace Xrpl.BinaryCodec.Util
{
    /// <summary>
    /// Utility to encode/decode the 20-byte currency code.
    /// </summary>
    public static class CurrencyType
    {
        /// <summary>
        /// For XRP, returns all-zero 20 bytes; for IOU, expects a 40-character hex string.
        /// </summary>
        public static byte[] EncodeCurrency(string currency)
        {
            if (string.Equals(currency, "XRP", StringComparison.OrdinalIgnoreCase))
            {
                return new byte[20];
            }
            if (currency.Length != 40)
                throw new ArgumentException("IOU currency code must be 40 hex characters.");
            return currency.FromHex();
        }
    }
}