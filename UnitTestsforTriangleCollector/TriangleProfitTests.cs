﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TriangleCollector.Models;
using TriangleCollector.Models.Interfaces;
using TriangleCollector.Models.Exchanges.Hitbtc;

namespace TriangleCollector.UnitTests
{
    [TestClass]
    public class TriangleProfitTests
    {
        private static ILoggerFactory _factory = new NullLoggerFactory();
        IOrderbook dummyOrderbook = new HitbtcOrderbook();

        //unprofitable triangles
        public Triangle EthEosBtc = new Triangle(new HitbtcOrderbook(), new HitbtcOrderbook(), new HitbtcOrderbook(), Triangle.Directions.BuyBuySell, (IExchange)Activator.CreateInstance(typeof(HitbtcExchange), typeof(HitbtcExchange).ToString()));
        public decimal EthEosBtcUnprofitableProfit = 0.9924677859176047787362868849m - 1;
        public decimal EthEosBtcUnprofitableVolume = 0.005536389908m;

        public Triangle EosEthBtc = new Triangle(new HitbtcOrderbook(), new HitbtcOrderbook(), new HitbtcOrderbook(), Triangle.Directions.BuySellSell, (IExchange)Activator.CreateInstance(typeof(HitbtcExchange), typeof(HitbtcExchange).ToString()));
        public decimal EosEthBtcUnprofitableProfit = 0.9994362518556066475976682718m - 1;
        public decimal EosEthBtcUnprofitableVolume = 0.005520685968m;

        public Triangle UsdEosBtc = new Triangle(new HitbtcOrderbook(), new HitbtcOrderbook(), new HitbtcOrderbook(), Triangle.Directions.SellBuySell, (IExchange)Activator.CreateInstance(typeof(HitbtcExchange), typeof(HitbtcExchange).ToString()));
        public decimal UsdEosBtcUnprofitableProfit = 0.9994800007008076808521821399m - 1;
        public decimal UsdEosBtcUnprofitableVolume = 0.01019975m;

        //Profitable expected outcomes: For each direction and bottleneck number 
        //BuyBuySell - Bottleneck is 1
        public decimal EthEosBtcProfitableBottleneckOneProfitPercent = 0.0115712778632038277275946787m; //profit expressed as a percentage of volume traded
        public decimal EthEosBtcProfitableBottleneckOneProfit = 0.0001422560403522186320696544m; //total profit returned in BTC
        public decimal EthEosBtcProfitableBottleneckOneVolume = 0.012293892m; //total volume traded

        //BuyBuySell - Bottleneck is 2
        public decimal EthEosBtcProfitableBottleneckTwoProfitPercent = 0.0071273462153337768951795121m; //profit expressed as a percentage of volume traded
        public decimal EthEosBtcProfitableBottleneckTwoProfit = 0.0009556552498559772740586650m; //total profit returned in BTC
        public decimal EthEosBtcProfitableBottleneckTwoVolume = 0.1340829m; //total volume traded

        //BuyBuySell - Bottleneck is 3
        public decimal EthEosBtcProfitableBottleneckThreeProfitPercent = 0.0092601928062477359015903732m; //profit expressed as a percentage of volume traded
        public decimal EthEosBtcProfitableBottleneckThreeProfit = 0.0035020514316271301617589103m; //total profit returned in BTC
        public decimal EthEosBtcProfitableBottleneckThreeVolume = 0.378183425m; //total volume traded

        //BuySellSell - Bottleneck is 1 and 2 (both are tested by these values)
        public decimal EosEthBtcProfitableBottleneckTwoProfitPercent = 0.0174905792813558993467008561m; //profit expressed as a percentage of volume traded
        public decimal EosEthBtcProfitableBottleneckTwoProfit = 0.0018468519546366782933333333m; //total profit returned in BTC
        public decimal EosEthBtcProfitableBottleneckTwoVolume = 0.1055912400000000000000000000m; //total volume traded

