﻿using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace Ray.Core
{
    public class OrleansClientFactory : IOrleansClientFactory
    {
        static IClusterClient _client;
        public static void Build(IClusterClient client)
        {
            _client = client;
        }
        readonly object connectLock = new object();
        public IClusterClient CreateClient()
        {
            if (!_client.IsInitialized)
            {
                lock (connectLock)
                {
                    if (!_client.IsInitialized)
                    {
                        _client.Connect().GetAwaiter().GetResult();
                    }
                }
            }
            return _client;
        }
    }
    public static class IClusterClientExtensions
    {
        public static async Task ConnectAndFill(this IClusterClient client)
        {
            await client.Connect();
            OrleansClientFactory.Build(client);
        }
    }
}
