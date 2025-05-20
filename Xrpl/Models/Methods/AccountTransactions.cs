using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Xrpl.Client.Json.Converters;
using Xrpl.Models.Transactions;

//https://github.com/XRPLF/xrpl.js/blob/main/packages/xrpl/src/models/methods/accountTx.ts
//https://xrpl.org/account_tx.html
namespace Xrpl.Models.Methods
{
    /// <summary>
    /// Expected response from an  <see cref="AccountTransactionsRequest"/>.
    /// </summary>
    public class AccountTransactions  //todo rename to response
    {
        /// <summary>
        /// Unique Address identifying the related account.
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }
        /// <summary>
        /// The ledger index of the earliest ledger actually searched for  transactions.
        /// </summary>
        [JsonProperty("ledger_index_min")]
        public uint LedgerIndexMin { get; set; }
        /// <summary>
        /// The ledger index of the most recent ledger actually searched for  transactions.
        /// </summary>
        [JsonProperty("ledger_index_max")]
        public uint LedgerIndexMax { get; set; }
        /// <summary>
        /// The limit value used in the request.
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }
        /// <summary>
        /// Server-defined value indicating the response is paginated.<br/>
        /// Pass this  to the next call to resume where this call left off.
        /// </summary>
        [JsonProperty("marker")]
        public object Marker { get; set; }
        /// <summary>
        /// Array of transactions matching the request's criteria, as explained  below.
        /// </summary>
        [JsonProperty("transactions")]
        public List<TransactionSummary> Transactions { get; set; }
        /// <summary>
        /// If included and set to true, the information in this response comes from  a validated ledger version.<br/>
        /// Otherwise, the information is subject to  change.
        /// </summary>
        [JsonProperty("validated")]
        public bool Validated { get; set; }
    }

    public interface IAccountTransaction
    {
        /// <summary>
        /// The ledger close time represented in ISO 8601 time format.
        /// </summary>
        public DateTime CloseTimeIso { get; set; }

        /// <summary>
        /// The unique hash identifier of the transaction.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// (Validated transactions only) The identifying hash of the ledger version that includes this transaction
        /// </summary>
        public string LedgerHash { get; set; }

        /// <summary>
        /// (Validated transactions only) The ledger index of the ledger version that includes this transaction.
        /// </summary>
        public ulong? LedgerIndex { get; set; }

        /// <summary>
        /// (Validated transactions only) The transaction metadata, which shows the exact outcome of the transaction in detail.
        /// </summary>
        public Meta Meta { get; set; }
        /// <summary>
        /// (JSON mode) JSON object defining the transaction.
        /// </summary>
        public TransactionResponseCommon Transaction { get; }

        /// <summary>
        /// If true, this transaction is included in a validated ledger and its outcome is final.<br/>
        /// Responses from the transaction stream should always be validated.
        /// </summary>
        public bool Validated { get; set; }
    }

    public class TransactionSummary : IAccountTransaction //todo rename to AccountTransaction
    {
        /// <summary>
        /// The ledger close time represented in ISO 8601 time format.
        /// </summary>
        [JsonProperty("close_time_iso")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CloseTimeIso { get; set; }

        /// <summary>
        /// A hex string of the ledger version that included this transaction.
        /// </summary>
        [JsonProperty("ledger_hash")]
        public string LedgerHash { get; set; }
        /// <summary>
        /// The ledger index of the ledger version that included this transaction.
        /// </summary>
        [JsonProperty("ledger_index")]
        public ulong? LedgerIndex { get; set; }
        /// <summary>
        /// If binary is True, then this is a hex string of the transaction metadata.<br/>
        /// Otherwise, the transaction metadata is included in JSON format.
        /// </summary>
        [JsonProperty("meta")]
        [JsonConverter(typeof(MetaBinaryConverter))]
        public Meta Meta { get; set; }
        /// <summary>
        /// JSON object defining the transaction.
        /// </summary>
        [JsonProperty("tx_json")]
        public TransactionResponseCommon Transaction { get; set; }

        /// <summary>
        /// Unique hashed String representing the transaction.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }
        /// <summary>
        /// Whether or not the transaction is included in a validated ledger.<br/>
        /// Any transaction not yet in a validated ledger is subject to change.
        /// </summary>
        [JsonProperty("validated")]
        public bool Validated { get; set; }
    }
    /// <summary>
    /// The account_tx method retrieves a list of transactions that involved the  specified account.<br/>
    /// Expects a response in the form of a  <see cref="AccountTransactions"/>.
    /// </summary>
    /// <code>
    /// {
    /// 	"id": 2,
    /// 	"command": "account_tx",
    /// 	"account": "rLNaPoKeeBjZe2qs6x52yVPZpZ8td4dc6w",
    /// 	"ledger_index_min": -1,
    /// 	"ledger_index_max": -1,
    /// 	"binary": false,
    /// 	"limit": 2,
    /// 	"forward": false
    /// }
    /// </code>
    public class AccountTransactionsRequest : BaseLedgerRequest
    {
        public AccountTransactionsRequest(string account)
        {
            Account = account;
            Command = "account_tx";
            LedgerIndexMin = -1;
            LedgerIndexMax = -1;
        }
        /// <summary>
        /// A unique identifier for the account, most commonly the account's address.
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }
        /// <summary>
        /// Use to specify the earliest ledger to include transactions from.<br/>
        /// A value of -1 instructs the server to use the earliest validated ledger version available.
        /// </summary>
        [JsonProperty("ledger_index_min")]
        public int? LedgerIndexMin { get; set; }
        /// <summary>
        /// Use to specify the most recent ledger to include transactions from.<br/>
        /// A value of -1 instructs the server to use the most recent validated ledger version available.
        /// </summary>
        [JsonProperty("ledger_index_max")]
        public int? LedgerIndexMax { get; set; }
        /// <summary>
        /// If true, return transactions as hex strings instead of JSON.<br/>
        /// The default is false.
        /// </summary>
        [JsonProperty("binary")]
        public bool? Binary { get; set; }
        /// <summary>
        /// If true, returns values indexed with the oldest ledger first.<br/>
        /// Otherwise, the results are indexed with the newest ledger first.
        /// </summary>
        [JsonProperty("forward")]
        public bool? Forward { get; set; }
        /// <summary>
        /// Default varies.<br/>
        /// Limit the number of transactions to retrieve.<br/>
        /// The server is not required to honor this value.
        /// </summary>
        [JsonProperty("limit")]
        public int? Limit { get; set; }
        /// <summary>
        /// Value from a previous paginated response.<br/>
        /// Resume retrieving data where that response left off.<br/>
        /// This value is stable even if there is a change in the server's range of available ledgers.
        /// </summary>
        [JsonProperty("marker")]
        public object Marker { get; set; }

        /// <summary>
        /// Optional) Clio Only Return only transactions of a specific type,<br/>
        /// such as "Clawback", "AccountSet", "AccountDelete", et al. Case-insensitive.<br/>
        /// Supports any transaction type except AMM* (See Transaction Types https://xrpl.org/transaction-types.html)
        /// </summary>
        [JsonProperty("tx_type")]
        public string TxType { get; set; }
    }
}
