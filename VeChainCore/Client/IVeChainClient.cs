using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VeChainCore.Models.Blockchain;

namespace VeChainCore.Client
{
    public interface IVeChainClient : IDisposable
    {
        /// <summary>
        /// The address of a running VeChain instance
        /// </summary>
        Uri BlockchainAddress { get; set; }

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
        Task<Transaction> GetTransaction(string id);

        /// <summary>
        /// Gets the <see cref="Receipt"/> object that contains all Receipt information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        Task<Receipt> GetReceipt(string id);

        Task<HttpResponseMessage> TestNetFaucet(string address);

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
        /// <returns></returns>
        Task<IEnumerable<CallResult>> ExecuteAddressCode(IEnumerable<Clause> clauses);
    }
}