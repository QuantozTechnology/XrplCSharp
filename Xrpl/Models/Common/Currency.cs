using Newtonsoft.Json;

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Xrpl.Client.Extensions;
using Xrpl.Models.Utils;
using Xrpl.Utils.Hashes;

//https://xrpl.org/currency-formats.html#currency-formats
//https://github.com/XRPLF/xrpl.js/blob/main/packages/xrpl/src/models/common/index.ts

namespace Xrpl.Models.Common;

/// <summary>
/// The XRP Ledger has two kinds of digital asset: XRP and tokens.<br/>
/// Both types have high precision, although their formats are different
/// </summary>
public class Currency
{
    /// <summary>
    /// base constructor.<br/>
    /// base currency code = XRP
    /// </summary>
    public Currency() { CurrencyCode = "XRP"; }

    /// <summary>
    /// The standard format for currency codes is a three-character string such as USD.<br/>
    /// This is intended for use with ISO 4217 Currency Codes <br/>
    /// As a 160-bit hexadecimal string, such as "0158415500000000C1F76FF6ECB0BAC600000000".<br/>
    /// The following characters are permitted:<br/>
    /// all uppercase and lowercase letters, digits, as well as the symbols ? ! @ # $ % ^ * ( ) { } [ ] | and symbols ampersand, less, greater<br/>
    /// Currency codes are case-sensitive.
    /// </summary>
    [JsonProperty(propertyName: "currency")]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Quoted decimal representation of the amount of the token.<br/>
    /// This can include scientific notation, such as 1.23e11 meaning 123,000,000,000.<br/>
    /// Both e and E may be used.<br/>
    /// This can be negative when displaying balances, but negative values are disallowed in other contexts such as specifying how much to send.
    /// </summary>
    [JsonProperty(propertyName: "value")]
    public string Value { get; set; }

    /// <summary>
    /// Generally, the account that issues this token.<br/>
    /// In special cases, this can refer to the account that holds the token instead.
    /// </summary>
    [JsonProperty(propertyName: "issuer")]
    public string Issuer { get; set; }

    /// <summary>
    /// Readable currency name 
    /// </summary>
    [JsonIgnore]
    public string CurrencyValidName => CurrencyCode.CurrencyReadableName();

