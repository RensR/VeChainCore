using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VeChainCore.Models.Blockchain;
using VeChainCore.Models.Extensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nethereum.Hex.HexConvertors.Extensions;
using Utf8Json;
using Utf8Json.ImmutableCollection;
using Utf8Json.Resolvers;
using VeChainCore.Models.Core;
using VeChainCore.Utils.Json;

namespace VeChainCore.Client
{
    public class VeChainClient : IVeChainClient
    {
        public static readonly IJsonFormatterResolver JsonFormatterResolver
            = CompositeResolver.Create(
                new IJsonFormatter[]
                {
                    new InterfaceImmutableDictionaryFormatter<string, object>(),
                    new ImmutableDictionaryFormatter<string, object>(),
                    new ImmutableSortedDictionaryFormatter<string, object>()
                },
                new[]
                {
                    VeChainFormatterResolver.Instance,
                    ClauseFormatterResolver.Instance,
                    EnumResolver.Default,
                    ImmutableCollectionResolver.Instance,
                    BuiltinResolver.Instance,
                    AttributeFormatterResolver.Instance,
                    DynamicGenericResolver.Instance,
                    StandardResolver.ExcludeNullCamelCase,
                    DynamicObjectResolver.ExcludeNullCamelCase
                });

        /// <summary>
        /// The address of a running VeChain instance
        /// </summary>
        public Uri BlockchainAddress
        {
            get => _blockchainAddress;
            set => _blockchainAddress = value;
        }

        private Uri _blockchainAddress = new Uri("https://sync-mainnet.vechain.org");

        private readonly HttpClient _client = new HttpClient();

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
        /// Gets an <see cref="Account"/> object that contains all Account information for
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

        public async Task<string> GetLatestBlockRef()
        {
            var bestBlockId = await GetBlock("best");
            var bestBlockIdHex = bestBlockId.id.HexToByteArray();
            var eightByte = new byte[8];
            Unsafe.CopyBlock(ref eightByte[0], ref bestBlockIdHex[0], 8);
            return eightByte.TrimLeading().ToHex(true);
        }

        /// <summary>
        /// Gets the <see cref="Transaction"/> object that contains all Transaction information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Transaction> GetTransaction(string id)
            => await SendGetRequest<Transaction>($"/transactions/{id}");

        /// <summary>
        /// Gets the <see cref="Receipt"/> object that contains all Receipt information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Receipt> GetReceipt(string id)
            => await SendGetRequest<Receipt>($"/transactions/{id}/receipt");

        public async Task<HttpResponseMessage> TestNetFaucet(string address)
        {
            if (!Address.IsValid(address))
                throw new ArgumentException("Invalid address.", nameof(address));

            var json = new ByteArrayContent(JsonSerializer.Serialize(new {to = address}, JsonFormatterResolver));

            json.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return await _client.PostAsync("https://faucet.outofgas.io/requests", json);
        }

        /// <summary>
        /// Initiate a transaction to be included in the blockchain
        /// </summary>
        /// <param name="txn">The transaction</param>
        /// <returns></returns>
        public async Task<TransferResult> SendTransaction(Transaction txn)
        {
            var bytes = txn.RLPData;

            var rlp = new ByteArrayContent(bytes);

            //rlp.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await SendPostRequest("/transactions", rlp);

            response.EnsureSuccessStatusCode();

            return new TransferResult {id = response.ToString()};
        }

        /// <summary>
        /// Checks the results of a mock contract execution
        /// </summary>
        /// <param name="clauses">Transaction clauses</param>
        /// <returns></returns>
        public async Task<IEnumerable<CallResult>> ExecuteAddressCode(IEnumerable<Clause> clauses)
        {
            //var debugJson = JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(new {clauses}, JsonFormatterResolver));

            var json = JsonSerializer.Serialize(new {clauses}, JsonFormatterResolver);

            var content = new ByteArrayContent(json);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await SendPostRequest($"/accounts/*", content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsByteArrayAsync();

            return JsonSerializer.Deserialize<IEnumerable<CallResult>>(body, JsonFormatterResolver);
        }

        /// <summary>
        /// Send a GET request and retrieves an object of type T
        /// </summary>
        /// <typeparam name="T">The given object type e.g. Transaction, Block</typeparam>
        /// <param name="path">The path to the wanted object</param>
        /// <returns></returns>
        private async Task<T> SendGetRequest<T>(string path)
            => JsonSerializer.Deserialize<T>(await _client.GetByteArrayAsync(GetUri(path)), JsonFormatterResolver);

        /// <summary>
        /// Sends a POST request and return the chosen return type
        /// </summary>
        /// <param name="path">The path to the wanted object</param>
        /// <param name="httpContent">Parameters of the request</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendPostRequest(string path, HttpContent httpContent)
            => await _client.PostAsync(GetUri(path), httpContent);

        /// <summary>
        /// Transforms the path to the full URL including the blockchain address
        /// </summary>
        /// <param name="path">The query specific part of the URL</param>
        /// <returns></returns>
        private Uri GetUri(string path)
            => new Uri(BlockchainAddress, path);

        public void Dispose()
            => _client.Dispose();
    }
}