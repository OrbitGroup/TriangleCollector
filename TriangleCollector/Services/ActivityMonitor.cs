﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;


namespace TriangleCollector.Services
{
    public class ActivityMonitor: BackgroundService
    {
        private readonly ILogger<ActivityMonitor> _logger;

        private readonly ILoggerFactory _factory;

        private int LoopTimer = 5; //the interval (in seconds) for each printout of the activity monitor

        private Dictionary<string, int> LastOBCounter = new Dictionary<string, int>();
        private Dictionary<string, int> LastTriarbCounter = new Dictionary<string, int>();

        public ActivityMonitor(ILoggerFactory factory, ILogger<ActivityMonitor> logger)
        {
            _logger = logger;
            _factory = factory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting Activity Monitor...");

            stoppingToken.Register(() => _logger.LogDebug("Stopping Activity Monitor..."));
            await Task.Run(async () =>
            {
                await BackgroundProcessing(stoppingToken);
            }, stoppingToken);
        }
        public async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            foreach(var exchange in ProjectOrbit.Exchanges)
            {
                LastOBCounter.Add(exchange.ExchangeName, 0);
                LastTriarbCounter.Add(exchange.ExchangeName, 0);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("*********************************************************************************************************************************************");
                foreach (var exchange in ProjectOrbit.Exchanges)
                {
                    var lastOBcount = LastOBCounter[exchange.ExchangeName];
                    var lastTriarbCount = LastTriarbCounter[exchange.ExchangeName];
                    var activeClientCount = exchange.Clients.Where(c => c.State == WebSocketState.Open).Count();
                    var abortedClientCount = exchange.Clients.Where(c => c.State != WebSocketState.Open).Count();
                    double activeSubscriptions = exchange.SubscribedMarkets.Count();
                    double targetSubscriptions = exchange.SubscribedMarkets.Count + exchange.SubscriptionQueue.Count;
                    double relevantRatio = Math.Round(targetSubscriptions / exchange.TradedMarkets.Count,2)*100;

                    double oldestClientAge = 0;
                    if(exchange.Clients.Count > 0)
                    {
                        var oldestClient = exchange.Clients.OrderByDescending(c => c.TimeStarted).Where(c => c.State == WebSocketState.Open);
                        oldestClientAge = Math.Round((DateTime.UtcNow - oldestClient.Last().TimeStarted).TotalMinutes,2);
                    }
                    
                    _logger.LogDebug($"{exchange.ExchangeName} --- Data Points Received: {exchange.AllOrderBookCounter}. Data Receipts/Second (last {LoopTimer}s): {(exchange.AllOrderBookCounter - lastOBcount) / LoopTimer}.");
                    _logger.LogDebug($"{exchange.ExchangeName} --- Triarb Opportunities Calculated: {exchange.RecalculatedTriangles.Count()}. Triarb Opportunities/ Second(last {LoopTimer}s): {(exchange.RecalculatedTriangles.Count() - lastTriarbCount) / LoopTimer}");
                    _logger.LogDebug($"{exchange.ExchangeName} --- Queue Size: {exchange.TrianglesToRecalculate.Count()} - Active Subscriptions: {activeSubscriptions} - {Math.Round(activeSubscriptions/targetSubscriptions,2)*100}% subscribed. {relevantRatio}% of markets are deemed relevant.");
                    _logger.LogDebug($"{exchange.ExchangeName} --- Active Clients: {activeClientCount} - Aborted Clients: {abortedClientCount} - Oldest Active Client: {oldestClientAge} minutes");

                    LastOBCounter[exchange.ExchangeName] = Convert.ToInt32(exchange.AllOrderBookCounter);
                    LastTriarbCounter[exchange.ExchangeName] = exchange.RecalculatedTriangles.Count();
                }
                _logger.LogDebug("*********************************************************************************************************************************************");
                await Task.Delay(LoopTimer * 1000);
            }
        }
    }
}
