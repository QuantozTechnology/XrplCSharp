using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xrpl.Client.Exceptions;
using Xrpl.Models.Utils;

namespace Xrpl.Models.Transactions;
//https://github.com/XRPLF/xrpl.js/blob/main/packages/xrpl/src/models/transactions/NFTokenModify.ts
/// <inheritdoc cref="INFTokenModify" />
public class NFTokenModify : TransactionCommon, INFTokenModify
{
    public NFTokenModify()
    {
        TransactionType = TransactionType.NFTokenModify;
    }

    /// <inheritdoc />
    public string NFTokenID { get; set; }

    /// <inheritdoc />
    public string? Owner { get; set; }

    /// <inheritdoc />
    public string? URI { get; set; }
}

/// <summary>
/// NFTokenModify is used to change the URI field of an NFT to point to a different URI
/// in order to update the supporting data for the NFT.<br/>
/// The NFT must have been minted with the tfMutable flag set.<br/>
/// See Dynamic Non-Fungible Tokens.<br/>
/// </summary>
public interface INFTokenModify : ITransactionCommon
{
    /// <summary>
    /// Identifies the NFTokenID of the NFToken object that the offer references.
    /// </summary>
    string NFTokenID { get; set; }

    /// <summary>
    /// Indicates the AccountID of the account that owns the corresponding NFToken.
    /// Can be omitted if the owner is the account submitting this transaction.
    /// </summary>
    string? Owner { get; set; }

    /// <summary>
    /// URI that points to the data and/or metadata associated with the NFT.
    /// This must be a hex-encoded string, not an empty string.
    /// Can be null or omitted if not used.
    /// </summary>
    string? URI { get; set; }
}

/// <inheritdoc cref="INFTokenModify" />
public class NFTokenModifyResponse : TransactionResponseCommon, INFTokenModify
{
    public string NFTokenID { get; set; }

    public string? Owner { get; set; }

    public string? URI { get; set; }
}

public partial class Validation
{
    //https://github.com/XRPLF/xrpl.js/blob/3c19700c64d57a344655081b0bbc6b65bd3044f5/packages/xrpl/src/models/transactions/NFTokenModify.ts#L52
    /// <summary>
    /// Verify the form and type of an NFTokenModify at runtime.
    /// </summary>
    /// <param name="tx"> An NFTokenModify Transaction.</param>
    /// <exception cref="ValidationException">When the NFTokenModify is Malformed.</exception>
    public static async Task ValidateNFTokenModify(Dictionary<string, dynamic> tx)
    {
        await Common.ValidateBaseTransaction(tx);

        if (tx.TryGetValue("URI", out var URI) && URI is string {} uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ValidationException("URI: URI must not be an empty string");
            }
            if (!uri.IsHex())
            {
                throw new ValidationException("URI: URI must be in hex format");
            }
        }
    }
}
