using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Nethereum.Hex.HexConvertors.Extensions;
using VeChainCore.Utils.Cryptography;
using Xunit;

namespace VeChainCoreTest
{
    public class CryptoTest
    {
        const string helloWordStr = "Hello world";
        static readonly byte[] helloBytes = Encoding.UTF8.GetBytes(helloWordStr);

        [Fact]
        public void TestBlake2B()
        {
            byte[] hashBytes = Hash.Blake2B(helloBytes);
            string hashHex = hashBytes.ToHex(true);
            Assert.Equal("0xa21cf4b3604cf4b2bc53e6f88f6a4d75ef5ff4ab415f3e99aea6b61c8249c4d0", hashHex);
        }

        [Fact]
        public void TestKeccak256()
        {
            byte[] hashBytes = Hash.Keccak256(helloBytes);
            string hashHex = hashBytes.ToHex(true);
            Assert.Equal("0xed6c11b0b5b808960df26f5bfc471d04c1995b0ffd2055925ad1be28d6baadfd", hashHex);
        }

        private static void ThreadedHashTest(Func<byte[], byte[]> hashFunc)
        {
            int threadCount = Environment.ProcessorCount * 4 + 1;

            var sources = new byte[threadCount][];
            var expected = new byte[threadCount][];
            var actual = new byte[threadCount][];
            var threads = new Thread[threadCount];

            var ev = new EventWaitHandle(false, EventResetMode.ManualReset);
            var cd = new CountdownEvent(threadCount);

            for (int i = 0; i < threadCount; ++i)
            {
                RandomNumberGenerator.Fill(sources[i] = new byte[1024]);
                expected[i] = Hash.Blake2B(sources[i]);
                (threads[i] = new Thread(o =>
                {
                    int k = (int) o;
                    var src = sources[k];
                    ref var result = ref actual[k];
                    ev.WaitOne();
                    result = hashFunc(src);
                    cd.Signal();
                })).Start(i);
            }

            ev.Set();
            cd.Wait();

            for (int i = 0; i < threadCount; i++)
                Assert.Equal(expected[i], actual[i]);
        }

        [Fact]
        public void TestBlake2BThreaded()
        {
            Func<byte[], byte[]> hashFunc = Hash.Blake2B;

            ThreadedHashTest(hashFunc);
        }

        [Fact]
        public void TestKeccak256Threaded()
        {
            Func<byte[], byte[]> hashFunc = Hash.Keccak256;

            ThreadedHashTest(hashFunc);
        }
    }
}