        //BuySellSell - Bottleneck is 3
        public decimal EosEthBtcProfitableBottleneckThreeProfitPercent = 0.0207868640295519956092501682m; //profit expressed as a percentage of volume traded
        public decimal EosEthBtcProfitableBottleneckThreeProfit = 0.0001133692698621333333333334m; //total profit returned in BTC
        public decimal EosEthBtcProfitableBottleneckThreeVolume = 0.00545389m; //total volume traded

        //SellBuySell - All 3 bottlenecks tested in one set of trades
        public decimal UsdEosBtcProfitableBottlenecksProfitPercent = 0.0154232556981579960243808671m; //profit expressed as a percentage of volume traded
        public decimal UsdEosBtcProfitableBottlenecksProfit = 0.0027124351549842805696714796m; //total profit returned in BTC
        public decimal UsdEosBtcProfitableBottlenecksVolume = 0.175866575m; //total volume traded

        class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return y.CompareTo(x);
            }
        }

        [TestMethod]
        public void TestProfitAndVolumeNoLayersBuyBuySell() // without any input, this tests the ProfitPercent output of an unprofitable triangle.
        {
            IOrderbook EthBtc = new HitbtcOrderbook();
            EthBtc.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialBids.TryAdd(0.034139m, 4.2344m);
            EthBtc.OfficialBids.TryAdd(0.034110m, 2.9281m);
            EthBtc.OfficialBids.TryAdd(0.034070m, 6.0711m);

            EthBtc.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialAsks.TryAdd(0.034172m, 3.6m);
            EthBtc.OfficialAsks.TryAdd(0.034200m, 0.3235m);
            EthBtc.OfficialAsks.TryAdd(0.035210m, 1.1731m);

            IOrderbook EosEth = new HitbtcOrderbook();
            EosEth.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEth.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEth.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEth.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialAsks.TryAdd(0.0081086m, 20m);
            EosEth.OfficialAsks.TryAdd(0.0081500m, 362.18m);
            EosEth.OfficialAsks.TryAdd(0.0081575m, 144.86m);
                
            IOrderbook EosBtcUnprofitable = new HitbtcOrderbook();
            EosBtcUnprofitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027619m, 104.95m);
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027750m, 123.82m);
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027900m, 160.66m);

            EosBtcUnprofitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027500m, 506.75m);
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027300m, 120.44m);
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027100m, 725.15m);              

            EthEosBtc.FirstSymbolOrderbook = EthBtc;
            EthEosBtc.SecondSymbolOrderbook = EosEth;
            EthEosBtc.ThirdSymbolOrderbook = EosBtcUnprofitable;
            CreateSnapshots.CreateOrderbookSnapshots(EthEosBtc);
            EthEosBtc.ProfitPercent = ProfitPercentCalculator.GetProfitPercent(EthEosBtc);
            EthEosBtc.MaxVolume = MaxVolumeCalculator.GetMaxVolume(EthEosBtc).Value;
            Assert.AreEqual(EthEosBtcUnprofitableProfit, EthEosBtc.ProfitPercent);
            Assert.AreEqual(EthEosBtcUnprofitableVolume, EthEosBtc.MaxVolume);
        }

        [TestMethod]
        public void TestProfitAndVolumeNoLayersSellBuySell()
        {
            IOrderbook BtcUsdBids = new HitbtcOrderbook();
            BtcUsdBids.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            BtcUsdBids.OfficialBids.TryAdd(10372.24m, 0.75m);
            BtcUsdBids.OfficialBids.TryAdd(10370.04m, 0.12m);
            BtcUsdBids.OfficialBids.TryAdd(10367.85m, 0.24m);

            IOrderbook EosUsdAsks = new HitbtcOrderbook();
            EosUsdAsks.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosUsdAsks.OfficialAsks.TryAdd(2.85385m, 37.09m);
            EosUsdAsks.OfficialAsks.TryAdd(2.86429m, 600m);
            EosUsdAsks.OfficialAsks.TryAdd(2.86940m, 363.86m);

            IOrderbook EosBtcUnprofitable = new HitbtcOrderbook();
            EosBtcUnprofitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027619m, 104.95m);
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027750m, 123.82m);
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027900m, 160.66m);

            EosBtcUnprofitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027500m, 506.75m);
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027300m, 120.44m);
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027100m, 725.15m);
            
            UsdEosBtc.FirstSymbolOrderbook = BtcUsdBids;
            UsdEosBtc.SecondSymbolOrderbook = EosUsdAsks;
            UsdEosBtc.ThirdSymbolOrderbook = EosBtcUnprofitable;
            CreateSnapshots.CreateOrderbookSnapshots(UsdEosBtc);

            UsdEosBtc.ProfitPercent = ProfitPercentCalculator.GetProfitPercent(UsdEosBtc);
            UsdEosBtc.MaxVolume = MaxVolumeCalculator.GetMaxVolume(UsdEosBtc).Value;

            Assert.AreEqual(UsdEosBtcUnprofitableProfit, UsdEosBtc.ProfitPercent);
            Assert.AreEqual(UsdEosBtcUnprofitableVolume, UsdEosBtc.MaxVolume);
        }

        [TestMethod]
        public void TestVolumeAndProfitNoLayersBuySellSell()
        {
            IOrderbook EosBtcUnprofitable = new HitbtcOrderbook();
            EosBtcUnprofitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027619m, 104.95m);
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027750m, 123.82m);
            EosBtcUnprofitable.OfficialAsks.TryAdd(0.00027900m, 160.66m);

            EosBtcUnprofitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027500m, 506.75m);
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027300m, 120.44m);
            EosBtcUnprofitable.OfficialBids.TryAdd(0.00027100m, 725.15m);

            IOrderbook EthBtc = new HitbtcOrderbook();
            EthBtc.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialBids.TryAdd(0.034139m, 4.2344m);
            EthBtc.OfficialBids.TryAdd(0.034110m, 2.9281m);
            EthBtc.OfficialBids.TryAdd(0.034070m, 6.0711m);

            EthBtc.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialAsks.TryAdd(0.034172m, 3.6m);
            EthBtc.OfficialAsks.TryAdd(0.034200m, 0.3235m);
            EthBtc.OfficialAsks.TryAdd(0.035210m, 1.1731m);

            IOrderbook EosEth = new HitbtcOrderbook();
            EosEth.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEth.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEth.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEth.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialAsks.TryAdd(0.0081086m, 20m);
            EosEth.OfficialAsks.TryAdd(0.0081500m, 362.18m);
            EosEth.OfficialAsks.TryAdd(0.0081575m, 144.86m);

            EosEthBtc.FirstSymbolOrderbook = EosBtcUnprofitable;
            EosEthBtc.SecondSymbolOrderbook = EosEth;
            EosEthBtc.ThirdSymbolOrderbook = EthBtc;
            CreateSnapshots.CreateOrderbookSnapshots(EosEthBtc);

            EosEthBtc.ProfitPercent = ProfitPercentCalculator.GetProfitPercent(EosEthBtc);
            EosEthBtc.MaxVolume = MaxVolumeCalculator.GetMaxVolume(EosEthBtc).Value;

            Assert.AreEqual(EosEthBtcUnprofitableProfit, EosEthBtc.ProfitPercent);
            Assert.AreEqual(EosEthBtcUnprofitableVolume, EosEthBtc.MaxVolume);
        }

        [TestMethod]
        public void TestLayersBuyBuySellBottleneckTwo() 
        {
            IOrderbook EthBtc = new HitbtcOrderbook();
            EthBtc.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialBids.TryAdd(0.034139m, 4.2344m);
            EthBtc.OfficialBids.TryAdd(0.034110m, 2.9281m);
            EthBtc.OfficialBids.TryAdd(0.034070m, 6.0711m);

            EthBtc.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialAsks.TryAdd(0.034172m, 3.6m);
            EthBtc.OfficialAsks.TryAdd(0.034200m, 0.3235m);
            EthBtc.OfficialAsks.TryAdd(0.035210m, 1.1731m);

            IOrderbook EosEth = new HitbtcOrderbook();
            EosEth.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEth.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEth.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEth.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialAsks.TryAdd(0.0081086m, 20m);
            EosEth.OfficialAsks.TryAdd(0.0081500m, 362.18m);
            EosEth.OfficialAsks.TryAdd(0.0081575m, 144.86m);

            //BUYBUYSELL BOTTLENECK = TRADE 2 (USE REGULAR TEST ORDER BOOKS FOR FIRST TWO TRADES): 
            IOrderbook EosBtcProfitable = new HitbtcOrderbook(); //since all of the unprofitable test values are very close to equilibrium, a 2% change in price here will make all triangles profitable
            EosBtcProfitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027000m, 104.95m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027100m, 123.82m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027200m, 160.66m);

            EosBtcProfitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialBids.TryAdd(0.00028050m, 506.75m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00028000m, 120.44m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00027900m, 725.15m);

            EthEosBtc.FirstSymbolOrderbook = EthBtc;
            EthEosBtc.SecondSymbolOrderbook = EosEth;
            EthEosBtc.ThirdSymbolOrderbook = EosBtcProfitable;
            CreateSnapshots.CreateOrderbookSnapshots(EthEosBtc);

            EthEosBtc.SetMaxVolumeAndProfitability();
            Assert.AreEqual(EthEosBtcProfitableBottleneckTwoProfit, EthEosBtc.Profit, "Incorrect Profit");
            Assert.AreEqual(EthEosBtcProfitableBottleneckTwoVolume, EthEosBtc.MaxVolume, "Incorrect Volume");
            Assert.AreEqual(EthEosBtcProfitableBottleneckTwoProfitPercent, EthEosBtc.ProfitPercent, "Incorrect ProfitPercent");
        }

        [TestMethod]
        public void TestLayersBuyBuySellBottleneckOne() //only profitable triangles require inputs - volume is calculated as well
        {
            //BUYBUYSELL BOTTLENECK = TRADE 1 (USE PROFITABLE TEST ORDERBOOK FOR THIRD TRADE):
            IOrderbook EthBtcBuyBuySellBottleneckOne = new HitbtcOrderbook();
            EthBtcBuyBuySellBottleneckOne.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtcBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.034139m, 4.2344m);
            EthBtcBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.034110m, 2.9281m);
            EthBtcBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.034070m, 6.0711m);

            EthBtcBuyBuySellBottleneckOne.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtcBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.034172m, 0.036m);
            EthBtcBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.034200m, 0.3235m);
            EthBtcBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.035210m, 1.1731m);

            //BUYBUYSELL BOTTLENECK = TRADE 2 (USE REGULAR TEST ORDER BOOKS FOR FIRST TWO TRADES): 
            IOrderbook EosBtcProfitable = new HitbtcOrderbook(); //since all of the unprofitable test values are very close to equilibrium, a 2% change in price here will make all triangles profitable
            EosBtcProfitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027000m, 104.95m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027100m, 123.82m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027200m, 160.66m);

            EosBtcProfitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialBids.TryAdd(0.00028050m, 506.75m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00028000m, 120.44m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00027900m, 725.15m);

            IOrderbook EosEthBuyBuySellBottleneckOne = new HitbtcOrderbook();
            EosEthBuyBuySellBottleneckOne.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEthBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEthBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEthBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEthBuyBuySellBottleneckOne.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEthBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.0081086m, 2000000m);
            EosEthBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.0081500m, 3620000.18m);
            EosEthBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.0081575m, 1440000.86m);

            EthEosBtc.FirstSymbolOrderbook = EthBtcBuyBuySellBottleneckOne;
            EthEosBtc.SecondSymbolOrderbook = EosEthBuyBuySellBottleneckOne;
            EthEosBtc.ThirdSymbolOrderbook = EosBtcProfitable;
            CreateSnapshots.CreateOrderbookSnapshots(EthEosBtc);

            EthEosBtc.SetMaxVolumeAndProfitability();
            Assert.AreEqual(EthEosBtcProfitableBottleneckOneProfit, EthEosBtc.Profit, "Incorrect Profit");
            Assert.AreEqual(EthEosBtcProfitableBottleneckOneVolume, EthEosBtc.MaxVolume, "Incorrect Volume");
            Assert.AreEqual(EthEosBtcProfitableBottleneckOneProfitPercent, EthEosBtc.ProfitPercent, "Incorrect ProfitPercent");
        }

        [TestMethod]
        public void TestLayersBuyBuySellBottleneckThree() //only profitable triangles require inputs - volume is calculated as well
        {
            //BUYBUYSELL BOTTLENECK = TRADE 2 (USE REGULAR TEST ORDER BOOKS FOR FIRST TWO TRADES): 
            IOrderbook EosBtcProfitable = new HitbtcOrderbook(); //since all of the unprofitable test values are very close to equilibrium, a 2% change in price here will make all triangles profitable
            EosBtcProfitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027000m, 104.95m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027100m, 123.82m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027200m, 160.66m);

            EosBtcProfitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialBids.TryAdd(0.00028050m, 506.75m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00028000m, 120.44m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00027900m, 725.15m);

            //BUYBUYSELL BOTTLENECK = TRADE 3 (USE OTHER PROFITABLE TEST ORDERBOOK FOR SECOND AND THIRD TRADE):
            IOrderbook EthBtcBuyBuySellBottleneckThree = new HitbtcOrderbook();
            EthBtcBuyBuySellBottleneckThree.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtcBuyBuySellBottleneckThree.OfficialBids.TryAdd(0.034139m, 4.2344m);
            EthBtcBuyBuySellBottleneckThree.OfficialBids.TryAdd(0.034110m, 2.9281m);
            EthBtcBuyBuySellBottleneckThree.OfficialBids.TryAdd(0.034070m, 6.0711m);

            EthBtcBuyBuySellBottleneckThree.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtcBuyBuySellBottleneckThree.OfficialAsks.TryAdd(0.034172m, 36m);
            EthBtcBuyBuySellBottleneckThree.OfficialAsks.TryAdd(0.034200m, 32.35m);
            EthBtcBuyBuySellBottleneckThree.OfficialAsks.TryAdd(0.035210m, 17.31m);

            IOrderbook EosEthBuyBuySellBottleneckOne = new HitbtcOrderbook();
            EosEthBuyBuySellBottleneckOne.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEthBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEthBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEthBuyBuySellBottleneckOne.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEthBuyBuySellBottleneckOne.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEthBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.0081086m, 2000000m);
            EosEthBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.0081500m, 3620000.18m);
            EosEthBuyBuySellBottleneckOne.OfficialAsks.TryAdd(0.0081575m, 1440000.86m);

            EthEosBtc.FirstSymbolOrderbook = EthBtcBuyBuySellBottleneckThree;
            EthEosBtc.SecondSymbolOrderbook = EosEthBuyBuySellBottleneckOne;
            EthEosBtc.ThirdSymbolOrderbook = EosBtcProfitable;
            CreateSnapshots.CreateOrderbookSnapshots(EthEosBtc);

            EthEosBtc.SetMaxVolumeAndProfitability();
            Assert.AreEqual(EthEosBtcProfitableBottleneckThreeProfit, EthEosBtc.Profit);
            Assert.AreEqual(EthEosBtcProfitableBottleneckThreeVolume, EthEosBtc.MaxVolume);
            Assert.AreEqual(EthEosBtcProfitableBottleneckThreeProfitPercent, EthEosBtc.ProfitPercent);
        }

        [TestMethod]
        public void TestLayersBuySellSellBottleneckTwo() //this will also test a bottleneck of one, so it accomplishes two tests in one.
        {
            IOrderbook EthBtc = new HitbtcOrderbook();
            EthBtc.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialBids.TryAdd(0.034139m, 4.2344m);
            EthBtc.OfficialBids.TryAdd(0.034110m, 2.9281m);
            EthBtc.OfficialBids.TryAdd(0.034070m, 6.0711m);

            EthBtc.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialAsks.TryAdd(0.034172m, 3.6m);
            EthBtc.OfficialAsks.TryAdd(0.034200m, 0.3235m);
            EthBtc.OfficialAsks.TryAdd(0.035210m, 1.1731m);

            IOrderbook EosEth = new HitbtcOrderbook();
            EosEth.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEth.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEth.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEth.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialAsks.TryAdd(0.0081086m, 20m);
            EosEth.OfficialAsks.TryAdd(0.0081500m, 362.18m);
            EosEth.OfficialAsks.TryAdd(0.0081575m, 144.86m);

            //BUYBUYSELL BOTTLENECK = TRADE 2 (USE REGULAR TEST ORDER BOOKS FOR FIRST TWO TRADES): 
            IOrderbook EosBtcProfitable = new HitbtcOrderbook(); //since all of the unprofitable test values are very close to equilibrium, a 2% change in price here will make all triangles profitable
            EosBtcProfitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027000m, 104.95m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027100m, 123.82m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027200m, 160.66m);

            EosBtcProfitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialBids.TryAdd(0.00028050m, 506.75m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00028000m, 120.44m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00027900m, 725.15m);

            EosEthBtc.FirstSymbolOrderbook = EosBtcProfitable;
            EosEthBtc.SecondSymbolOrderbook = EosEth;
            EosEthBtc.ThirdSymbolOrderbook = EthBtc;
            CreateSnapshots.CreateOrderbookSnapshots(EosEthBtc);

            EosEthBtc.SetMaxVolumeAndProfitability();
            Assert.AreEqual(EosEthBtcProfitableBottleneckTwoProfit, EosEthBtc.Profit, "Incorrect Profit");
            Assert.AreEqual(EosEthBtcProfitableBottleneckTwoVolume, EosEthBtc.MaxVolume, "Incorrect Volume");
            Assert.AreEqual(EosEthBtcProfitableBottleneckTwoProfitPercent, EosEthBtc.ProfitPercent, "Incorrect ProfitPercent");
        }
        [TestMethod]
        public void TestLayersBuySellSellBottleneckThree()
        {
            IOrderbook EthBtc = new HitbtcOrderbook();
            EthBtc.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialBids.TryAdd(0.034139m, 0.01m);
            EthBtc.OfficialBids.TryAdd(0.034110m, 0.05m);
            EthBtc.OfficialBids.TryAdd(0.034070m, 0.1m);

            EthBtc.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EthBtc.OfficialAsks.TryAdd(0.034172m, 3.6m);
            EthBtc.OfficialAsks.TryAdd(0.034200m, 0.3235m);
            EthBtc.OfficialAsks.TryAdd(0.035210m, 1.1731m);

            IOrderbook EosEth = new HitbtcOrderbook();
            EosEth.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialBids.TryAdd(0.0080856m, 20m);
            EosEth.OfficialBids.TryAdd(0.0080810m, 543.14m);
            EosEth.OfficialBids.TryAdd(0.0080500m, 144.83m);

            EosEth.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosEth.OfficialAsks.TryAdd(0.0081086m, 20m);
            EosEth.OfficialAsks.TryAdd(0.0081500m, 362.18m);
            EosEth.OfficialAsks.TryAdd(0.0081575m, 144.86m);

            //BUYBUYSELL BOTTLENECK = TRADE 3 (USE REGULAR TEST ORDER BOOKS FOR FIRST TWO TRADES): 
            IOrderbook EosBtcProfitable = new HitbtcOrderbook(); //since all of the unprofitable test values are very close to equilibrium, a 2% change in price here will make all triangles profitable
            EosBtcProfitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027000m, 104.95m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027100m, 123.82m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027200m, 160.66m);

            EosBtcProfitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialBids.TryAdd(0.00028050m, 506.75m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00028000m, 120.44m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00027900m, 725.15m);

            EosEthBtc.FirstSymbolOrderbook = EosBtcProfitable;
            EosEthBtc.SecondSymbolOrderbook = EosEth;
            EosEthBtc.ThirdSymbolOrderbook = EthBtc;
            CreateSnapshots.CreateOrderbookSnapshots(EosEthBtc);

            EosEthBtc.SetMaxVolumeAndProfitability();
            Assert.AreEqual(EosEthBtcProfitableBottleneckThreeProfit, EosEthBtc.Profit, "Incorrect Profit");
            Assert.AreEqual(EosEthBtcProfitableBottleneckThreeVolume, EosEthBtc.MaxVolume, "Incorrect Volume");
            Assert.AreEqual(EosEthBtcProfitableBottleneckThreeProfitPercent, EosEthBtc.ProfitPercent, "Incorrect ProfitPercent");
        }
        [TestMethod]
        public void TestLayersSellBuySellBottlenecks()
        {
            IOrderbook BtcUsdSortedBids = new HitbtcOrderbook();
            BtcUsdSortedBids.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            BtcUsdSortedBids.OfficialBids.TryAdd(10372.24m, 0.01m);
            BtcUsdSortedBids.OfficialBids.TryAdd(10370.04m, 1m);
            BtcUsdSortedBids.OfficialBids.TryAdd(10367.85m, 2m);
            BtcUsdSortedBids.OfficialAsks.TryAdd(1m, 1m); //dummy to avoid null exception

            IOrderbook EosUsdSortedAsks = new HitbtcOrderbook();
            EosUsdSortedAsks.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosUsdSortedAsks.OfficialAsks.TryAdd(2.85385m, 37.09m);
            EosUsdSortedAsks.OfficialAsks.TryAdd(2.86429m, 600m);
            EosUsdSortedAsks.OfficialAsks.TryAdd(2.86940m, 363.86m);

            IOrderbook EosBtcProfitable = new HitbtcOrderbook(); //since all of the unprofitable test values are very close to equilibrium, a 2% change in price here will make all triangles profitable
            EosBtcProfitable.OfficialAsks = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027000m, 104.95m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027100m, 123.82m);
            EosBtcProfitable.OfficialAsks.TryAdd(0.00027200m, 160.66m);

            EosBtcProfitable.OfficialBids = new ConcurrentDictionary<decimal, decimal>();
            EosBtcProfitable.OfficialBids.TryAdd(0.00028050m, 506.75m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00028000m, 120.44m);
            EosBtcProfitable.OfficialBids.TryAdd(0.00027300m, 725.15m);

            UsdEosBtc.FirstSymbolOrderbook = BtcUsdSortedBids;
            UsdEosBtc.SecondSymbolOrderbook = EosUsdSortedAsks;
            UsdEosBtc.ThirdSymbolOrderbook = EosBtcProfitable;
            CreateSnapshots.CreateOrderbookSnapshots(UsdEosBtc);

            UsdEosBtc.SetMaxVolumeAndProfitability();
            Assert.AreEqual(UsdEosBtcProfitableBottlenecksProfit, UsdEosBtc.Profit, "Incorrect Profit");
            Assert.AreEqual(UsdEosBtcProfitableBottlenecksVolume, UsdEosBtc.MaxVolume, "Incorrect Volume");
            Assert.AreEqual(UsdEosBtcProfitableBottlenecksProfitPercent, UsdEosBtc.ProfitPercent, "Incorrect ProfitPercent");
        }
    }
}
