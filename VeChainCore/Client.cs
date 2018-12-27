using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using VeChainCore.Models;

namespace VeChainCore
{
    public static class Client
    {
        private static readonly HttpClient client = new HttpClient();


        private static async Task ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

            var msg = await stringTask;
            Console.Write(msg);


            var serializer = new DataContractJsonSerializer(typeof(List<Wallet>));
        }

        public static async Task GetWallet(string address)
        {
            var serializer = new DataContractJsonSerializer(typeof(Wallet));

            var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories = serializer.ReadObject(await streamTask) as Wallet;

        }

        public static async Task<Block> GetBlock(uint blockNumber)
        {
            var serializer = new DataContractJsonSerializer(typeof(Block));

            var streamTask = client.GetStreamAsync($"http://localhost:8669/blocks/{blockNumber}");
            Console.WriteLine(streamTask.ToString());
            return serializer.ReadObject(await streamTask) as Block;
        }
    }
}
