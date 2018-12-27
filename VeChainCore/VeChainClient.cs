using System;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using VeChainCore.Models;

namespace VeChainCore
{
    public static class VeChainClient
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<Account> GetAccount(string address)
        {
            var streamTask = Client.GetStreamAsync($"http://localhost:8669/accounts/{address}");
            Console.WriteLine(streamTask.ToString());

            var serializer = new DataContractJsonSerializer(typeof(Account));
            return serializer.ReadObject(await streamTask) as Account;
        }

        public static async Task<Block> GetBlock(uint blockNumber)
        {
            var streamTask = Client.GetStreamAsync($"http://localhost:8669/blocks/{blockNumber}");
            Console.WriteLine(streamTask.ToString());

            var serializer = new DataContractJsonSerializer(typeof(Block));
            return serializer.ReadObject(await streamTask) as Block;
        }
    }
}
