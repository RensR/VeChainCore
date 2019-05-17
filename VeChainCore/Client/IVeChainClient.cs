using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Client
{
    public interface IVeChainClient : IDisposable
    {
        /// <summary>
        /// The address of a running VeChain instance
        /// </summary>
        Uri ServerUri { get; set; }

        /// <summary>
        /// Gets the blockchain tag that indicates what network is connected; Main, Testnet or a dev instance
        /// </summary>
        /// <returns></returns>
        Task<Network> GetChainTag();

        /// <summary>
        /// Gets an <see cref="Account"/> object that contains all Account information for
        /// the given address.
        /// </summary>
        /// <param name="address">The address id in 0x notation</param>
        /// <param name="revision">The block number or ID to be able to look at past balances</param>
        /// <returns></returns>
        Task<Account> GetAccount(string address, string revision = "best");

        /// <summary>
        /// Gets the contract token balance of a given contract address for an token holding account address.
        /// </summary>
        /// <param name="contract">The contract address id in 0x notation</param>
        /// <param name="account">The token holder address id in 0x notation</param>
        /// <param name="revision">The block number or ID to be able to look at past balances</param>
        /// <param name="decimalPlaces">The amount of decimal places to adjust the value down by (defaults to 18).</param>
        /// <returns></returns>
        Task<decimal> GetContractBalance(string contract, string account, string revision = "best", uint decimalPlaces = 18);

        /// <summary>
        /// Gets the <see cref="Block"/> object that contains all Block information for
        /// the given block number
        /// </summary>
        /// <param name="blockNumber">The block number or "best" for the latest</param>
        /// <returns></returns>
        Task<Block> GetBlock(string blockNumber);

        Task<ulong> GetLatestBlockRef();

        /// <summary>
        /// Gets the <see cref="Transaction"/> object that contains all Transaction information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        Task<TransactionLog> GetTransaction(string id);

        /// <summary>
        /// Gets the <see cref="Receipt"/> object that contains all Receipt information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        Task<Receipt> GetReceipt(string id);

        /// <summary>
        /// Initiate a transaction to be included in the blockchain
        /// </summary>
        /// <param name="txn">The transaction</param>
        /// <returns></returns>
        Task<TransferResult> SendTransaction(Transaction txn);

        /// <summary>
        /// Checks the results of a mock contract execution
        /// </summary>
        /// <param name="clauses">Transaction clauses</param>
        /// <param name="caller"></param>
        /// <param name="blockNum"></param>
        /// <param name="gasLimit"></param>
        /// <param name="gasPrice"></param>
        /// <returns></returns>
        Task<IEnumerable<CallResult>> ExecuteAddressCode(IEnumerable<Clause> clauses, ulong? blockNum = null, string caller = null, ulong? gasLimit = null, ulong? gasPrice = null);

        IEnumerable<Transfer> GetTransfers(TransferCriteria[] criteriaSet, CancellationToken ct, ulong from = 0, ulong to = 9007199254740991, uint pageSize = 10, bool lazy = true);

        IEnumerable<Transfer> GetTransfers(out Task fetchCompletion, TransferCriteria[] criteriaSet, CancellationToken ct, ulong from = 0, ulong to = 9007199254740991, uint pageSize = 10, bool lazy = true);

        IEnumerable<Event> GetEvents(EventCriteria[] criteriaSet, CancellationToken ct, ulong from = 0, ulong to = 9007199254740991, uint pageSize = 10, bool lazy = true);

        IEnumerable<Event> GetEvents(out Task fetchCompletion, EventCriteria[] criteriaSet, CancellationToken ct, ulong from = 0, ulong to = 9007199254740991, uint pageSize = 10, bool lazy = true);
    }
}