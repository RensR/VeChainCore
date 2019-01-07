﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using VeChainCore.Utils;
using VeChainCore.Models;
using VeChainCore.Models.Transaction;

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

        /// <summary>
        /// Gets the blockchain tag that indicates what network is connected, main or testnet
        /// </summary>
        /// <returns></returns>
        public async Task<uint> GetChainTag()
        {
            var genesis = await GetBlock("0");
            var lastByte = genesis.id.Substring(genesis.id.Length - 2);

            return uint.Parse(lastByte, System.Globalization.NumberStyles.HexNumber);
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

            return await SendGetRequest<Account>($"/accounts/{address}");
        }

        /// <summary>
        /// Gets the <see cref="Block"/> object that contains all Block information for
        /// the given block number
        /// </summary>
        /// <param name="blockNumber">The block number or "best" for the latest</param>
        /// <returns></returns>
        public async Task<Block> GetBlock(string blockNumber)
        {
            return await SendGetRequest<Block>($"/blocks/{blockNumber}");
        }

        /// <summary>
        /// Gets the <see cref="Transaction"/> object that contains all Transaction information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Transaction> GetTransaction(string id)
        {
            return await SendGetRequest<Transaction>($"/transactions/{id}");
        }

        /// <summary>
        /// Gets the <see cref="Receipt"/> object that contains all Receipt information for
        /// the given transaction id
        /// </summary>
        /// <param name="id">The transaction id</param>
        /// <returns></returns>
        public async Task<Receipt> GetReciept(string id)
        {
            return await SendGetRequest<Receipt>($"/transactions/{id}/receipt");
        }

        public async Task<HttpResponseMessage> TestnetFaucet(string address)
        {
            if (!CheckIfValid.Address(address))
                return null;

            var content = new StringContent($"{{\"to\":\"{address}\"}}", Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return  await _client.PostAsync("https://faucet.outofgas.io/requests", content);
        }

        /// <summary>
        /// Send a GET request and retrieves an object of type T
        /// </summary>
        /// <typeparam name="T">The given object type e.g. Transaction, Block</typeparam>
        /// <param name="path">The path to the wanted object</param>
        /// <returns></returns>
        private async Task<T> SendGetRequest<T>(string path)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T) serializer.ReadObject(await _client.GetStreamAsync(RawUrl(path)));
        }

        /// <summary>
        /// Sends a POST request and return the chosen return type
        /// </summary>
        /// <param name="path">The path to the wanted object</param>
        /// <param name="httpContent">Parameters of the request</param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendPostRequest(string path, HttpContent httpContent)
        {
            return await _client.PostAsync(RawUrl(path), httpContent);
        }

        /// <summary>
        /// Transforms the path to the full URL including the blockchain address
        /// </summary>
        /// <param name="path">The query specific part of the URL</param>
        /// <returns></returns>
        private string RawUrl(string path)
        {
            return _blockchainAddress + path;
        }
    }
}
