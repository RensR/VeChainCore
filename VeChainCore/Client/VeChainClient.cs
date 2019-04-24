using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VeChainCore.Models.Blockchain;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Nethereum.Hex.HexConvertors.Extensions;
using Utf8Json;
using Utf8Json.Formatters;
using Utf8Json.ImmutableCollection;
using Utf8Json.Resolvers;
using VeChainCore.Models.Core;
using VeChainCore.Utils.Json;
using Account = VeChainCore.Models.Blockchain.Account;

namespace VeChainCore.Client
{
    public class VeChainClient : IVeChainClient
    {
        public static readonly IJsonFormatterResolver JsonFormatterResolver
            = CompositeResolver.Create(
                VeChainFormatterResolver.Instance,
                ClauseFormatterResolver.Instance,
                AttributeFormatterResolver.Instance,
                EnumResolver.UnderlyingValue,
                ImmutableCollectionResolver.Instance,
                BuiltinResolver.Instance,
                DynamicGenericResolver.Instance,
                StandardResolver.ExcludeNullCamelCase
            );

        /// <summary>
        /// The address of a running VeChain instance
        /// </summary>
        public Uri ServerUri
        {
            get => _client.BaseAddress;
            set => _client.BaseAddress = value;
        }

        private readonly HttpClient _client = new HttpClient();

        public VeChainClient(string serverUri)
        {
            ServerUri = new Uri(serverUri);
        }

        public VeChainClient()
            : this("https://sync-mainnet.vechain.org")
        {
        }

        /// <summary>
        /// Gets the blockchain tag that indicates what network is connected; Main, Testnet or a dev instance
        /// </summary>
        /// <returns></returns>
        public async Task<Network> GetChainTag()
        {
            var genesis = await GetBlock("0");
            var lastByte = genesis.id.Substring(genesis.id.Length - 2);

            return (Network) byte.Parse(lastByte, System.Globalization.NumberStyles.HexNumber);
        }

        // Logic methods
        /// <summary>
        /// Gets an <see cref="Models.Blockchain.Account"/> object that contains all Account information for
        /// the given address.
        /// </summary>
        /// <param name="address">The address id in 0x notation</param>
        /// <param name="revision">The block number or ID to be able to look at past balances</param>
        /// <returns></returns>
        public async Task<Account> GetAccount(string address, string revision = "best")
        {
            if (!Address.IsValid(address))
                throw new ArgumentException("Address is not valid");

            if (revision != "best")
                address += $"?revision={revision}";

            return await SendGetRequest<Account>($"/accounts/{address}");
        }

        /// <summary>
        /// Gets the <see cref="Block"/> object that contains all Block information for
        /// the given block number
        /// </summary>
        /// <param name="blockNumber">The block number or "best" for the latest</param>
        /// <returns></returns>
        public async Task<Block> GetBlock(string blockNumber)
            => await SendGetRequest<Block>($"/blocks/{blockNumber}");

