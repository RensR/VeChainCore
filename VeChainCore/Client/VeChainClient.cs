using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using VeChainCore.Logic;
using VeChainCore.Models;

namespace VeChainCore.Client
{
    public class VeChainClient
    {
        private string _blockchainAddress = "http://localhost:8669";

        private readonly HttpClient _client = new HttpClient();

        // Config methods
        /// <summary>
        /// Sets the address of the blockchain that the client is interacting with.
        /// </summary>
        /// <param name="address">The address of the blockchain, by default "http://localhost:8669"</param>
        public void SetBlockchainAddress(string address)
        {
            _blockchainAddress = address.TrimEnd('/');
        }

        /// <summary>
        /// Gets the address of the blockchain that the client is interacting with.
        /// </summary>
        /// <returns></returns>
        public string GetBlockchainAddress()
        {
            return _blockchainAddress;
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
            if (address == null || address.Length != 42)
                throw new ArgumentException("Address is not valid");

            if (revision != "best")
                address += $"?revision={revision}";

            return await SendGetRequest<Account>($"{_blockchainAddress}/accounts/{address}");
        }

        /// <summary>
        /// Gets the <see cref="Block"/> object that contains all Block information for
        /// the given block number
        /// </summary>
        /// <param name="blockNumber">The block number</param>
        /// <returns></returns>
        public async Task<Block> GetBlock(uint blockNumber)
        {
            return await SendGetRequest<Block>($"{_blockchainAddress}/blocks/{blockNumber}");
        }


        /// <summary>
        /// Gets the <see cref="Transaction"/> object that contains all Transaction information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Transaction> GetTransaction(string id)
        {
            return await SendGetRequest<Transaction>($"{_blockchainAddress}/transactions/{id}");
        }

        /// <summary>
        /// Gets the <see cref="Receipt"/> object that contains all Receipt information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Receipt> GetReciept(string id)
        {
            return await SendGetRequest<Receipt>($"{_blockchainAddress}/transactions/{id}/receipt");
        }

        public async Task<HttpResponseMessage> TestnetFaucet(string address)
        {
            if (!CheckIfValid.Address(address))
                return null;

            var content = new StringContent($"{{\"to\":\"{address}\"}}", Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return  await _client.PostAsync("https://faucet.outofgas.io/requests", content);
        }

        private async Task<T> SendGetRequest<T>(string path)
        {
            var streamTask = await _client.GetStreamAsync(path);

            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T) serializer.ReadObject(streamTask);
        }
    }
}
