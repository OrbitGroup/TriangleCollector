﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TriangleCollector.Models.Interfaces;
using TriangleCollector.Services;

namespace TriangleCollector.Models.Exchanges.Huobi
{
    public class HuobiExchange : IExchange
    {
        public string ExchangeName { get; }

        public IExchangeClient ExchangeClient { get; } = new HuobiClient();

        public List<IClientWebSocket> Clients { get; } = new List<IClientWebSocket>();

        public Type OrderbookType { get; } = typeof(HuobiOrderbook);

        public HashSet<IOrderbook> TradedMarkets { get; set;  } = new HashSet<IOrderbook>();

        public ConcurrentDictionary<string, IOrderbook> OfficialOrderbooks { get; } = new ConcurrentDictionary<string, IOrderbook>();

        public ConcurrentQueue<Triangle> TrianglesToRecalculate { get; set; } = new ConcurrentQueue<Triangle>();

        public ConcurrentDictionary<string, Triangle> Triangles { get; } = new ConcurrentDictionary<string, Triangle>();

        public HashSet<IOrderbook> TriarbEligibleMarkets { get; set; } = new HashSet<IOrderbook>();

        public Queue<IOrderbook> SubscriptionQueue { get; set; } = new Queue<IOrderbook>();

        public ConcurrentDictionary<string, IOrderbook> SubscribedMarkets { get; set; } = new ConcurrentDictionary<string, IOrderbook>();

        public ConcurrentDictionary<string, List<Triangle>> TriarbMarketMapping { get; } = new ConcurrentDictionary<string, List<Triangle>>();

        public double ImpactedTriangleCounter { get; set; } = 0;

        public double RedundantTriangleCounter { get; set; } = 0;

        public double AllOrderBookCounter { get; set; } = 0;

        public double InsideLayerCounter { get; set; } = 0;

        public double OutsideLayerCounter { get; set; } = 0;

        public double PositivePriceChangeCounter { get; set; } = 0;

        public double NegativePriceChangeCounter { get; set; } = 0;

        public int UniqueTriangleCount { get; set; } = 0;

        public ConcurrentDictionary<string, int> ProfitableSymbolMapping { get; } = new ConcurrentDictionary<string, int>();

        public ConcurrentDictionary<string, DateTime> TriangleRefreshTimes { get; } = new ConcurrentDictionary<string, DateTime>();

        public ConcurrentQueue<Triangle> RecalculatedTriangles { get; } = new ConcurrentQueue<Triangle>();

        public HuobiExchange(string name)
        {

            ExchangeName = name;
            ExchangeClient.Exchange = this;

        }
    }
}