    /// <summary>
    /// decimal currency amount (drops for XRP)
    /// </summary>
    [JsonIgnore]
    public decimal ValueAsNumber
    {
        get
        {
            try
            {
                return string.IsNullOrWhiteSpace(Value)
                    ? 0
                    : decimal.Parse(
                        Value,
                        NumberStyles.AllowLeadingSign
                        | (NumberStyles.AllowLeadingSign & NumberStyles.AllowDecimalPoint)
                        | (NumberStyles.AllowLeadingSign & NumberStyles.AllowExponent)
                        | (NumberStyles.AllowLeadingSign & NumberStyles.AllowExponent & NumberStyles.AllowDecimalPoint)
                        | (NumberStyles.AllowExponent & NumberStyles.AllowDecimalPoint)
                        | NumberStyles.AllowExponent
                        | NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                try
                {
                    var num = double.Parse(
                        Value,
                        (NumberStyles.Float & NumberStyles.AllowExponent) | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture);
                    var valid = $"{num:#########e00}";
                    if (valid.Contains(value: "e-"))
                    {
                        return 0;
                    }

                    if (valid.Contains(value: '-'))
                    {
                        return decimal.MinValue;
                    }

                    return decimal.MaxValue;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }
        set => Value = value.ToString(
            CurrencyCode == "XRP"
                ? "G0"
                : "G15",
            CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// XRP token amount (non drops value)
    /// </summary>
    [JsonIgnore]
    public decimal? ValueAsXrp
    {
        get
        {
            if (CurrencyCode != "XRP" || string.IsNullOrWhiteSpace(Value))
            {
                return null;
            }

            return ValueAsNumber / 1000000;
        }
        set
        {
            if (value.HasValue)
            {
                CurrencyCode = "XRP";
                var val = value.Value * 1000000;
                Value = val.ToString(format: "G0", CultureInfo.InvariantCulture);
            }
            else
            {
                Value = "0";
            }
        }
    }

    #region Overrides of Object

    public override string ToString()
    {
        return CurrencyValidName == "XRP" ? $"XRP: {ValueAsXrp:0.######}" : $"{CurrencyValidName}: {ValueAsNumber:0.###############}";
    }

    public override bool Equals(object o) { return o is Currency model && model.Issuer == Issuer && model.CurrencyCode == CurrencyCode; }

    public static bool operator ==(Currency c1, Currency c2) { return c1.Equals(c2); }

    public static bool operator !=(Currency c1, Currency c2) { return !c1.Equals(c2); }

    #endregion
}

public static class CurrencyExtensions
{
    /// <summary>
    /// check that currency is NFT XLS14D
    /// </summary>
    /// <param name="cur"></param>
    /// <returns></returns>
    public static bool IsNFT14D(this Currency cur) => cur is { } c && c.CurrencyCode.StartsWith("02");
    //cur is { ValueAsNumber: 0.000000000000000000000000000000000000000000000000000000000000000000000000000000001m };

    /// <summary> get readable token code </summary>
    /// <param name="currencyCode">token code</param>
    /// <returns>readable token code</returns>
    public static string CurrencyReadableName(this string currencyCode)
    {
        if (!IsValidCurrencyCode(currencyCode))
        {
            return string.Empty;
        }

        return NormalizeCurrencyCode(currencyCode);
    }

    private static bool IsValidCurrencyCode(string currencyCode) =>
        !string.IsNullOrEmpty(currencyCode) && currencyCode.Length > 0;

    public static bool IsLpToken(this Currency currency)
    {
        return currency.CurrencyCode.IsLpToken();
    }
    public static bool IsLpToken(this string currencyCode)
    {
        return currencyCode.IsHexCurrencyCode() && currencyCode.StartsWith("03");
    }

    public static string NormalizeCurrencyCode(this string currencyCode, int maxLength = 20)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return currencyCode;

        // Стандартный 3-символьный код
        if (currencyCode.Length == 3)
        {
            return currencyCode.Trim();
        }

        // Проверка на 40-символьный шестнадцатеричный код
        if (currencyCode.IsHexCurrencyCode())
        {
            string hex = currencyCode;

            // Устаревший код с демереджем (начинается с 01)
            if (hex.StartsWith("01"))
            {
                return ConvertDemurrageToUTF8(currencyCode);
            }

            // XLS-16d NFT Metadata (начинается с 02)
            if (hex.StartsWith("02"))
            {
                string xlf15d = Encoding.UTF8.GetString(HexToBytes(hex)).Substring(8, Math.Min(maxLength, hex.Length / 2 - 8)).Trim();
                if (Regex.IsMatch(xlf15d, "[a-zA-Z0-9]{3,}") && xlf15d.ToLower() != "xrp")
                {
                    return xlf15d;
                }
            }

            if (hex.StartsWith("03"))
            {
                return $"LP {currencyCode[2..6]}..";
            }

            // Обычный шестнадцатеричный код
            var decodedHex = hex.FromHexString().Replace("\0", null).Trim('\0');
            if (string.IsNullOrWhiteSpace(decodedHex))
            {
                return currencyCode;
            }
            return decodedHex;
        }

        return currencyCode;
    }

    public static bool NormalCurrency(this string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return false;

        // Стандартный 3-символьный код
        if (currencyCode.Length == 3 && currencyCode.Trim().ToLower() != "xrp")
        {
            return true;
        }

        // Проверка на 40-символьный шестнадцатеричный код
        if (currencyCode.IsHexCurrencyCode())
        {
            string hex = currencyCode;

            // Устаревший код с демереджем (начинается с 01)
            if (hex.StartsWith("01"))
            {
                return false;
            }

            // XLS-16d NFT Metadata (начинается с 02)
            if (hex.StartsWith("02"))
            {
                return false;
            }

            if (hex.StartsWith("03"))
            {
                return false;
            }

            // Обычный шестнадцатеричный код
            return true;
        }

        return false;
    }
    static string ConvertDemurrageToUTF8(string demurrageCode)
    {
        byte[] bytes = HexToBytes(demurrageCode);
        string code = $"{(char)bytes[1]}{(char)bytes[2]}{(char)bytes[3]}";

        // Вычисление процентной ставки
        int interestStart = (bytes[4] << 24) | (bytes[5] << 16) | (bytes[6] << 8) | bytes[7];
        double interestPeriod = BitConverter.ToDouble(bytes.Skip(8).Take(8).Reverse().ToArray(), 0);
        const int yearSeconds = 31536000; // Фиксированное количество секунд в году
        double interestAfterYear = Math.Pow(Math.E, (interestStart + yearSeconds - interestStart) / interestPeriod);
        double interest = (interestAfterYear * 100) - 100;

        return $"{code} ({interest:F1}% pa)";
    }
    static byte[] HexToBytes(string hex)
    {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }

}