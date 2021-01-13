﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TriangleCollector.Models.Interfaces
{
    public interface IExchangeClient
    {
        public IExchange Exchange { get; set; }
        public string TickerRestApi { get; }
        public string SocketClientApi { get; }
        public JsonElement.ArrayEnumerator Tickers { get; }
        public List<IClientWebSocket> Clients { get; set; }
        public IClientWebSocket Client { get; }
        public int MaxMarketsPerClient { get; }

        public void GetTickers();

        public Task<WebSocketAdapter> GetExchangeClientAsync(); //establishes initial connection to exchange for websocket

        public Task Subscribe(List<IOrderbook> Markets);
    }
}