        public async Task<ulong> GetLatestBlockRef()
        {
            var bestBlockId = await GetBlock("best");
            var bestBlockIdHex = bestBlockId.id.HexToByteArray();
            var eightByte = new byte[8];
            Unsafe.CopyBlock(ref eightByte[0], ref bestBlockIdHex[0], 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(eightByte);
            return BitConverter.ToUInt64(eightByte);
        }

        /// <summary>
        /// Gets the <see cref="Transaction"/> object that contains all Transaction information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<TransactionLog> GetTransaction(string id)
            => await SendGetRequest<TransactionLog>($"/transactions/{id}");

        /// <summary>
        /// Gets the <see cref="Receipt"/> object that contains all Receipt information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Receipt> GetReceipt(string id)
            => await SendGetRequest<Receipt>($"/transactions/{id}/receipt");

        /// <summary>
        /// Initiate a transaction to be included in the blockchain
        /// </summary>
        /// <param name="txn">The transaction</param>
        /// <returns></returns>
        public async Task<TransferResult> SendTransaction(Transaction txn)
        {
            if (string.IsNullOrEmpty(txn.signature))
                throw new ArgumentException("Transaction must be signed.", nameof(txn.signature));

            var rlp = txn.RLPData;

            var json = JsonSerializer.Serialize(new {raw = rlp.ToHex(true)}, JsonFormatterResolver);

            var bytes = new ByteArrayContent(json);

            bytes.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await SendPostRequest("/transactions", bytes);

            await DetailedThrowOnUnsuccessfulResponse(response, bytes);

            return JsonSerializer.Deserialize<TransferResult>(await response.Content.ReadAsByteArrayAsync(), JsonFormatterResolver);
        }

        public IEnumerable<TransactionLog> GetTransfers(TransferCriteria[] criteriaSet, CancellationToken ct, ulong from = 0, ulong to = 9007199254740991, uint pageSize = 10, bool lazy = true)
        {
            return GetTransfers(out _, criteriaSet, ct, from, to, pageSize, lazy);
        }

        public IEnumerable<TransactionLog> GetTransfers(out Task fetchCompletion, TransferCriteria[] criteriaSet, CancellationToken ct, ulong from = 0, ulong to = 9007199254740991, uint pageSize = 10, bool lazy = true)
        {
            if (from >= to)
                throw new ArgumentOutOfRangeException(nameof(from), from, "From must be less than or equal to.");
            if (to > 9007199254740991)
                throw new ArgumentOutOfRangeException(nameof(to), to, "To must be less than or equal to JSON maximum safe integer (9007199254740991).");
            if (criteriaSet != null && criteriaSet.Length == 0)
                throw new ArgumentException("The criteriaSet parameter must be null or contain at least one criteria.", nameof(criteriaSet));


            var transfers = new BlockingCollection<TransactionLog>(new ConcurrentQueue<TransactionLog>());

            async Task FetchTransfers()
            {
                try
                {
                    for (uint offset = 0;; offset += pageSize)
                    {
                        var json = JsonSerializer.Serialize(new
                        {
                            range = new
                            {
                                unit = "block",
                                from,
                                to
                            },
                            options = new
                            {
                                offset,
                                limit = pageSize
                            },
                            criteriaSet,
                            order = "asc"
                        }, JsonFormatterResolver);

                        var content = new ByteArrayContent(json);

                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await SendPostRequest("/logs/transfer", content);

                        await DetailedThrowOnUnsuccessfulResponse(response, content);

                        var bytes = await response.Content.ReadAsByteArrayAsync();

                        var transferPage = JsonSerializer.Deserialize<TransactionLog[]>(bytes, JsonFormatterResolver);

                        if (transferPage.Length == 0)
                            break;

                        foreach (var transfer in transferPage)
                            transfers.Add(transfer, ct);

                        while (lazy && transfers.Count > 0)
                            await Task.Delay(15, ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    // ok
                }
                catch (ObjectDisposedException)
                {
                    // ok
                }
                finally
                {
                    try
                    {
                        transfers.CompleteAdding();
                    }
                    catch (ObjectDisposedException)
                    {
                        // ok
                    }
                }
            }

            fetchCompletion = FetchTransfers();

            return transfers.GetConsumingEnumerable(ct);
        }

        /// <summary>
        /// Checks the results of a mock contract execution
        /// </summary>
        /// <param name="clauses">Transaction clauses</param>
        /// <returns></returns>
        public async Task<IEnumerable<CallResult>> ExecuteAddressCode(IEnumerable<Clause> clauses)
        {
            //var debugJson = JsonSerializer.ToJsonString(new {clauses}, JsonFormatterResolver);

            var json = JsonSerializer.Serialize(new {clauses}, JsonFormatterResolver);

            var content = new ByteArrayContent(json);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //var debugJson = Encoding.UTF8.GetString(json);

            var response = await SendPostRequest("/accounts/*", content);

            await DetailedThrowOnUnsuccessfulResponse(response, content);

            var body = await response.Content.ReadAsByteArrayAsync();

            return JsonSerializer.Deserialize<IEnumerable<CallResult>>(body, JsonFormatterResolver);
        }

        private static async Task DetailedThrowOnUnsuccessfulResponse(HttpResponseMessage response, ByteArrayContent content)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {(uint) response.StatusCode} {response.ReasonPhrase}\n{response.Content.Headers}\n\n{await response.Content.ReadAsStringAsync()}\n\n")
                    {Data = {{typeof(HttpContent), content}}};
        }

        /// <summary>
        /// Send a GET request and retrieves an object of type T
        /// </summary>
        /// <typeparam name="T">The given object type e.g. Transaction, Block</typeparam>
        /// <param name="path">The path to the wanted object</param>
        /// <returns></returns>
        private async Task<T> SendGetRequest<T>(string path)
            => JsonSerializer.Deserialize<T>(await _client.GetByteArrayAsync(path), JsonFormatterResolver);

        /// <summary>
        /// Sends a POST request and return the chosen return type
        /// </summary>
        /// <param name="path">The path to the wanted object</param>
        /// <param name="httpContent">Parameters of the request</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendPostRequest(string path, HttpContent httpContent)
            => await _client.PostAsync(path, httpContent);

        public void Dispose()
            => _client.Dispose();
    }
}